using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public class EmployeeVM
    {
        public int EmpID { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Vui lòng nhập tên nhân viên")]
        [StringLength(50, ErrorMessage = "Tên không được quá 50 ký tự")]
        
        public string FirstName { get; set; }

        [Display(Name = "Họ")]
        [Required(ErrorMessage = "Vui lòng nhập họ nhân viên")]
        [StringLength(50, ErrorMessage = "Họ không được quá 50 ký tự")]
        public string LastName { get; set; }

        [Display(Name = "Ngày sinh")]
        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)] 
        public Nullable<System.DateTime> DateofBirth { get; set; }

        [Display(Name = "Giới tính")]
        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public string Gender { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Địa chỉ")]
        [DataType(DataType.MultilineText)] 
        public string Address { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        // Regex cho số điện thoại VN: Bắt đầu bằng 0, dài 10-11 số
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ (Phải bắt đầu bằng số 0 và có 10-11 số)")]
        public string Phone { get; set; }

        // Đường dẫn ảnh lưu trong DB
        public string PicturePath { get; set; }

        [Display(Name = "Ảnh đại diện")]
        // File upload từ Form
        public HttpPostedFileBase Picture { get; set; }
    }
}