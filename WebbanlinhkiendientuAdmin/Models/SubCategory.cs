using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Cần thiết để dùng [Table]
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    
    
    [Table("SubCategories")]
    public partial class SubCategory
    {
        // --- HÀM KHỞI TẠO (CONSTRUCTOR) ---
        public SubCategory()
        {
            
            this.Products = new HashSet<Product>();

            // Mặc định kích hoạt khi tạo mới
            this.isActive = true;
        }

        // --- CÁC THUỘC TÍNH (PROPERTIES) ---

        [Key] // Khóa chính
        public int SubCategoryID { get; set; }

        [Display(Name = "Danh mục cha")]
        [Required(ErrorMessage = "Vui lòng chọn danh mục cha")]
        public int CategoryID { get; set; }

        [Display(Name = "Tên danh mục con")]
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục con")]
        [StringLength(100, ErrorMessage = "Tên danh mục con không được quá 100 ký tự")]
        public string Name { get; set; }

        
        
        [Display(Name = "Mô tả / Ghi chú")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Trạng thái kích hoạt")]
        public Nullable<bool> isActive { get; set; }

        // --- LIÊN KẾT (RELATIONSHIPS) ---

        // Liên kết đến bảng cha (Category)
        public virtual Category Category { get; set; }

        // Liên kết đến bảng con (Products)
        public virtual ICollection<Product> Products { get; set; }
    }
}