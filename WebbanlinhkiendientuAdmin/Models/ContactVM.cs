using System.ComponentModel.DataAnnotations;

namespace WebbanlinhkiendientuAdmin.Models
{
    public class ContactVM
    {
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        public string Subject { get; set; }

        [Display(Name = "Nội dung tin nhắn")]
        [Required(ErrorMessage = "Vui lòng nhập nội dung tin nhắn.")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
    }
}