using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class RecentlyView
    {
        [Key]
        public int RViewID { get; set; }

        [Display(Name = "Khách hàng")]
        public int CustomerID { get; set; }

        [Display(Name = "Sản phẩm")]
        public int ProductID { get; set; }

        [Display(Name = "Thời gian xem")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public System.DateTime ViewDate { get; set; }

        [Display(Name = "Ghi chú")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}