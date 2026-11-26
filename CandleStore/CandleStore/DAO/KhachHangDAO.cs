using System;
using Microsoft.Data.SqlClient;
using CandleStore.DTO;

namespace CandleStore.DAO
{
    public class KhachHangDAO
    {
        // 1. Tìm thông tin khách thành viên dựa vào Số điện thoại
        public KhachHangDTO? GetByPhone(string sdt)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = "SELECT * FROM KhachHang WHERE SDT = @sdt";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@sdt", sdt);
                
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return new KhachHangDTO {
                            MaKH = (int)rd["MaKH"],
                            TenKH = rd["TenKH"].ToString() ?? string.Empty,
                            SDT = rd["SDT"].ToString() ?? string.Empty,
                            DiaChi = rd["DiaChi"].ToString() ?? string.Empty
                        };
                    }
                }
            }
            return null; // Khách mới, chưa từng mua hàng
        }

        // 2. Thêm nhanh thông tin khách hàng mới tại quầy thu ngân
        public bool Insert(KhachHangDTO kh)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = "INSERT INTO KhachHang (TenKH, SDT, DiaChi) VALUES (@ten, @sdt, @diachi)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ten", kh.TenKH);
                cmd.Parameters.AddWithValue("@sdt", kh.SDT);
                cmd.Parameters.AddWithValue("@diachi", string.IsNullOrEmpty(kh.DiaChi) ? DBNull.Value : kh.DiaChi);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<KhachHangDTO> GetAll()
{
    List<KhachHangDTO> list = new List<KhachHangDTO>();
    using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
    {
        string sql = "SELECT * FROM KhachHang";
        SqlCommand cmd = new SqlCommand(sql, conn);
        conn.Open();
        using (SqlDataReader rd = cmd.ExecuteReader())
        {
            while (rd.Read())
            {
                list.Add(new KhachHangDTO {
                    MaKH = (int)rd["MaKH"],
                    TenKH = rd["TenKH"].ToString() ?? string.Empty,
                    SDT = rd["SDT"].ToString() ?? string.Empty,
                    DiaChi = rd["DiaChi"].ToString() ?? string.Empty
                });
            }
        }
    }
    return list;
}

public bool Update(KhachHangDTO kh)
{
    using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
    {
        string sql = "UPDATE KhachHang SET TenKH = @ten, SDT = @sdt, DiaChi = @diachi WHERE MaKH = @ma";
        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@ma", kh.MaKH);
        cmd.Parameters.AddWithValue("@ten", kh.TenKH);
        cmd.Parameters.AddWithValue("@sdt", kh.SDT);
        cmd.Parameters.AddWithValue("@diachi", string.IsNullOrEmpty(kh.DiaChi) ? DBNull.Value : kh.DiaChi);
        conn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }
}

public bool Delete(int maKH)
{
    using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
    {
        string sql = "DELETE FROM KhachHang WHERE MaKH = @ma";
        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@ma", maKH);
        conn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }
}
    }
}