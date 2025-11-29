using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;
using PagedList; // Thư viện phân trang

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class SupplierController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        // --- Kiểm tra đăng nhập ---
        private bool IsLoggedIn()
        {
            return Session["AdminUser"] != null || Session["EmpID"] != null;
        }

        
        // 1. GET: Danh sách (TÌM KIẾM + PHÂN TRANG)
        
        // Bổ sung tham số searchString để chức năng tìm kiếm hoạt động
        public ActionResult Index(int? page, string searchString)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            // Tạo truy vấn (chưa thực thi ngay)
            var suppliers = db.Suppliers.AsQueryable();

            // 1. Xử lý tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(s => s.CompanyName.Contains(searchString) ||
                                                 s.ContactName.Contains(searchString) ||
                                                 s.Phone.Contains(searchString));
            }

            // 2. Sắp xếp (Mới nhất lên đầu)
            suppliers = suppliers.OrderByDescending(s => s.SupplierID);

            // Lưu từ khóa tìm kiếm để View hiển thị lại
            ViewBag.CurrentFilter = searchString;

            // 3. Phân trang
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(suppliers.ToPagedList(pageNumber, pageSize));
        }

        
        // 2. GET: Trang Thêm mới
        
        public ActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            return View();
        }

        
        // 3. POST: Xử lý Thêm mới
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Supplier supplier)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra trùng tên công ty
                    if (db.Suppliers.Any(x => x.CompanyName == supplier.CompanyName))
                    {
                        ModelState.AddModelError("CompanyName", "Nhà cung cấp này đã tồn tại.");
                        return View(supplier);
                    }

                    // Mặc định ngày tạo nếu Model chưa có (tùy chọn)
                    // supplier.CreatedDate = DateTime.Now; 

                    db.Suppliers.Add(supplier);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }
            return View(supplier);
        }

        
        // 4. GET: Trang Sửa (Edit)
        
        public ActionResult Edit(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null) return HttpNotFound();

            return View(supplier);
        }

        
        // 5. POST: Xử lý Sửa
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Supplier supplier)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                try
                {
                    var oldItem = db.Suppliers.Find(supplier.SupplierID);
                    if (oldItem != null)
                    {
                        // Cập nhật các trường cho phép sửa
                        oldItem.CompanyName = supplier.CompanyName;
                        oldItem.ContactName = supplier.ContactName;
                        oldItem.Phone = supplier.Phone;
                        oldItem.Email = supplier.Email;
                        oldItem.Address = supplier.Address;
                        oldItem.isActive = supplier.isActive;

                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật: " + ex.Message);
                }
            }
            return View(supplier);
        }

        
        // 6. GET: Trang Chi tiết (Details) 
        
        public ActionResult Details(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null) return HttpNotFound();

            return View(supplier);
        }

        
        // 7. GET: Trang Xác nhận Xóa (Delete) 
        
        public ActionResult Delete(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null) return HttpNotFound();

            return View(supplier);
        }

        
        // 8. POST: Thực hiện Xóa (DeleteConfirm) 
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            Supplier supplier = db.Suppliers.Find(id);
            if (supplier != null)
            {
                // KIỂM TRA RÀNG BUỘC: Nếu NCC có sản phẩm -> Không cho xóa
                bool hasProduct = db.Products.Any(p => p.SupplierID == id);
                if (hasProduct)
                {
                    TempData["ErrorMessage"] = "Không thể xóa! Nhà cung cấp này đang có sản phẩm. Vui lòng chuyển trạng thái sang 'Ngừng hợp tác'.";
                    return RedirectToAction("Delete", new { id = id });
                }

                try
                {
                    db.Suppliers.Remove(supplier);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Đã xóa nhà cung cấp thành công!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi xóa dữ liệu: " + ex.Message;
                    return RedirectToAction("Delete", new { id = id });
                }
            }
            return RedirectToAction("Index");
        }

        // Giải phóng tài nguyên
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}