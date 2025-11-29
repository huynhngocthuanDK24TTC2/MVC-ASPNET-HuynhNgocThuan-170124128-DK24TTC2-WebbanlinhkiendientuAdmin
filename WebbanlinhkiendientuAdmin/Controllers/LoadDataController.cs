using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebbanlinhkiendientuAdmin.Models;

namespace WebbanlinhkiendientuAdmin.Controllers
{
    // Đổi tên class thành Helper để đúng ý nghĩa (vì đây không phải là Controller thực thụ)
    public static class LayoutHelper
    {
        // Hàm mở rộng (Extension Method) để các Controller khác có thể gọi: this.GetDefaultData()
        public static List<OrderDetail> GetDefaultData(this ControllerBase controller)
        {
            // 1. LẤY GIỎ HÀNG TỪ SESSION (Thay vì TempShpData)
            var sessionCart = HttpContext.Current.Session["Cart"];
            List<OrderDetail> data = new List<OrderDetail>();

            if (sessionCart != null)
            {
                data = (List<OrderDetail>)sessionCart;
            }
            else
            {
                // Nếu chưa có giỏ thì tạo list rỗng
                data = new List<OrderDetail>();
            }

            // 2. GÁN DỮ LIỆU VÀO VIEWBAG (để hiển thị lên Header/Menu)
            controller.ViewBag.cartBox = data.Count == 0 ? null : data;
            controller.ViewBag.NoOfItem = data.Count;

            // Tính tổng tiền
            // Đảm bảo TotalAmount trong OrderDetail đã được tính toán trước đó
            decimal subTotal = 0;
            if (data.Count > 0)
            {
                // Dùng nullable decimal để tránh lỗi nếu TotalAmount bị null
                subTotal = data.Sum(x => x.TotalAmount ?? 0);
            }

            controller.ViewBag.Total = subTotal;
            controller.ViewBag.SubTotal = subTotal;

            // Giảm giá (Hiện tại set cứng = 0, sau này có thể lấy từ Session Voucher)
            decimal discount = 0;
            controller.ViewBag.Discount = discount;
            controller.ViewBag.TotalAmount = subTotal - discount;

            // 3. LẤY SỐ LƯỢNG YÊU THÍCH (Wishlist)
            int wishlistCount = 0;

            // Kiểm tra xem User đã đăng nhập chưa
            if (HttpContext.Current.Session["UserID"] != null)
            {
                int userId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);

                // KHỞI TẠO DB CONTEXT MỚI (Không dùng static)
                using (var db = new WebbanlinhkiendientuAdminDbModels())
                {
                    wishlistCount = db.Wishlists.Count(x => x.CustomerID == userId);
                }
            }

            controller.ViewBag.WlItemsNo = wishlistCount;

            return data;
        }
    }
}