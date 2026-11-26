using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CandleStore.DAO;
using CandleStore.DTO;

namespace CandleStore.BUS
{
    public class SanPhamBUS
    {
        private SanPhamDAO dao = new SanPhamDAO();

        // Đọc kho 
        public ServiceResult<List<SanPhamDTO>> GetAllProducts()
        {
            try {
                List<SanPhamDTO> data = dao.GetAll();
                return ServiceResult<List<SanPhamDTO>>.Success(data);
            }
            catch (SqlException ex) {
                return ServiceResult<List<SanPhamDTO>>.Failure($"[Lỗi DB Runtime] Mất kết nối máy chủ SQL không thể nạp danh sách nến thơm!\nChi tiết: {ex.Message}", ex);
            }
        }

        // Thêm sản phẩm 
        public ServiceResult AddProduct(SanPhamDTO sp, string userRole)
        {
            try {
                // Xác thực phân quyền 
                if (userRole.ToLower() != "admin") return ServiceResult.Failure("TỪ CHỐI TRUY CẬP: Nhân viên không có quyền tự ý thêm sản phẩm mới!");
                if (string.IsNullOrWhiteSpace(sp.TenSP)) return ServiceResult.Failure("Tên mẫu nến thơm mới không được để trống!");
                if (sp.Gia <= 0) return ServiceResult.Failure("Giá bán ra thị trường phải lớn hơn 0 VND!");
                if (sp.SoLuongTon < 0) return ServiceResult.Failure("Số lượng nhập kho ban đầu không được gõ số âm!");

                bool check = dao.Insert(sp);
                return check ? ServiceResult.Success("Thêm mới sản phẩm nến thơm thành công!") : ServiceResult.Failure("Lỗi thực thi hệ thống.");
            }
            catch (SqlException ex) {
                return ServiceResult.Failure($"[Lỗi DB Runtime] Không thể chèn dữ liệu do sự cố SQL Server!\nChi tiết: {ex.Message}", ex);
            }
            catch (Exception ex) {
                return ServiceResult.Failure($"Lỗi hệ thống bất ngờ: {ex.Message}", ex);
            }
        }

        // Sửa sản phẩm 
        public ServiceResult EditProduct(SanPhamDTO sp, string userRole)
        {
            try {
                if (userRole.ToLower() != "admin") return ServiceResult.Failure("TỪ CHỐI TRUY CẬP: Chỉ tài khoản Admin mới được hiệu chỉnh kho hàng!");
                if (sp.MaSP <= 0) return ServiceResult.Failure("Mã sản phẩm yêu cầu chỉnh sửa không hợp lệ!");
                if (string.IsNullOrWhiteSpace(sp.TenSP)) return ServiceResult.Failure("Tên nến thơm không được để trống!");
                if (sp.Gia <= 0) return ServiceResult.Failure("Giá bán cập nhật phải lớn hơn 0 VND!");
                if (sp.SoLuongTon < 0) return ServiceResult.Failure("Số lượng tồn kho không được cập nhật số âm!");

                bool check = dao.Update(sp);
                return check ? ServiceResult.Success("Cập nhật thông tin nến thơm thành công!") : ServiceResult.Failure("Sản phẩm không tồn tại hoặc đã bị xóa.");
            }
            catch (SqlException ex) {
                return ServiceResult.Failure($"[Lỗi DB Runtime] Sự cố SQL Server khi cập nhật dữ liệu hũ nến!\nChi tiết: {ex.Message}", ex);
            }
        }

        // Xóa sản phẩm
        public ServiceResult DeleteProduct(int maSP, string userRole)
        {
            try {
                if (userRole.ToLower() != "admin") return ServiceResult.Failure("TỪ CHỐI TRUY CẬP: Quyền hạn của bạn không được phép thực hiện chức năng xóa!");
                if (maSP <= 0) return ServiceResult.Failure("Mã sản phẩm yêu cầu xóa không hợp lệ!");

                bool check = dao.Delete(maSP);
                return check ? ServiceResult.Success("Đã xóa hoàn toàn mẫu nến thơm ra khỏi danh mục kho!") : ServiceResult.Failure("Xóa thất bại. Sản phẩm có thể không tồn tại.");
            }
            catch (SqlException ex) {
                if (ex.Number == 547) {
                    return ServiceResult.Failure("CHẶN NGHIỆP VỤ: Mẫu nến này đã từng được bán và thông tin của nó đang nằm trong các hóa đơn cũ! Không được phép xóa để bảo toàn dữ liệu lịch sử. Hãy chỉnh Số lượng tồn về 0 nếu ngừng kinh doanh sản phẩm này.", ex);
                }
                return ServiceResult.Failure($"[Lỗi DB Runtime] Trình quản lý SQL chặn thao tác xóa dữ liệu!\nChi tiết: {ex.Message}", ex);
            }
        }
    }
}