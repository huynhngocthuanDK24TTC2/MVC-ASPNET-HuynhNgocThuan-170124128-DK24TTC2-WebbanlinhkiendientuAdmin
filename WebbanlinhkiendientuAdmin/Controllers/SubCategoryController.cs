using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity; // Cần thiết để dùng .Include()
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class SubCategoryController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        // ---  Kiểm tra đăng nhập ---
        private bool IsLoggedIn()
        {
            return Session["AdminUser"] != null || Session["EmpID"] != null;
        }

        // ==========================================
        // 1. GET: Danh sách danh mục con
        // ==========================================
        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            // Include("Category") để lấy được tên của danh mục cha
            var subCats = db.SubCategories.Include(s => s.Category).ToList();
            return View(subCats);
        }

        // ==========================================
        // 2. GET: Trang Thêm mới
        // ==========================================
        public ActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            // Tạo danh sách chọn danh mục cha
            // Lưu ý: Đặt tên là CategoryList để tránh trùng với thuộc tính CategoryID
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "Name");
            return View();
        }

        
        // 3. POST: Xử lý Thêm mới
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubCategory sctg)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng tên (Logic nâng cao)
                    bool isDuplicate = db.SubCategories.Any(x => x.Name == sctg.Name && x.CategoryID == sctg.CategoryID);
                    if (isDuplicate)
                    {
                        ModelState.AddModelError("Name", "Tên danh mục con này đã tồn tại trong nhóm này!");

                        // Load lại dropdown nếu lỗi
                        ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "Name", sctg.CategoryID);
                        return View(sctg);
                    }

                    db.SubCategories.Add(sctg);
                    db.SaveChanges();

                    // THỐNG NHẤT: Dùng SuccessMessage khớp với _Layout
                    TempData["SuccessMessage"] = "Thêm danh mục con thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            // Nếu validate sai, load lại dropdown để người dùng chọn lại
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "Name", sctg.CategoryID);
            return View(sctg);
        }

        
        // 4. GET: Trang Chỉnh sửa
        
        public ActionResult Edit(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var subCategory = db.SubCategories.Find(id);
            if (subCategory == null) return HttpNotFound();

            // Load lại dropdown và chọn sẵn giá trị cũ (selectedValue)
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "Name", subCategory.CategoryID);
            return View(subCategory);
        }

        
        // 5. POST: Xử lý Chỉnh sửa
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SubCategory sctg)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật dữ liệu
                    db.Entry(sctg).State = EntityState.Modified;
                    db.SaveChanges();

                    // THỐNG NHẤT: Dùng SuccessMessage
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật: " + ex.Message);
                }
            }

            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "Name", sctg.CategoryID);
            return View(sctg);
        }

        
        // 6. ACTION: Xóa (Xử lý Logic)
        
        // Action này dành cho nút Xóa nhanh ở trang Index
        public ActionResult Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            try
            {
                var subCat = db.SubCategories.Find(id);
                if (subCat != null)
                {
                    // --- KIỂM TRA RÀNG BUỘC DỮ LIỆU ---
                    // Nếu danh mục này đã có sản phẩm -> KHÔNG ĐƯỢC XÓA
                    bool hasProduct = db.Products.Any(p => p.SubCategoryID == id);

                    if (hasProduct)
                    {
                        // THỐNG NHẤT: Dùng ErrorMessage để hiện thông báo đỏ
                        TempData["ErrorMessage"] = "Không thể xóa! Danh mục này đang chứa sản phẩm. Vui lòng xóa sản phẩm trước.";
                    }
                    else
                    {
                        db.SubCategories.Remove(subCat);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Đã xóa danh mục con thành công!";
                    }
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Lỗi hệ thống: Không thể xóa danh mục này vào lúc này.";
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}