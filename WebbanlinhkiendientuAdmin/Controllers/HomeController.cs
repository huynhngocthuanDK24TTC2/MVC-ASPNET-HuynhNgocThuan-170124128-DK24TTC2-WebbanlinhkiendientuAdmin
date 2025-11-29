using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        public ActionResult Index()
        {
            
            try
            {
                // Lấy danh sách slider từ Database
                var sliderList = db.genMainSliders.ToList();

                // Kiểm tra: Nếu có dữ liệu thì mới gán vào ViewBag
                if (sliderList != null && sliderList.Count > 0)
                {
                    ViewBag.Slider = sliderList;
                }
                else
                {
                    // Nếu danh sách rỗng -> Gán null để View hiện Slider tĩnh mặc định
                    ViewBag.Slider = null;
                }
            }
            catch
            {
                // Nếu lỗi (ví dụ chưa có bảng genMainSliders) -> Gán null
                ViewBag.Slider = null;
            }

            
            // CÁC DANH SÁCH SẢN PHẨM
            

            // Sản phẩm mới
            ViewBag.NewProducts = db.Products
                .OrderByDescending(x => x.ProductID)
                .Take(8)
                .ToList();

            // VGA - Card màn hình
            ViewBag.GraphicsCards = db.Products
                .Where(x => x.Category.Name.Contains("VGA") || x.Category.Name.Contains("Card màn hình"))
                .OrderByDescending(x => x.ProductID)
                .Take(8)
                .ToList();

            // CPU
            ViewBag.CPUs = db.Products
                .Where(x => x.Category.Name.Contains("CPU"))
                .OrderByDescending(x => x.ProductID)
                .Take(8)
                .ToList();

            // Mainboard
            ViewBag.Motherboards = db.Products
                .Where(x => x.Category.Name.Contains("Mainboard") || x.Category.Name.Contains("Bo mạch"))
                .OrderByDescending(x => x.ProductID)
                .Take(8)
                .ToList();

            // RAM
            ViewBag.RAMs = db.Products
                .Where(x => x.Category.Name.Contains("RAM"))
                .OrderByDescending(x => x.ProductID)
                .Take(8)
                .ToList();

            // Sản phẩm Giảm giá
            ViewBag.DealProducts = db.Products
                .Where(x => x.OldPrice > x.UnitPrice && x.OldPrice != null)
                .OrderByDescending(x => x.ProductID)
                .Take(8)
                .ToList();

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}