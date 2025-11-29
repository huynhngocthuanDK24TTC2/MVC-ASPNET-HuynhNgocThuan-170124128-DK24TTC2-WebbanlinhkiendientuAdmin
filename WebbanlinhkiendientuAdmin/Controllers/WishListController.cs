using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;
using System.Data.Entity; // Để dùng .Include

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class WishListController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        // 1. HIỂN THỊ DANH SÁCH YÊU THÍCH
        public ActionResult Index()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int userID = (int)Session["UserID"];

            // Lấy danh sách Wishlist của User hiện tại, kèm thông tin Sản phẩm
            var list = db.Wishlists
                         .Include(w => w.Product)
                         .Where(w => w.CustomerID == userID)
                         .OrderByDescending(w => w.WishlistID)
                         .ToList();

            // Cập nhật số lượng lên Header
            ViewBag.WlItemsNo = list.Count;

            return View(list);
        }

        // 2. THÊM SẢN PHẨM VÀO YÊU THÍCH
        public ActionResult AddItem(int id)
        {
            if (Session["UserID"] == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để lưu sản phẩm yêu thích!";
                return RedirectToAction("Login", "Account", new { returnUrl = Request.UrlReferrer?.ToString() });
            }

            int userID = (int)Session["UserID"];

            // Kiểm tra xem đã tồn tại chưa
            var existItem = db.Wishlists.FirstOrDefault(w => w.CustomerID == userID && w.ProductID == id);

            if (existItem == null)
            {
                Wishlist newItem = new Wishlist();
                newItem.CustomerID = userID;
                newItem.ProductID = id;
                // Nếu bảng Wishlist có cột Date, hãy thêm: newItem.CreatedDate = DateTime.Now;

                db.Wishlists.Add(newItem);
                db.SaveChanges();

                TempData["Success"] = "Đã thêm vào danh sách yêu thích!";
            }
            else
            {
                TempData["Error"] = "Sản phẩm này đã có trong danh sách yêu thích rồi!";
            }

            // Quay lại trang cũ
            if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.ToString());
            return RedirectToAction("Index", "Home");
        }

        // 3. XÓA KHỎI YÊU THÍCH
        public ActionResult Remove(int id)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            var item = db.Wishlists.Find(id);
            if (item != null)
            {
                db.Wishlists.Remove(item);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa sản phẩm khỏi danh sách yêu thích.";
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}