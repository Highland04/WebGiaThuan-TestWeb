using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using buoi08.Models;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Web.Helpers;
using System.Configuration;

namespace buoi08.Controllers
{
    public class Home44Controller : Controller
    {

        private const string CartSessionKey = "Cart";
        //
        // GET: /Home44/
        public ActionResult Index()
        {

            Hoa hoaModel = new Hoa();
            hoaModel.loadHoa();

            // Truyền danh sách hoa vào view
            return View(hoaModel.danhsach);
        }
        public ActionResult chude(string id)
        {
         
            Hoa data = new Hoa();
            data.loadHoa2(id);
            ChuDe data2 = new ChuDe();
            data2.loadChuDe();
            ViewData["cd"] = data2;
            return View(data);

        }
        public ActionResult Login()
        {
            System.Diagnostics.Debug.WriteLine($"Login: {Session["Login"]}, MaKH: {Session["MaKH"]}");

            string u = Request.Form["username"];
            string p = Request.Form["password"];
            if (u == null)
                return View();

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ketnoi1"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                if (conn == null)
                    return Content("No connect");

                // Chỉnh sửa câu truy vấn SQL để kiểm tra Email và MatKhau
                string sql = "SELECT * FROM KhachHang WHERE Email = @Email AND MatKhau = @MatKhau";

                SqlCommand cmm = new SqlCommand(sql, conn);
                cmm.Parameters.AddWithValue("@Email", u);
                cmm.Parameters.AddWithValue("@MatKhau", p);

                SqlDataReader data = cmm.ExecuteReader();
                if (data.HasRows)
                {
                    data.Read();
                    int maKH = (int)data["MaKH"]; // Lấy giá trị MaKH từ DataReader
                    Session["Login"] = data["HoTen"].ToString();
                    Session["MaKH"] = maKH; // Lưu MaKH vào Session
                                            // Không gửi email ở đây, chỉ lưu MaKH vào Session
                                            //return RedirectToAction("Index", "Home44");
                    if (Session["Login"] != null && Session["Login"]== "admin" && Session["MaKH"] =="123")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home44");
                    }
                }
                else 
                {
                    ViewBag.Message = "Thông tin sai!";
                }
            }
            return View();
        }

        public ActionResult Register(FormCollection form)
        {
            
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ketnoi1"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = "INSERT INTO KhachHang ( HoTen, NgaySinh, GioiTinh, DienThoai, Email, DiaChi, MatKhau, VaiTro) " +
                                 "VALUES (@HoTen, @NgaySinh, @GioiTinh, @DienThoai, @Email, @DiaChi, @MatKhau, @VaiTro)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // cmd.Parameters.AddWithValue("@MaKH", Convert.ToInt32(form["MaKH"])); // Loại bỏ dòng này nếu MaKH là AUTO_INCREMENT

                        cmd.Parameters.AddWithValue("@HoTen", form["hoten"]);
                        cmd.Parameters.AddWithValue("@NgaySinh", Convert.ToDateTime(form["date"]));
                        cmd.Parameters.AddWithValue("@GioiTinh", form["gender"]);
                        cmd.Parameters.AddWithValue("@DienThoai", form["dienthoai"]);
                        cmd.Parameters.AddWithValue("@Email", form["email"]);
                        cmd.Parameters.AddWithValue("@DiaChi", form["diachi"]);
                        cmd.Parameters.AddWithValue("@MatKhau", form["psw"]);
                        cmd.Parameters.AddWithValue("@VaiTro", form["vaitro"]);

                        cmd.ExecuteNonQuery();
                    }

                    ViewBag.Message = "Đăng ký thành công!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Có lỗi xảy ra: " + ex.Message;
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult Logout()
        {
            Session["Login"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult Lienhe(Lienhe model) {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ketnoi1"].ConnectionString;
            //string connectionString = @"Server=MSI\SQLEXPRESS; Database=Doanltweb2;User ID=sa; Password=123; Integrated Security=True;";
            if (ModelState.IsValid)
            {
                // Gửi email
                try
                {
                    var mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(model.Email);
                    mailMessage.To.Add("lamhoang.obt@gmail.com"); // Địa chỉ email nhận
                    mailMessage.Subject = "Thông tin liên hệ từ " + model.Name;
                    mailMessage.Body = model.Message;

                    using (var smtpClient = new SmtpClient("smtp.example.com")) // Cấu hình SMTP server
                    {
                        smtpClient.Credentials = new NetworkCredential("lamhoang.obt@gmail.com", "lamkute_123");
                        smtpClient.Send(mailMessage);
                    }

                    ViewBag.Message = "Gửi thành công!";
                }
                catch (Exception)
                {
                    ViewBag.Message = "Đã xảy ra lỗi trong quá trình gửi email.";
                }

                // Lưu thông tin vào cơ sở dữ liệu
                try
                {

                    using (var connection = new SqlConnection(connStr))
                    {
                        string query = "INSERT INTO LienHe (Ten, Gmail, TinNhan, MaKH) VALUES (@Ten, @Gmail, @TinNhan, @MaKH)";
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Ten", model.Name);
                            command.Parameters.AddWithValue("@Gmail", model.Email);
                            command.Parameters.AddWithValue("@TinNhan", model.Message);
                            command.Parameters.AddWithValue("@MaKH", DBNull.Value); // hoặc sử dụng MaKH tương ứng nếu có

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có vấn đề khi lưu vào cơ sở dữ liệu
                    ViewBag.DbError = "Đã xảy ra lỗi khi lưu vào cơ sở dữ liệu: " + ex.Message;
                }
            }

            return View("Lienhe");
        }

        public ActionResult ThanhToan()
        {
            var cart = GetCart();
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart"); // Chuyển hướng nếu giỏ hàng rỗng
            }

            var viewModel = new ThanhToanViewModel
            {
                CartItems = cart.Items,
                TongTien = cart.GetTotal(),
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToan(ThanhToanViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Lấy MaKH từ session
                int maKH = GetLoggedInCustomerId();
                if (maKH == 0) // Nếu không có MaKH, redirect đến trang đăng nhập
                {
                    return RedirectToAction("Login", "Home44");
                }

                // Lấy MaPhuongThuc từ form
                int maPhuongThuc = model.MaPhuongThuc;

                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ketnoi1"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Thêm vào bảng DonHang
                            string sql = "INSERT INTO DonHang (NgayGiao, MaKH, ThanhTien, NgayLap, MaPhuongThuc, HoTenNgNhan, GioitinhNgNhan, SDTNgNhan, LoiChuc) " +
                                         "OUTPUT INSERTED.MaDonHang " +
                                         "VALUES (@NgayGiao, @MaKH, @ThanhTien, @NgayLap, @MaPhuongThuc, @HoTenNgNhan, @GioitinhNgNhan, @SDTNgNhan, @LoiChuc)";

                            int maDonHang;
                            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@NgayGiao", model.NgayGiao);
                                cmd.Parameters.AddWithValue("@MaKH", maKH);
                                cmd.Parameters.AddWithValue("@ThanhTien", model.TongTien);
                                cmd.Parameters.AddWithValue("@NgayLap", DateTime.Now);
                                cmd.Parameters.AddWithValue("@MaPhuongThuc", maPhuongThuc);
                                cmd.Parameters.AddWithValue("@HoTenNgNhan", model.HoTenNgNhan);
                                cmd.Parameters.AddWithValue("@GioitinhNgNhan", model.GioitinhNgNhan);
                                cmd.Parameters.AddWithValue("@SDTNgNhan", model.SDTNgNhan);
                                cmd.Parameters.AddWithValue("@LoiChuc", model.LoiChuc);

                                maDonHang = (int)cmd.ExecuteScalar(); // Lấy MaDonHang vừa được thêm vào
                            }

                            // Lưu chi tiết đơn hàng
                            if (model.CartItems != null)
                            {
                                foreach (var item in model.CartItems)
                                {
                                    string sqlChiTiet = "INSERT INTO ChiTietDonHang (MaDonHang, MaHoa, SoLuong, DonGia) " +
                                                        "VALUES (@MaDonHang, @MaHoa, @SoLuong, @DonGia)";
                                    using (SqlCommand cmdChiTiet = new SqlCommand(sqlChiTiet, conn, transaction))
                                    {
                                        cmdChiTiet.Parameters.AddWithValue("@MaDonHang", maDonHang);
                                        cmdChiTiet.Parameters.AddWithValue("@MaHoa", item.ProductId);
                                        cmdChiTiet.Parameters.AddWithValue("@SoLuong", item.Quantity);
                                        cmdChiTiet.Parameters.AddWithValue("@DonGia", item.Price);

                                        cmdChiTiet.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();

                            // Xóa giỏ hàng sau khi thanh toán thành công
                            Session[CartSessionKey] = null;

                            // Gửi email cảm ơn
                            SendEmail(maKH);

                            return RedirectToAction("PaymentSuccess");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ViewBag.Message = "Có lỗi xảy ra: " + ex.Message;
                        }
                    }
                }
            }

            return View(model);

        }


        // Phương thức lấy giỏ hàng từ session
        private Cart GetCart()
        {
            var cart = Session[CartSessionKey] as Cart;
            if (cart == null)
            {
                cart = new Cart();
                Session[CartSessionKey] = cart;
            }
            return cart;
        }

        private int GetLoggedInCustomerId()
        {
            if (Session["MaKH"] != null)
            {
                return (int)Session["MaKH"];
            }
            else
            {
                return 0; // Trả về 0 nếu khách hàng chưa đăng nhập
            }
        }


        public ActionResult Tintuc()
        {
            return View();
        }
        public ActionResult httt()
        {
            return View();
        }
        public ActionResult csgh()
        {
            return View();
        }
        public ActionResult csbm()
        {
            return View();
        }
        public ActionResult dksd()
        {
            return View();
        }
        public ActionResult bht()
        {
            Hoa hoaModel = new Hoa();
            hoaModel.loadHoa();
            hoaModel.SortHoaByName();

            // Truyền danh sách hoa vào view
            return View(hoaModel.danhsach);
        }

        public ActionResult kehoa()
        {
            Hoa hoaModel = new Hoa();
            hoaModel.loadHoa();
            hoaModel.SortHoaByName();

            // Truyền danh sách hoa vào view
            return View(hoaModel.danhsach);
        }

        public ActionResult ghcm()
        {
            Hoa hoaModel = new Hoa();
            hoaModel.loadHoa();
            hoaModel.SortHoaByName();

            // Truyền danh sách hoa vào view
            return View(hoaModel.danhsach);
        }
        public ActionResult hoacuoi()
        {
            Hoa hoaModel = new Hoa();
            hoaModel.loadHoa();
            hoaModel.SortHoaByName();

            // Truyền danh sách hoa vào view
            return View(hoaModel.danhsach);
        }
        public ActionResult hoasap()
        {
            Hoa hoaModel = new Hoa();
            hoaModel.loadHoa();
            hoaModel.SortHoaByName();

            // Truyền danh sách hoa vào view
            return View(hoaModel.danhsach);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompletePayment(ThanhToanViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Xử lý thanh toán ở đây, ví dụ: lưu vào cơ sở dữ liệu, gửi email xác nhận, v.v.

                // Chuyển hướng đến trang thông báo thanh toán thành công
                return RedirectToAction("PaymentSuccess");
            }
            else
            {
                // Nếu có lỗi, quay lại trang thanh toán với thông báo lỗi
                return View("ThanhToan", model);
            }
        }

        public ActionResult PaymentSuccess()
        {
            return View();
        }

        public ActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        private void SendEmail(int maKH)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ketnoi1"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT Email FROM KhachHang WHERE MaKH = @MaKH";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaKH", maKH);

                    var email = (string)cmd.ExecuteScalar();
                    if (!string.IsNullOrEmpty(email))
                    {
                        try
                        {
                            var fromAddress = new MailAddress(ConfigurationManager.AppSettings["FromEmailAddress"], ConfigurationManager.AppSettings["FromEmailDisplayName"]);
                            var toAddress = new MailAddress(email);
                            string subject = "Thank You For Your Order";
                            string body = @"
 <html>
       <head>
<style>
                 body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            line-height: 1.6;
            color: #333;
            background-color: #f4f4f4;
        }
        .container {
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 10px;
            background-color: #fff;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        .header {
            background-color: #4CAF50;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 10px 10px 0 0;
            font-size: 24px;
        }
        .content {
            padding: 20px;
            font-size: 16px;
        }
        .content p {
            margin: 0 0 10px;
        }
        .content a {
            color: #4CAF50;
            text-decoration: none;
        }
        .footer {
            background-color: #f1f1f1;
            text-align: center;
            padding: 10px;
            border-radius: 0 0 10px 10px;
            font-size: 14px;
            color: #777;
        }
        .footer a {
            color: #4CAF50;
            text-decoration: none;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            Thank You For Your Order!
        </div>
        <div class='content'>
            <p>Dear Valued Customer,</p>
            <p>Thank you for your recent purchase! We truly appreciate your business and are delighted to have you as a customer.</p>
            <p>Your order has been received and is being processed. We hope you enjoy your new products and have a wonderful experience.</p>
            <p>If you have any questions or need further assistance, please don't hesitate to contact us:</p>
            <p>Email: <a href='mailto:lehongductri2911@gmail.com'>lehongductri2911@gmail.com</a></p>
            <p>Phone: 0386826641</p>
        </div>
        <div class='footer'>
            <p>© 2024 Your Company. All rights reserved.</p>
            <p>Visit us at <a href='https://yourcompanywebsite.com'>our website</a></p>
        </div>
    </div>
</body>
</html>";

                            using (var smtpClient = new SmtpClient
                            {
                                Host = ConfigurationManager.AppSettings["SMTPHost"],
                                Port = int.Parse(ConfigurationManager.AppSettings["SMTPPort"]),
                                EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnabledSSL"]),
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false,
                                Credentials = new NetworkCredential(fromAddress.Address, ConfigurationManager.AppSettings["FromEmailPassword"])
                            })
                            {
                                using (var message = new MailMessage(fromAddress, toAddress)
                                {
                                    Subject = subject,
                                    Body = body,
                                    IsBodyHtml = true // Đây là phần quan trọng để email hiển thị HTML đúng cách
                                })
                                {
                                    smtpClient.Send(message);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log lỗi nếu cần
                        }
                    }
                }
            }
        }
    }
}