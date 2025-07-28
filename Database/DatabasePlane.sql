-- Xóa cơ sở dữ liệu cũ nếu tồn tại để đảm bảo tạo mới
USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'DatabasePlane')
BEGIN
    ALTER DATABASE DatabasePlane SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE DatabasePlane;
END
GO

-- Tạo cơ sở dữ liệu mới
CREATE DATABASE DatabasePlane;
GO
USE DatabasePlane;
GO

-- Tạo và chèn dữ liệu cho bảng SanBay
CREATE TABLE SanBay (
    MaSanBay NVARCHAR(10) PRIMARY KEY,
    TenSanBay NVARCHAR(100) NOT NULL,
    ThanhPho NVARCHAR(50),
    QuocGia NVARCHAR(50) DEFAULT N'Việt Nam'
);
GO
INSERT INTO SanBay(MaSanBay, TenSanBay, ThanhPho, QuocGia) VALUES
('HAN', N'Sân Bay Quốc Tế Nội Bài', N'Hà Nội', N'Việt Nam'),
('SGN', N'Sân Bay Quốc Tế Tân Sơn Nhất', N'Tp. Hồ Chí Minh', N'Việt Nam'),
('DAD', N'Sân Bay Quốc Tế Đà Nẵng', N'Đà Nẵng', N'Việt Nam'),
('PQC', N'Sân Bay Quốc Tế Phú Quốc', N'Kiên Giang', N'Việt Nam'),
('CXR', N'Sân Bay Quốc Tế Cam Ranh', N'Khánh Hòa', N'Việt Nam'),
('BMV', N'Sân Bay Buôn Ma Thuột', N'Buôn Ma Thuột', N'Việt Nam'),
('CAH', N'Sân Bay Cà Mau', N'Cà Mau', N'Việt Nam'),
('VCA', N'Sân Bay Quốc Tế Cần Thơ', N'Cần Thơ', N'Việt Nam'),
('VCL', N'Sân Bay Chu Lai', N'Quảng Nam', N'Việt Nam'),
('VCS', N'Sân Bay Côn Đảo', N'Bà Rịa - Vũng Tàu', N'Việt Nam'),
('DLI', N'Sân Bay Quốc Tế Liên Khương', N'Lâm Đồng', N'Việt Nam'),
('DIN', N'Sân Bay Điện Biên Phủ', N'Điện Biên', N'Việt Nam'),
('VDH', N'Sân Bay Đồng Hới', N'Quảng Bình', N'Việt Nam'),
('HPH', N'Sân Bay Quốc Tế Cát Bi', N'Hải Phòng', N'Việt Nam'),
('HUI', N'Sân Bay Quốc Tế Phú Bài', N'Thừa Thiên Huế', N'Việt Nam'),
('PXU', N'Sân Bay Pleiku', N'Gia Lai', N'Việt Nam'),
('UIH', N'Sân Bay Quốc Tế Phù Cát', N'Bình Định', N'Việt Nam'),
('VKG', N'Sân Bay Rạch Giá', N'Kiên Giang', N'Việt Nam'),
('THD', N'Sân Bay Thọ Xuân', N'Thanh Hóa', N'Việt Nam'),
('TBB', N'Sân Bay Tuy Hòa', N'Phú Yên', N'Việt Nam'),
('VDO', N'Sân Bay Quốc Tế Vân Đồn', N'Quảng Ninh', N'Việt Nam'),
('VII', N'Sân Bay Quốc Tế Vinh', N'Nghệ An', N'Việt Nam');
GO

-- Tạo và chèn dữ liệu cho bảng HangHangKhong
CREATE TABLE HangHangKhong (
    MaHangHangKhong NVARCHAR(10) PRIMARY KEY,
    TenHang NVARCHAR(100) NOT NULL
);
GO
INSERT INTO HangHangKhong(MaHangHangKhong, TenHang) VALUES
('VN', N'Vietnam Airlines'),
('VJ', N'Vietjet Air'),
('QH', N'Bamboo Airways'),
('BL', N'Pacific Airline'),
('VU', N'Vietravel Airlines'),
('0V', N'VASCO');
GO

