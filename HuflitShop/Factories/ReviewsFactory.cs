using HuflitShop.Models;
using HuflitShop.ViewModels;
using System;

namespace HuflitShop.Factories
{
    /// <summary>
    /// ===== FACTORY METHOD PATTERN =====
    /// Factory để tạo Reviews objects từ ReviewsViewModel.
    /// 
    /// Giúp:
    /// - Tập trung logic tạo Reviews object ở 1 chỗ
    /// - Validation tự động khi tạo Review
    /// - Dễ mở rộng: thêm loại review mới mà không sửa Controller
    /// - Tránh lặp lại code trong Controller
    /// 
    /// Thay vì trong Controller:
    ///   var reviews = new Reviews();
    ///   reviews.Rate = reviewsModel.Rate;
    ///   reviews.Content = reviewsModel.Content;
    ///   ...
    /// 
    /// Ta gọi Factory:
    ///   var reviews = _reviewsFactory.CreateReview(reviewsModel, userId, productId, imagePath);
    /// </summary>
    public interface IReviewsFactory
    {
        // ===== ĐÃ ÁP DỤNG FACTORY METHOD PATTERN =====
        // Tạo Reviews object từ ReviewsViewModel
        Reviews CreateReview(ReviewsViewModel reviewsModel, string userId, int productId, string imagePath);

        // ===== ĐÃ ÁP DỤNG FACTORY METHOD PATTERN =====
        // Validate Reviews data
        (bool IsValid, string ErrorMessage) ValidateReview(ReviewsViewModel reviewsModel);
    }

    public class ReviewsFactory : IReviewsFactory
    {
        // ===== ĐÃ ÁP DỤNG FACTORY METHOD PATTERN =====
        // Tạo Reviews object với tất cả validations
        public Reviews CreateReview(ReviewsViewModel reviewsModel, string userId, int productId, string imagePath)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FACTORY - REVIEWS] Bắt đầu tạo Reviews object");
            
            // Validation
            var (isValid, errorMessage) = ValidateReview(reviewsModel);
            if (!isValid)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FACTORY - REVIEWS] ❌ Validation failed: {errorMessage}");
                throw new InvalidOperationException($"Reviews validation failed: {errorMessage}");
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FACTORY - REVIEWS] ✓ Validation passed");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FACTORY - REVIEWS]   Rate: {reviewsModel.Rate}/5");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FACTORY - REVIEWS]   Content length: {reviewsModel.Content?.Length ?? 0} chars");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FACTORY - REVIEWS]   Image: {(string.IsNullOrEmpty(imagePath) ? "No image" : imagePath)}");

            // Tạo Reviews object
            var review = new Reviews
            {
                Rate = reviewsModel.Rate,
                Content = reviewsModel.Content,
                SelectedSize = reviewsModel.Size,
                Image = imagePath,
                ProductId = productId,
                UserId = userId
            };

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FACTORY - REVIEWS] ✓ Reviews object được tạo thành công");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FACTORY - REVIEWS]   UserId: {userId}, ProductId: {productId}");
            
            return review;
        }

        // ===== ĐÃ ÁP DỤNG FACTORY METHOD PATTERN =====
        // Validate Reviews data
        public (bool IsValid, string ErrorMessage) ValidateReview(ReviewsViewModel reviewsModel)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FACTORY - REVIEWS] Validating Reviews data...");

            // Kiểm tra Rate
            if (reviewsModel.Rate < 1 || reviewsModel.Rate > 5)
            {
                return (false, $"Rate phải trong khoảng từ 1 đến 5 (nhận được: {reviewsModel.Rate})");
            }

            // Kiểm tra Content
            if (string.IsNullOrWhiteSpace(reviewsModel.Content))
            {
                return (false, "Content không được để trống");
            }

            if (reviewsModel.Content.Length < 10)
            {
                return (false, $"Content phải có ít nhất 10 ký tự (nhận được: {reviewsModel.Content.Length})");
            }

            if (reviewsModel.Content.Length > 1000)
            {
                return (false, $"Content không được vượt quá 1000 ký tự (nhận được: {reviewsModel.Content.Length})");
            }

            // Kiểm tra Size
            if (reviewsModel.Size <= 0)
            {
                return (false, "Size phải được chọn");
            }

            return (true, "");
        }
    }
}
