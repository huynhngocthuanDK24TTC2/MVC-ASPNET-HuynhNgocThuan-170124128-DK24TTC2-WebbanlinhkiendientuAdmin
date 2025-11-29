using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO; // Xử lý file ảnh
using System.Data.Entity; // Xử lý Include, State
using PagedList; // Phân trang
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class ProductController : Controller
    {
        // 1. KẾT NỐI DB 
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        // ---  Kiểm tra đăng nhập ---
        private bool IsLoggedIn()
        {
            return Session["AdminUser"] != null || Session["EmpID"] != null;
        }

        
        // 1. GET: Danh sách sản phẩm (Index)
        
        public ActionResult Index(int? page, string searchString)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            var products = db.Products
                             .Include(p => p.Category)
                             .Include(p => p.SubCategory)
                             .AsQueryable();

            // Tìm kiếm theo Tên hoặc Danh mục cha
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) || p.Category.Name.Contains(searchString));
            }

            // Sắp xếp: ID giảm dần (Mới nhất lên đầu)
            products = products.OrderByDescending(p => p.ProductID);
            ViewBag.CurrentFilter = searchString;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        
        // 2. GET: Thêm mới
        
        public ActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            // Load danh sách cho Dropdown
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name");
            // SubCategory ban đầu để trống, sẽ load bằng Ajax khi chọn Category
            ViewBag.SubCategoryID = new SelectList(new List<SubCategory>(), "SubCategoryID", "Name");
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName");

            return View();
        }

        
        // 3. POST: Thêm mới (Xử lý lưu)
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase Picture)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý Upload ảnh
                    if (Picture != null && Picture.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Picture.FileName);
                        string pathDir = Server.MapPath("~/Images/Products/");

                        // Tự động tạo thư mục nếu chưa có
                        if (!Directory.Exists(pathDir)) Directory.CreateDirectory(pathDir);

                        string pathSave = Path.Combine(pathDir, fileName);
                        Picture.SaveAs(pathSave);

                        product.PicturePath = "~/Images/Products/" + fileName;
                    }
                    else
                    {
                        product.PicturePath = "~/Images/no-image.jpg"; // Ảnh mặc định
                    }

                    // Lưu vào DB
                    db.Products.Add(product);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            // Nếu lỗi, load lại Dropdown để không bị mất dữ liệu
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryID);
            ViewBag.SubCategoryID = new SelectList(db.SubCategories.Where(x => x.CategoryID == product.CategoryID), "SubCategoryID", "Name", product.SubCategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);

            return View(product);
        }

        
        // 4. GET: Chỉnh sửa (Edit)
        
        public ActionResult Edit(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            Product product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryID);
            // Load đúng danh sách con của danh mục hiện tại
            ViewBag.SubCategoryID = new SelectList(db.SubCategories.Where(s => s.CategoryID == product.CategoryID), "SubCategoryID", "Name", product.SubCategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);

            return View(product);
        }

       
        // 5. POST: Chỉnh sửa (Cập nhật DB)
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase Picture)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                try
                {
                    var proInDb = db.Products.Find(product.ProductID);
                    if (proInDb != null)
                    {
                        // Cập nhật thông tin
                        proInDb.Name = product.Name;
                        proInDb.CategoryID = product.CategoryID;
                        proInDb.SubCategoryID = product.SubCategoryID;
                        proInDb.SupplierID = product.SupplierID;
                        proInDb.UnitPrice = product.UnitPrice;
                        proInDb.OldPrice = product.OldPrice;
                        proInDb.UnitInStock = product.UnitInStock;
                        proInDb.ShortDescription = product.ShortDescription;
                        // proInDb.Note = product.Note; // Bỏ comment nếu Model có cột Note

                        // Xử lý ảnh mới (Nếu có chọn)
                        if (Picture != null && Picture.ContentLength > 0)
                        {
                            // Xóa ảnh cũ (Trừ ảnh mặc định)
                            if (!string.IsNullOrEmpty(proInDb.PicturePath) && !proInDb.PicturePath.Contains("no-image"))
                            {
                                string oldPath = Server.MapPath(proInDb.PicturePath);
                                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                            }

                            // Lưu ảnh mới
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Picture.FileName);
                            string pathDir = Server.MapPath("~/Images/Products/");
                            if (!Directory.Exists(pathDir)) Directory.CreateDirectory(pathDir);

                            string pathSave = Path.Combine(pathDir, fileName);
                            Picture.SaveAs(pathSave);

                            proInDb.PicturePath = "~/Images/Products/" + fileName;
                        }

                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật: " + ex.Message);
                }
            }

            // Load lại Dropdown nếu lỗi
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryID);
            ViewBag.SubCategoryID = new SelectList(db.SubCategories.Where(s => s.CategoryID == product.CategoryID), "SubCategoryID", "Name", product.SubCategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
            return View(product);
        }

        
        // 6. GET: Chi tiết (Details)
        
        public ActionResult Details(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            Product product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            return View(product);
        }

        
        // 7. GET: Trang Xóa (Delete)
        
        public ActionResult Delete(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            Product product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            return View(product);
        }

        
        // 8. POST: Xác nhận Xóa
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            try
            {
                var product = db.Products.Find(id);
                if (product != null)
                {
                    // Kiểm tra ràng buộc: Sản phẩm đã có trong đơn hàng chưa?
                    bool hasOrder = db.OrderDetails.Any(od => od.ProductID == id);
                    if (hasOrder)
                    {
                        TempData["ErrorMessage"] = "Không thể xóa! Sản phẩm này đã bán. Vui lòng set tồn kho = 0.";
                        return RedirectToAction("Delete", new { id = id });
                    }

                    // Xóa ảnh vật lý
                    if (!string.IsNullOrEmpty(product.PicturePath) && !product.PicturePath.Contains("no-image"))
                    {
                        string path = Server.MapPath(product.PicturePath);
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    }

                    db.Products.Remove(product);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Đã xóa sản phẩm thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi xóa: " + ex.Message;
                return RedirectToAction("Delete", new { id = id });
            }
            return RedirectToAction("Index");
        }

        
        // 9. AJAX: Load danh mục con (Cho View Create/Edit)
        
        [HttpGet]
        public JsonResult GetSubCategories(int categoryId)
        {
            // Ngắt Proxy để tránh lỗi vòng lặp khi trả về JSON
            db.Configuration.ProxyCreationEnabled = false;

            var subList = db.SubCategories
                .Where(x => x.CategoryID == categoryId)
                .Select(x => new { Value = x.SubCategoryID, Text = x.Name })
                .ToList();

            return Json(subList, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}