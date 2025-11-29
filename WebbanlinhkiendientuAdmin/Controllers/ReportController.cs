using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class ReportController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        // Kiểm tra đăng nhập
        private bool IsLoggedIn()
        {
            return Session["AdminUser"] != null || Session["EmpID"] != null;
        }

        // GET: Report
        public ActionResult Index(DateTime? fromDate, DateTime? toDate)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            // 1. Thiết lập thời gian mặc định (Tháng hiện tại) nếu chưa chọn
            if (fromDate == null) fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (toDate == null) toDate = DateTime.Now;

            // Đảm bảo toDate lấy hết cuối ngày (23:59:59)
            DateTime endDate = toDate.Value.Date.AddDays(1).AddTicks(-1);

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            // 2. Lấy danh sách đơn hàng đã HOÀN THÀNH trong khoảng thời gian
            // Chỉ tính doanh thu từ những đơn đã giao thành công (Deliver = true hoặc isCompleted = true)
            var orders = db.Orders
                .Where(x => x.OrderDate >= fromDate && x.OrderDate <= endDate)
                .ToList();

            // 3. Tính toán các chỉ số (KPIs)
            decimal totalRevenue = orders.Where(x => x.isCompleted == true).Sum(x => x.TotalAmount ?? 0);
            int totalOrders = orders.Count();
            int completedOrders = orders.Count(x => x.isCompleted == true);
            int cancelledOrders = orders.Count(x => x.CancelOrder == true);

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.CompletedOrders = completedOrders;
            ViewBag.CancelledOrders = cancelledOrders;

            // 4. Chuẩn bị dữ liệu cho Biểu đồ (Chart.js)
            // Nhóm theo ngày để vẽ biểu đồ
            var chartData = orders
                .Where(x => x.isCompleted == true) // Chỉ vẽ biểu đồ đơn thành công
                .GroupBy(x => x.OrderDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd/MM"),
                    Revenue = g.Sum(x => x.TotalAmount ?? 0)
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Tách mảng để gửi sang View
            ViewBag.Labels = chartData.Select(x => x.Date).ToArray();
            ViewBag.Values = chartData.Select(x => x.Revenue).ToArray();

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}