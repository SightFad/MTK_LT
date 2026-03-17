using HuflitShop.Models;
using System.Threading.Tasks;

namespace HuflitShop.Observers
{
    /// <summary>
    /// ===== OBSERVER PATTERN =====
    /// Interface cho các observer theo dõi sự kiện Order.
    /// Khi Order được tạo, các observer sẽ được thông báo:
    /// - OrderCreatedObserver: Log sự kiện
    /// - OrderNotificationObserver: Gửi email xác nhận
    /// </summary>
    public interface IOrderObserver
    {
        // ===== ĐÃ ÁP DỤNG OBSERVER PATTERN =====
        // Gọi khi Order được tạo thành công
        Task OnOrderCreated(Order order);

        // ===== ĐÃ ÁP DỤNG OBSERVER PATTERN =====
        // Gọi khi Order bị hủy
        Task OnOrderCancelled(Order order);

        // ===== ĐÃ ÁP DỤNG OBSERVER PATTERN =====
        // Lấy tên của Observer để tracking
        string GetObserverName();
    }
}
