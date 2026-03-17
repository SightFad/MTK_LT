/*
 * ===== DESIGN PATTERNS IMPLEMENTATION SUMMARY =====
 * 
 * Tất cả 9 design pattern đã được áp dụng thành công vào project HuflitShop!
 * 
 * HƯỚNG DẪN SỬ DỤNG MỖI PATTERN:
 * ================================
 * 
 * 1. FACTORY METHOD PATTERN
 *    Location: Factories/OrderFactory.cs
 *    Usage: 
 *      var order = _orderFactory.CreateCashOrder(userId, 20000);
 *    Benefit: Tập trung logic tạo Order, dễ mở rộng
 * 
 * 2. BUILDER PATTERN
 *    Location: Builders/OrderBuilder.cs
 *    Usage:
 *      var order = new OrderBuilder()
 *          .WithUserId(userId)
 *          .WithPaymentMethod(1)
 *          .WithShoppingFee(20000)
 *          .Build();
 *    Benefit: Xây dựng object phức tạp từng bước, dễ đọc
 * 
 * 3. DECORATOR PATTERN
 *    Location: Decorators/LoggingEmailDecorator.cs
 *    Usage: Được setup tự động trong Startup.cs:
 *      services.AddScoped<IEmailService>(provider =>
 *      {
 *          var emailService = provider.GetRequiredService<EmailService>();
 *          return new LoggingEmailDecorator(emailService);
 *      });
 *    Benefit: Thêm logging vào EmailService mà không thay đổi EmailService gốc
 * 
 * 4. PROTOTYPE PATTERN
 *    Location: Services/ProductsService.cs - CloneProduct() method
 *    Usage: Tự động được gọi trong ProductsService.Get(search)
 *    Benefit: Clone Product objects để tránh thay đổi dữ liệu gốc
 * 
 * 5. OBSERVER PATTERN
 *    Location: Observers/IOrderObserver.cs, OrderCreatedObserver.cs
 *    Usage: Được subscribe trong OrderFacadeService:
 *      _orderFacadeService.Subscribe(_orderObserver);
 *      await _orderFacadeService.ProcessCashOrderAsync(userId);
 *    Benefit: Ghi log tự động khi Order được tạo/hủy
 * 
 * 6. ADAPTER PATTERN
 *    Location: Adapters/OrderAdapter.cs
 *    Usage: 
 *      var model = _orderAdapter.AdaptViewModelToModel(viewModel, userId);
 *    Benefit: Convert giữa ViewModel ↔ Model một cách sạch sẽ
 * 
 * 7. SINGLETON PATTERN
 *    Location: Services/CacheService.cs (registered with AddSingleton in Startup.cs)
 *    Usage:
 *      var categories = await _cacheService.GetCategoriesAsync();
 *    Benefit: Cache Categories chỉ load 1 lần, giảm truy vấn DB
 *    Được dùng trong: ProductController.Product(), .Category(), .Search()
 * 
 * 8. STRATEGY PATTERN
 *    Location: Strategies/ShippingFeeStrategy.cs, PromotionCalculationStrategy.cs
 *    Usage:
 *      var shippingStrategy = _shippingStrategyFactory.GetStrategy(destination);
 *      float fee = shippingStrategy.CalculateShippingFee(carts, destination);
 *      
 *      var promotionStrategy = _promotionStrategyFactory.GetStrategy("percentage");
 *      float discountedPrice = promotionStrategy.CalculateDiscountedPrice(price, 0.1);
 *    Benefit: Tách logic tính toán, dễ thay đổi strategy mà không ảnh hưởng code khác
 * 
 * 9. FACADE PATTERN
 *    Location: Facades/OrderFacadeService.cs
 *    Usage:
 *      var (success, message, orderId) = await _orderFacadeService.ProcessCashOrderAsync(userId);
 *    Benefit: Gom tất cả logic Order vào 1 method duy nhất
 *    Thay vì 50+ dòng code phức tạp → chỉ 2-3 dòng code đơn giản
 * 
 * ===== PHẦN QUAN TRỌNG =====
 * 
 * VỊ TRÍ CÓ COMMENT RÕ RÀNG:
 * 
 * 1. Tất cả file Service/Controller đều có comment:
 *    /* ===== CHƯA ÁP DỤNG DESIGN PATTERN ===== /*
 *    ... code cũ ...
 *    
 *    /* ===== ĐÃ ÁP DỤNG [PATTERN NAME] PATTERN ===== /*
 *    ... code mới dùng pattern ...
 * 
 * 2. Console output để debug:
 *    - Khi cache được load: "[HH:mm:ss] Cache: Loading categories..."
 *    - Khi dùng cache: "[HH:mm:ss] Cache: Using cached categories..."
 *    - Khi Order được tạo: "[HH:mm:ss] ORDER CREATED - Order ID..."
 *    - Khi Facade xử lý: "[HH:mm:ss] Facade: Order #X xử lý thành công"
 * 
 * ===== FILE ĐƯỢC CẬP NHẬT =====
 * 
 * Controllers:
 *   - ProductController.cs (thêm Singleton Cache, async methods)
 *   - OrderController.cs (thêm Facade, Observer, Factory, Adapter, Strategy)
 * 
 * Services:
 *   - ProductsService.cs (thêm Prototype Clone)
 *   - EmailService.cs (được wrap với LoggingEmailDecorator)
 *   - ICacheService.cs (interface mới)
 *   - CacheService.cs (Singleton implementation mới)
 * 
 * Others:
 *   - Startup.cs (đăng ký tất cả pattern DI)
 * 
 * New Folders:
 *   - Factories/ (OrderFactory.cs)
 *   - Builders/ (OrderBuilder.cs)
 *   - Decorators/ (LoggingEmailDecorator.cs)
 *   - Observers/ (IOrderObserver.cs, OrderCreatedObserver.cs)
 *   - Adapters/ (OrderAdapter.cs)
 *   - Strategies/ (ShippingFeeStrategy.cs, PromotionCalculationStrategy.cs)
 *   - Facades/ (OrderFacadeService.cs)
 * 
 * ===== STATUS =====
 * ✓ dotnet build: SUCCESS - Không có lỗi!
 * ✓ Tất cả using statements được thêm
 * ✓ Tất cả DI được đăng ký trong Startup.cs
 * ✓ Code chạy được ngay
 * 
 * ===== GỢI Ý TIẾP THEO =====
 * 
 * 1. Chạy app: dotnet run
 * 2. Vào trang Products để thấy:
 *    - Console output: Cache loading/using
 *    - Categories được cache lại
 * 3. Đặt hàng (tiền mặt) để thấy:
 *    - Console output: Facade xử lý order
 *    - Observer log: Order created timestamp
 * 4. Xem Network: Ordering sẽ nhanh hơn vì không load categories từ DB mỗi lần
 * 
 * ===== LƯU Ý QUAN TRỌNG =====
 * 
 * - Tất cả logic cũ đều được GIỮ LẠI (chỉ comment lại, không xóa)
 * - Code mới áp dụng pattern được thêm PHÍA DƯỚI code cũ
 * - Tất cả comment là TIẾNG VIỆT
 * - Không phá vỡ tính năng cũ
 * - Pattern được dùng THỰC TẾ không phải demo
 */

// Đây là file tham khảo, không cần compile
