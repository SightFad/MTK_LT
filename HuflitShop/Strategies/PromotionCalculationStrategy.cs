using System;

namespace HuflitShop.Strategies
{
    /// <summary>
    /// ===== STRATEGY PATTERN =====
    /// Interface để định nghĩa cách tính promotion/discount.
    /// Cho phép chọn strategy tính giảm giá khác nhau:
    /// - Phần trăm giảm (10%, 20%, ...)
    /// - Giảm cố định (50,000 VND, ...)
    /// - Giảm tiêu chuẩn (mua 3 tặng 1, ...)
    /// </summary>
    public interface IPromotionCalculationStrategy
    {
        // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
        // Tính giá sau khi áp dụng promotion
        float CalculateDiscountedPrice(float originalPrice, float promotionValue);

        // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
        // Lấy tên strategy
        string GetStrategyName();
    }

    // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
    // Strategy: Giảm theo phần trăm (0-1, ví dụ: 0.1 = 10%)
    public class PercentagePromotionStrategy : IPromotionCalculationStrategy
    {
        public float CalculateDiscountedPrice(float originalPrice, float promotionValue)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] PercentagePromotionStrategy: Giá gốc: {originalPrice}, Giảm: {promotionValue * 100}%");
            // promotionValue từ 0-1, ví dụ: 0.1 = 10% off
            // Giá sau giảm = giá gốc * (1 - promotionValue)
            float discountedPrice = originalPrice * (1 - promotionValue);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] Giá sau giảm: {discountedPrice} VND");
            return discountedPrice;
        }

        public string GetStrategyName()
        {
            return "PercentagePromotion";
        }
    }

    // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
    // Strategy: Giảm cố định (VND)
    public class FixedAmountPromotionStrategy : IPromotionCalculationStrategy
    {
        public float CalculateDiscountedPrice(float originalPrice, float promotionValue)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] FixedAmountPromotionStrategy: Giá gốc: {originalPrice}, Giảm: {promotionValue} VND");
            // promotionValue là số tiền cố định (VND)
            // Giá sau giảm = giá gốc - promotionValue
            var discountedPrice = originalPrice - promotionValue;
            if (discountedPrice < 0) discountedPrice = 0;
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] Giá sau giảm: {discountedPrice} VND");
            return discountedPrice;
        }

        public string GetStrategyName()
        {
            return "FixedAmountPromotion";
        }
    }

    // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
    // Strategy: Không có giảm giá
    public class NoPromotionStrategy : IPromotionCalculationStrategy
    {
        public float CalculateDiscountedPrice(float originalPrice, float promotionValue)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [STRATEGY] NoPromotionStrategy: Không áp dụng giảm giá - Giá: {originalPrice} VND");
            // Không giảm gì
            return originalPrice;
        }

        public string GetStrategyName()
        {
            return "NoPromotion";
        }
    }

    // ===== ĐÃ ÁP DỤNG STRATEGY PATTERN =====
    // Factory để chọn promotion strategy
    public class PromotionCalculationStrategyFactory
    {
        public IPromotionCalculationStrategy GetStrategy(string strategyType)
        {
            return strategyType?.ToLower() switch
            {
                "percentage" => new PercentagePromotionStrategy(),
                "fixed" => new FixedAmountPromotionStrategy(),
                "none" => new NoPromotionStrategy(),
                _ => new PercentagePromotionStrategy() // Mặc định: percentage
            };
        }
    }
}
