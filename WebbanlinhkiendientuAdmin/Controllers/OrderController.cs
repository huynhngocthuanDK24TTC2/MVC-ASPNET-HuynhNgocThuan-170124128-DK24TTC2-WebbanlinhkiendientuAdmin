using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Data.Entity; // Bắt buộc để dùng .Include()
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class OrderController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        // --- Kiểm tra đăng nhập ---
        private bool IsLoggedIn()
        {
            return Session["AdminUser"] != null || Session["EmpID"] != null;
        }

        // ==========================================
        // 1. GET: Danh sách đơn hàng
        // ==========================================
        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            // Load kèm thông tin Khách hàng và Thanh toán để hiển thị lên bảng
            var orders = db.Orders
                           .Include(o => o.Customer)
                           .Include(o => o.Payment)
                           .Include(o => o.Payment.PaymentType)
                           .OrderByDescending(x => x.OrderID) // Mới nhất lên đầu
                           .ToList();

            return View(orders);
        }

        
        // 2. GET: Chi tiết đơn hàng 
        
        public ActionResult Details(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Lấy đơn hàng + Chi tiết sản phẩm (Load sâu vào trong)
            Order order = db.Orders
                          .Include(o => o.Customer)
                          .Include(o => o.Payment.PaymentType)
                          .Include(o => o.OrderDetails) // Load bảng con chi tiết
                          .Include("OrderDetails.Product") // Load tên sản phẩm trong chi tiết
                          .FirstOrDefault(x => x.OrderID == id);

            if (order == null) return HttpNotFound();

            // Trả về đúng Model 'Order' để khớp với View Details.cshtml
            return View(order);
        }

        
        // 3. CÁC CHỨC NĂNG XỬ LÝ TRẠNG THÁI
        

        // BƯỚC 1: Xác nhận đơn hàng (Mới -> Đã duyệt/Đóng gói)
        public ActionResult ConfirmOrder(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            var order = db.Orders.Find(id);
            if (order != null)
            {
                order.Dispatched = true; // Đánh dấu là Đã duyệt
                db.SaveChanges();
                TempData["SuccessMessage"] = "Đã duyệt đơn hàng #" + id;
            }
            return RedirectToAction("Details", new { id = id });
        }

        // BƯỚC 2: Bắt đầu giao hàng (Đã duyệt -> Đang vận chuyển)
        public ActionResult StartShip(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            var order = db.Orders.Find(id);
            if (order != null)
            {
                order.Shipped = true; // Đánh dấu là Đang giao
                order.ShippingDate = DateTime.Now; // Lưu ngày giao
                db.SaveChanges();
                TempData["SuccessMessage"] = "Đơn hàng #" + id + " đang được vận chuyển.";
            }
            return RedirectToAction("Details", new { id = id });
        }

        // BƯỚC 3: Hoàn tất đơn hàng (Đang giao -> Thành công)
        public ActionResult MarkCompleted(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            var order = db.Orders.Find(id);
            if (order != null)
            {
                order.Deliver = true;     // Đã giao xong
                order.isCompleted = true; // Hoàn tất quy trình
                order.DeliveryDate = DateTime.Now; // Lưu ngày nhận

                // Logic mở rộng: Trừ tồn kho sản phẩm tại đây nếu chưa trừ

                db.SaveChanges();
                TempData["SuccessMessage"] = "Đơn hàng #" + id + " đã hoàn thành xuất sắc!";
            }
            return RedirectToAction("Details", new { id = id });
        }

        // BƯỚC 4: Hủy đơn hàng (Nếu khách boom hàng hoặc hết hàng)
        public ActionResult CancelOrder(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            var order = db.Orders.Find(id);
            if (order != null)
            {
                // Sử dụng tên biến CancelOrder 
                order.CancelOrder = true;

                db.SaveChanges();
                TempData["ErrorMessage"] = "Đã hủy đơn hàng #" + id;
            }
            return RedirectToAction("Details", new { id = id });
        }

        // Giải phóng tài nguyên
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}