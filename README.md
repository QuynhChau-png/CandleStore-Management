# 🕯️ CandleStore - Quản lý cửa hàng nến

Ứng dụng desktop (Windows Forms) xây dựng đơn giản hỗ trợ quản lý bán hàng cho cửa hàng nến thơm: quản lý sản phẩm, khách hàng, hóa đơn và phân quyền người dùng. Dự án xây dựng theo mô hình **3 lớp (GUI - BUS - DAO)** kết hợp **DTO**, sử dụng **SQL Server** làm cơ sở dữ liệu.

## Chức năng chính

- **Đăng nhập / đăng ký** tài khoản, phân quyền theo vai trò `admin` và `staff`.
- **Quản lý sản phẩm**: thêm, sửa, xóa, theo dõi số lượng tồn kho, mùi hương, giá bán.
- **Quản lý loại sản phẩm** (danh mục): nến thơm, nến decor, phụ kiện, combo quà tặng...
- **Quản lý khách hàng**: lưu họ tên, số điện thoại, địa chỉ.
- **Lập hóa đơn bán hàng**: chọn khách hàng, thêm nhiều sản phẩm vào một hóa đơn, tự tính tổng tiền, ghi nhận nhân viên lập hóa đơn.

## Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ | C# |
| Framework | .NET 8 (Windows Forms) |
| Cơ sở dữ liệu | Microsoft SQL Server |
| Thư viện truy cập dữ liệu | Microsoft.Data.SqlClient (ADO.NET) |
| IDE khuyến nghị | Visual Studio 2022 |

## Kiến trúc dự án

```
CandleStore/
├── CandleStore.sln
├── CandleStore/
│   ├── GUI/          # Giao diện: FrmLogin, FrmRegister, FrmAdmin, FrmStaff
│   ├── BUS/          # Business logic: xử lý nghiệp vụ, kiểm tra dữ liệu
│   ├── DAO/          # Data access: truy vấn SQL Server
│   ├── DTO/          # Các lớp đối tượng truyền dữ liệu giữa các tầng
│   ├── Resources/    # Hình ảnh sản phẩm
│   └── Program.cs
└── database/
    ├── script_db.sql   # Tạo database và các bảng
    └── init_data.sql   # Dữ liệu mẫu
```

Luồng xử lý: `GUI → BUS → DAO → SQL Server`, dữ liệu được trao đổi giữa các tầng thông qua các lớp `DTO`.

## Cơ sở dữ liệu

Database `CandleStore` gồm 6 bảng:

| Bảng | Mô tả |
|---|---|
| `LoaiSanPham` | Danh mục sản phẩm |
| `SanPham` | Sản phẩm (giá, mùi hương, tồn kho, thuộc loại nào) |
| `NguoiDung` | Tài khoản hệ thống (admin / staff) |
| `KhachHang` | Thông tin khách hàng |
| `HoaDon` | Thông tin chung của hóa đơn |
| `ChiTietHoaDon` | Chi tiết từng sản phẩm trong hóa đơn (quan hệ nhiều - nhiều giữa `HoaDon` và `SanPham`) |

## Hướng dẫn cài đặt và chạy

### Yêu cầu

- Windows 10/11
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (workload **.NET desktop development**)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (bản Express hoặc Developer) và SQL Server Management Studio (SSMS)

### Các bước

1. **Clone repo**

   ```bash
   git clone https://github.com/QuynhChau-png/CandleStore-Management.git
   cd CandleStore-Management
   ```

2. **Tạo cơ sở dữ liệu**: mở SSMS, chạy lần lượt hai file trong thư mục `CandleStore/database/` (đúng thứ tự):

   1. `script_db.sql` (tạo database và các bảng)
   2. `init_data.sql` (thêm dữ liệu mẫu)

3. **Cấu hình chuỗi kết nối**: mở file `CandleStore/CandleStore/DAO/DatabaseConfig.cs` và sửa tên server cho khớp với máy của bạn, ví dụ:

   ```csharp
   Server=TEN_MAY\SQLEXPRESS;Database=CandleStore;Integrated Security=True;TrustServerCertificate=True;
   ```

4. **Mở và chạy dự án**: mở file `CandleStore/CandleStore.sln` bằng Visual Studio, bấm **F5** để chạy.

## Tài khoản demo

| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Quản lý | `admin` | `123` |
| Nhân viên | `staff` | `123` |

> ⚠️ Đây chỉ là tài khoản dùng cho mục đích demo.

## Hạn chế và hướng phát triển

- Mật khẩu hiện lưu dạng văn bản thường; nên băm (BCrypt/SHA-256 kèm salt) trước khi triển khai thực tế.
- Bổ sung báo cáo, thống kê doanh thu theo ngày/tháng và xuất hóa đơn ra PDF.
- Bổ sung quản lý nhập hàng và lịch sử thay đổi tồn kho.

## Tác giả

** Trương Thị Quỳnh Châu** · chautruong232@gmail.com · [GitHub](https://github.com/QuynhChau-png)
