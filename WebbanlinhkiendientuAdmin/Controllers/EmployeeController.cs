using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        //  Kiểm tra đăng nhập
        private bool IsLoggedIn()
        {
            return Session["AdminUser"] != null || Session["EmpID"] != null;
        }

        // GET: Employee - Danh sách nhân viên
        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            return View(db.admin_Employee.ToList());
        }

        // GET: Create - Trang thêm mới
        public ActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");
            return View();
        }

        // POST: Create - Xử lý thêm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeVM evm)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                // Kiểm tra email đã tồn tại chưa
                if (db.admin_Employee.Any(x => x.Email == evm.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng bởi nhân viên khác.");
                    return View(evm);
                }

                string filePath = "~/Images/no-image.jpg"; // Ảnh mặc định

                // Xử lý upload ảnh
                if (evm.Picture != null && evm.Picture.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(evm.Picture.FileName);
                    // THỐNG NHẤT: Lưu vào UserAvatar giống Customer
                    string uploadDir = Server.MapPath("~/Images/UserAvatar");

                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                    filePath = "~/Images/UserAvatar/" + fileName;
                    evm.Picture.SaveAs(Path.Combine(uploadDir, fileName));
                }

                admin_Employee e = new admin_Employee
                {
                    FirstName = evm.FirstName,
                    LastName = evm.LastName,
                    DateofBirth = evm.DateofBirth,
                    Gender = evm.Gender,
                    Email = evm.Email,
                    Address = evm.Address,
                    Phone = evm.Phone,
                    PicturePath = filePath
                };

                db.admin_Employee.Add(e);
                db.SaveChanges();

                TempData["Success"] = "Thêm nhân viên thành công!";
                return RedirectToAction("Index");
            }

            return View(evm);
        }

        // GET: Edit - Trang sửa
        // Dùng int? id để tránh lỗi "parameters dictionary contains a null entry"
        public ActionResult Edit(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            admin_Employee emp = db.admin_Employee.Find(id);
            if (emp == null) return HttpNotFound();

            EmployeeVM evm = new EmployeeVM
            {
                EmpID = emp.EmpID,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                DateofBirth = emp.DateofBirth,
                Gender = emp.Gender,
                Email = emp.Email,
                Address = emp.Address,
                Phone = emp.Phone,
                PicturePath = emp.PicturePath
            };
            return View(evm);
        }

        // POST: Edit - Xử lý sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeVM evm)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng Email (loại trừ chính mình)
                if (db.admin_Employee.Any(x => x.Email == evm.Email && x.EmpID != evm.EmpID))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng bởi nhân viên khác.");
                    return View(evm);
                }

                var existingEmp = db.admin_Employee.Find(evm.EmpID);
                if (existingEmp == null) return HttpNotFound();

                // Cập nhật thông tin
                existingEmp.FirstName = evm.FirstName;
                existingEmp.LastName = evm.LastName;
                existingEmp.DateofBirth = evm.DateofBirth;
                existingEmp.Gender = evm.Gender;
                existingEmp.Email = evm.Email;
                existingEmp.Address = evm.Address;
                existingEmp.Phone = evm.Phone;

                // Xử lý ảnh nếu có upload mới
                if (evm.Picture != null && evm.Picture.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(evm.Picture.FileName);
                    string uploadDir = Server.MapPath("~/Images/UserAvatar");

                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                    string path = Path.Combine(uploadDir, fileName);
                    evm.Picture.SaveAs(path);

                    // Cập nhật đường dẫn mới
                    existingEmp.PicturePath = "~/Images/UserAvatar/" + fileName;
                }

                db.SaveChanges();

                TempData["Success"] = "Cập nhật thông tin nhân viên thành công!";
                return RedirectToAction("Index");
            }
            return View(evm);
        }

        // GET: Info - Xem chi tiết
        // Dùng int? id
        public ActionResult Info(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            admin_Employee emp = db.admin_Employee.Find(id);
            if (emp == null) return HttpNotFound();

            return View(emp);
        }

        // GET: Delete - Xác nhận xóa
        public ActionResult Delete(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            admin_Employee employee = db.admin_Employee.Find(id);
            if (employee == null) return HttpNotFound();

            return View(employee);
        }

        // POST: Delete - Thực hiện xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "admin_Login");

            admin_Employee employee = db.admin_Employee.Find(id);
            if (employee != null)
            {
                // Kiểm tra xem nhân viên này có tài khoản đăng nhập không trước khi xóa
                // Nếu có, cần xóa tài khoản Login trước hoặc dùng Cascade Delete (đã config ở Context)

                try
                {
                    if (!string.IsNullOrEmpty(employee.PicturePath) && !employee.PicturePath.Contains("no-image"))
                    {
                        string path = Server.MapPath(employee.PicturePath);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }
                catch
                {
                    // Bỏ qua lỗi xóa file
                }

                db.admin_Employee.Remove(employee);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa nhân viên thành công!";
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