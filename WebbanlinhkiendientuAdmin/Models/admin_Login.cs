using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // thêm thư viện này để dùng [ForeignKey]
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class admin_Login
    {
        [Key]
        public int LoginID { get; set; }

        [Display(Name = "Nhân viên")]
        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        public int EmpID { get; set; }

        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được quá 50 ký tự")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        //  Mật khẩu phải từ 6 ký tự trở lên (giống Customer)
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        public string Password { get; set; }

        [Display(Name = "Quyền truy cập")]
        [Required(ErrorMessage = "Vui lòng phân quyền cho tài khoản")]
        public Nullable<int> RoleType { get; set; }

        [Display(Name = "Ghi chú")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        // Định nghĩa rõ khóa ngoại để Entity Framework hiểu
        [ForeignKey("EmpID")]
        public virtual admin_Employee admin_Employee { get; set; }

        [ForeignKey("RoleType")]
        public virtual Role Role { get; set; }
    }
}