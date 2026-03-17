using HuflitShop.Data;
using HuflitShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HuflitShop.Services
{
    /// <summary>
    /// ===== SINGLETON PATTERN =====
    /// Implement ICacheService.
    /// Singleton: được đăng ký với AddSingleton() trong DI container
    /// Đảm bảo mỗi request sử dụng cùng 1 instance cache.
    /// 
    /// Lợi ích:
    /// - Giảm truy vấn database (categories ít khi thay đổi)
    /// - Tăng performance lên đáng kể
    /// - Dữ liệu cache chỉ tải 1 lần
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly AppDbContext _context;
        private List<Category> _categoriesCache;
        private DateTime _cacheLoadTime;
        private const int CACHE_DURATION_MINUTES = 60; // Cache tồn tại 60 phút

        public CacheService(AppDbContext context)
        {
            _context = context;
            _categoriesCache = null;
            _cacheLoadTime = DateTime.MinValue;
        }

        // ===== ĐÃ ÁP DỤNG SINGLETON PATTERN =====
        // Lấy categories từ cache, nếu hết hạn thì reload từ DB
        public async Task<List<Category>> GetCategoriesAsync()
        {
            // Nếu cache trống hoặc hết hạn → reload từ database
            if (_categoriesCache == null || IsExpired())
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Cache: Loading categories from database (Singleton)");
                
                _categoriesCache = await Task.Run(() => 
                    _context.Category.ToList()
                );
                
                _cacheLoadTime = DateTime.Now;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Cache: Using cached categories (Singleton)");
            }

            return _categoriesCache;
        }

        // ===== ĐÃ ÁP DỤNG SINGLETON PATTERN =====
        // Xóa cache khi dữ liệu thay đổi
        public async Task InvalidateCategoryCacheAsync()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Cache: Clearing category cache (Singleton)");
            _categoriesCache = null;
            _cacheLoadTime = DateTime.MinValue;
            await Task.CompletedTask;
        }

        // ===== ĐÃ ÁP DỤNG SINGLETON PATTERN =====
        // Kiểm tra cache có hết hạn không
        private bool IsExpired()
        {
            return DateTime.Now - _cacheLoadTime > TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
        }

        // ===== ĐÃ ÁP DỤNG SINGLETON PATTERN =====
        public string GetPatternName()
        {
            return "Singleton Cache Service";
        }
    }
}
