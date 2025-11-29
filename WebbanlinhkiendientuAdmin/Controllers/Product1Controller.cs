using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;
using PagedList;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class PShopController : Controller
    {
        private readonly WebbanlinhkiendientuDbModels db = new WebbanlinhkiendientuDbModels();

        private int CurrentUserID => Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;

        private List<OrderDetail> CurrentCart
        {
            get
            {
                var cart = Session["Cart"] as List<OrderDetail>;
                if (cart == null)
                {
                    cart = new List<OrderDetail>();
                    Session["Cart"] = cart;
                }
                return cart;
            }
        }

        // --- Trang cửa hàng chính, hiển thị tất cả sản phẩm ---
        public ActionResult Index(int? page, string sortBy)
        {
            LoadCommonViewBags();
            IQueryable<Product> allProducts = db.Products.Where(p => p.ProductAvailable == true);

            allProducts = SortProducts(allProducts, sortBy); // Áp dụng sắp xếp

            int pageSize = 9;
            int pageNumber = (page ?? 1);
            return View("Products", allProducts.ToPagedList(pageNumber, pageSize));
        }

        // --- Hiển thị giỏ hàng nhỏ trên Menu ---
        [ChildActionOnly]
        public ActionResult GetCartBox()
        {
            var cart = CurrentCart;
            ViewBag.NoOfItem = cart.Count;
            ViewBag.Total = cart.Sum(x => x.TotalAmount ?? 0);
            return PartialView("_CartBox", cart);
        }

        // --- NÂNG CẤP: THÊM VÀO GIỎ HÀNG (Xử lý số lượng) ---
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var cart = CurrentCart;
            var existingItem = cart.FirstOrDefault(x => x.ProductID == id);

            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            if (product.UnitInStock < quantity)
            {
                TempData["ErrorMessage"] = "Xin lỗi, số lượng sản phẩm trong kho không đủ.";
                return RedirectToAction("ViewDetails", new { id = id });
            }

            if (existingItem != null)
            {
                int newQuantity = (existingItem.Quantity ?? 0) + quantity;
                if (product.UnitInStock < newQuantity)
                {
                    newQuantity = product.UnitInStock ?? 0;
                    TempData["InfoMessage"] = $"Số lượng sản phẩm trong kho không đủ. Đã cập nhật giỏ hàng với số lượng tối đa ({newQuantity}).";
                }
                else
                {
                    TempData["SuccessMessage"] = "Đã cập nhật số lượng sản phẩm trong giỏ hàng!";
                }
                existingItem.Quantity = newQuantity;
                existingItem.TotalAmount = existingItem.Quantity * (existingItem.UnitPrice ?? 0);
            }
            else
            {
                cart.Add(new OrderDetail
                {
                    ProductID = id,
                    Quantity = quantity,
                    UnitPrice = product.UnitPrice,
                    TotalAmount = product.UnitPrice * quantity,
                    Product = product
                });
                TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";
            }

            Session["Cart"] = cart;
            AddRecentViewProduct(id);

            // Quay lại trang chi tiết sản phẩm để người dùng thấy thông báo
            return RedirectToAction("ViewDetails", new { id = id });
        }

        // --- XEM CHI TIẾT SẢN PHẨM ---
        public ActionResult ViewDetails(int id)
        {
            var prod = db.Products.Find(id);
            if (prod == null) return HttpNotFound();

            var reviews = db.Reviews.Where(x => x.ProductID == id).ToList();
            ViewBag.Reviews = reviews;
            ViewBag.TotalReviews = reviews.Count;
            ViewBag.AvgRate = reviews.Any() ? Math.Round(reviews.Average(x => x.Rate ?? 0), 1) : 0;

            ViewBag.RelatedProducts = db.Products
                .Where(y => y.CategoryID != null && y.CategoryID == prod.CategoryID && y.ProductID != id && y.ProductAvailable == true)
                .Take(4).ToList();

            AddRecentViewProduct(id);
            TempData["returnURL"] = Request.Url.PathAndQuery;
            return View(prod);
        }

        // --- THÊM VÀO YÊU THÍCH ---
        public ActionResult WishList(int id)
        {
            if (CurrentUserID == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để sử dụng chức năng này!";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("ViewDetails", new { id }) });
            }

            bool exists = db.Wishlists.Any(x => x.ProductID == id && x.CustomerID == CurrentUserID);
            if (!exists)
            {
                db.Wishlists.Add(new Wishlist { ProductID = id, CustomerID = CurrentUserID, isActive = true, CreatedDate = DateTime.Now });
                db.SaveChanges();
                TempData["SuccessMessage"] = "Đã thêm sản phẩm vào danh sách yêu thích!";
            }
            else
            {
                TempData["InfoMessage"] = "Sản phẩm này đã có trong danh sách yêu thích của bạn.";
            }

            AddRecentViewProduct(id);
            return RedirectToAction("ViewDetails", new { id = id });
        }

        // --- LỌC THEO DANH MỤC CON ---
        public ActionResult ProductsBySubCategory(int subCatID, int? page, string sortBy)
        {
            LoadCommonViewBags();
            IQueryable<Product> prods = db.Products.Where(y => y.SubCategoryID == subCatID && y.ProductAvailable == true);
            prods = SortProducts(prods, sortBy);
            return View("Products", prods.ToPagedList(page ?? 1, 9));
        }

        // --- LỌC THEO DANH MỤC CHA ---
        public ActionResult ProductsByCategory(string categoryName, int? page, string sortBy)
        {
            LoadCommonViewBags();
            IQueryable<Product> prods = db.Products.Where(x => x.Category != null && x.Category.Name.ToLower() == categoryName.ToLower() && x.ProductAvailable == true);
            prods = SortProducts(prods, sortBy);
            ViewBag.CategoryName = categoryName;
            return View("Products", prods.ToPagedList(page ?? 1, 9));
        }

        // --- TÌM KIẾM (Đã tích hợp sắp xếp) ---
        public ActionResult Search(string product, int? page, string sortBy)
        {
            LoadCommonViewBags();
            IQueryable<Product> products = db.Products.Where(x => x.ProductAvailable == true);

            if (!string.IsNullOrEmpty(product))
            {
                products = products.Where(x => x.Name.Contains(product));
            }

            products = SortProducts(products, sortBy);
            ViewBag.SearchTerm = product;
            return View("Products", products.ToPagedList(page ?? 1, 9));
        }

        // --- HELPER: Logic sắp xếp sản phẩm ---
        private IQueryable<Product> SortProducts(IQueryable<Product> products, string sortBy)
        {
            switch (sortBy)
            {
                case "Name":
                    return products.OrderBy(p => p.Name);
                case "PriceAsc":
                    return products.OrderBy(p => p.UnitPrice);
                case "PriceDesc":
                    return products.OrderByDescending(p => p.UnitPrice);
                default:
                    return products.OrderByDescending(p => p.ProductID);
            }
        }

        // --- HELPER: Load các ViewBag chung ---
        private void LoadCommonViewBags()
        {
            ViewBag.Categories = db.Categories.Where(x => x.isActive == true).ToList();
            ViewBag.SubCategories = db.SubCategories.Where(x => x.isActive == true).ToList();
            ViewBag.RecentViewsProducts = GetRecentViewProducts();
        }

        // --- HELPER: Lấy sản phẩm đã xem ---
        private IEnumerable<Product> GetRecentViewProducts()
        {
            if (CurrentUserID > 0)
            {
                var top3ProductIds = db.RecentlyViews.Where(r => r.CustomerID == CurrentUserID).OrderByDescending(r => r.ViewDate).Select(r => r.ProductID).Take(3).ToList();
                return db.Products.Where(p => top3ProductIds.Contains(p.ProductID)).ToList();
            }
            return db.Products.Where(p => p.ProductAvailable == true).OrderByDescending(p => p.ProductID).Take(3).ToList();
        }

        // --- HELPER: LƯU LỊCH SỬ XEM ---
        private void AddRecentViewProduct(int pid)
        {
            if (CurrentUserID > 0 && !db.RecentlyViews.Any(x => x.CustomerID == CurrentUserID && x.ProductID == pid))
            {
                db.RecentlyViews.Add(new RecentlyView { CustomerID = CurrentUserID, ProductID = pid, ViewDate = DateTime.Now });
                db.SaveChanges();
            }
        }

        // --- GỬI ĐÁNH GIÁ SẢN PHẨM ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(FormCollection getReview)
        {
            if (CurrentUserID == 0) return RedirectToAction("Login", "Account");

            int.TryParse(getReview["productID"], out int productID);
            int.TryParse(getReview["rate"], out int rate);

            if (productID > 0)
            {
                db.Reviews.Add(new Review
                {
                    CustomerID = CurrentUserID,
                    ProductID = productID,
                    Name = getReview["name"],
                    Email = getReview["email"],
                    Review1 = getReview["message"],
                    Rate = rate,
                    DateTime = DateTime.Now,
                    isDelete = false
                });
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi đánh giá!";
            }
            return RedirectToAction("ViewDetails", new { id = productID });
        }

        // --- AUTOCOMPLETE (Gợi ý tìm kiếm) ---
        public JsonResult GetProducts(string term)
        {
            if (string.IsNullOrEmpty(term)) return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            var prodNames = db.Products.Where(x => x.Name.Contains(term) && x.ProductAvailable == true).Select(y => y.Name).Take(10).ToList();
            return Json(prodNames, JsonRequestBehavior.AllowGet);
        }

        // --- LỌC THEO GIÁ ---
        public ActionResult FilterByPrice(int minPrice = 0, int maxPrice = 100000000, int? page = 1, string sortBy = "Default")
        {
            LoadCommonViewBags();
            ViewBag.filterByPrice = true;
            IQueryable<Product> filterProducts = db.Products.Where(x => x.UnitPrice >= minPrice && x.UnitPrice <= maxPrice && x.ProductAvailable == true);

            // Áp dụng sắp xếp cho kết quả lọc giá
            switch (sortBy)
            {
                case "Name": filterProducts = filterProducts.OrderBy(p => p.Name); break;
                case "PriceAsc": filterProducts = filterProducts.OrderBy(p => p.UnitPrice); break;
                case "PriceDesc": filterProducts = filterProducts.OrderByDescending(p => p.UnitPrice); break;
                default: filterProducts = filterProducts.OrderBy(p => p.UnitPrice); break; // Mặc định lọc giá thì sắp xếp theo giá tăng dần
            }

            return View("Products", filterProducts.ToPagedList(page ?? 1, 9));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}