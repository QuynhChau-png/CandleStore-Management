using System;

namespace CandleStore.DTO
{
    // Dùng cho các tác vụ Thêm, Sửa, Xóa (Chỉ cần biết thành công hay thất bại)
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public Exception? ErrorDetails { get; set; }

        public static ServiceResult Success(string message = "") 
            => new ServiceResult { IsSuccess = true, Message = message };
            
        public static ServiceResult Failure(string message, Exception? ex = null) 
            => new ServiceResult { IsSuccess = false, Message = message, ErrorDetails = ex };
    }

    // Dùng cho các tác vụ lấy dữ liệu (Trả về List, DataTable hoặc một đối tượng DTO)
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }

        public static ServiceResult<T> Success(T data, string message = "") 
            => new ServiceResult<T> { IsSuccess = true, Data = data, Message = message };
            
        public static new ServiceResult<T> Failure(string message, Exception? ex = null) 
            => new ServiceResult<T> { IsSuccess = false, Message = message, ErrorDetails = ex };
    }
}