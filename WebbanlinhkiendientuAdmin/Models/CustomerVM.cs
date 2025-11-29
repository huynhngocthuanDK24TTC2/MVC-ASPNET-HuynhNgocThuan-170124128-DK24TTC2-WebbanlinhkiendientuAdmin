using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public class CustomerVM
    {
        public int CustomerID { get; set; }

        // --- THÔNG TIN BẮT BUỘC (ĐĂNG KÝ CẦN) ---

        [Display(Name = "Họ đệm")]
        [Required(ErrorMessage = "Vui lòng nhập họ đệm.")]
        [StringLength(50, ErrorMessage = "Họ không quá 50 ký tự.")]
        public string Last_Name { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        [StringLength(50, ErrorMessage = "Tên không quá 50 ký tự.")]
        public string First_Name { get; set; }

        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên.")]
        public string Password { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ.")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [DataType(DataType.PhoneNumber)]
        
        [RegularExpression(@"^(\+?0?)[0-9]{9,11}$", ErrorMessage = "Số điện thoại phải từ 10 đến 12 số.")]
        public string Phone { get; set; }

        [Display(Name = "Ngày sinh")]
        [Required(ErrorMessage = "Vui lòng chọn ngày sinh.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateofBirth { get; set; }


        // --- THÔNG TIN TÙY CHỌN (CẬP NHẬT SAU) ---

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [Display(Name = "Quốc gia")]
        public string Country { get; set; }

        [Display(Name = "Thành phố")]
        public string City { get; set; }

        [Display(Name = "Mã bưu chính")]
        public string PostalCode { get; set; }

        [Display(Name = "Địa chỉ")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        // --- XỬ LÝ ẢNH ---

        public string PicturePath { get; set; } // Đường dẫn lưu DB

        [Display(Name = "Ảnh đại diện")]
        // Không để Required để khách đăng ký nhanh
        public HttpPostedFileBase Picture { get; set; }

        [Display(Name = "Trạng thái")]
        public bool status { get; set; }

        public Nullable<System.DateTime> LastLogin { get; set; }
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "Ghi chú")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
}