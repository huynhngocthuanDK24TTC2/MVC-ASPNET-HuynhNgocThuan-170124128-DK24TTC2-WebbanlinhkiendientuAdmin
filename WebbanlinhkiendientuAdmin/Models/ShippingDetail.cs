using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class ShippingDetail
    {
        public ShippingDetail()
        {
            this.Orders = new HashSet<Order>();
        }

        [Key]
        public int ShippingID { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        [StringLength(50, ErrorMessage = "Tên không được quá 50 ký tự")]
        public string FirstName { get; set; }

        [Display(Name = "Họ")]
        [Required(ErrorMessage = "Vui lòng nhập họ người nhận")]
        [StringLength(50, ErrorMessage = "Họ không được quá 50 ký tự")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ (Phải bắt đầu bằng số 0 và có 10-11 số)")]
        public string Mobile { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [DataType(DataType.MultilineText)] 
        public string Address { get; set; }

        [Display(Name = "Thành phố")]
        [Required(ErrorMessage = "Vui lòng nhập thành phố")]
        public string City { get; set; }

        [Display(Name = "Mã bưu chính")]
        public string PostCode { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}