namespace CandleStore.DTO
{
    public class NguoiDungDTO
    {
        public int MaND { get; set; }
        public string TenNguoiDung { get; set; } = string.Empty;
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty; // 'admin' hoặc 'staff'
    }
}