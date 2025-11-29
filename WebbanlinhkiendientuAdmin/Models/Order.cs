using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class Order
    {
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        [Display(Name = "Mã ĐH")]
        public int OrderID { get; set; }

        
        [Display(Name = "Mã khách hàng")]
        public Nullable<int> CustomerID { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public Nullable<int> PaymentID { get; set; }

        [Display(Name = "Mã vận đơn")]
        public Nullable<int> ShippingID { get; set; }

        [Display(Name = "Giảm giá")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = false)]
        public Nullable<decimal> Discount { get; set; }

        [Display(Name = "Thuế")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = false)]
        public Nullable<decimal> Taxes { get; set; }

        [Display(Name = "Tổng tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = false)]
        public Nullable<decimal> TotalAmount { get; set; }

        // ---------------------------------------------------------------

        [Display(Name = "Đã hoàn thành")]
        public Nullable<bool> isCompleted { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> OrderDate { get; set; }

        [Display(Name = "Trạng thái giao hàng")] 
        public Nullable<bool> Dispatched { get; set; }

        [Display(Name = "Ngày giao đi")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DispatchedDate { get; set; }

        [Display(Name = "Đang vận chuyển")]
        public Nullable<bool> Shipped { get; set; }

        [Display(Name = "Ngày vận chuyển")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ShippingDate { get; set; }

        [Display(Name = "Đã nhận hàng")]
        public Nullable<bool> Deliver { get; set; }

        [Display(Name = "Ngày nhận")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DeliveryDate { get; set; }

        
        [Display(Name = "Ghi chú đơn hàng")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        [Display(Name = "Đã hủy")]
        public Nullable<bool> CancelOrder { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ShippingDetail ShippingDetail { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}