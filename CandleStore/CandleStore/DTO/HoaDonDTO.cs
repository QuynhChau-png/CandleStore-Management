using System;

namespace CandleStore.DTO
{
    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public DateTime NgayLap { get; set; }
        public int? MaKH { get; set; } // Dấu ? cho phép null nếu là khách vãng lai
        public int? MaND { get; set; } // Nhân viên lập hóa đơn
        public decimal TongTien { get; set; }
    }
}