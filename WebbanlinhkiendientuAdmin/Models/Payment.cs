using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class Payment
    {
        public Payment()
        {
            this.Orders = new HashSet<Order>();
        }

        [Key]
        [Display(Name = "Mã giao dịch")]
        public int PaymentID { get; set; }

        [Display(Name = "Loại thanh toán")]
        [Required(ErrorMessage = "Vui lòng chọn loại thanh toán")]
        public int Type { get; set; }

        [Display(Name = "Số tiền ghi có")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = true)]
        public Nullable<decimal> CreditAmount { get; set; }

        [Display(Name = "Số tiền ghi nợ")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = true)]
        public Nullable<decimal> DebitAmount { get; set; }

        [Display(Name = "Số dư")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Balance { get; set; }

        [Display(Name = "Thời gian thanh toán")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> PaymentDateTime { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual PaymentType PaymentType { get; set; }
    }
}