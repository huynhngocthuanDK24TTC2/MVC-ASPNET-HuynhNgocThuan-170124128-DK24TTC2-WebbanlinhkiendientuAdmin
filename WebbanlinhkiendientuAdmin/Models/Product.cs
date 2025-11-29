using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc; 

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class Product
    {
        public Product()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.RecentlyViews = new HashSet<RecentlyView>();
            this.Reviews = new HashSet<Review>();
            this.Wishlists = new HashSet<Wishlist>();
        }

        [Key]
        public int ProductID { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được quá 200 ký tự")]
        public string Name { get; set; }

        [Display(Name = "Nhà cung cấp")]
        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        public int SupplierID { get; set; }

        [Display(Name = "Danh mục")]
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryID { get; set; }

        [Display(Name = "Danh mục con")]
        public Nullable<int> SubCategoryID { get; set; }

        [Display(Name = "Đơn giá")]
        [Required(ErrorMessage = "Vui lòng nhập đơn giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = true)]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Giá cũ")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá cũ phải lớn hơn hoặc bằng 0")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = true)]
        public Nullable<decimal> OldPrice { get; set; }

        [Display(Name = "Giảm giá")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = true)]
        public Nullable<decimal> Discount { get; set; }

        [Display(Name = "Số lượng tồn")]
        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không hợp lệ")]
        public Nullable<int> UnitInStock { get; set; }

        [Display(Name = "Đang kinh doanh")]
        public Nullable<bool> ProductAvailable { get; set; }

        [Display(Name = "Mô tả ngắn")]
        [DataType(DataType.MultilineText)] 
        
        
        public string ShortDescription { get; set; }

        [Display(Name = "Hình ảnh")]
        public string PicturePath { get; set; }

        [Display(Name = "Ghi chú")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<RecentlyView> RecentlyViews { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}