-- Tạo và chèn dữ liệu cho bảng LoaiMayBay
CREATE TABLE LoaiMayBay (
    MaLoaiMayBay NVARCHAR(10) PRIMARY KEY,
    TenLoaiMayBay NVARCHAR(50) NOT NULL,
    SoGheToiDa INT NOT NULL
);
GO
INSERT INTO LoaiMayBay (MaLoaiMayBay, TenLoaiMayBay, SoGheToiDa) VALUES
('A320', 'Airbus A320-200', 180),
('A321', 'Airbus A321-200', 203),
('A21N', 'Airbus A321neo', 203),
('A359', 'Airbus A350-900', 305),
('B789', 'Boeing 787-9 Dreamliner', 274),
('B78X', 'Boeing 787-10 Dreamliner', 343),
('A333', 'Airbus A330-300', 300),
('E190', 'Embraer E190', 112),
('AT75', 'ATR 72-500', 68),
('C208', 'Cessna Grand Caravan 208B-EX', 12),
('B738', 'Boeing 737-800', 189);
GO

-- Tạo và chèn dữ liệu cho bảng HangVe
CREATE TABLE HangVe (
    MaHangVe NVARCHAR(10) PRIMARY KEY,
    TenHangVe NVARCHAR(50) NOT NULL,
    TyLeGia DECIMAL(5, 2) NOT NULL
);
GO
INSERT INTO HangVe (MaHangVe, TenHangVe, TyLeGia) VALUES
('HV01', 'Phổ Thông', 1.00),
('HV02', 'Phổ Thông Cao Cấp', 1.50),
('HV03', 'Thương Gia', 2.00),
('HV04', 'Hạng Nhất', 2.50);
GO

-- Tạo và chèn dữ liệu cho bảng NhanVien
CREATE TABLE NhanVien (
    MaNhanVien NVARCHAR(10) PRIMARY KEY,
    MatKhau VARCHAR(50) NOT NULL,
    TenNhanVien NVARCHAR(50) NOT NULL,
    SoDienThoai NVARCHAR(15),
    Email NVARCHAR(50)
);
GO
INSERT INTO NhanVien (MaNhanVien, MatKhau, TenNhanVien, SoDienThoai, Email) VALUES
('Admin', 'admin123', N'Nhân Viên Quản Trị', N'0909876543', N'admin@gmail.com'),
('VNAL', 'vnal123', N'VietNamAirline', N'0918765432', N'airline@gmail.com');
GO

-- Tạo và chèn dữ liệu cho bảng KhachHang
CREATE TABLE KhachHang (
    MaKhachHang NVARCHAR(10) PRIMARY KEY,
    MatKhau VARCHAR(50) NOT NULL,
    TenKhachHang NVARCHAR(50) NOT NULL,
    DiaChi NVARCHAR(100),
    GioiTinh NVARCHAR(10),
    SoDienThoai NVARCHAR(15),
    Email NVARCHAR(50)
);
GO
INSERT INTO KhachHang (MaKhachHang, MatKhau, TenKhachHang, DiaChi, GioiTinh, SoDienThoai, Email) VALUES
('KH01', 'kh01pass', N'Nguyễn Văn An', N'123 Đường Láng, TP. Hồ Chí Minh', N'Nam', N'0901234567', N'nva@gmail.com'),
('KH02', 'kh02pass', N'Trần Thị Bình', N'456 Đường Giải Phóng, Hà Nội', N'Nữ', N'0912345678', N'ttb@gmail.com');
GO

-- Tạo và chèn dữ liệu cho bảng MayBay
CREATE TABLE MayBay (
    MaMayBay NVARCHAR(10) PRIMARY KEY,
    MaLoaiMayBay NVARCHAR(10) FOREIGN KEY REFERENCES LoaiMayBay(MaLoaiMayBay),
    MaHangHangKhong NVARCHAR(10) FOREIGN KEY REFERENCES HangHangKhong(MaHangHangKhong)
);
GO
INSERT INTO MayBay (MaMayBay, MaLoaiMayBay, MaHangHangKhong) VALUES
('VN-A123', 'A321', 'VN'),
('VN-A456', 'B789', 'VN'),
('VJ-A789', 'A320', 'VJ'),
('VJ-A012', 'A333', 'VJ'),
('QH-B345', 'B789', 'QH'),
('QH-A678', 'A21N', 'QH'),
('BL-A901', 'A320', 'BL'),
('VU-A234', 'A321', 'VU'),
('0V-A567', 'AT75', '0V');
GO

