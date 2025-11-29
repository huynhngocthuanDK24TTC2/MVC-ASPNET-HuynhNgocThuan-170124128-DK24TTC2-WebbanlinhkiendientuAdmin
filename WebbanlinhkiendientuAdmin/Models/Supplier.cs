using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebbanlinhkiendientuAdmin.Models
{
    [Table("Suppliers")] 
    public partial class Supplier
    {
        public Supplier()
        {
            this.Products = new HashSet<Product>();
            
            this.isActive = true;
        }

        [Key]
        public int SupplierID { get; set; }

        [Display(Name = "Tên công ty")]
        [Required(ErrorMessage = "Vui lòng nhập tên nhà cung cấp")]
        public string CompanyName { get; set; }

        [Display(Name = "Người liên hệ")]
        public string ContactName { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Trạng thái")]
        public Nullable<bool> isActive { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}