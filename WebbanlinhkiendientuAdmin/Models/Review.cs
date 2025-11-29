using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [Display(Name = "Khách hàng")]
        public Nullable<int> CustomerID { get; set; }

        [Display(Name = "Sản phẩm")]
        public Nullable<int> ProductID { get; set; }

        [Display(Name = "Tên người gửi")]
        [StringLength(50, ErrorMessage = "Tên không được quá 50 ký tự")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Nội dung đánh giá")]
        [DataType(DataType.MultilineText)] 
        public string Review1 { get; set; }

        [Display(Name = "Điểm đánh giá (Sao)")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5 sao")]
        public Nullable<int> Rate { get; set; }

        [Display(Name = "Thời gian gửi")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateTime { get; set; }

        [Display(Name = "Đã xóa")]
        public Nullable<bool> isDelete { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}