using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CandleStore.DTO;

namespace CandleStore.DAO
{
    public class SanPhamDAO
    {
        public List<SanPhamDTO> GetAll()
        {
            List<SanPhamDTO> list = new List<SanPhamDTO>();
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = @"SELECT sp.MaSP, sp.TenSP, sp.Gia, sp.MuiHuong, sp.SoLuongTon, sp.MaLoai, l.TenLoai 
                               FROM SanPham sp
                               LEFT JOIN LoaiSanPham l ON sp.MaLoai = l.MaLoai";
                               
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new SanPhamDTO {
                            MaSP = (int)rd["MaSP"],
                            TenSP = rd["TenSP"].ToString() ?? string.Empty,
                            Gia = (decimal)rd["Gia"],
                            MuiHuong = rd["MuiHuong"].ToString() ?? string.Empty,
                            SoLuongTon = (int)rd["SoLuongTon"],
                            MaLoai = rd["MaLoai"] != DBNull.Value ? (int)rd["MaLoai"] : 0,
                            TenLoai = rd["TenLoai"] != DBNull.Value ? rd["TenLoai"].ToString()! : "Chưa phân loại"
                        });
                    }
                }
            }
            return list;
        }

        public bool Insert(SanPhamDTO sp)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                // Thay vì gán cứng số 1, giờ truyền @loai động từ ComboBox GUI xuống
                string sql = "INSERT INTO SanPham (TenSP, Gia, MuiHuong, SoLuongTon, MaLoai) VALUES (@ten, @gia, @mui, @ton, @loai)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ten", sp.TenSP);
                cmd.Parameters.AddWithValue("@gia", sp.Gia);
                cmd.Parameters.AddWithValue("@mui", sp.MuiHuong);
                cmd.Parameters.AddWithValue("@ton", sp.SoLuongTon);
                cmd.Parameters.AddWithValue("@loai", sp.MaLoai == 0 ? DBNull.Value : sp.MaLoai);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(SanPhamDTO sp)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                // Cập nhật luôn cả cột MaLoai khi Admin chọn đổi nhóm nến thơm
                string sql = "UPDATE SanPham SET TenSP = @ten, Gia = @gia, MuiHuong = @mui, SoLuongTon = @ton, MaLoai = @loai WHERE MaSP = @ma";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", sp.MaSP);
                cmd.Parameters.AddWithValue("@ten", sp.TenSP);
                cmd.Parameters.AddWithValue("@gia", sp.Gia);
                cmd.Parameters.AddWithValue("@mui", sp.MuiHuong);
                cmd.Parameters.AddWithValue("@ton", sp.SoLuongTon);
                cmd.Parameters.AddWithValue("@loai", sp.MaLoai == 0 ? DBNull.Value : sp.MaLoai);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int maSP)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = "DELETE FROM SanPham WHERE MaSP = @ma";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maSP);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}