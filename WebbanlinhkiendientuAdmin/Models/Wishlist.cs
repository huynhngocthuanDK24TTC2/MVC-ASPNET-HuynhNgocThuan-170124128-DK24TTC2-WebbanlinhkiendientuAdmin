using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Cần thêm thư viện này cho [ForeignKey]
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class Wishlist
    {
        [Key]
        public int WishlistID { get; set; }

        [Display(Name = "Khách hàng")]
        [Required(ErrorMessage = "Vui lòng xác định khách hàng")]
        public int CustomerID { get; set; }

        [Display(Name = "Sản phẩm")]
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int ProductID { get; set; }

        
        [Display(Name = "Ngày thêm")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        // THỐNG NHẤT: Dùng Nullable<bool> giống bảng Category và Product
        [Display(Name = "Kích hoạt")]
        public Nullable<bool> isActive { get; set; }

        // Định nghĩa khóa ngoại rõ ràng
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}