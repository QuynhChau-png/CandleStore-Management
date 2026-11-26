using System.Data;
using Microsoft.Data.SqlClient;
using CandleStore.DTO;

namespace CandleStore.DAO
{
    public class HoaDonDAO
    {
        // Admin lấy toàn bộ lịch sử đơn hàng của mọi nhân viên để báo cáo doanh thu
        public DataTable GetAllInvoices()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = @"SELECT 'HD' + RIGHT('000' + CAST(h.MaHD AS VARCHAR), 3) AS MaHD,h.NgayLap, 
                               ISNULL(k.TenKH, N'Khách vãng lai') AS TenKhachHang, 
                               n.TenNguoiDung AS NhanVienLap, 
                               h.TongTien 
                        FROM HoaDon h
                        LEFT JOIN KhachHang k ON h.MaKH = k.MaKH
                        LEFT JOIN NguoiDung n ON h.MaND = n.MaND
                        ORDER BY h.NgayLap DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // 1. Chốt đơn hàng và tự động trừ số lượng tồn kho vật lý (Transaction)
        public int CreateInvoice(HoaDonDTO hd, List<ChiTietHoaDonDTO> listDetails)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    string sqlHD = @"INSERT INTO HoaDon 
                            (NgayLap, MaKH, MaND, TongTien) 
                            VALUES (@ngay, @makh, @mand, @tongtien); 
                            SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdHD = new SqlCommand(sqlHD, conn, trans);

                    cmdHD.Parameters.AddWithValue("@ngay", hd.NgayLap);
                    cmdHD.Parameters.AddWithValue("@makh",
                        hd.MaKH.HasValue ? hd.MaKH.Value : DBNull.Value);

                    cmdHD.Parameters.AddWithValue("@mand",
                        hd.MaND.HasValue ? hd.MaND.Value : DBNull.Value);

                    cmdHD.Parameters.AddWithValue("@tongtien", hd.TongTien);

                    int newInvoiceId = Convert.ToInt32(cmdHD.ExecuteScalar());

                    foreach (var detail in listDetails)
                    {
                        string sqlCT = @"INSERT INTO ChiTietHoaDon
                                (MaHD, MaSP, SoLuong, DonGia)
                                VALUES (@mahd, @masp, @soluong, @dongia)";

                        SqlCommand cmdCT = new SqlCommand(sqlCT, conn, trans);

                        cmdCT.Parameters.AddWithValue("@mahd", newInvoiceId);
                        cmdCT.Parameters.AddWithValue("@masp", detail.MaSP);
                        cmdCT.Parameters.AddWithValue("@soluong", detail.SoLuong);
                        cmdCT.Parameters.AddWithValue("@dongia", detail.DonGia);

                        cmdCT.ExecuteNonQuery();

                        string sqlUpdateStock = @"UPDATE SanPham
                                          SET SoLuongTon = SoLuongTon - @soluong
                                          WHERE MaSP = @masp";

                        SqlCommand cmdStock = new SqlCommand(sqlUpdateStock, conn, trans);

                        cmdStock.Parameters.AddWithValue("@soluong", detail.SoLuong);
                        cmdStock.Parameters.AddWithValue("@masp", detail.MaSP);

                        cmdStock.ExecuteNonQuery();
                    }

                    trans.Commit();

                    hd.MaHD = newInvoiceId;

                    return newInvoiceId;
                }
                catch
                {
                    trans.Rollback();
                    return -1;
                }
            }
        }

        // 2. Nhân viên chỉ được quyền xem lịch sử hóa đơn do mình lập
        public DataTable GetInvoicesByStaff(int maND)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = @"SELECT 'HD' + RIGHT('000' + CAST(h.MaHD AS VARCHAR), 3) AS MaHD,h.NgayLap, 
                               ISNULL(k.TenKH, N'Khách vãng lai') AS TenKhachHang, 
                               h.TongTien 
                        FROM HoaDon h
                        LEFT JOIN KhachHang k ON h.MaKH = k.MaKH
                        WHERE h.MaND = @maND
                        ORDER BY h.NgayLap DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@maND", maND);
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public Dictionary<string, object> GetDashboardStats()
        {
            var stats = new Dictionary<string, object>();
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = @"SELECT 
            (SELECT COUNT(*) FROM SanPham) AS TotalProducts,
            (SELECT COUNT(*) FROM HoaDon) AS TotalInvoices,
            ISNULL((SELECT SUM(TongTien) FROM HoaDon WHERE CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE)), 0) AS TodayRevenue,
            (SELECT COUNT(*) FROM KhachHang) AS TotalCustomers";

                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        stats["TotalProducts"] = rd["TotalProducts"];
                        stats["TotalInvoices"] = rd["TotalInvoices"];
                        stats["TodayRevenue"] = rd["TodayRevenue"];
                        stats["TotalCustomers"] = rd["TotalCustomers"];
                    }
                }
            }
            return stats;
        }

        public DataTable GetRevenueByDay()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = @"SELECT CAST(NgayLap AS DATE) AS Ngay,
                                SUM(TongTien) AS DoanhThu
                            FROM HoaDon
                            GROUP BY CAST(NgayLap AS DATE)
                            ORDER BY Ngay ASC";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }

            return dt;
        }
    }
}