namespace WebbanlinhkiendientuAdmin.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebbanlinhkiendientuAdmin.Models;

    
    internal sealed class Configuration : DbMigrationsConfiguration<WebbanlinhkiendientuAdmin.Models.WebbanlinhkiendientuAdminDbModels>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        
        protected override void Seed(WebbanlinhkiendientuAdmin.Models.WebbanlinhkiendientuAdminDbModels context)
        {
            // 1. Tạo quyền (Role) Admin nếu chưa có
            context.Roles.AddOrUpdate(x => x.RoleName,
                new Role { RoleID = 1, RoleName = "Admin", Description = "Quản trị viên toàn quyền" },
                new Role { RoleID = 2, RoleName = "Staff", Description = "Nhân viên bán hàng" }
            );

            context.SaveChanges();

            // 2. Tạo thông tin Nhân viên Admin (EmpID = 1)
            context.admin_Employee.AddOrUpdate(x => x.Email,
                new admin_Employee
                {
                    EmpID = 1,
                    FirstName = "Quản Trị",
                    LastName = "Hệ Thống",
                    Email = "admin@gmail.com",
                    Phone = "0909123456",
                    Address = "TP. Hồ Chí Minh",
                    DateofBirth = new DateTime(1990, 1, 1),
                    Gender = "Nam",
                    PicturePath = "/Images/UserAvatar/no-image.jpg"
                }
            );

            context.SaveChanges();

            // 3. Tạo tài khoản đăng nhập cho Admin
            context.admin_Login.AddOrUpdate(x => x.UserName,
                new admin_Login
                {
                    UserName = "admin",
                    Password = "123456",
                    EmpID = 1,
                    RoleType = 1,
                    Notes = "Tài khoản quản trị cao cấp"
                }
            );

            // 4. Tạo danh mục mẫu
            context.Categories.AddOrUpdate(x => x.Name,
                new Category { Name = "Linh kiện bán dẫn", Description = "IC, Transistor, Diode...", isActive = true },
                new Category { Name = "Cảm biến", Description = "Cảm biến nhiệt, ánh sáng...", isActive = true },
                new Category { Name = "Module ứng dụng", Description = "Arduino, ESP8266...", isActive = true }
            );

            context.SaveChanges();
        }
    }
}