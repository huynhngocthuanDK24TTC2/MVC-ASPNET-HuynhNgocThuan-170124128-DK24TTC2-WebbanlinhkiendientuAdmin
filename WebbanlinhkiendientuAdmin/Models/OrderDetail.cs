using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class OrderDetail
    {
        [Key]
        public int OrderDetailsID { get; set; }

        [Display(Name = "Mã đơn hàng")]
        public int OrderID { get; set; }

        [Display(Name = "Sản phẩm")]
        public int ProductID { get; set; }

        [Display(Name = "Đơn giá")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = true)]
        
        public Nullable<decimal> UnitPrice { get; set; }

        [Display(Name = "Số lượng")]
        public Nullable<int> Quantity { get; set; }

        [Display(Name = "Giảm giá")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Discount { get; set; }

        [Display(Name = "Thành tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = true)]
        public Nullable<decimal> TotalAmount { get; set; }

        [Display(Name = "Ngày đặt")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> OrderDate { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}