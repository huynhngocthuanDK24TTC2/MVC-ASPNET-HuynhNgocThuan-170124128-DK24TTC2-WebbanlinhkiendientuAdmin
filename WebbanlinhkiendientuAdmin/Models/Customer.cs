using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class Customer
    {
        public Customer()
        {
            this.Orders = new HashSet<Order>();
            this.RecentlyViews = new HashSet<RecentlyView>();
            this.Reviews = new HashSet<Review>();
            this.Wishlists = new HashSet<Wishlist>();
        }

        [Key]
        public int CustomerID { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(50, ErrorMessage = "Tên không được quá 50 ký tự")]
        public string First_Name { get; set; }

        [Display(Name = "Họ")]
        [Required(ErrorMessage = "Vui lòng nhập họ")]
        [StringLength(50, ErrorMessage = "Họ không được quá 50 ký tự")]
        public string Last_Name { get; set; }

        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)] // Ẩn ký tự khi nhập
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        public string Password { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)] // Hiển thị lịch chọn ngày
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateofBirth { get; set; }

        [Display(Name = "Quốc gia")]
        public string Country { get; set; }

        [Display(Name = "Thành phố")]
        public string City { get; set; }

        [Display(Name = "Mã bưu chính")]
        public string PostalCode { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string PicturePath { get; set; }

        [Display(Name = "Trạng thái")]
        public Nullable<bool> status { get; set; }

        [Display(Name = "Lần đăng nhập cuối")]
        public Nullable<System.DateTime> LastLogin { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "Ghi chú")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RecentlyView> RecentlyViews { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}