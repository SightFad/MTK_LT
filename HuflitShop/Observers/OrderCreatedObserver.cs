using HuflitShop.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HuflitShop.Observers
{
    /// <summary>
    /// ===== OBSERVER PATTERN =====
    /// Observer để log sự kiện Order.
    /// Implements IOrderObserver để theo dõi sự kiện Order được tạo/hủy.
    /// </summary>
    public class OrderCreatedObserver : IOrderObserver
    {
        private readonly ILogger<OrderCreatedObserver> _logger;

        public OrderCreatedObserver(ILogger<OrderCreatedObserver> logger)
        {
            _logger = logger;
        }

        // ===== ĐÃ ÁP DỤNG OBSERVER PATTERN =====
        // Ghi log khi Order được tạo
        public Task OnOrderCreated(Order order)
        {
            var message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ORDER CREATED - " +
                         $"OrderId: {order.Id}, UserId: {order.UserId}, " +
                         $"PaymentMethod: {order.PaymentMenthodId}, " +
                         $"ShoppingFee: {order.ShoppingFee}, Status: {order.Status}";
            
            _logger.LogInformation(message);
            Console.WriteLine(message); // Hiển thị trên console để dễ debug
            
            return Task.CompletedTask;
        }

        // ===== ĐÃ ÁP DỤNG OBSERVER PATTERN =====
        // Ghi log khi Order bị hủy
        public Task OnOrderCancelled(Order order)
        {
            var message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ORDER CANCELLED - " +
                         $"OrderId: {order.Id}, UserId: {order.UserId}";
            
            _logger.LogWarning(message);
            Console.WriteLine(message);
            
            return Task.CompletedTask;
        }

        // ===== ĐÃ ÁP DỤNG OBSERVER PATTERN =====
        // Lấy tên observer
        public string GetObserverName()
        {
            return "OrderCreatedObserver (Logging)";
        }
    }
}
