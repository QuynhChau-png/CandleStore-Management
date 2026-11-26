using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CandleStore.DAO;
using CandleStore.DTO;

namespace CandleStore.BUS
{
    public class HoaDonBUS
    {
        private HoaDonDAO hoaDonDao = new HoaDonDAO();
        private SanPhamDAO sanPhamDao = new SanPhamDAO();

        // Chốt đơn và trừ kho
        public ServiceResult ProcessCheckout(HoaDonDTO hd, List<ChiTietHoaDonDTO> listDetails)
        {
            try
            {
                if (listDetails == null || listDetails.Count == 0) 
                    return ServiceResult.Failure("Yêu cầu chặn: Giỏ hàng hiện tại đang trống!");

                // Kiểm tra định mức tồn kho vật lý
                List<SanPhamDTO> currentStock = sanPhamDao.GetAll();
                foreach (var detail in listDetails)
                {
                    SanPhamDTO? dbProduct = currentStock.Find(p => p.MaSP == detail.MaSP);
                    if (dbProduct == null) return ServiceResult.Failure($"Mẫu sản phẩm mã số {detail.MaSP} đã bị gỡ khỏi hệ thống!");
                    
                    if (dbProduct.SoLuongTon < detail.SoLuong)
                    {
                        return ServiceResult.Failure($"CHẶN XUẤT KHO: '{dbProduct.TenSP}' trong kho chỉ còn {dbProduct.SoLuongTon} hũ, không đủ chốt đơn ({detail.SoLuong} hũ)!");
                    }
                }

                // Tính tổng tiền tại BUS chống hacker sửa dữ liệu Client
                decimal total = 0;
                foreach (var detail in listDetails) total += detail.SoLuong * detail.DonGia;
                hd.TongTien = total;
                hd.NgayLap = DateTime.Now;

                int newInvoiceId = hoaDonDao.CreateInvoice(hd, listDetails);

                if (newInvoiceId > 0)
                {
                    string maHoaDon = "HD" + newInvoiceId.ToString("000");

                    return ServiceResult.Success(
                        $"Chốt đơn hàng thành công! Mã hóa đơn: {maHoaDon}");
                }

                return ServiceResult.Failure("Lỗi ghi nhận hóa đơn.");
            }
            catch (SqlException ex)
            {
                return ServiceResult.Failure($"[Lỗi DB Runtime] Mất kết nối dữ liệu. Đã kích hoạt Rollback bảo toàn kho!\nChi tiết: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Gặp sự cố lỗi hệ thống: {ex.Message}", ex);
            }
        }

        // 2. 🔴 ĐÃ SỬA: TẢI LỊCH SỬ ĐƠN HÀNG CÓ PHÂN LUỒNG QUYỀN HẠN THÔNG MINH
        public ServiceResult<DataTable> GetSalesHistory(int maND, string userRole)
        {
            try
            {
                DataTable dt;
                
                // Tầng BUS phân luồng:
                if (userRole.ToLower() == "admin") 
                {
                    // Admin lấy toàn bộ lịch sử
                    dt = hoaDonDao.GetAllInvoices();
                }
                else 
                {
                    // Nhân viên chỉ được lấy danh sách đơn do chính mình lập
                    if (maND <= 0) return ServiceResult<DataTable>.Failure("Mã tài khoản nhân viên không hợp lệ!");
                    dt = hoaDonDao.GetInvoicesByStaff(maND);
                }
                
                return ServiceResult<DataTable>.Success(dt);
            }
            catch (SqlException ex)
            {
                return ServiceResult<DataTable>.Failure($"Lỗi máy chủ: Không thể tải lịch sử hóa đơn do mất kết nối SQL!\n{ex.Message}", ex);
            }
        }

        // Tính toán doanh số cho dashboard
        public ServiceResult<Dictionary<string, object>> GetDashboardStatistics()
        {
            try {
                return ServiceResult<Dictionary<string, object>>.Success(hoaDonDao.GetDashboardStats());
            }
            catch (SqlException ex) {
                return ServiceResult<Dictionary<string, object>>.Failure("Lỗi kết nối DB không thể nạp số liệu thống kê!", ex);
            }
        }

        // Biểu đồ doanh thu ngày cho admin
        public ServiceResult<DataTable> GetRevenueByDayStatistics()
        {
            try {
                return ServiceResult<DataTable>.Success(hoaDonDao.GetRevenueByDay());
            }
            catch (SqlException ex) {
                return ServiceResult<DataTable>.Failure("Không thể trích xuất đồ thị do lỗi kết nối SQL!", ex);
            }
        }
       
    }
}