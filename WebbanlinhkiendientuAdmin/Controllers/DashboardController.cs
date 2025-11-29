using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class DashboardController : Controller
    {
        // Khởi tạo kết nối CSDL
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        
        // ACTION: TRANG CHỦ QUẢN TRỊ (DASHBOARD)
        
        public ActionResult Index()
        {
            // 1. KIỂM TRA ĐĂNG NHẬP (BẢO MẬT)
            // Kiểm tra Session["EmpID"] (đã tạo ở bước đăng nhập). Nếu null => chưa đăng nhập.
            if (Session["EmpID"] == null)
            {
                // Chuyển hướng về trang đăng nhập của Admin
                return RedirectToAction("Login", "admin_Login");
            }

            // 2. THỐNG KÊ TRẠNG THÁI ĐƠN HÀNG (KPIs)
            // Đếm số lượng đơn hàng theo các trạng thái xử lý để hiển thị lên 4 ô màu trên Dashboard
            try
            {
                // Box 1: Đơn hàng mới (Chưa xử lý gì cả: Dispatched, Shipped, Deliver đều false)
                ViewBag.NewOrders = db.Orders.Count(x => x.Dispatched == false && x.Shipped == false && x.Deliver == false);

                // Box 2: Đơn đã xác nhận/Đang đóng gói (Dispatched = true, chưa giao)
                ViewBag.DispatchedOrders = db.Orders.Count(x => x.Dispatched == true && x.Shipped == false && x.Deliver == false);

                // Box 3: Đang giao hàng (Shipped = true, chưa hoàn tất)
                ViewBag.ShippedOrders = db.Orders.Count(x => x.Dispatched == true && x.Shipped == true && x.Deliver == false);

                // Box 4: Giao thành công (Tất cả đều true)
                ViewBag.DeliveredOrders = db.Orders.Count(x => x.Dispatched == true && x.Shipped == true && x.Deliver == true);

                // --- BỔ SUNG THÊM ---
                // Thống kê tổng số khách hàng (Để hiển thị nếu cần)
                ViewBag.TotalCustomers = db.Customers.Count();
            }
            catch (Exception)
            {
                // Phòng trường hợp CSDL bị lỗi hoặc thiếu cột, gán bằng 0 để web không bị chết (Crash)
                ViewBag.NewOrders = 0;
                ViewBag.DispatchedOrders = 0;
                ViewBag.ShippedOrders = 0;
                ViewBag.DeliveredOrders = 0;
                ViewBag.TotalCustomers = 0;
            }

            // 3. LẤY DANH SÁCH ĐƠN HÀNG GẦN ĐÂY
            // Lấy 10 đơn mới nhất (sắp xếp theo ID giảm dần) để hiện bảng "Đơn hàng mới nhận"
            var latestOrders = db.Orders.OrderByDescending(x => x.OrderID).Take(10).ToList();

            // Truyền list này sang View để hiển thị dạng bảng
            return View(latestOrders);
        }

        
        // ACTION: GIẢI PHÓNG TÀI NGUYÊN
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // Ngắt kết nối CSDL khi xong việc
            }
            base.Dispose(disposing);
        }
    }
}