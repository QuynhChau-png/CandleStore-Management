namespace CandleStore.DTO
{
    public class SanPhamDTO
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public string MuiHuong { get; set; } = string.Empty;
        public int SoLuongTon { get; set; }
        public int MaLoai { get; set; }
        
        // Dùng để hiển thị Tên phân loại
        public string TenLoai { get; set; } = string.Empty; 
    }
}