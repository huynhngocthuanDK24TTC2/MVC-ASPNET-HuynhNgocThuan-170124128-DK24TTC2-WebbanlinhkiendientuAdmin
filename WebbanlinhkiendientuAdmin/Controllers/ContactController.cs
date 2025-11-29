using System;
using System.Web.Mvc;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        // POST: Xử lý gửi tin nhắn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(string Name, string Email, string Subject, string Message)
        {
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Message))
            {
                // --- XỬ LÝ LƯU DATABASE (NẾU CÓ BẢNG CONTACT) ---
                // Ví dụ:
                // var contact = new Contact { Name = Name, ... };
                // db.Contacts.Add(contact);
                // db.SaveChanges();

                // Tạm thời hiển thị thông báo thành công
                TempData["Success"] = "Cảm ơn bạn đã liên hệ! Chúng tôi đã nhận được tin nhắn và sẽ phản hồi sớm nhất.";
            }
            else
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin.";
            }

            return RedirectToAction("Index");
        }
    }
}