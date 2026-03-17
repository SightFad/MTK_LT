using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HuflitShop.Data;
using HuflitShop.Models;
using HuflitShop.ViewModels;
// ===== ĐÃ ÁP DỤNG DESIGN PATTERN - Factory Pattern =====
using HuflitShop.Factories;

namespace HuflitShop.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ILogger<ReviewsController> _logger;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private IHostingEnvironment _hostingEnvironment;
        // ===== ĐÃ ÁP DỤNG DESIGN PATTERN - Factory Pattern =====
        private readonly IReviewsFactory _reviewsFactory;

        public ReviewsController(ILogger<ReviewsController> logger, AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IHostingEnvironment hostingEnvironment, IReviewsFactory reviewsFactory)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            // ===== ĐÃ ÁP DỤNG DESIGN PATTERN - Factory Pattern =====
            _reviewsFactory = reviewsFactory;
        }

        // GET: ReviewsController/Create
        [HttpGet]
        [Authorize]
        [Route("reviews", Name = "reviews")]
        public IActionResult Create(int id)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            AppUser user = _userManager.FindByIdAsync(userid).Result;
            var product = _context.Product.Where(p => p.Id == id).First();
            var image = _context.Image.Where(i => i.ProductId == id).First();
            var productSize = _context.ProductSize.Where(p => p.ProductId == id).ToList();
            List<Size> sizes = new List<Size>();
            foreach (var item in productSize)
            {
                var qr = _context.Sizes.Where(s => s.Id == item.SizeId).First();
                sizes.Add(qr);
            }
            var reviews = new ReviewsViewModel();
            reviews.UserId = userid;
            reviews.Avatar = user.Avatar;
            reviews.ProductId = product.Id;
            reviews.ProductName = product.Name;
            reviews.ImageProduct = image.Path;
            reviews.Sizes = sizes;
            return View(reviews);
        }

        // POST: ReviewsController/Create
        [HttpPost]
        [Authorize]
        [Route("reviews", Name = "reviews")]
        public async Task<ActionResult> Create(int id, ReviewsViewModel reviewsModel)
        {
            /* ===== CHƯA ÁP DỤNG DESIGN PATTERN =====
            // Cách cũ: Tạo Reviews object bằng tay, set từng property
            var reviews = new Reviews();
            reviews.Rate = reviewsModel.Rate;
            reviews.Content = reviewsModel.Content;
            reviews.SelectedSize = reviewsModel.Size;
            reviews.Image = imagePath;
            reviews.ProductId = id;
            reviews.UserId = userid;
            */

            try
            {
                string fileName = "";
                if(reviewsModel.Image != null)
                {
                    string wwwRootPath = _hostingEnvironment.WebRootPath;
                    fileName = Path.GetFileNameWithoutExtension(reviewsModel.Image.FileName);
                    string extension = Path.GetExtension(reviewsModel.Image.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string filePath = Path.Combine(wwwRootPath + "/img/reviews/", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await reviewsModel.Image.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    fileName = "";
                }
                
                string imagePath = (fileName == "") ? "" : "/img/reviews/" + fileName;
                var userid = _userManager.GetUserId(HttpContext.User);

                // ===== ĐÃ ÁP DỤNG FACTORY METHOD PATTERN =====
                // Sử dụng Factory để tạo Reviews object thay vì new Reviews()
                // Factory sẽ:
                // - Validate tất cả fields (Rate 1-5, Content 10-1000 chars, Size > 0)
                // - Create Reviews object với tất cả properties
                // - Log toàn bộ quá trình
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - REVIEWS] Bắt đầu tạo Review qua Factory");
                
                var reviews = _reviewsFactory.CreateReview(reviewsModel, userid, id, imagePath);
                
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - REVIEWS] Factory đã tạo review thành công, lưu vào database");

                _context.Add(reviews);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - REVIEWS] Review #{reviews.Id} được lưu thành công");
                return RedirectToAction("Details", "Product", new { id = reviews.ProductId });
            }
            catch (InvalidOperationException ex)
            {
                // Factory validation failed - Populate Sizes trước khi return View
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - REVIEWS] ❌ Validation error: {ex.Message}");
                
                // Lấy product info và populate Sizes để view có thể render được dropdown
                var product = _context.Product.Where(p => p.Id == id).FirstOrDefault();
                if (product != null)
                {
                    var productSize = _context.ProductSize.Where(p => p.ProductId == id).ToList();
                    List<Size> sizes = new List<Size>();
                    foreach (var item in productSize)
                    {
                        var qr = _context.Sizes.Where(s => s.Id == item.SizeId).FirstOrDefault();
                        if (qr != null)
                            sizes.Add(qr);
                    }
                    reviewsModel.Sizes = sizes;
                }
                
                ViewBag.Error = ex.Message;
                return View(reviewsModel);
            }
            catch (Exception ex)
            {
                // Error other than validation - Populate Sizes trước khi return View
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - REVIEWS] ❌ Error: {ex.Message}");
                
                // Lấy product info và populate Sizes để view có thể render được dropdown
                var product = _context.Product.Where(p => p.Id == id).FirstOrDefault();
                if (product != null)
                {
                    var productSize = _context.ProductSize.Where(p => p.ProductId == id).ToList();
                    List<Size> sizes = new List<Size>();
                    foreach (var item in productSize)
                    {
                        var qr = _context.Sizes.Where(s => s.Id == item.SizeId).FirstOrDefault();
                        if (qr != null)
                            sizes.Add(qr);
                    }
                    reviewsModel.Sizes = sizes;
                }
                
                ViewBag.Error = "Có lỗi khi tạo review";
                return View(reviewsModel);
            }
        }
    }
}
