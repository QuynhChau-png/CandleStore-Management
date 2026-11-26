using System;
using Microsoft.Data.SqlClient;
using CandleStore.DTO;

namespace CandleStore.DAO
{
    public class NguoiDungDAO
    {
        // 1. Kiểm tra tài khoản mật khẩu khi Đăng nhập
        public NguoiDungDTO? CheckLogin(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = "SELECT * FROM NguoiDung WHERE TenDangNhap = @user AND MatKhau = @pass";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return new NguoiDungDTO {
                            MaND = (int)rd["MaND"],
                            TenNguoiDung = rd["TenNguoiDung"].ToString() ?? string.Empty,
                            TenDangNhap = rd["TenDangNhap"].ToString() ?? string.Empty,
                            VaiTro = rd["VaiTro"].ToString() ?? "staff"
                        };
                    }
                }
            }
            return null; // Không tìm thấy tài khoản hợp lệ
        }

        // 2. Kiểm tra trùng tên đăng nhập khi Đăng ký
        public bool IsUsernameExists(string username)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = "SELECT COUNT(*) FROM NguoiDung WHERE TenDangNhap = @user";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@user", username);

                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // 3. Chèn nhân viên mới vào Database
        public bool Insert(NguoiDungDTO user)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = "INSERT INTO NguoiDung (TenNguoiDung, TenDangNhap, MatKhau, VaiTro) VALUES (@ten, @user, @pass, @vaitro)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ten", user.TenNguoiDung);
                cmd.Parameters.AddWithValue("@user", user.TenDangNhap);
                cmd.Parameters.AddWithValue("@pass", user.MatKhau);
                cmd.Parameters.AddWithValue("@vaitro", user.VaiTro);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        // Lấy toàn bộ danh sách nhân viên trong hệ thống
public List<NguoiDungDTO> GetAll()
{
    List<NguoiDungDTO> list = new List<NguoiDungDTO>();
    using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
    {
        string sql = "SELECT MaND, TenNguoiDung, TenDangNhap, VaiTro FROM NguoiDung";
        SqlCommand cmd = new SqlCommand(sql, conn);
        conn.Open();
        using (SqlDataReader rd = cmd.ExecuteReader())
        {
            while (rd.Read())
            {
                list.Add(new NguoiDungDTO {
                    MaND = (int)rd["MaND"],
                    TenNguoiDung = rd["TenNguoiDung"].ToString() ?? string.Empty,
                    TenDangNhap = rd["TenDangNhap"].ToString() ?? string.Empty,
                    VaiTro = rd["VaiTro"].ToString() ?? "staff"
                });
            }
        }
    }
    return list;
}

// Cập nhật họ tên, tài khoản hoặc vai trò nhân viên
public bool Update(NguoiDungDTO user)
{
    using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
    {
        string sql = "UPDATE NguoiDung SET TenNguoiDung = @ten, VaiTro = @vaitro WHERE MaND = @ma";
        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@ma", user.MaND);
        cmd.Parameters.AddWithValue("@ten", user.TenNguoiDung);
        cmd.Parameters.AddWithValue("@vaitro", user.VaiTro);
        conn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }
}

// Khai tử một tài khoản ra khỏi hệ thống
public bool Delete(int maND)
{
    using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
    {
        string sql = "DELETE FROM NguoiDung WHERE MaND = @ma";
        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@ma", maND);
        conn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }
}
    }
}