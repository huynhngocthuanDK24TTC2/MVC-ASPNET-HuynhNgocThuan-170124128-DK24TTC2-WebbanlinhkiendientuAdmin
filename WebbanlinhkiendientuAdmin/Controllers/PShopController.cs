using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList; // BẮT BUỘC
using WebbanlinhkiendientuAdmin.Models;
using System.Data.Entity;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class PShopController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        
        // 1. GET: Danh sách sản phẩm (TỔNG HỢP: PHÂN TRANG + LỌC GIÁ + TÌM KIẾM + SẮP XẾP)
        
        public ActionResult Index(int? page, string sortBy, string searchString, int? minPrice, int? maxPrice, string categoryName)
        {
            // Khởi tạo truy vấn
            var products = db.Products.AsQueryable();

            // A. Xử lý Lọc theo Tìm kiếm (Search)
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
                ViewBag.SearchString = searchString; // Lưu lại để hiện trên ô tìm kiếm
            }

            // B. Xử lý Lọc theo Danh mục (Category) - Nếu có
            if (!string.IsNullOrEmpty(categoryName))
            {
                products = products.Where(p => p.Category.Name == categoryName || p.SubCategory.Name == categoryName);
                ViewBag.CategoryName = categoryName;
            }

            // C. Xử lý Lọc theo Giá (Price Range) 
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.UnitPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.UnitPrice <= maxPrice.Value);
            }
            // Lưu lại giá trị để thanh trượt hiển thị đúng
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            // D. Xử lý Sắp xếp (Sort)
            ViewBag.SortBy = sortBy; // Lưu lại để phân trang không bị mất sắp xếp
            switch (sortBy)
            {
                case "Price":
                    products = products.OrderBy(p => p.UnitPrice);
                    break;
                case "Name":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "Date":
                default:
                    products = products.OrderByDescending(p => p.ProductID);
                    break;
            }

            // E. Phân trang
            int pageSize = 12; // 12 sản phẩm (3 hàng x 4 cột)
            int pageNumber = (page ?? 1);

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        
        // 2. Action hỗ trợ khác
        

        // Tìm kiếm nhanh (Redirect về Index để tận dụng logic trên)
        public ActionResult Search(string product)
        {
            return RedirectToAction("Index", new { searchString = product });
        }

        // Lọc danh mục (Redirect về Index)
        public ActionResult GetProductsByCategory(string categoryName)
        {
            return RedirectToAction("Index", new { categoryName = categoryName });
        }

        // Autocomplete cho thanh tìm kiếm
        public JsonResult GetProducts(string term)
        {
            var result = db.Products
                .Where(x => x.Name.Contains(term))
                .Select(x => new { value = x.Name })
                .Take(10)
                .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        // 3. GET: Chi tiết sản phẩm
        
        public ActionResult Details(int? id)
        {
            if (id == null) return RedirectToAction("Index", "Home");

            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            // Lấy đánh giá
            // 1. Lấy đánh giá (Reviews) + KÈM THEO THÔNG TIN KHÁCH HÀNG (Include)
            // Phải có .Include(r => r.Customer) thì mới lấy được PicturePath của khách
            var reviews = db.Reviews
                            .Include(r => r.Customer)
                            .Where(r => r.ProductID == id)
                            .OrderByDescending(r => r.DateTime) // Mới nhất lên đầu
                            .ToList();

            ViewBag.Reviews = reviews;
            ViewBag.TotalReviews = reviews.Count;

            // Tính sao trung bình
            if (reviews.Count > 0)
                ViewBag.AvgRate = (int)Math.Round(reviews.Average(r => r.Rate ?? 0));
            else
                ViewBag.AvgRate = 0;

            // Sản phẩm liên quan (Cùng SubCategory, khác ID)
            ViewBag.RelatedProducts = db.Products
                .Where(p => p.SubCategoryID == product.SubCategoryID && p.ProductID != product.ProductID)
                .Take(4)
                .ToList();

            return View(product);
        }

        
        // 4. POST: Thêm đánh giá
        
        [HttpPost]
        public ActionResult AddReview(int productID, int rate, string message)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            try
            {
                Review newReview = new Review();
                newReview.ProductID = productID;
                newReview.CustomerID = (int)Session["UserID"];
                newReview.Name = Session["username"] != null ? Session["username"].ToString() : "Khách hàng";
                newReview.Review1 = message;
                newReview.Rate = rate;
                newReview.DateTime = DateTime.Now;
                newReview.isDelete = false;

                db.Reviews.Add(newReview);
                db.SaveChanges();
            }
            catch { }

            return RedirectToAction("Details", new { id = productID });
        }

        
        // 5. Mini Cart (Giỏ hàng nhỏ trên Header)
        
        public ActionResult GetCartBox()
        {
            List<OrderDetail> cart = Session["Cart"] as List<OrderDetail>;
            if (cart == null) cart = new List<OrderDetail>();

            ViewBag.TotalQuantity = cart.Sum(x => x.Quantity);
            ViewBag.TotalPrice = cart.Sum(x => (x.Quantity ?? 0) * (x.UnitPrice ?? 0));

            return PartialView("_CartBox", cart);
        }

        
        // 6. Thêm vào giỏ (Dùng cho nút "Mua ngay" ở trang Index)
        
        public ActionResult AddToCart(int id)
        {
            // Gọi sang MyCartController để xử lý cho gọn
            return RedirectToAction("AddToCart", "MyCart", new { id = id });
        }

        // WishList (Tạm thời)
        public ActionResult WishList(int id)
        {
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}