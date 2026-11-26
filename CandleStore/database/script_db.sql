-- 1. Khởi tạo Database mới
CREATE DATABASE CandleStore;
GO
USE CandleStore;
GO

-- 2. Tạo bảng Loại Sản Phẩm (Danh mục)
CREATE TABLE LoaiSanPham
(
    MaLoai INT IDENTITY(1,1) PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL
);
GO

-- 3. Tạo bảng Sản Phẩm (Thêm số lượng tồn kho và khóa ngoại tới Loại SP)
CREATE TABLE SanPham
(
    MaSP INT IDENTITY(1,1) PRIMARY KEY,
    TenSP NVARCHAR(100) NOT NULL,
    Gia DECIMAL(18,2) NOT NULL,
    MuiHuong NVARCHAR(50),
    SoLuongTon INT DEFAULT 0,
    MaLoai INT,
    CONSTRAINT FK_SanPham_Loai FOREIGN KEY (MaLoai) REFERENCES LoaiSanPham(MaLoai) ON DELETE SET NULL
);
GO

-- 4. Tạo bảng Người Dùng (Gộp Tài khoản và Nhân viên làm một)
CREATE TABLE NguoiDung
(
    MaND INT IDENTITY(1,1) PRIMARY KEY,
    TenNguoiDung NVARCHAR(100) NOT NULL,
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(50) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL -- 'admin' hoặc 'staff'
);
GO

-- 5. Tạo bảng Khách Hàng
CREATE TABLE KhachHang
(
    MaKH INT IDENTITY(1,1) PRIMARY KEY,
    TenKH NVARCHAR(100) NOT NULL,
    SDT VARCHAR(15) UNIQUE,
    DiaChi NVARCHAR(200)
);
GO

-- 6. Tạo bảng Hóa Đơn (Lưu thông tin tổng quan của đơn hàng)
CREATE TABLE HoaDon
(
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    NgayLap DATETIME DEFAULT GETDATE(),
    MaKH INT,
    MaND INT, -- Nhân viên nào lập hóa đơn này
    TongTien DECIMAL(18,2) DEFAULT 0,
    CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH) ON DELETE SET NULL,
    CONSTRAINT FK_HoaDon_NguoiDung FOREIGN KEY (MaND) REFERENCES NguoiDung(MaND) ON DELETE SET NULL
);
GO
ALTER TABLE HoaDon
ADD MaHoaDon VARCHAR(10);
UPDATE HoaDon
SET MaHoaDon = 'HD' + RIGHT('000' + CAST(MaHD AS VARCHAR), 3);

-- 7. Tạo bảng Chi Tiết Hóa Đơn (Bảng trung gian giải quyết quan hệ Nhiều - Nhiều)
CREATE TABLE ChiTietHoaDon
(
    MaHD INT,
    MaSP INT,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (MaHD, MaSP),
    CONSTRAINT FK_CTHD_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD) ON DELETE CASCADE,
    CONSTRAINT FK_CTHD_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
GO