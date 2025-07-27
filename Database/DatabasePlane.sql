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
    TenNhanVien NVARCHAR(50) NOT NULL,
    SoDienThoai NVARCHAR(15),
    Email NVARCHAR(50)
);
GO
INSERT INTO NhanVien (MaNhanVien, TenNhanVien, SoDienThoai, Email) VALUES
('Admin', N'Nhân Viên Quản Trị', N'0909876543', N'admin@gmail.com'),
('VNAL', N'VietNamAirline', N'0918765432', N'airline@gmail.com');
GO

-- Tạo và chèn dữ liệu cho bảng KhachHang
CREATE TABLE KhachHang (
    MaKhachHang NVARCHAR(10) PRIMARY KEY,
    TenKhachHang NVARCHAR(50) NOT NULL,
    DiaChi NVARCHAR(100),
    GioiTinh NVARCHAR(10),
    SoDienThoai NVARCHAR(15),
    Email NVARCHAR(50)
);
GO
INSERT INTO KhachHang (MaKhachHang, TenKhachHang, DiaChi, GioiTinh, SoDienThoai, Email) VALUES
('KH01', N'Nguyễn Văn An', N'123 Đường Láng, TP. Hồ Chí Minh', N'Nam', N'0901234567', N'nva@gmail.com'),
('KH02', N'Trần Thị Bình', N'456 Đường Giải Phóng, Hà Nội', N'Nữ', N'0912345678', N'ttb@gmail.com');
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
('0V-A567', 'AT75', '0V'); -- Đảm bảo mã 0V khớp với HangHangKhong
GO

-- Tạo và chèn dữ liệu cho bảng TuyenBay
CREATE TABLE TuyenBay (
    MaTuyenBay NVARCHAR(10) PRIMARY KEY,
    MaSanBayCatCanh NVARCHAR(10) FOREIGN KEY REFERENCES SanBay(MaSanBay),
    MaSanBayHaCanh NVARCHAR(10) FOREIGN KEY REFERENCES SanBay(MaSanBay),
    KhoangCach INT
);
GO
INSERT INTO TuyenBay (MaTuyenBay, MaSanBayCatCanh, MaSanBayHaCanh, KhoangCach) VALUES
('TB01', 'SGN', 'HAN', 1160),
('TB02', 'HAN', 'DAD', 630),
('TB03', 'SGN', 'PQC', 300),
('TB04', 'SGN', 'CXR', 310),
('TB05', 'HAN', 'VCS', 1350),
('TB06', 'SGN', 'CAH', 250),
('TB07', 'DAD', 'HPH', 600);
GO

-- Tạo và chèn dữ liệu cho bảng ChuyenBay
CREATE TABLE ChuyenBay (
    MaChuyenBay NVARCHAR(10) PRIMARY KEY,
    MaTuyenBay NVARCHAR(10) FOREIGN KEY REFERENCES TuyenBay(MaTuyenBay),
    MaMayBay NVARCHAR(10) FOREIGN KEY REFERENCES MayBay(MaMayBay),
    MaHangHangKhong NVARCHAR(10) FOREIGN KEY REFERENCES HangHangKhong(MaHangHangKhong),
    TrangThai NVARCHAR(20),
    SoHieuChuyenBay NVARCHAR(20) NOT NULL
);
GO
INSERT INTO ChuyenBay (MaChuyenBay, MaTuyenBay, MaMayBay, MaHangHangKhong, TrangThai, SoHieuChuyenBay) VALUES
('CBVN001', 'TB01', 'VN-A123', 'VN', 'Đã lên lịch', 'VN123'),
('CBVJ002', 'TB02', 'VJ-A789', 'VJ', 'Đã lên lịch', 'VJ456'),
('CBQH003', 'TB03', 'QH-A678', 'QH', 'Đã lên lịch', 'QH789'),
('CBBL004', 'TB04', 'BL-A901', 'BL', 'Đã lên lịch', 'BL234'),
('CBVU005', 'TB01', 'VU-A234', 'VU', 'Đã lên lịch', 'VU567'),
('CB0V006', 'TB05', '0V-A567', '0V', 'Đã lên lịch', '0V890');
GO

-- Tạo và chèn dữ liệu cho bảng LichBay
CREATE TABLE LichBay (
    MaLichBay NVARCHAR(10) PRIMARY KEY,
    MaChuyenBay NVARCHAR(10) FOREIGN KEY REFERENCES ChuyenBay(MaChuyenBay),
    NgayGioKhoiHanh DATETIME NOT NULL,
    NgayGioHaCanh DATETIME NOT NULL,
    NgayBay DATE NOT NULL
);
GO
INSERT INTO LichBay (MaLichBay, MaChuyenBay, NgayGioKhoiHanh, NgayGioHaCanh, NgayBay) VALUES
('LBVN001', 'CBVN001', '2025-07-28 07:00:00', '2025-07-28 09:00:00', '2025-07-28'),
('LBVJ002', 'CBVJ002', '2025-07-28 10:00:00', '2025-07-28 11:15:00', '2025-07-28'),
('LBQH003', 'CBQH003', '2025-07-28 14:00:00', '2025-07-28 14:45:00', '2025-07-28'),
('LBBL004', 'CBBL004', '2025-07-28 16:00:00', '2025-07-28 16:50:00', '2025-07-28'),
('LBVU005', 'CBVU005', '2025-07-29 08:00:00', '2025-07-29 10:00:00', '2025-07-29'),
('LB0V006', 'CB0V006', '2025-07-29 09:00:00', '2025-07-29 11:30:00', '2025-07-29');
GO

