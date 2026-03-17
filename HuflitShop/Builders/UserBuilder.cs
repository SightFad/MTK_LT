using HuflitShop.Models;
using System;

namespace HuflitShop.Builders
{
    /// <summary>
    /// ===== BUILDER PATTERN =====
    /// Builder để tạo AppUser objects với nhiều properties và validations.
    /// Giúp:
    /// - Xây dựng User object từng bước
    /// - Tránh constructor có quá nhiều tham số
    /// - Dễ đọc hơn: userBuilder.WithName(...).WithEmail(...).Build()
    /// - Có validation trước khi trả về object
    /// </summary>
    public class UserBuilder
    {
        private readonly AppUser _user = new AppUser();

        // ===== ĐÃ ÁP DỤNG BUILDER PATTERN =====
        // Thiết lập tên người dùng
        public UserBuilder WithName(string name)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [BUILDER - USER] .WithName({name}) được gọi");
            _user.Name = name;
            return this;
        }

        // ===== ĐÃ ÁP DỤNG BUILDER PATTERN =====
        // Thiết lập email
        public UserBuilder WithEmail(string email)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [BUILDER - USER] .WithEmail({email}) được gọi");
            _user.Email = email;
            return this;
        }

        // ===== ĐÃ ÁP DỤNG BUILDER PATTERN =====
        // Thiết lập số điện thoại
        public UserBuilder WithPhoneNumber(string phoneNumber)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [BUILDER - USER] .WithPhoneNumber({phoneNumber}) được gọi");
            _user.PhoneNumber = phoneNumber;
            return this;
        }

        // ===== ĐÃ ÁP DỤNG BUILDER PATTERN =====
        // Thiết lập giới tính
        public UserBuilder WithGender(string gender)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [BUILDER - USER] .WithGender({gender}) được gọi");
            _user.Gender = gender;
            return this;
        }

        // ===== ĐÃ ÁP DỤNG BUILDER PATTERN =====
        // Thiết lập ngày sinh
        public UserBuilder WithBirthday(DateTime? birthday)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [BUILDER - USER] .WithBirthday({birthday:yyyy-MM-dd}) được gọi");
            _user.Birthday = birthday;
            return this;
        }

        // ===== ĐÃ ÁP DỤNG BUILDER PATTERN =====
        // Thiết lập avatar path
        public UserBuilder WithAvatar(string avatarPath)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [BUILDER - USER] .WithAvatar({avatarPath}) được gọi");
            _user.Avatar = avatarPath;
            return this;
        }

        // ===== ĐÃ ÁP DỤNG BUILDER PATTERN =====
        // Thiết lập UserName
        public UserBuilder WithUserName(string userName)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [BUILDER - USER] .WithUserName({userName}) được gọi");
            _user.UserName = userName;
            return this;
        }

        // ===== ĐÃ ÁP DỤNG BUILDER PATTERN =====
        // Xây dựng xong, trả về AppUser instance sau validation
        public AppUser Build()
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [BUILDER - USER] .Build() được gọi - Bắt đầu xây dựng User object");
            
            // Validation
            if (string.IsNullOrEmpty(_user.UserName))
                throw new InvalidOperationException("UserName phải được thiết lập");

            if (string.IsNullOrEmpty(_user.Email))
                throw new InvalidOperationException("Email phải được thiết lập");

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [BUILDER - USER] User được xây dựng thành công - UserName: {_user.UserName}, Email: {_user.Email}, Name: {_user.Name}");
            return _user;
        }
    }
}
