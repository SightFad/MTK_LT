using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuflitShop.Data;
using HuflitShop.Models;
using HuflitShop.ViewModels;
// ===== ĐÃ ÁP DỤNG DESIGN PATTERN - Adapter Pattern =====
using HuflitShop.Adapters;

namespace HuflitShop.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        // ===== ĐÃ ÁP DỤNG DESIGN PATTERN - Adapter Pattern =====
        private readonly IChatAdapter _chatAdapter;

        public ChatController(ILogger<ChatController> logger, AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IChatAdapter chatAdapter)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            // ===== ĐÃ ÁP DỤNG DESIGN PATTERN - Adapter Pattern =====
            _chatAdapter = chatAdapter;
        }

        [Authorize]
        [HttpGet("/chat")]
        public ActionResult Chat()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var chats = _context.Chats.Where(c => c.UserId.Contains(userId)).ToList();

            ViewBag.Start = _context.Messages.FirstOrDefault(m => m.RequestMessage.ToLower().Contains("Bắt đầu"))?.ResponseMessage;

            if (chats.Count == 0)
            {
                ViewBag.HasChat = false;
                return View();
            }
            else
            {
                ViewBag.HasChat = true;
                List<ChatViewModel> chatViews = new List<ChatViewModel>();
                
                foreach(var item in chats)
                {
                    /* ===== CHƯA ÁP DỤNG DESIGN PATTERN =====
                    // Cách cũ: Mapping thủ công, lặp lại code
                    var message = _context.Messages.FirstOrDefault(m => m.Id == item.MessageId);
                    var chat = _context.Chats.FirstOrDefault(c => c.Id == item.Id);
                    
                    List<ResponseMessageViewModel> responses = new List<ResponseMessageViewModel>();
                    responses.Add(new ResponseMessageViewModel
                    {
                        CreatedAt = chat.CreatedAt,
                        Message = message.ResponseMessage
                    });
                    ...
                    */

                    // ===== ĐÃ ÁP DỤNG ADAPTER PATTERN =====
                    // Sử dụng Adapter để convert Models → ViewModel
                    var message = _context.Messages.FirstOrDefault(m => m.Id == item.MessageId);
                    if (message == null) continue;

                    var chat = _context.Chats.FirstOrDefault(c => c.Id == item.Id);
                    if (chat == null) continue;

                    var responseMessages = _context.ResponseMessages.Where(r => r.ChatId == chat.Id).ToList();
                    var user = _context.Users.FirstOrDefault(u => u.Id.Contains(item.UserId));

                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - CHAT] Bắt đầu convert Chat → ViewModel qua Adapter");
                    
                    var chatView = _chatAdapter.AdaptChatToViewModel(chat, message, responseMessages, user);
                    
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - CHAT] Adapter đã convert thành công");
                    chatViews.Add(chatView);
                }
                
                ViewBag.Chat = chatViews;
                return View();
            }  
        }

        [Authorize]
        [HttpPost("/chat")]
        public async Task<IActionResult> Chat(RequestMessageViewModel request)
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);

                // Tìm message khớp
                var message = await _context.Messages
                    .FirstOrDefaultAsync(m => request.Message.ToLower().Contains(m.RequestMessage.ToLower()));

                // Nếu không tìm được, dùng message "Khác"
                if (message == default)
                {
                    message = await _context.Messages
                        .FirstOrDefaultAsync(m => m.RequestMessage.ToLower().Contains("Khác"));
                }

                if (message == null)
                {
                    ViewBag.Message = "Không tìm được câu trả lời phù hợp.";
                    return RedirectToAction(nameof(Chat));
                }

                /* ===== CHƯA ÁP DỤNG DESIGN PATTERN =====
                // Cách cũ: Tạo Chat object bằng tay
                var chat = new Chat()
                {
                    UserId = userId,
                    MessageId = message.Id,
                    Request = request.Message
                };
                */

                // ===== ĐÃ ÁP DỤNG ADAPTER PATTERN =====
                // Sử dụng Adapter để convert RequestMessageViewModel → Chat Model
                // Adapter sẽ validate dữ liệu trước khi tạo
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - CHAT] Bắt đầu convert RequestMessageViewModel → Chat Model qua Adapter");
                
                var chat = _chatAdapter.AdaptRequestToChat(request, userId, message.Id, request.Message);
                
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - CHAT] Adapter đã convert thành công");

                await _context.Chats.AddAsync(chat);
                var result = await _context.SaveChangesAsync();

                if (result == 1)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - CHAT] Chat #{chat.Id} được lưu thành công");
                    ViewBag.Message = "";
                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - CHAT] ❌ Lỗi lưu database");
                    ViewBag.Message = "Hệ thống đang xảy ra sự cố, vui lòng thử lại.";
                }

                return RedirectToAction(nameof(Chat));
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - CHAT] ❌ Adapter validation error: {ex.Message}");
                ViewBag.Message = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Chat));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [CONTROLLER - CHAT] ❌ Error: {ex.Message}");
                ViewBag.Message = "Hệ thống đang xảy ra sự cố, vui lòng thử lại.";
                return RedirectToAction(nameof(Chat));
            }
        }
    }
}
