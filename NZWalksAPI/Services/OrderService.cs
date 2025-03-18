using Hangfire;

namespace NZWalksAPI.Services;

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IRecurringJobManager _recurringJobManager;
    
    [Obsolete("Obsolete")]
    public OrderService(ILogger<OrderService> logger, IRecurringJobManager recurringJobManager)
    {
        _logger = logger;
        _recurringJobManager = recurringJobManager;

        // Đăng ký job chạy tự động ngay trong constructor
        _recurringJobManager.AddOrUpdate(
            "process_orders",
            () => ProcessPendingOrders(),
            Cron.Minutely); // Chạy mỗi 10 phút
    }

    public void ProcessPendingOrders()
    {
        _logger.LogInformation($"🕒 Processing pending orders at {DateTime.Now}");
        // Thêm logic xử lý đơn hàng ở đây
    }
}
