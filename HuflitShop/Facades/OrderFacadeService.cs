using HuflitShop.Data;
using HuflitShop.Models;
using HuflitShop.Observers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HuflitShop.Facades
{
    /// <summary>
    /// ===== FACADE PATTERN =====
    /// Facade để gom tất cả logic xử lý Order vào 1 điểm duy nhất.
    /// 
    /// Thay vì Controller gọi nhiều service/method, ta chỉ gọi 1 facade method.
    /// OrderFacadeService bao gồm:
    /// - Tạo Order
    /// - Cập nhật Carts
    /// - Tạo OrderDetails
    /// - Cập nhật inventory (ProductSize)
    /// - Gợi observers (logging, notification)
    /// 
    /// Lợi ích:
    /// - Code đơn giản, dễ đọc
    /// - Giảm complexity trong Controller
    /// - Tập trung logic ở 1 chỗ
    /// </summary>
    public interface IOrderFacadeService
    {
        // ===== ĐÃ ÁP DỤNG FACADE PATTERN =====
        // Xử lý toàn bộ quá trình đặt hàng tiền mặt
        Task<(bool Success, string Message, int OrderId)> ProcessCashOrderAsync(string userId);

        // ===== ĐÃ ÁP DỤNG FACADE PATTERN =====
        // Kiểm tra cart trước khi đặt hàng
        Task<(bool IsValid, string Message)> ValidateCartAsync(string userId);
    }

    public class OrderFacadeService : IOrderFacadeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderFacadeService> _logger;
        private readonly List<IOrderObserver> _observers; // Danh sách observers

        public OrderFacadeService(AppDbContext context, ILogger<OrderFacadeService> logger)
        {
            _context = context;
            _logger = logger;
            _observers = new List<IOrderObserver>();
        }

        // ===== ĐÃ ÁP DỤNG FACADE PATTERN =====
        // Đăng ký observer
        public void Subscribe(IOrderObserver observer)
        {
            _observers.Add(observer);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Facade: Observer đăng ký: {observer.GetObserverName()}");
        }

        // ===== ĐÃ ÁP DỤNG FACADE PATTERN =====
        // Thực hiện toàn bộ quá trình đặt hàng tiền mặt
        public async Task<(bool Success, string Message, int OrderId)> ProcessCashOrderAsync(string userId)
        {
            const float SHOPPING_FEE = 20000;

            try
            {
                // 1. Kiểm tra cart hợp lệ
                var validationResult = await ValidateCartAsync(userId);
                if (!validationResult.IsValid)
                    return (false, validationResult.Message, 0);

                // 2. Tạo Order
                var order = new Order
                {
                    UserId = userId,
                    PaymentMenthodId = 1, // 1 = Tiền mặt
                    ShoppingFee = SHOPPING_FEE,
                    Created_date = DateTime.Now,
                    Status = 0 // 0 = Pending
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 3. Gọi observers (log sự kiện)
                await NotifyObserversOrderCreated(order);

                // 4. Lấy cart items
                var cartItems = _context.Carts
                    .Where(c => c.UserId == userId && c.IsBuy == false)
                    .ToList();

                // 5. Đánh dấu cart items là đã mua
                foreach (var item in cartItems)
                {
                    item.IsBuy = true;
                    _context.Carts.Update(item);
                }
                await _context.SaveChangesAsync();

                // 6. Tạo OrderDetails cho mỗi cart item
                foreach (var cartItem in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        SelectedSize = cartItem.SelectedSize,
                        Price = cartItem.UnitPrice * (1 - cartItem.Promotion),
                        Promotion = cartItem.Promotion
                    };
                    _context.Add(orderDetail);

                    // 7. Cập nhật inventory (ProductSize)
                    var productSize = _context.ProductSize
                        .FirstOrDefault(ps => ps.ProductId == cartItem.ProductId && 
                                            ps.SizeId == cartItem.SelectedSize);
                    if (productSize != null)
                    {
                        productSize.Quantity -= cartItem.Quantity;
                        _context.ProductSize.Update(productSize);
                    }
                }
                await _context.SaveChangesAsync();

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Facade: Order #{order.Id} xử lý thành công");
                return (true, "Đặt hàng thành công!", order.Id);
            }
            catch (Exception ex)
            {
                var errorMsg = $"Lỗi xử lý order: {ex.Message}";
                _logger.LogError($"[Facade] {errorMsg}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Facade ERROR: {errorMsg}");
                return (false, errorMsg, 0);
            }
        }

        // ===== ĐÃ ÁP DỤNG FACADE PATTERN =====
        // Kiểm tra cart hợp lệ
        public async Task<(bool IsValid, string Message)> ValidateCartAsync(string userId)
        {
            var cartItems = _context.Carts
                .Where(c => c.UserId == userId && c.IsBuy == false)
                .ToList();

            if (!cartItems.Any())
                return (false, "Giỏ hàng trống!");

            foreach (var item in cartItems)
            {
                var productSize = _context.ProductSize
                    .FirstOrDefault(ps => ps.ProductId == item.ProductId && 
                                        ps.SizeId == item.SelectedSize);

                if (productSize == null || productSize.Quantity < item.Quantity)
                    return (false, $"Sản phẩm không đủ số lượng!");
            }

            return (true, "OK");
        }

        // ===== ĐÃ ÁP DỤNG FACADE PATTERN + OBSERVER PATTERN =====
        // Gọi tất cả observers khi Order được tạo
        private async Task NotifyObserversOrderCreated(Order order)
        {
            foreach (var observer in _observers)
            {
                await observer.OnOrderCreated(order);
            }
        }
    }
}
