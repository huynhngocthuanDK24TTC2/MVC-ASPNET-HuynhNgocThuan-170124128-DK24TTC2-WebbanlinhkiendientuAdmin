using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class admin_Employee
    {
        public admin_Employee()
        {
            this.admin_Login = new HashSet<admin_Login>();
        }

        [Key]
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
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateofBirth { get; set; }

        [Display(Name = "Giới tính")]
        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public string Gender { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được quá 100 ký tự")]
        public string Email { get; set; }

        [Display(Name = "Địa chỉ")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        
        
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ (Phải bắt đầu bằng số 0 và có 10-11 số)")]
        public string Phone { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string PicturePath { get; set; }

        public virtual ICollection<admin_Login> admin_Login { get; set; }
    }
}