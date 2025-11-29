using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        
        // 1. GET: Hiển thị trang thanh toán
        
        public ActionResult Index()
        {
            // Lấy giỏ hàng từ Session
            List<OrderDetail> cart = Session["Cart"] as List<OrderDetail>;

            // Nếu giỏ hàng rỗng -> Đá về trang giỏ hàng
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "MyCart");
            }

            // Kiểm tra đăng nhập
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "CheckOut") });
            }

            // Lấy thông tin khách hàng để điền sẵn vào form
            int userId = (int)Session["UserID"];
            var customer = db.Customers.Find(userId);
            ViewBag.Customer = customer;

            return View(cart);
        }

        
        // 2. POST: XỬ LÝ ĐẶT HÀNG
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(string ShippingAddress, string Notes)
        {
            // A. Kiểm tra điều kiện cơ bản
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            List<OrderDetail> cart = Session["Cart"] as List<OrderDetail>;
            if (cart == null || cart.Count == 0) return RedirectToAction("Index", "Home");

            // B. Sử dụng Transaction để đảm bảo dữ liệu nhất quán (Lưu cả Đơn và Chi tiết cùng lúc)
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // --- BƯỚC 1: TẠO ĐƠN HÀNG (ORDER HEAD) ---
                    Order newOrder = new Order();
                    newOrder.CustomerID = (int)Session["UserID"];
                    newOrder.OrderDate = DateTime.Now;

                    // Tính tổng tiền
                    newOrder.TotalAmount = cart.Sum(x => x.UnitPrice * x.Quantity);

                    // Gộp Địa chỉ giao hàng vào Ghi chú (Vì Model Order không có cột Address riêng)
                    // Format: "Ghi chú khách nhập - Đ/C: Địa chỉ giao hàng"
                    string finalNote = "";
                    if (!string.IsNullOrEmpty(Notes)) finalNote += Notes;
                    if (!string.IsNullOrEmpty(ShippingAddress)) finalNote += " | Đ/C Giao: " + ShippingAddress;

                    newOrder.Note = finalNote;

                    // Thiết lập các trạng thái mặc định (False)
                    newOrder.Dispatched = false; // Chưa xác nhận
                    newOrder.Shipped = false;    // Chưa giao
                    newOrder.Deliver = false;    // Chưa nhận
                    newOrder.isCompleted = false; // Chưa hoàn tất

                    
                    newOrder.CancelOrder = false;

                    db.Orders.Add(newOrder);
                    db.SaveChanges(); // Lưu lệnh này trước để sinh ra OrderID

                    // --- BƯỚC 2: LƯU CHI TIẾT ĐƠN HÀNG (ORDER DETAILS) ---
                    foreach (var item in cart)
                    {
                        OrderDetail detail = new OrderDetail();
                        detail.OrderID = newOrder.OrderID; // Lấy ID vừa sinh ra ở trên
                        detail.ProductID = item.ProductID;
                        detail.UnitPrice = item.UnitPrice;
                        detail.Quantity = item.Quantity;

                        // Nếu Model OrderDetail có cột TotalAmount thì tính luôn, không thì bỏ qua
                        // detail.TotalAmount = item.UnitPrice * item.Quantity;

                        db.OrderDetails.Add(detail);
                    }

                    db.SaveChanges(); // Lưu toàn bộ chi tiết sản phẩm

                    // --- BƯỚC 3: HOÀN TẤT ---
                    transaction.Commit(); // Xác nhận lưu vào SQL

                    // Xóa giỏ hàng
                    Session["Cart"] = null;
                    Session["NoOfItem"] = 0;

                    // Chuyển hướng
                    TempData["Success"] = "Đặt hàng thành công! Mã đơn hàng của bạn là #" + newOrder.OrderID;
                    return RedirectToAction("OrderSuccess");
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Nếu lỗi thì hoàn tác toàn bộ
                    TempData["Error"] = "Có lỗi xảy ra khi đặt hàng: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
        }

        // 3. GET: Trang thông báo thành công
        public ActionResult OrderSuccess()
        {
            return View();
        }
    }
}