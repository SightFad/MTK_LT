using HuflitShop.Models;
using System;

namespace HuflitShop.Prototypes
{
    /// <summary>
    /// ===== PROTOTYPE PATTERN =====
    /// Prototype để clone Product objects cho use cases cụ thể.
    /// 
    /// Giúp:
    /// - Clone Product khi thêm vào cart (tránh thay đổi dữ liệu gốc)
    /// - Tạo Product snapshots độc lập cho OrderDetail
    /// - Bảo vệ dữ liệu gốc từ DB
    /// - Dễ thao tác với product copy mà không ảnh hưởng DB
    /// 
    /// Thay vì tạo Cart từ Product gốc:
    ///   cart.ProductId = product.Id;
    ///   cart.UnitPrice = product.Price;
    /// 
    /// Ta clone Product trước:
    ///   var clonedProduct = _productPrototype.CloneForCart(product);
    ///   var cart = CreateCartFromClonedProduct(clonedProduct);
    /// </summary>
    public interface IProductPrototype
    {
        // ===== ĐÃ ÁP DỤNG PROTOTYPE PATTERN =====
        // Clone Product cho thêm vào cart (chỉ clone cần thiết cho cart)
        Product CloneForCart(Product original);

        // ===== ĐÃ ÁP DỤNG PROTOTYPE PATTERN =====
        // Clone Product toàn bộ (deep copy đầy đủ)
        Product CloneFull(Product original);
    }

    public class ProductPrototype : IProductPrototype
    {
        // ===== ĐÃ ÁP DỤNG PROTOTYPE PATTERN =====
        // Clone Product cho use case thêm vào cart
        public Product CloneForCart(Product original)
        {
            if (original == null)
                return null;

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [PROTOTYPE - PRODUCT] Bắt đầu clone Product cho Cart");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [PROTOTYPE - PRODUCT]   Original - Id: {original.Id}, Name: {original.Name}, Price: {original.Price}");

            // Clone chỉ những properties cần cho Cart
            var clonedProduct = new Product
            {
                Id = original.Id,
                Name = original.Name,
                Price = original.Price,
                Description = original.Description,
                Origin = original.Origin,
                Trandemark = original.Trandemark,
                Active = original.Active,
                CategoryId = original.CategoryId,
                PromotionId = original.PromotionId,
                // Giữ references đến Category và Promotion
                Category = original.Category,
                Promotion = original.Promotion
                // Không clone: Image, OrderDetail, Cart, ProductSize, Reviews (tránh đệ quy)
            };

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [PROTOTYPE - PRODUCT] ✓ Clone thành công - Product snapshot được tạo cho Cart");
            return clonedProduct;
        }

        // ===== ĐÃ ÁP DỤNG PROTOTYPE PATTERN =====
        // Clone Product toàn bộ (deep copy đầy đủ)
        public Product CloneFull(Product original)
        {
            if (original == null)
                return null;

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [PROTOTYPE - PRODUCT] Bắt đầu full clone Product");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [PROTOTYPE - PRODUCT]   Original - Id: {original.Id}, Name: {original.Name}");

            // Clone toàn bộ properties
            var clonedProduct = new Product
            {
                Id = original.Id,
                Name = original.Name,
                Price = original.Price,
                Description = original.Description,
                Origin = original.Origin,
                Trandemark = original.Trandemark,
                Active = original.Active,
                CategoryId = original.CategoryId,
                PromotionId = original.PromotionId,
                Year_SX = original.Year_SX,
                Category = original.Category,
                Promotion = original.Promotion
                // Lưu ý: Không clone collections (Image, OrderDetail, Cart, ProductSize, Reviews)
                // để tránh đệ quy vô hạn và memory issues
            };

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [PROTOTYPE - PRODUCT] ✓ Full clone thành công");
            return clonedProduct;
        }
    }
}
