using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebbanlinhkiendientuAdmin
{
    /// <summary>
    /// Cấu hình định tuyến (Route) cho toàn bộ website
    /// Tác giả: Huỳnh Ngọc Thuận - DK24TTC2
    /// Đề tài: Website bán linh kiện điện tử - ShopHNT
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Đăng ký các route cho ứng dụng
        /// </summary>
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Bỏ qua các file tài nguyên tĩnh (script, css, axd...)
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            

            // Route chính - Trang khách hàng (Client)
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",      // Trang mặc định khi vào web
                    action = "Index",         // Action mặc định
                    id = UrlParameter.Optional // id là tùy chọn
                },
                namespaces: new[] { "WebbanlinhkiendientuAdmin.Controllers" }
            ).DataTokens["area"] = ""; // Đảm bảo route thuộc area trống (Client)
        }
    }
}