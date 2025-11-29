using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class Category
    {
        public Category()
        {
            this.genPromoRights = new HashSet<genPromoRight>();
            this.Products = new HashSet<Product>();
            this.SubCategories = new HashSet<SubCategory>();
        }

        [Key]
        public int CategoryID { get; set; }

        [Display(Name = "Tên danh mục")]
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được quá 100 ký tự")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)] // Hiển thị dạng khung văn bản nhiều dòng
        public string Description { get; set; }

        [Display(Name = "Trạng thái kích hoạt")]
        public Nullable<bool> isActive { get; set; }

        public virtual ICollection<genPromoRight> genPromoRights { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}