using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CandleStore.DTO;

namespace CandleStore.DAO
{
    public class LoaiSPDAO
    {
        public List<LoaiSPDTO> GetAll()
        {
            List<LoaiSPDTO> list = new List<LoaiSPDTO>();
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                string sql = "SELECT MaLoai, TenLoai FROM LoaiSanPham";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new LoaiSPDTO {
                            MaLoai = (int)rd["MaLoai"],
                            TenLoai = rd["TenLoai"].ToString() ?? string.Empty
                        });
                    }
                }
            }
            return list;
        }

        public bool Insert(LoaiSPDTO loai)
{
    using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
    {
        string sql = "INSERT INTO LoaiSanPham (TenLoai) VALUES (@ten)";
        SqlCommand cmd = new SqlCommand(sql, conn); cmd.Parameters.AddWithValue("@ten", loai.TenLoai);
        conn.Open(); return cmd.ExecuteNonQuery() > 0;
    }
}
public bool Update(LoaiSPDTO loai)
{
    using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
    {
        string sql = "UPDATE LoaiSanPham SET TenLoai = @ten WHERE MaLoai = @ma";
        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@ma", loai.MaLoai); cmd.Parameters.AddWithValue("@ten", loai.TenLoai);
        conn.Open(); return cmd.ExecuteNonQuery() > 0;
    }
}
public bool Delete(int maLoai)
{
    using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
    {
        string sql = "DELETE FROM LoaiSanPham WHERE MaLoai = @ma";
        SqlCommand cmd = new SqlCommand(sql, conn); cmd.Parameters.AddWithValue("@ma", maLoai);
        conn.Open(); return cmd.ExecuteNonQuery() > 0;
    }
}
    }
}