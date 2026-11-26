USE CandleStore;
GO

-- Chèn Loại sản phẩm
INSERT INTO LoaiSanPham (TenLoai)
VALUES
(N'Nến thơm thư giãn'),
(N'Nến decor nghệ thuật'),
(N'Phụ kiện nến'),
(N'Nến tinh dầu thiên nhiên'),
(N'Combo quà tặng');
GO

-- Chèn Sản phẩm
INSERT INTO SanPham (TenSP, Gia, MuiHuong, SoLuongTon, MaLoai) VALUES 
(N'Nến Lavender Thư Giãn', 120000, 'Lavender', 50, 1),
(N'Nến Vanilla Ngọt Ngào', 150000, 'Vanilla', 40, 1),
(N'Nến Gỗ Thông Ấm Áp', 180000, 'Pine Wood', 30, 2),
(N'Nến Thảo Mộc Bình Yên', 130000, 'Herbal', 25, 2),
(N'Kéo Cắt Tim Nến Inox', 65000, N'Không có', 15, 3),
(N'Khay Đựng Nến Gỗ', 99000, N'Không có', 20, 3),
(N'Nến Sả Chanh Đuổi Muỗi', 115000, N'Lemongrass', 60, 4),
(N'Nến Cam Quế Mùa Đông', 160000, N'Orange Cinnamon', 22, 4),
(N'Nến Bạc Hà Tươi Mát', 125000, N'Mint', 28, 4),
(N'Combo Relax Cuối Tuần', 399000, N'Hỗn hợp', 12, 5),
(N'Combo Quà Sinh Nhật', 499000, N'Hỗn hợp', 8, 5),
(N'Combo Premium Luxury', 799000, N'Hỗn hợp', 5, 5),
(N'Nến Decor Galaxy', 250000, N'Không mùi', 15, 2),
(N'Nến Hoa Hồng Nghệ Thuật', 220000, N'Rose', 18, 2),
(N'Nến Điêu Khắc Thiên Nga', 270000, N'Jasmine', 10, 2);
GO

-- Chèn Tài khoản hệ thống
INSERT INTO NguoiDung (TenNguoiDung, TenDangNhap, MatKhau, VaiTro) VALUES 
(N'Quản Lý', 'admin', '123', 'admin'),
(N'Nhân Viên', 'staff', '123', 'staff');
GO

-- Chèn Khách hàng mẫu
INSERT INTO KhachHang (TenKH, SDT, DiaChi) VALUES 
(N'Nguyễn Văn A', '0901234567', N'123 Nguyễn Trãi, Q.5, HCMC'),
(N'Trần Thị B', '0918765432', N'456 Lê Lợi, Q.1, HCMC'),
(N'Lê Minh Anh', '0901111111', N'88 Nguyễn Huệ, Q.1, HCMC'),
(N'Phạm Thu Hà', '0902222222', N'45 Cộng Hòa, Tân Bình, HCMC'),
(N'Nguyễn Hoàng Nam', '0903333333', N'19 Phan Văn Trị, Gò Vấp, HCMC'),
(N'Trương Gia Hân', '0904444444', N'220 Võ Văn Ngân, Thủ Đức, HCMC'),
(N'Võ Quốc Bảo', '0905555555', N'77 Ba Tháng Hai, Q.10, HCMC'),
(N'Đặng Mỹ Linh', '0906666666', N'155 Xô Viết Nghệ Tĩnh, Bình Thạnh, HCMC'),
(N'Bùi Nhật Minh', '0907777777', N'62 Phan Xích Long, Phú Nhuận, HCMC'),
(N'Hồ Thanh Tâm', '0908888888', N'11 Nguyễn Đình Chiểu, Q.3, HCMC');
GO

-- Chèn thử 1 Hóa đơn mẫu
INSERT INTO HoaDon (NgayLap, MaKH, MaND, TongTien) VALUES 
(GETDATE(), 1, 2, 390000);
GO

-- Chi tiết của Hóa đơn mẫu trên (Mua 2 nến Lavender và 1 nến Vanilla)
INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(1, 1, 2, 120000), -- 240,000
(1, 2, 1, 150000); -- 150,000
GO

-- HÓA ĐƠN 2
INSERT INTO HoaDon (NgayLap, MaKH, MaND, TongTien) VALUES 
(GETDATE()-8, 2, 2, 520000);
GO

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(2, 3, 1, 180000), -- 180,000
(2, 4, 2, 130000), -- 260,000
(2, 5, 1, 65000);  -- 65,000
GO

-- HÓA ĐƠN 3
INSERT INTO HoaDon (NgayLap, MaKH, MaND, TongTien) VALUES 
(GETDATE()-7, 3, 2, 240000);
GO

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(3, 1, 2, 120000); -- 240,000
GO

-- HÓA ĐƠN 4
INSERT INTO HoaDon (NgayLap, MaKH, MaND, TongTien) VALUES 
(GETDATE()-10, 4, 3, 799000);
GO

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(4, 12, 1, 799000); -- 799,000
GO

-- HÓA ĐƠN 5
INSERT INTO HoaDon (NgayLap, MaKH, MaND, TongTien) VALUES 
(GETDATE()-5, 5, 3, 279000);
GO

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(5, 6, 1, 99000),  -- 99,000
(5, 7, 1, 115000), -- 115,000
(5, 5, 1, 65000);  -- 65,000
GO

-- HÓA ĐƠN 6
INSERT INTO HoaDon (NgayLap, MaKH, MaND, TongTien) VALUES 
(GETDATE()-4, 6, 5, 445000);
GO

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(6, 8, 2, 160000), -- 320,000
(6, 9, 1, 125000); -- 125,000
GO

-- HÓA ĐƠN 7
INSERT INTO HoaDon (NgayLap, MaKH, MaND, TongTien) VALUES 
(GETDATE()-8, 7, 5, 898000);
GO

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(7, 10, 1, 399000), -- 399,000
(7, 11, 1, 499000); -- 499,000
GO

-- HÓA ĐƠN 8
INSERT INTO HoaDon (NgayLap, MaKH, MaND, TongTien) VALUES 
(GETDATE()-4, 8, 3, 164000);
GO

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(8, 6, 1, 99000), -- 99,000
(8, 5, 1, 65000); -- 65,000
GO

-- HÓA ĐƠN 9
INSERT INTO HoaDon (NgayLap, MaKH, MaND, TongTien) VALUES 
(GETDATE()-1, 9, 3, 1049000);
GO

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES 
(9, 12, 1, 799000), -- 799,000
(9, 13, 1, 250000); -- 250,000
GO