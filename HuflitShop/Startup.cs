using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Identity.Core;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using HuflitShop.Models;
using HuflitShop.Data;
using HuflitShop.Repository;
using HuflitShop.Services;
using HuflitShop.Helpers;
using HuflitShop.ViewModels;
using Microsoft.AspNetCore.Authentication;
// ===== ĐÃ ÁP DỤNG DESIGN PATTERN - Thêm using cho các pattern =====
using HuflitShop.Factories;
using HuflitShop.Decorators;
using HuflitShop.Adapters;
using HuflitShop.Facades;
using HuflitShop.Observers;
using HuflitShop.Prototypes;
using HuflitShop.Strategies;

namespace HuflitShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private readonly IConfiguration Configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();           // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
            services.AddSession(cfg => {                    // Đăng ký dịch vụ Session
                cfg.Cookie.Name = "HUFLIT SHOP";             // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
                cfg.IdleTimeout = new TimeSpan(0, 30, 0);    // Thời gian tồn tại của Session
            });

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(5);
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });


            // Đăng ký AppDbContext
            services.AddDbContext<AppDbContext>(options => {
                // Đọc chuỗi kết nối
                string connectstring = Configuration.GetConnectionString("AppDbContext");
                // Sử dụng MS SQL Server
                options.UseSqlServer(connectstring);
            });

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

#if DEBUG
            services.AddRazorPages().AddRazorRuntimeCompilation();
#endif

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 6; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

            });

            // Cấu hình đăng nhập từ facebook, google

            services.AddAuthentication()

                .AddGoogle(googleOptions =>
                {
                    // Đọc thông tin Authentication:Google từ appsettings.json
                    IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");

                    // Thiết lập ClientID và ClientSecret để truy cập API google
                    googleOptions.ClientId = googleAuthNSection["ClientId"];
                    googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
                    googleOptions.ClaimActions.MapJsonKey("image", "picture");
                    // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
                    googleOptions.CallbackPath = "/dang-nhap-tu-google";

                })
                .AddFacebook(facebookOptions =>
                {
                    // Đọc cấu hình
                    IConfigurationSection facebookAuthNSection = Configuration.GetSection("Authentication:Facebook");
                    facebookOptions.AppId = facebookAuthNSection["AppId"];
                    facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];
                    facebookOptions.ClaimActions.MapJsonKey("image", "picture");
                    // Thiết lập đường dẫn Facebook chuyển hướng đến
                    facebookOptions.CallbackPath = "/dang-nhap-tu-facebook";
                });

            // Cấu hình Cookie
            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = Configuration["Application:LoginPath"];
            });

            services.Configure<SecurityStampValidatorOptions>(options => {
                // Trên 5 giây truy cập lại sẽ nạp lại thông tin User (Role)
                // SecurityStamp trong bảng User đổi -> nạp lại thông tinn Security
                options.ValidationInterval = TimeSpan.FromSeconds(5);
            });

            services.Configure<RouteOptions>(options => {
                options.AppendTrailingSlash = false; // Thêm dấu / vào cuối URL
                options.LowercaseUrls = true; // url chữ thường
                options.LowercaseQueryStrings = false; // không bắt query trong url phải in thường
            });

            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, AppUserClaimsPrincipalFactory>();
            services.AddScoped<IAvatarSevice, AvatarService>();

            /* ===== ĐÃ ÁP DỤNG DESIGN PATTERN =====
            Đăng ký tất cả các Design Pattern services vào Dependency Injection container.
            */

            // ===== FACTORY METHOD PATTERN =====
            // ===== ĐÃ ÁP DỤNG FACTORY METHOD PATTERN - ReviewsFactory =====
            services.AddScoped<IReviewsFactory, ReviewsFactory>();

            // ===== DECORATOR PATTERN =====
            // Wrap EmailService với LoggingEmailDecorator
            services.AddScoped<EmailService>(); // Đăng ký EmailService gốc
            services.AddScoped<IEmailService>(provider =>
            {
                var emailService = provider.GetRequiredService<EmailService>();
                return new LoggingEmailDecorator(emailService);
            });

            // ===== ĐÃ ÁP DỤNG ADAPTER PATTERN - ChatAdapter =====
            services.AddScoped<IChatAdapter, ChatAdapter>();

            // ===== SINGLETON PATTERN =====
            // Singleton: chỉ tạo 1 instance duy nhất cho toàn bộ app
            services.AddSingleton<ICacheService, CacheService>();

            // ===== FACADE PATTERN =====
            services.AddScoped<IOrderFacadeService, OrderFacadeService>();

            // ===== OBSERVER PATTERN =====
            // Đăng ký OrderCreatedObserver
            services.AddScoped<IOrderObserver, OrderCreatedObserver>();

            // ===== STRATEGY PATTERN =====
            // Đăng ký factories cho các strategy
            services.AddScoped<ShippingFeeStrategyFactory>();
            services.AddScoped<PromotionCalculationStrategyFactory>();

            // ===== BUILDER PATTERN =====
            // OrderBuilder không cần đăng ký vì nó được tạo khi cần

            // ===== PROTOTYPE PATTERN =====
            // ProductsService đã được update để áp dụng Prototype Pattern
            services.AddScoped<ProductsService>();
            // ===== ĐÃ ÁP DỤNG PROTOTYPE PATTERN - ProductPrototype =====
            services.AddScoped<IProductPrototype, ProductPrototype>();

            services.Configure<SMTPConfigModel>(Configuration.GetSection("SMTPConfig")); // đăng ký để Inject 
                                                                                         // Configuration.GetSection("SMTPConfig") : đọc config 

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();   // Phục hồi thông tin đăng nhập (xác thực)
            app.UseAuthorization();   // Phục hồi thông tin về quyền của User

            app.UseSession();


            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                  name: "Admin",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

               
            });
        }
    }
}
