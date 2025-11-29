using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class MyCartController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        // 1. GET: Xem giỏ hàng (Trang chi tiết giỏ hàng)
        public ActionResult Index()
        {
            List<OrderDetail> cart = Session["Cart"] as List<OrderDetail>;
            if (cart == null) cart = new List<OrderDetail>();

            // Tính toán tổng tiền
            ViewBag.TotalAmount = cart.Sum(x => x.Quantity * x.UnitPrice);

            return View(cart);
        }

        // 2. ACTION: THÊM VÀO GIỎ (Đây là cái nút Thêm ngay gọi đến)
        public ActionResult AddToCart(int id)
        {
            // Lấy giỏ hàng từ Session
            List<OrderDetail> cart = Session["Cart"] as List<OrderDetail>;
            if (cart == null)
            {
                cart = new List<OrderDetail>();
            }

            // Tìm xem sản phẩm này đã có trong giỏ chưa
            var item = cart.FirstOrDefault(x => x.ProductID == id);

            if (item != null)
            {
                // Nếu có rồi -> Tăng số lượng
                item.Quantity++;
            }
            else
            {
                // Nếu chưa có -> Lấy thông tin từ DB và thêm mới
                var product = db.Products.Find(id);
                if (product != null)
                {
                    item = new OrderDetail();
                    item.ProductID = product.ProductID;
                    item.Product = product; // Lưu luôn object Product để hiển thị ảnh/tên
                    item.Quantity = 1;
                    item.UnitPrice = product.UnitPrice;
                    cart.Add(item);
                }
            }

            // Cập nhật lại Session
            Session["Cart"] = cart;
            Session["NoOfItem"] = cart.Count; // Số loại sản phẩm

            // Thông báo nhỏ (Nếu Layout có hiển thị TempData)
            TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng!";

            // Quay lại trang cũ (Trang chủ hoặc trang chi tiết)
            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            return RedirectToAction("Index", "Home");
        }

        // 3. ACTION: XÓA KHỎI GIỎ (Cho nút X ở giỏ hàng nhỏ)
        public ActionResult Remove(int id)
        {
            List<OrderDetail> cart = Session["Cart"] as List<OrderDetail>;
            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductID == id);
                if (item != null)
                {
                    cart.Remove(item);
                    Session["Cart"] = cart;
                    Session["NoOfItem"] = cart.Count;
                }
            }
            // Quay lại trang cũ
            if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.ToString());
            return RedirectToAction("Index");
        }

        // 4. CẬP NHẬT SỐ LƯỢNG (Cho trang giỏ hàng chính)
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            List<OrderDetail> cart = Session["Cart"] as List<OrderDetail>;
            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductID == id);
                if (item != null)
                {
                    item.Quantity = quantity;
                    Session["Cart"] = cart;
                }
            }
            return RedirectToAction("Index");
        }
    }
}