using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class PaymentType
    {
        public PaymentType()
        {
            this.Payments = new HashSet<Payment>();
        }

        [Key]
        public int PayTypeID { get; set; }

        [Display(Name = "Tên loại thanh toán")]
        [Required(ErrorMessage = "Vui lòng nhập tên loại thanh toán")]
        [StringLength(50, ErrorMessage = "Tên loại thanh toán không được quá 50 ký tự")]
        public string TypeName { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)] 
        public string Description { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}