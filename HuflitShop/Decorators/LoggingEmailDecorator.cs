using HuflitShop.Services;
using HuflitShop.ViewModels;
using System;
using System.Threading.Tasks;

namespace HuflitShop.Decorators
{
    /// <summary>
    /// ===== DECORATOR PATTERN =====
    /// Decorator để thêm tính năng logging vào EmailService.
    /// Thay vì sửa EmailService trực tiếp, ta wrap nó với LoggingEmailDecorator:
    /// - Log mỗi khi gửi email (ai, khi nào, đến ai, status)
    /// - Vẫn giữ nguyên EmailService cũ
    /// - Dễ thêm/bỏ tính năng logging
    /// </summary>
    public class LoggingEmailDecorator : IEmailService
    {
        private readonly IEmailService _innerEmailService;

        public LoggingEmailDecorator(IEmailService innerEmailService)
        {
            _innerEmailService = innerEmailService;
        }

        // ===== ĐÃ ÁP DỤNG DECORATOR PATTERN =====
        // Wrap SendTestEmail() với logging
        public async Task SendTestEmail(UserEmailOptions userEmailOptions)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [DecoratorLogging]: Gửi Test Email tới: {string.Join(", ", userEmailOptions.ToEmails)}");
            
            await _innerEmailService.SendTestEmail(userEmailOptions);
            
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [DecoratorLogging]: Test Email đã gửi thành công!");
        }

        // ===== ĐÃ ÁP DỤNG DECORATOR PATTERN =====
        // Wrap SendEmailForEmailConfirmation() với logging
        public async Task SendEmailForEmailConfirmation(UserEmailOptions userEmailOptions)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [DecoratorLogging]: Gửi Email Confirmation tới: {string.Join(", ", userEmailOptions.ToEmails)}");
            
            await _innerEmailService.SendEmailForEmailConfirmation(userEmailOptions);
            
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [DecoratorLogging]: Email Confirmation đã gửi thành công!");
        }

        // ===== ĐÃ ÁP DỤNG DECORATOR PATTERN =====
        // Wrap SendEmailForForgotPassword() với logging
        public async Task SendEmailForForgotPassword(UserEmailOptions userEmailOptions)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [DecoratorLogging]: Gửi Email Forgot Password tới: {string.Join(", ", userEmailOptions.ToEmails)}");
            
            await _innerEmailService.SendEmailForForgotPassword(userEmailOptions);
            
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [DecoratorLogging]: Email Forgot Password đã gửi thành công!");
        }

    }
}