-- Tạo và chèn dữ liệu cho bảng TuyenBay
CREATE TABLE TuyenBay (
    MaTuyenBay INT IDENTITY(1,1) PRIMARY KEY,
    MaSanBayCatCanh NVARCHAR(10) FOREIGN KEY REFERENCES SanBay(MaSanBay),
    MaSanBayHaCanh NVARCHAR(10) FOREIGN KEY REFERENCES SanBay(MaSanBay),
    KhoangCach INT
);
GO
INSERT INTO TuyenBay (MaSanBayCatCanh, MaSanBayHaCanh, KhoangCach) VALUES
('SGN', 'HAN', 1160),
('HAN', 'DAD', 630),
('SGN', 'PQC', 300),
('SGN', 'CXR', 310),
('HAN', 'VCS', 1350),
('SGN', 'CAH', 250),
('DAD', 'HPH', 600);
GO

-- Tạo và chèn dữ liệu cho bảng ChuyenBay
CREATE TABLE ChuyenBay (
    MaChuyenBay INT IDENTITY(1,1) PRIMARY KEY,
    MaTuyenBay INT FOREIGN KEY REFERENCES TuyenBay(MaTuyenBay),
    MaMayBay NVARCHAR(10) FOREIGN KEY REFERENCES MayBay(MaMayBay),
    MaHangHangKhong NVARCHAR(10) FOREIGN KEY REFERENCES HangHangKhong(MaHangHangKhong),
    TrangThai INT, -- 0: Hoạt động, 1: Tạm ngưng
    SoHieuChuyenBay NVARCHAR(20) NOT NULL
);
GO
INSERT INTO ChuyenBay (MaTuyenBay, MaMayBay, MaHangHangKhong, TrangThai, SoHieuChuyenBay) VALUES
(1, 'VN-A123', 'VN', 0, 'VN123'),
(2, 'VJ-A789', 'VJ', 0, 'VJ456'),
(3, 'QH-A678', 'QH', 0, 'QH789'),
(4, 'BL-A901', 'BL', 0, 'BL234'),
(1, 'VU-A234', 'VU', 0, 'VU567'),
(5, '0V-A567', '0V', 0, '0V890');
GO

-- Tạo và chèn dữ liệu cho bảng LichBay
CREATE TABLE LichBay (
    MaLichBay INT IDENTITY(1,1) PRIMARY KEY,
    MaChuyenBay INT FOREIGN KEY REFERENCES ChuyenBay(MaChuyenBay),
    NgayGioKhoiHanh DATETIME NOT NULL,
    NgayGioHaCanh DATETIME NOT NULL,
    NgayBay DATE NOT NULL
);
GO
INSERT INTO LichBay (MaChuyenBay, NgayGioKhoiHanh, NgayGioHaCanh, NgayBay) VALUES
(1, '2025-07-28 07:00:00', '2025-07-28 09:00:00', '2025-07-28'),
(2, '2025-07-28 10:00:00', '2025-07-28 11:15:00', '2025-07-28'),
(3, '2025-07-28 14:00:00', '2025-07-28 14:45:00', '2025-07-28'),
(4, '2025-07-28 16:00:00', '2025-07-28 16:50:00', '2025-07-28'),
(5, '2025-07-29 08:00:00', '2025-07-29 10:00:00', '2025-07-29'),
(6, '2025-07-29 09:00:00', '2025-07-29 11:30:00', '2025-07-29'),
(7, '2025-08-03 09:00:00', '2025-08-03 11:30:00', '2025-08-03'),
(8, '2025-08-03 09:00:00', '2025-08-03 11:30:00', '2025-08-03'),
(9, '2025-08-03 09:00:00', '2025-08-03 11:30:00', '2025-08-03'),
(10, '2025-08-03 09:00:00', '2025-08-03 11:30:00', '2025-08-03');
GO

