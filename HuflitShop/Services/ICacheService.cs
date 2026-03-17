using HuflitShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HuflitShop.Services
{
    /// <summary>
    /// ===== SINGLETON PATTERN =====
    /// Interface cho Cache Service.
    /// Singleton đảm bảo chỉ có 1 instance cache duy nhất trong suốt vòng đời app.
    /// Lưu trữ dữ liệu không thay đổi thường xuyên:
    /// - Categories (danh mục sản phẩm)
    /// - System configuration
    /// </summary>
    public interface ICacheService
    {
        // ===== ĐÃ ÁP DỤNG SINGLETON PATTERN =====
        // Lấy tất cả categories từ cache
        Task<List<Category>> GetCategoriesAsync();

        // ===== ĐÃ ÁP DỤNG SINGLETON PATTERN =====
        // Xóa cache (khi data thay đổi)
        Task InvalidateCategoryCacheAsync();

        // ===== ĐÃ ÁP DỤNG SINGLETON PATTERN =====
        // Lấy tên pattern được áp dụng
        string GetPatternName();
    }
}
