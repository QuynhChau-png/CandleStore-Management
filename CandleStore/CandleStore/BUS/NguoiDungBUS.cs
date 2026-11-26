using System;
using Microsoft.Data.SqlClient;
using CandleStore.DAO;
using CandleStore.DTO;

namespace CandleStore.BUS
{
    public class NguoiDungBUS
    {
        private NguoiDungDAO dao = new NguoiDungDAO();

        // Đăng nhập
        public ServiceResult<NguoiDungDTO> Login(string username, string password)
        {
            try
            {
                // 1. Kiểm tra logic nghiệp vụ
                if (string.IsNullOrWhiteSpace(username)) return ServiceResult<NguoiDungDTO>.Failure("Tên đăng nhập không được để trống!");
                if (string.IsNullOrWhiteSpace(password)) return ServiceResult<NguoiDungDTO>.Failure("Mật khẩu không được để trống!");

                // Gọi xuống DAO thực thi kết nối
                NguoiDungDTO? user = dao.CheckLogin(username, password);

                if (user != null) {
                    return ServiceResult<NguoiDungDTO>.Success(user, "Đăng nhập hệ thống thành công!");
                }
                return ServiceResult<NguoiDungDTO>.Failure("Tên đăng nhập hoặc mật khẩu không chính xác!");
            }
            catch (SqlException ex)
            {
                
                return ServiceResult<NguoiDungDTO>.Failure($"[Lỗi DB Runtime] Máy chủ cơ sở dữ liệu từ chối kết nối hoặc đang bị tắt!\nChi tiết kỹ thuật: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                return ServiceResult<NguoiDungDTO>.Failure($"[Lỗi Hệ Thống] Đã xảy ra sự cố bất ngờ: {ex.Message}", ex);
            }
        }

        // Đăng ký
        public ServiceResult Register(NguoiDungDTO user, string confirmPassword)
        {
            try
            {
                // Kiểm tra toàn bộ logic nghiệp vụ đăng ký thành viên
                if (string.IsNullOrWhiteSpace(user.TenNguoiDung)) return ServiceResult.Failure("Vui lòng nhập đầy đủ Họ và tên nhân viên!");
                if (string.IsNullOrWhiteSpace(user.TenDangNhap)) return ServiceResult.Failure("Tên đăng nhập tài khoản không được để trống!");
                if (user.TenDangNhap.Length < 4) return ServiceResult.Failure("Tên đăng nhập phải có độ dài tối thiểu từ 4 ký tự!");
                if (string.IsNullOrWhiteSpace(user.MatKhau)) return ServiceResult.Failure("Mật khẩu tài khoản không được để trống!");
                if (user.MatKhau != confirmPassword) return ServiceResult.Failure("Xác nhận mật khẩu không trùng khớp với mật khẩu đã nhập!");

                // Kiểm tra trùng tài khoản 
                if (dao.IsUsernameExists(user.TenDangNhap)) {
                    return ServiceResult.Failure("Tên đăng nhập này đã tồn tại trong hệ thống! Vui lòng chọn tên khác.");
                }

                user.VaiTro = "staff"; // Tài khoản tự tạo luôn gán quyền nhân viên 
                bool isInserted = dao.Insert(user);

                return isInserted ? ServiceResult.Success("Tạo tài khoản nhân viên mới thành công!") 
                                  : ServiceResult.Failure("Hệ thống ghi nhận thành công 0 dòng dữ liệu xuống SQL.");
            }
            catch (SqlException ex)
            {
               
                return ServiceResult.Failure($"[Lỗi DB Runtime] Không thể hoàn tất đăng ký do sự cố máy chủ SQL Server!\nChi tiết: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"[Lỗi Hệ Thống] Sự cố Runtime nghiêm trọng: {ex.Message}", ex);
            }
        }


        public ServiceResult<List<NguoiDungDTO>> GetAllUsers()
{
    try { return ServiceResult<List<NguoiDungDTO>>.Success(dao.GetAll()); }
    catch (SqlException ex) { return ServiceResult<List<NguoiDungDTO>>.Failure("Lỗi kết nối SQL Server không thể tải danh sách người dùng!", ex); }
}

public ServiceResult EditUser(NguoiDungDTO user, string adminRole)
{
    try {
        if (adminRole.ToLower() != "admin") return ServiceResult.Failure("TỪ CHỐI CHỈNH SỬA: Bạn không phải Admin!");
        if (string.IsNullOrWhiteSpace(user.TenNguoiDung)) return ServiceResult.Failure("Họ tên nhân viên không được để trống!");
        
        return dao.Update(user) ? ServiceResult.Success("Cập nhật thông tin nhân viên thành công!") : ServiceResult.Failure("Tài khoản không tồn tại.");
    }
    catch (SqlException ex) { return ServiceResult.Failure("Lỗi cập nhật tài khoản người dùng!", ex); }
}

public ServiceResult DeleteUser(int maND, string adminRole, int currentAdminId)
{
    try {
        if (adminRole.ToLower() != "admin") return ServiceResult.Failure("TỪ CHỐI THAO TÁC: Chỉ Admin được quyền xóa tài khoản!");
        if (maND == currentAdminId) return ServiceResult.Failure("CHẶN NGHIỆP VỤ: Bạn không thể tự xóa tài khoản quản trị của chính mình!");

        return dao.Delete(maND) ? ServiceResult.Success("Đã xóa hoàn toàn tài khoản người dùng khỏi hệ thống!") : ServiceResult.Failure("Xóa tài khoản thất bại.");
    }
    catch (SqlException ex) {
        if (ex.Number == 547) return ServiceResult.Failure("CHẶN NGHIỆP VỤ: Nhân viên này đã từng lập hóa đơn bán hàng cho tiệm, không được xóa bừa bãi để giữ toàn vẹn dữ liệu kế toán!", ex);
        return ServiceResult.Failure("Lỗi SQL khi thực hiện xóa tài khoản!", ex);
    }
}
    }
}