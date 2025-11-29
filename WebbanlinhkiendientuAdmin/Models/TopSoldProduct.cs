using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Cần thêm thư viện này
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public class TopSoldProduct
    {
        [Display(Name = "Sản phẩm")]
        public Product Product { get; set; } 

        [Display(Name = "Số lượng đã bán")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)] // Định dạng số (vd: 1,000)
        public int CountSold { get; set; }
    }
}