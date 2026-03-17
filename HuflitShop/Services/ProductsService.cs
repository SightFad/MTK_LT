using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuflitShop.Data;
using HuflitShop.Models;

namespace HuflitShop.Services
{
    public class ProductsService
    {
        private readonly AppDbContext _context;

        public ProductsService(AppDbContext context)
        {
            _context = context;
        }

        /* ===== CHƯA ÁP DỤNG DESIGN PATTERN =====
        // Cách cũ: Truy vấn trực tiếp mà không có tính năng clone
        public Product[] Get(string search)
        {
            var s = search.ToLower();
            return _context.Product.Where(p =>
                p.Name.ToLower().Contains(s) ||
                p.Category.Name.ToLower().Contains(s) ||
                p.Trandemark.ToLower().Contains(s) ||
                p.Origin.Contains(s)
            ).ToArray();
        }
        */

        /* ===== ĐÃ ÁP DỤNG PROTOTYPE PATTERN =====
        Prototype Pattern: Clone object để tránh thay đổi dữ liệu gốc.
        Khi tìm kiếm sản phẩm, ta có thể clone sản phẩm để:
        - Tránh modification trực tiếp trên object từ DB
        - Giữ dữ liệu gốc nguyên vẹn
        - Dễ thao tác với product copy mà không ảnh hưởng dữ liệu thật
        */
        public Product[] Get(string search)
        {
            var s = search.ToLower();
            var foundProducts = _context.Product.Where(p =>
                p.Name.ToLower().Contains(s) ||
                p.Category.Name.ToLower().Contains(s) ||
                p.Trandemark.ToLower().Contains(s) ||
                p.Origin.Contains(s)
            ).ToList();

            // ===== ĐÃ ÁP DỤNG PROTOTYPE PATTERN =====
            // Clone mỗi product tìm được để tránh thay đổi dữ liệu gốc
            var clonedProducts = new List<Product>();
            foreach (var product in foundProducts)
            {
                clonedProducts.Add(CloneProduct(product));
            }
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ProductsService: Cloned {clonedProducts.Count} products (Prototype Pattern)");
            return clonedProducts.ToArray();
        }

        // ===== ĐÃ ÁP DỤNG PROTOTYPE PATTERN =====
        // Phương thức clone Product (Deep Copy)
        private Product CloneProduct(Product original)
        {
            if (original == null)
                return null;

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [PROTOTYPE] Clone Product - Id: {original.Id}, Name: {original.Name}");
            
            var clonedProduct = new Product
            {
                Id = original.Id,
                Name = original.Name,
                Description = original.Description,
                Price = original.Price,
                Origin = original.Origin,
                Trandemark = original.Trandemark,
                Active = original.Active,
                CategoryId = original.CategoryId,
                PromotionId = original.PromotionId,
                Category = original.Category,
                Promotion = original.Promotion
                // Lưu ý: Không clone Image, OrderDetail, Cart, ProductSize để tránh đệ quy vô hạn
            };
            
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [PROTOTYPE] Clone thành công - Tạo deep copy của Product");
            return clonedProduct;
        }
    }
}
