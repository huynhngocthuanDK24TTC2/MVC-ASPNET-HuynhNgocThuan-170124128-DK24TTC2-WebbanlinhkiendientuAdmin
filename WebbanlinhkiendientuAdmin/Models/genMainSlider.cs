using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class genMainSlider
    {
        [Key]
        public int MainSliderID { get; set; }

        [Display(Name = "Đường dẫn ảnh")]
        [Required(ErrorMessage = "Vui lòng nhập đường dẫn ảnh hoặc upload ảnh")]
        public string ImageURL { get; set; }

        [Display(Name = "Văn bản thay thế (Alt)")]
        [StringLength(200, ErrorMessage = "Văn bản thay thế không được quá 200 ký tự")]
        public string AltText { get; set; }

        [Display(Name = "Nhãn ưu đãi (Tag)")]
        [StringLength(50, ErrorMessage = "Nhãn ưu đãi quá dài")]
        public string OfferTag { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
        public string Title { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)] 
        public string Description { get; set; }

        [Display(Name = "Nội dung nút bấm")]
        [StringLength(50, ErrorMessage = "Nội dung nút bấm quá dài")]
        public string BtnText { get; set; }

        [Display(Name = "Trạng thái xóa")]
        public Nullable<bool> isDeleted { get; set; }
    }
}