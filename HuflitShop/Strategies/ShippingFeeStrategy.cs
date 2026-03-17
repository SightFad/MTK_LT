using System;
using HuflitShop.Models;
using System.Collections.Generic;

namespace HuflitShop.Strategies
{
    /// <summary>
    /// ===== STRATEGY PATTERN =====
    /// Interface để định nghĩa cách tính phí vận chuyển.
    /// Cho phép chọn strategy khác nhau dựa theo:
    /// - Địa điểm giao hàng (TP, tỉnh, quốc tế)
    /// - Cân nặng/kích cỡ hàng
    /// - Loại dịch vụ (nhanh, thường, ...)
    /// </summary>
    public interface IShippingFeeStrategy
    {
        // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
        // Tính phí vận chuyển dựa theo strategy
        float CalculateShippingFee(Cart[] cartItems, string destination);

        // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
        // Lấy tên strategy
        string GetStrategyName();
    }

    // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
    // Strategy cho vận chuyển trong TP (cố định)
    public class CityShippingStrategy : IShippingFeeStrategy
    {
        public float CalculateShippingFee(Cart[] cartItems, string destination)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] Sử dụng CityShippingStrategy cho destination: {destination}");
            // Phí vận chuyển trong TP luôn là 20,000 VND
            float fee = 20000;
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] Phí vận chuyển tính được: {fee} VND");
            return fee;
        }

        public string GetStrategyName()
        {
            return "CityShipping (20,000 VND)";
        }
    }

    // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
    // Strategy cho vận chuyển tỉnh (tính dựa theo khoảng cách)
    public class ProvinceShippingStrategy : IShippingFeeStrategy
    {
        public float CalculateShippingFee(Cart[] cartItems, string destination)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] Sử dụng ProvinceShippingStrategy cho destination: {destination}");
            // Phí vận chuyển tỉnh: 35,000 VND
            float fee = 35000;
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] Phí vận chuyển tính được: {fee} VND");
            return fee;
        }

        public string GetStrategyName()
        {
            return "ProvinceShipping (35,000 VND)";
        }
    }

    // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
    // Strategy cho vận chuyển quốc tế
    public class InternationalShippingStrategy : IShippingFeeStrategy
    {
        public float CalculateShippingFee(Cart[] cartItems, string destination)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] Sử dụng InternationalShippingStrategy cho destination: {destination}");
            // Phí vận chuyển quốc tế: 100,000 VND
            float fee = 100000;
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] Phí vận chuyển tính được: {fee} VND");
            return fee;
        }

        public string GetStrategyName()
        {
            return "InternationalShipping (100,000 VND)";
        }
    }

    // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
    // Factory để chọn strategy dựa theo destination
    public class ShippingFeeStrategyFactory
    {
        public IShippingFeeStrategy GetStrategy(string destination)
        {
            if (string.IsNullOrEmpty(destination))
                return new CityShippingStrategy();

            destination = destination.ToLower();

            // Nếu là TP HCM, Hà Nội → dùng CityShippingStrategy
            if (destination.Contains("thành phố hồ chí minh") || destination.Contains("hà nội"))
                return new CityShippingStrategy();

            // Nếu là quốc tế → dùng InternationalShippingStrategy
            if (destination.Contains("singapore") || destination.Contains("usa") || destination.Contains("japan"))
                return new InternationalShippingStrategy();

            // Mặc định: ProvinceShippingStrategy
            return new ProvinceShippingStrategy();
        }
    }
}
