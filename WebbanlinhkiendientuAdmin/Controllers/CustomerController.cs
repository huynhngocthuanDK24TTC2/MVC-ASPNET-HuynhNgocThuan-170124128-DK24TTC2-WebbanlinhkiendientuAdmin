using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebbanlinhkiendientuAdmin.Models;
using PagedList;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class CustomerController : Controller
    {
        // 1. KHỞI TẠO DB 
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        // Kiểm tra đăng nhập
        private bool IsLoggedIn()
        {
            return Session["AdminUser"] != null || Session["EmpID"] != null;
        }

        // ==========================================
        // 1. GET: Danh sách khách hàng
        // ==========================================
        public ActionResult Index(int? page, string searchString)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            var query = db.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.First_Name.Contains(searchString) ||
                                         c.Last_Name.Contains(searchString) ||
                                         c.Phone.Contains(searchString) ||
                                         c.Email.Contains(searchString));
            }

            var customers = query.OrderByDescending(c => c.CustomerID);
            ViewBag.CurrentFilter = searchString;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(customers.ToPagedList(pageNumber, pageSize));
        }

        // ==========================================
        // 2. GET: Trang Thêm mới
        // ==========================================
        public ActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            return View();
        }

        // ==========================================
        // 3. POST: Xử lý Thêm mới
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerVM cvm)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp
                if (db.Customers.Any(x => x.UserName == cvm.UserName))
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập này đã có người dùng.");
                    return View(cvm);
                }
                if (db.Customers.Any(x => x.Email == cvm.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(cvm);
                }

                try
                {
                    // Xử lý Upload Ảnh
                    string pathDB = null;
                    if (cvm.Picture != null && cvm.Picture.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(cvm.Picture.FileName);
                        string pathDir = Server.MapPath("~/Images/UserAvatar/");
                        if (!Directory.Exists(pathDir)) Directory.CreateDirectory(pathDir);

                        string pathSave = Path.Combine(pathDir, fileName);
                        cvm.Picture.SaveAs(pathSave);
                        pathDB = "~/Images/UserAvatar/" + fileName;
                    }
                    else
                    {
                        pathDB = "~/Images/UserAvatar/no-image.jpg";
                    }

                    Customer c = new Customer
                    {
                        First_Name = cvm.First_Name,
                        Last_Name = cvm.Last_Name,
                        UserName = cvm.UserName,
                        Password = cvm.Password,
                        Gender = cvm.Gender,
                        DateofBirth = cvm.DateofBirth,
                        Country = cvm.Country,
                        City = cvm.City,
                        PostalCode = cvm.PostalCode,
                        Email = cvm.Email,
                        Phone = cvm.Phone,
                        Address = cvm.Address,
                        PicturePath = pathDB,
                        status = true,
                        LastLogin = DateTime.Now,
                        Created = DateTime.Now,
                        Notes = cvm.Notes
                    };

                    db.Customers.Add(c);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }
            return View(cvm);
        }


        // 4. GET: Trang Sửa

        public ActionResult Edit(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            Customer cus = db.Customers.Find(id);
            if (cus == null) return HttpNotFound();

            CustomerVM cvm = new CustomerVM
            {
                CustomerID = cus.CustomerID,
                First_Name = cus.First_Name,
                Last_Name = cus.Last_Name,
                UserName = cus.UserName,
                Gender = cus.Gender,
                DateofBirth = cus.DateofBirth,
                Country = cus.Country,
                City = cus.City,
                PostalCode = cus.PostalCode,
                Email = cus.Email,
                Phone = cus.Phone,
                Address = cus.Address,
                PicturePath = cus.PicturePath,
                status = cus.status.GetValueOrDefault(false),
                Notes = cus.Notes
            };
            return View(cvm);
        }

        
        // 5. POST: Xử lý Sửa
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerVM cvm)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (string.IsNullOrEmpty(cvm.Password)) ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                var cusInDb = db.Customers.Find(cvm.CustomerID);
                if (cusInDb == null) return HttpNotFound();

                cusInDb.First_Name = cvm.First_Name;
                cusInDb.Last_Name = cvm.Last_Name;
                cusInDb.Gender = cvm.Gender;
                cusInDb.DateofBirth = cvm.DateofBirth;
                cusInDb.Country = cvm.Country;
                cusInDb.City = cvm.City;
                cusInDb.PostalCode = cvm.PostalCode;
                cusInDb.Email = cvm.Email;
                cusInDb.Phone = cvm.Phone;
                cusInDb.Address = cvm.Address;
                cusInDb.Notes = cvm.Notes;
                cusInDb.status = cvm.status;

                if (!string.IsNullOrEmpty(cvm.Password)) cusInDb.Password = cvm.Password;

                if (cvm.Picture != null && cvm.Picture.ContentLength > 0)
                {
                    if (!string.IsNullOrEmpty(cusInDb.PicturePath) && !cusInDb.PicturePath.Contains("no-image"))
                    {
                        string oldPath = Server.MapPath(cusInDb.PicturePath);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(cvm.Picture.FileName);
                    string pathDir = Server.MapPath("~/Images/UserAvatar/");
                    if (!Directory.Exists(pathDir)) Directory.CreateDirectory(pathDir);
                    string pathSave = Path.Combine(pathDir, fileName);
                    cvm.Picture.SaveAs(pathSave);
                    cusInDb.PicturePath = "~/Images/UserAvatar/" + fileName;
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            return View(cvm);
        }

        
        // 6. GET: Chi tiết
        
        public ActionResult Details(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Customer cus = db.Customers.Find(id);
            if (cus == null) return HttpNotFound();
            return View(cus);
        }

        
        // 7. GET & POST: Xóa
        
        public ActionResult Delete(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Customer customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            Customer customer = db.Customers.Find(id);
            if (customer != null)
            {
                // A. KIỂM TRA ĐƠN HÀNG (Nếu có đơn -> CẤM XÓA)
                if (db.Orders.Any(o => o.CustomerID == id))
                {
                    TempData["ErrorMessage"] = "Không thể xóa! Khách hàng này đã có lịch sử mua hàng.";
                    return RedirectToAction("Index");
                }

                try
                {
                    // B. XÓA DỮ LIỆU PHỤ (Để tránh lỗi khóa ngoại FK)
                    // 1. Xóa Wishlist (Sản phẩm yêu thích)
                    var wishlists = db.Wishlists.Where(w => w.CustomerID == id).ToList();
                    if (wishlists.Count > 0) db.Wishlists.RemoveRange(wishlists);

                    // 2. Xóa Reviews (Đánh giá)
                    var reviews = db.Reviews.Where(r => r.CustomerID == id).ToList();
                    if (reviews.Count > 0) db.Reviews.RemoveRange(reviews);

                    // 3. Xóa ảnh vật lý
                    if (!string.IsNullOrEmpty(customer.PicturePath) && !customer.PicturePath.Contains("no-image"))
                    {
                        string path = Server.MapPath(customer.PicturePath);
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    }

                    // C. XÓA KHÁCH HÀNG
                    db.Customers.Remove(customer);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Đã xóa thành công!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
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