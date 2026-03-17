using HuflitShop.Models;
using HuflitShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HuflitShop.Adapters
{
    /// <summary>
    /// ===== ADAPTER PATTERN =====
    /// Adapter để chuyển đổi dữ liệu giữa Chat Models và ViewModels.
    /// 
    /// Giúp:
    /// - Tách biệt Model khỏi ViewModel
    /// - Tập trung logic mapping ở 1 chỗ
    /// - Dễ thay đổi ViewModel mà không ảnh hưởng tới Model
    /// - Controller code ngắn gọn hơn
    /// 
    /// Chuyển đổi:
    /// Chat + Message + ResponseMessage (Models) → ChatViewModel
    /// RequestMessageViewModel (ViewModel) → Chat (Model)
    /// </summary>
    public interface IChatAdapter
    {
        // ===== ĐÃ ÁP DỤNG ADAPTER PATTERN =====
        // Convert Chat + Message + ResponseMessages Models thành ChatViewModel
        ChatViewModel AdaptChatToViewModel(Chat chat, Message message, List<ResponseMessage> responseMessages, AppUser user);

        // ===== ĐÃ ÁP DỤNG ADAPTER PATTERN =====
        // Convert RequestMessageViewModel thành Chat Model
        Chat AdaptRequestToChat(RequestMessageViewModel requestView, string userId, int messageId, string request);

        // ===== ĐÃ ÁP DỤNG ADAPTER PATTERN =====
        // Convert ResponseMessage Model thành ResponseMessageViewModel
        ResponseMessageViewModel AdaptResponseMessageToViewModel(ResponseMessage responseMessage);
    }

    public class ChatAdapter : IChatAdapter
    {
        // ===== ĐÃ ÁP DỤNG ADAPTER PATTERN =====
        // Chuyển Chat + Message + ResponseMessages thành ChatViewModel
        public ChatViewModel AdaptChatToViewModel(Chat chat, Message message, List<ResponseMessage> responseMessages, AppUser user)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER] Chuyển đổi Chat Models → ChatViewModel");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER]   ChatId: {chat.Id}, MessageId: {message.Id}");

            // Tạo danh sách responses
            var responses = new List<ResponseMessageViewModel>();

            // Thêm response từ chatbot (từ Message model)
            if (!string.IsNullOrEmpty(message.ResponseMessage))
            {
                responses.Add(new ResponseMessageViewModel
                {
                    CreatedAt = chat.CreatedAt,
                    Message = message.ResponseMessage
                });
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER]   ✓ Thêm response từ chatbot");
            }

            // Thêm responses từ admin (từ ResponseMessage models)
            if (responseMessages != null && responseMessages.Count > 0)
            {
                foreach (var res in responseMessages)
                {
                    responses.Add(AdaptResponseMessageToViewModel(res));
                }
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER]   ✓ Thêm {responseMessages.Count} responses từ admin");
            }

            // Tạo ChatViewModel
            var chatViewModel = new ChatViewModel
            {
                CreatedAt = chat.CreatedAt,
                UserName = user?.UserName ?? "Unknown",
                Request = chat.Request,
                Responses = responses
            };

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER] ✓ ChatViewModel được tạo thành công - {responses.Count} responses");
            return chatViewModel;
        }

        // ===== ĐÃ ÁP DỤNG ADAPTER PATTERN =====
        // Chuyển RequestMessageViewModel thành Chat Model
        public Chat AdaptRequestToChat(RequestMessageViewModel requestView, string userId, int messageId, string request)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER] Chuyển đổi RequestMessageViewModel → Chat Model");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER]   UserId: {userId}, MessageId: {messageId}");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER]   Request: {request?.Substring(0, Math.Min(30, request?.Length ?? 0))}...");

            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(request))
                throw new InvalidOperationException("Request message không được để trống");

            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("UserId không được để trống");

            if (messageId <= 0)
                throw new InvalidOperationException("MessageId phải lớn hơn 0");

            // Tạo Chat Model
            var chat = new Chat
            {
                UserId = userId,
                MessageId = messageId,
                Request = request,
                CreatedAt = DateTime.Now
            };

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER] ✓ Chat Model được tạo thành công");
            return chat;
        }

        // ===== ĐÃ ÁP DỤNG ADAPTER PATTERN =====
        // Chuyển ResponseMessage Model thành ResponseMessageViewModel
        public ResponseMessageViewModel AdaptResponseMessageToViewModel(ResponseMessage responseMessage)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER] Chuyển đổi ResponseMessage → ResponseMessageViewModel");
            
            var viewModel = new ResponseMessageViewModel
            {
                CreatedAt = responseMessage.CreatedAt,
                Message = responseMessage.Response
            };

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ADAPTER] ✓ ResponseMessageViewModel được tạo thành công");
            return viewModel;
        }
    }
}
