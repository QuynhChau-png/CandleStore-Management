using System;
using Microsoft.Data.SqlClient;
using CandleStore.DAO;
using CandleStore.DTO;

namespace CandleStore.BUS
{
    public class KhachHangBUS
    {
        private KhachHangDAO dao = new KhachHangDAO();

        public ServiceResult<KhachHangDTO> FindCustomerByPhone(string sdt)
        {
            try {
                if (string.IsNullOrWhiteSpace(sdt) || sdt.Length < 10) 
                    return ServiceResult<KhachHangDTO>.Failure("Số điện thoại khách hàng không hợp lệ (Tối thiểu phải từ 10 số)!");

                KhachHangDTO? kh = dao.GetByPhone(sdt);
                if (kh != null) return ServiceResult<KhachHangDTO>.Success(kh, "Tìm thấy thông tin thành viên thân thiết!");
                
                return ServiceResult<KhachHangDTO>.Failure("Khách hàng vãng lai mới (Chưa có trên hệ thống).");
            }
            catch (SqlException ex) {
                return ServiceResult<KhachHangDTO>.Failure("Lỗi máy chủ: Sự cố SQL Server không thể tra cứu thông tin khách hàng!", ex);
            }
        }

        public ServiceResult AddCustomer(KhachHangDTO kh)
        {
            try {
                if (string.IsNullOrWhiteSpace(kh.TenKH)) return ServiceResult.Failure("Tên khách hàng mới không được để trống!");
                if (string.IsNullOrWhiteSpace(kh.SDT) || kh.SDT.Length < 10) return ServiceResult.Failure("Số điện thoại đăng ký thành viên không hợp lệ!");

                if (dao.GetByPhone(kh.SDT) != null) return ServiceResult.Failure("Số điện thoại này đã được đăng ký cho một thành viên khác!");

                return dao.Insert(kh) ? ServiceResult.Success("Đăng ký thông tin thành viên mới thành công!") : ServiceResult.Failure("Lỗi ghi nhận dữ liệu khách hàng.");
            }
            catch (SqlException ex) {
                return ServiceResult.Failure("Lỗi Runtime SQL Server: Không thể đăng ký khách hàng!", ex);
            }
        }

        public ServiceResult<List<KhachHangDTO>> GetAllCustomers()
{
    try { return ServiceResult<List<KhachHangDTO>>.Success(dao.GetAll()); }
    catch (SqlException ex) { return ServiceResult<List<KhachHangDTO>>.Failure("Lỗi SQL Server không thể nạp danh sách khách hàng!", ex); }
}

public ServiceResult EditCustomer(KhachHangDTO kh)
{
    try {
        if (string.IsNullOrWhiteSpace(kh.TenKH)) return ServiceResult.Failure("Tên khách hàng không được để trống!");
        if (string.IsNullOrWhiteSpace(kh.SDT) || kh.SDT.Length < 10) return ServiceResult.Failure("Số điện thoại khách hàng không hợp lệ!");

        return dao.Update(kh) ? ServiceResult.Success("Cập nhật thông tin khách hàng thành công!") : ServiceResult.Failure("Khách hàng không tồn tại.");
    }
    catch (SqlException ex) { return ServiceResult.Failure("Lỗi cập nhật thông tin khách hàng!", ex); }
}

public ServiceResult DeleteCustomer(int maKH)
{
    try {
        return dao.Delete(maKH) ? ServiceResult.Success("Đã xóa thông tin khách hàng khỏi hệ thống!") : ServiceResult.Failure("Xóa thất bại.");
    }
    catch (SqlException ex) {
        if (ex.Number == 547) return ServiceResult.Failure("CHẶN XÓA: Khách hàng này đã từng mua hàng và có hóa đơn lưu trữ, không được xóa bừa bãi!", ex);
        return ServiceResult.Failure("Lỗi SQL Server khi xóa khách hàng!", ex);
    }
}
    }
}