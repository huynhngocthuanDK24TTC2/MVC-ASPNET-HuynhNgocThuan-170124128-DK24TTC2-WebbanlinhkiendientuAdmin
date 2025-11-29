using System;
using System.Collections.Generic;
using System.Data.Entity; 
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        // ---  Kiểm tra đăng nhập Admin ---
        private bool IsLoggedIn()
        {
            // Kiểm tra Session Admin (dựa theo logic dùng ở các Controller khác)
            return Session["AdminUser"] != null || Session["EmpID"] != null;
        }

        // 1. GET: Danh sách & Tìm kiếm
        public ActionResult Index(string searchString)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            var categories = from c in db.Categories select c;

            // Lọc theo tên nếu có từ khóa
            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c => c.Name.Contains(searchString));
            }

            // Sắp xếp giảm dần theo ID để thấy cái mới nhất
            categories = categories.OrderByDescending(c => c.CategoryID);

            ViewBag.CurrentFilter = searchString;
            return View(categories.ToList());
        }

        // 2. GET: Tạo mới
        public ActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            return View();
        }

        // 3. POST: Lưu Tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category ctg)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng tên (Tùy chọn)
                    if (db.Categories.Any(c => c.Name.ToLower() == ctg.Name.ToLower()))
                    {
                        ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại!");
                        return View(ctg);
                    }

                    db.Categories.Add(ctg);
                    db.SaveChanges();

                    TempData["Success"] = "Thêm danh mục thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }
            return View(ctg);
        }

        // 4. GET: Sửa (Edit)
        public ActionResult Edit(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();

            return View(category);
        }

        // 5. POST: Lưu Sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật: " + ex.Message);
                }
            }
            return View(category);
        }

        // 6. GET: Trang Xác nhận Xóa (Delete)
        public ActionResult Delete(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Include SubCategories để đếm số lượng con hiển thị cảnh báo ở View
            var category = db.Categories.Include(c => c.SubCategories).FirstOrDefault(c => c.CategoryID == id);

            if (category == null) return HttpNotFound();

            return View(category);
        }

        // 7. POST: Thực hiện Xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            var category = db.Categories.Find(id);
            if (category != null)
            {
                // --- KIỂM TRA AN TOÀN DỮ LIỆU ---
                // 1. Có danh mục con không?
                bool hasSub = db.SubCategories.Any(s => s.CategoryID == id);
                // 2. Có sản phẩm nào đang gắn trực tiếp không? (Nếu có logic này)
                bool hasPro = db.Products.Any(p => p.CategoryID == id);

                if (hasSub || hasPro)
                {
                    TempData["Error"] = "Không thể xóa! Danh mục này đang chứa Dữ liệu con (SubCategory hoặc Sản phẩm).";
                    return RedirectToAction("Delete", new { id = id });
                }

                try
                {
                    db.Categories.Remove(category);
                    db.SaveChanges();
                    TempData["Success"] = "Đã xóa danh mục thành công!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi SQL: " + ex.Message;
                    return RedirectToAction("Delete", new { id = id });
                }
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