using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class admin_LoginController : Controller
    {
        
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: Trang đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            // Nếu đã đăng nhập thì vào thẳng Dashboard
            if (Session["EmpID"] != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(admin_Login loginModel)
        {
            
            if (!string.IsNullOrEmpty(loginModel.UserName) && !string.IsNullOrEmpty(loginModel.Password))
            {
                // Tìm tài khoản
                var adminUser = db.admin_Login
                                  .FirstOrDefault(m => m.UserName == loginModel.UserName
                                                    && m.Password == loginModel.Password);

                if (adminUser != null)
                {
                    // Đăng nhập thành công
                    Session["AdminUser"] = adminUser.UserName;
                    Session["EmpID"] = adminUser.EmpID;

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                }
            }
            else
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
            }

            // Nếu thất bại
            return View(loginModel);
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "admin_Login");
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