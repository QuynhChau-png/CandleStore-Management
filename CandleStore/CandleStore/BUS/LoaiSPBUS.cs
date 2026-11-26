using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CandleStore.DAO;
using CandleStore.DTO;

namespace CandleStore.BUS
{
    public class LoaiSPBUS
    {
        private LoaiSPDAO dao = new LoaiSPDAO();

        public ServiceResult<List<LoaiSPDTO>> GetAllCategories()
        {
            try 
            {
                return ServiceResult<List<LoaiSPDTO>>.Success(dao.GetAll());
            }
            catch (SqlException ex) 
            {
                return ServiceResult<List<LoaiSPDTO>>.Failure("Không thể tải danh sách nhóm phân loại nến thơm từ SQL Server!", ex);
            }
            catch (Exception ex) 
            {
                return ServiceResult<List<LoaiSPDTO>>.Failure($"Lỗi hệ thống bất ngờ: {ex.Message}", ex);
            }
        }





        public ServiceResult AddCategory(LoaiSPDTO loai)
{
    try {
        if (string.IsNullOrWhiteSpace(loai.TenLoai)) return ServiceResult.Failure("Tên phân loại không được để trống!");
        return dao.Insert(loai) ? ServiceResult.Success("Thêm dòng phân loại nến mới thành công!") : ServiceResult.Failure("Thất bại.");
    } catch (SqlException ex) { return ServiceResult.Failure("Lỗi SQL Server!", ex); }
}
public ServiceResult EditCategory(LoaiSPDTO loai)
{
    try {
        if (string.IsNullOrWhiteSpace(loai.TenLoai)) return ServiceResult.Failure("Tên phân loại không được để trống!");
        return dao.Update(loai) ? ServiceResult.Success("Cập nhật tên phân loại thành công!") : ServiceResult.Failure("Thất bại.");
    } catch (SqlException ex) { return ServiceResult.Failure("Lỗi SQL Server!", ex); }
}
public ServiceResult DeleteCategory(int maLoai)
{
    try {
        return dao.Delete(maLoai) ? ServiceResult.Success("Xóa phân loại thành công!") : ServiceResult.Failure("Thất bại.");
    } catch (SqlException ex) {
        if (ex.Number == 547) return ServiceResult.Failure("Không thể xóa phân loại này vì đang có các sản phẩm nến thơm thuộc nhóm này!", ex);
        return ServiceResult.Failure("Lỗi SQL hệ thống!", ex);
    }
}

    }
}