-- Tạo và chèn dữ liệu cho bảng VeChuyenBay
CREATE TABLE VeChuyenBay (
    MaVe INT IDENTITY(1,1) PRIMARY KEY,
    MaChuyenBay INT FOREIGN KEY REFERENCES ChuyenBay(MaChuyenBay),
    MaHangVe NVARCHAR(10) FOREIGN KEY REFERENCES HangVe(MaHangVe),
    SoGhe NVARCHAR(10),
    TrangThai INT, -- 0: Còn trống, 1: Đang Giữ, 2: Đã Đặt
    GiaVND DECIMAL(10, 2)
);
GO
INSERT INTO VeChuyenBay (MaChuyenBay, MaHangVe, SoGhe, TrangThai, GiaVND) VALUES
(1, 'HV01', '1A', 2, 4000000),
(1, 'HV03', '10C', 0, 2000000),
(2, 'HV04', '2B', 2, 1800000),
(2, 'HV01', '15D', 0, 800000),
(3, 'HV02', '1C', 2, 2500000),
(4, 'HV03', '5A', 2, 1200000);
GO

-- Tạo và chèn dữ liệu cho bảng PhieuDatVe
CREATE TABLE PhieuDatVe (
    MaPhieuDatVe INT IDENTITY(1,1) PRIMARY KEY,
    MaKhachHang NVARCHAR(10) FOREIGN KEY REFERENCES KhachHang(MaKhachHang),
    MaVe INT FOREIGN KEY REFERENCES VeChuyenBay(MaVe),
    HoTenHanhKhach NVARCHAR(100) NOT NULL,
    NgaySinh VARCHAR(20) NOT NULL,
    CanCuoc VARCHAR(20) NULL, -- Dưới 16 tuổi không có căn cước
    NgayDat DATE NOT NULL,
    TrangThai INT -- 0: Đã thanh toán, 1: Chưa thanh toán, 2: Đã đặt cọc
);
GO
INSERT INTO PhieuDatVe (MaKhachHang, MaVe, HoTenHanhKhach, NgaySinh, CanCuoc, NgayDat, TrangThai) VALUES
('KH01', 1, N'Nguyễn Văn An', '1990-05-15', '123456789012', '2025-07-27', 0),
('KH02', 3, N'Trần Thị Bình', '1992-08-20', '987654321098', '2025-07-27', 0),
('KH01', 5, N'Nguyễn Văn An', '1990-05-15', '123456789012', '2025-07-27', 1);
GO

-- Tạo và chèn dữ liệu cho bảng HoaDon
CREATE TABLE HoaDon (
    MaHoaDon INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuDatVe INT FOREIGN KEY REFERENCES PhieuDatVe(MaPhieuDatVe),
    NgayLapHoaDon DATE NOT NULL,
    TraTruoc DECIMAL(10, 2),
    TongTien DECIMAL(10, 2),
    TrangThaiThanhToan INT, -- 0: Đã Thanh Toán, 1: Chưa Thanh Toán, 2: Đang giữ (Đã cọc 40% tổng tiền)
    MaNhanVien NVARCHAR(10) FOREIGN KEY REFERENCES NhanVien(MaNhanVien)
);
GO
INSERT INTO HoaDon (MaPhieuDatVe, NgayLapHoaDon, TraTruoc, TongTien, TrangThaiThanhToan, MaNhanVien) VALUES
(1, '2025-07-27', 0, 4000000, 0, 'Admin'),
(2, '2025-07-27', 0, 1800000, 0, 'VNAL');
GO

-- Tạo và chèn dữ liệu cho bảng HangVeHoaDon
CREATE TABLE HangVeHoaDon (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    MaHangVe NVARCHAR(10) FOREIGN KEY REFERENCES HangVe(MaHangVe),
    MaHoaDon INT FOREIGN KEY REFERENCES HoaDon(MaHoaDon)
);
GO
INSERT INTO HangVeHoaDon (MaHangVe, MaHoaDon) VALUES
('HV01', 1),
('HV04', 2);
GO