-- Tạo và chèn dữ liệu cho bảng VeChuyenBay
CREATE TABLE VeChuyenBay (
    MaVe NVARCHAR(10) PRIMARY KEY,
    MaChuyenBay NVARCHAR(10) FOREIGN KEY REFERENCES ChuyenBay(MaChuyenBay),
    MaHangVe NVARCHAR(10) FOREIGN KEY REFERENCES HangVe(MaHangVe),
    SoGhe NVARCHAR(10),
    TrangThai NVARCHAR(20),
    GiaVND DECIMAL(10, 2),
    GiaUSD DECIMAL(10, 2)
);
GO
INSERT INTO VeChuyenBay (MaVe, MaChuyenBay, MaHangVe, SoGhe, TrangThai, GiaVND, GiaUSD) VALUES
('VVN1231A', 'CBVN001', 'HV01', '1A', 'Đã bán', 4000000, 160.00),
('VVN1231B', 'CBVN001', 'HV03', '10C', 'Có sẵn', 2000000, 80.00),
('VVJ4562A', 'CBVJ002', 'HV04', '2B', 'Đã bán', 1800000, 72.00),
('VVJ4562C', 'CBVJ002', 'HV07', '15D', 'Có sẵn', 800000, 32.00),
('VQH7893A', 'CBQH003', 'HV05', '1C', 'Đã bán', 2500000, 100.00),
('VBL2344B', 'CBBL004', 'HV08', '5A', 'Đã đặt', 1200000, 48.00);
GO

-- Tạo và chèn dữ liệu cho bảng PhieuDatVe
CREATE TABLE PhieuDatVe (
    MaPhieuDatVe NVARCHAR(10) PRIMARY KEY,
    MaKhachHang NVARCHAR(10) FOREIGN KEY REFERENCES KhachHang(MaKhachHang),
    MaVe NVARCHAR(10) FOREIGN KEY REFERENCES VeChuyenBay(MaVe),
    NgayDat DATE NOT NULL,
    TrangThai NVARCHAR(20)
);
GO
INSERT INTO PhieuDatVe (MaPhieuDatVe, MaKhachHang, MaVe, NgayDat, TrangThai) VALUES
('PDV01', 'KH01', 'VVN1231A', '2025-07-27', 'Đã xác nhận'),
('PDV02', 'KH02', 'VVJ4562A', '2025-07-27', 'Đã xác nhận'),
('PDV03', 'KH01', 'VQH7893A', '2025-07-27', 'Chờ xác nhận');
GO

-- Tạo và chèn dữ liệu cho bảng HoaDon
CREATE TABLE HoaDon (
    MaHoaDon NVARCHAR(10) PRIMARY KEY,
    MaPhieuDatVe NVARCHAR(10) FOREIGN KEY REFERENCES PhieuDatVe(MaPhieuDatVe),
    NgayLapHoaDon DATE NOT NULL,
    TongTienVND DECIMAL(10, 2),
    TongTienUSD DECIMAL(10, 2),
    TrangThaiThanhToan NVARCHAR(20),
    MaNhanVien NVARCHAR(10) FOREIGN KEY REFERENCES NhanVien(MaNhanVien)
);
GO
INSERT INTO HoaDon (MaHoaDon, MaPhieuDatVe, NgayLapHoaDon, TongTienVND, TongTienUSD, TrangThaiThanhToan, MaNhanVien) VALUES
('HD01', 'PDV01', '2025-07-27', 4000000, 160.00, 'Đã thanh toán', 'Admin'),
('HD02', 'PDV02', '2025-07-27', 1800000, 72.00, 'Đã thanh toán', 'VNAL');
GO

-- Tạo và chèn dữ liệu cho bảng ThongKe
CREATE TABLE ThongKe (
    MaThongKe NVARCHAR(10) PRIMARY KEY,
    ThangNam NVARCHAR(7) NOT NULL,
    SoLuongVe INT NOT NULL,
    DoanhThuVND DECIMAL(15, 2),
    DoanhThuUSD DECIMAL(15, 2)
);
GO
INSERT INTO ThongKe (MaThongKe, ThangNam, SoLuongVe, DoanhThuVND, DoanhThuUSD) VALUES
('TK01', '07-2025', 3, 8300000, 332.00);
GO

-- Tạo và chèn dữ liệu cho bảng HangVeHoaDon
CREATE TABLE HangVeHoaDon (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    MaHangVe NVARCHAR(10) FOREIGN KEY REFERENCES HangVe(MaHangVe),
    MaHoaDon NVARCHAR(10) FOREIGN KEY REFERENCES HoaDon(MaHoaDon)
);
GO
INSERT INTO HangVeHoaDon (MaHangVe, MaHoaDon) VALUES
('HV01', 'HD01'),
('HV04', 'HD02');
GO