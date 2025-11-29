using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class AccountController : Controller
    {
        // 1. KẾT NỐI DATABASE
        private readonly WebbanlinhkiendientuAdminDbModels db = new WebbanlinhkiendientuAdminDbModels();

        //  Lấy UserID từ Session
        private int CurrentUserID
        {
            get { return Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0; }
        }

        
        // 0. ĐIỀU HƯỚNG MẶC ĐỊNH
        
        public ActionResult Index()
        {
            if (CurrentUserID > 0)
            {
                return RedirectToAction("Profile");
            }
            return RedirectToAction("Login");
        }

        
        // 1. ĐĂNG KÝ 
        
        [HttpGet]
        public ActionResult RegisterCustomer()
        {
            if (CurrentUserID > 0) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCustomer(CustomerVM cvm)
        {
            
            if (cvm.DateofBirth == null) ModelState.Remove("DateofBirth");

            // Kiểm tra dữ liệu
            if (ModelState.IsValid)
            {
                // A. Kiểm tra trùng lặp
                if (db.Customers.Any(x => x.UserName == cvm.UserName))
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập này đã tồn tại.");
                    ViewBag.Error = "Tên đăng nhập đã có người dùng.";
                    return View(cvm);
                }
                if (db.Customers.Any(x => x.Email == cvm.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    ViewBag.Error = "Email này đã được sử dụng.";
                    return View(cvm);
                }

                try
                {
                    // B. Xử lý Ảnh
                    string pathDB = "~/Images/UserAvatar/no-image.jpg";
                    if (cvm.Picture != null && cvm.Picture.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(cvm.Picture.FileName);
                        string dir = Server.MapPath("~/Images/UserAvatar/");
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                        cvm.Picture.SaveAs(Path.Combine(dir, fileName));
                        pathDB = "~/Images/UserAvatar/" + fileName;
                    }

                    // C. Tạo đối tượng
                    Customer c = new Customer
                    {
                        First_Name = cvm.First_Name,
                        Last_Name = cvm.Last_Name,
                        UserName = cvm.UserName,
                        Password = cvm.Password,
                        Gender = cvm.Gender ?? "Nam",
                        Email = cvm.Email,
                        Phone = cvm.Phone,
                        Address = cvm.Address,
                        
                        DateofBirth = cvm.DateofBirth ?? DateTime.Now,
                        PicturePath = pathDB,
                        status = true,
                        Created = DateTime.Now,
                        LastLogin = DateTime.Now,
                        Notes = "Khách tự đăng ký"
                    };

                    db.Customers.Add(c);
                    db.SaveChanges();

                    // D. Tự động đăng nhập
                    Session["UserID"] = c.CustomerID;
                    Session["username"] = c.Last_Name + " " + c.First_Name;
                    Session["UserEmail"] = c.Email;

                    TempData["Success"] = "Đăng ký thành công! Chào mừng bạn.";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    // Lấy lỗi chi tiết từ InnerException nếu có
                    string msg = ex.Message;
                    if (ex.InnerException != null) msg += " | " + ex.InnerException.Message;

                    ViewBag.Error = "LỖI HỆ THỐNG: " + msg;
                }
            }
            else
            {
                // ---  HIỂN THỊ LỖI VALIDATION RA MÀN HÌNH ---
                // Nếu không vào được đây, nghĩa là dữ liệu nhập thiếu hoặc sai
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ViewBag.Error = "Dữ liệu chưa đúng: " + string.Join(", ", errors);
            }

            return View(cvm);
        }

        
        // 2. ĐĂNG NHẬP (LOGIN)
        
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (CurrentUserID > 0) return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName, string Password, string returnUrl)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            var user = db.Customers.FirstOrDefault(u => u.UserName == UserName && u.Password == Password);

            if (user != null)
            {
                if (user.status == false)
                {
                    ViewBag.Error = "Tài khoản bị khóa. Vui lòng liên hệ Admin.";
                    return View();
                }

                Session["UserID"] = user.CustomerID;
                Session["username"] = user.Last_Name + " " + user.First_Name;
                Session["UserEmail"] = user.Email;

                user.LastLogin = DateTime.Now;
                db.SaveChanges();

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        
        // 3. HỒ SƠ (PROFILE) 
        

        // Sử dụng 'new' để tránh cảnh báo trùng tên hệ thống
        [HttpGet]
        public new ActionResult Profile()
        {
            if (CurrentUserID == 0) return RedirectToAction("Login");

            var user = db.Customers.Find(CurrentUserID);
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Customer cus, HttpPostedFileBase Picture)
        {
            if (CurrentUserID == 0) return RedirectToAction("Login");

            var user = db.Customers.Find(CurrentUserID);
            if (user != null)
            {
                try
                {
                    // Cập nhật thông tin
                    user.First_Name = cus.First_Name;
                    user.Last_Name = cus.Last_Name;
                    user.Phone = cus.Phone;
                    user.Email = cus.Email;
                    user.Address = cus.Address;
                    // Cập nhật thêm nếu form có gửi lên
                    if (cus.Gender != null) user.Gender = cus.Gender;
                    if (cus.DateofBirth != null) user.DateofBirth = cus.DateofBirth;

                    // Xử lý ảnh
                    if (Picture != null && Picture.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(Picture.FileName);
                        string dir = Server.MapPath("~/Images/UserAvatar/");
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                        // Xóa ảnh cũ (trừ ảnh mặc định)
                        if (!string.IsNullOrEmpty(user.PicturePath) && !user.PicturePath.Contains("no-image"))
                        {
                            string oldPath = Server.MapPath(user.PicturePath);
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }

                        Picture.SaveAs(Path.Combine(dir, fileName));
                        user.PicturePath = "~/Images/UserAvatar/" + fileName;
                    }

                    db.SaveChanges();

                    // Cập nhật lại Session tên
                    Session["username"] = user.Last_Name + " " + user.First_Name;
                    Session["UserEmail"] = user.Email; // Cập nhật cả email

                    TempData["Success"] = "Cập nhật thông tin thành công!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi: " + ex.Message;
                }
            }
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (CurrentUserID == 0) return RedirectToAction("Login");

            var user = db.Customers.Find(CurrentUserID);
            if (user != null)
            {
                if (user.Password != currentPassword)
                {
                    TempData["ErrorPass"] = "Mật khẩu hiện tại không đúng!";
                }
                else if (newPassword != confirmPassword)
                {
                    TempData["ErrorPass"] = "Xác nhận mật khẩu mới không khớp!";
                }
                else
                {
                    user.Password = newPassword;
                    db.SaveChanges();
                    TempData["SuccessPass"] = "Đổi mật khẩu thành công!";
                }
            }
            return RedirectToAction("Profile");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}