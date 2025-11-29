using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // 1. Đăng ký các thành phần MVC cơ bản
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // 2. CẤU HÌNH BẢO VỆ DỮ LIỆU (QUAN TRỌNG)
            // Thay vì dùng DropCreate..., ta dùng null để tắt tính năng tự động can thiệp Database.
            // Điều này giúp dữ liệu cũ được giữ nguyên.
            Database.SetInitializer<WebbanlinhkiendientuAdminDbModels>(null);
        }
    }
}