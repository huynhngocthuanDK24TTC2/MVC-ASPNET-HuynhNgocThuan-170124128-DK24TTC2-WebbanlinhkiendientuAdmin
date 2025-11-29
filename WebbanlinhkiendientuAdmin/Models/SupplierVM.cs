using System;
using System.ComponentModel.DataAnnotations;

namespace WebbanlinhkiendientuAdmin.Models
{
    // ViewModel: Dùng để hứng dữ liệu từ View (Form nhập liệu)
    // Giúp tách biệt với Entity gốc, dễ dàng validate dữ liệu
    public class SupplierVM
    {
        
        public SupplierVM()
        {
            
            this.isActive = true;

            // Mặc định ngày tạo là hôm nay
            this.CreatedDate = DateTime.Now;
        }

        [Key]
        public int SupplierID { get; set; }

        // --- THÔNG TIN CHÍNH ---

        [Display(Name = "Tên công ty")]
        [Required(ErrorMessage = "Vui lòng nhập tên công ty")]
        [StringLength(100, ErrorMessage = "Tên công ty không được quá 100 ký tự")]
        public string CompanyName { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ (Bắt đầu bằng 0, dài 10-11 số)")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Địa chỉ")]
        [DataType(DataType.MultilineText)] 
        public string Address { get; set; }

        

        [Display(Name = "Người liên hệ")]
        [StringLength(50, ErrorMessage = "Tên người liên hệ tối đa 50 ký tự")]
        public string ContactName { get; set; }

        [Display(Name = "Chức vụ")]
        [StringLength(50, ErrorMessage = "Chức vụ tối đa 50 ký tự")]
        public string ContactTitle { get; set; }

        [Display(Name = "Thành phố")]
        public string City { get; set; }

        [Display(Name = "Quốc gia")]
        public string Country { get; set; }

        // --- HỆ THỐNG ---

        [Display(Name = "Ngày tạo")]
        [DataType(DataType.Date)]
        // Định dạng hiển thị ngày/tháng/năm
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Trạng thái")]
        public bool isActive { get; set; }
        
    }
}