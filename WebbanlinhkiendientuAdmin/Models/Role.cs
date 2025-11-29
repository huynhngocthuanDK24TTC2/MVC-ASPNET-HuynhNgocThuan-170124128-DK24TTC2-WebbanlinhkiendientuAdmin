using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebbanlinhkiendientuAdmin.Models
{
    public partial class Role
    {
        public Role()
        {
            this.admin_Login = new HashSet<admin_Login>();
        }

        [Key]
        public int RoleID { get; set; }

        [Display(Name = "Tên quyền")]
        [Required(ErrorMessage = "Vui lòng nhập tên quyền")]
        [StringLength(50, ErrorMessage = "Tên quyền không được quá 50 ký tự")]
        public string RoleName { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)] 
        public string Description { get; set; }

        public virtual ICollection<admin_Login> admin_Login { get; set; }
    }
}