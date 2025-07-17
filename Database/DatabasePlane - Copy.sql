-- Xóa cơ sở dữ liệu cũ nếu tồn tại để đảm bảo tạo mới
USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'DatabasePlane')
BEGIN
    ALTER DATABASE DatabasePlane SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE DatabasePlane;
END
GO

-- Tạo cơ sở dữ liệu mới để lưu thông tin kế hoạch chuyến bay
CREATE DATABASE DatabasePlane;
GO
USE DatabasePlane;
GO

-- Bảng SanBay: Lưu thông tin sân bay (khởi hành/đến)
-- Ánh xạ với AviationStack API: MaSanBay tương ứng departure.iata/arrival.iata, TenSanBay tương ứng departure.airport/arrival.airport
CREATE TABLE SanBay (
    MaSanBay NVARCHAR(10) PRIMARY KEY,				-- Mã IATA của sân bay (SGN, HAN, ...)
    TenSanBay NVARCHAR(100) NOT NULL,				-- Tên sân bay (Tân Sơn Nhất, Nội Bài, ...)
    ThanhPho NVARCHAR(50),							-- Thành phố của sân bay (TP.HCM, Hà Nội, ...)
    QuocGia NVARCHAR(50) DEFAULT N'Việt Nam'		-- Quốc gia, mặc định là Việt Nam cho chuyến bay nội địa
);
GO

-- Bảng HangHangKhong: Lưu thông tin hãng hàng không (Vietnam Airlines, VietJet, ...)
-- Ánh xạ với API: MaHangHangKhong tương ứng airline.iata, TenHang tương ứng airline.name
CREATE TABLE HangHangKhong (
    MaHangHangKhong NVARCHAR(10) PRIMARY KEY,		-- Mã IATA của hãng (VN, VJ, ...)
    TenHang NVARCHAR(100) NOT NULL					-- Tên hãng hàng không (Vietnam Airlines, VietJet Air, ...)
);
GO

-- Bảng LoaiMayBay: Lưu thông tin loại máy bay (A321, B787, ...)
-- Ánh xạ với API: MaLoaiMayBay tương ứng aircraft.iata
CREATE TABLE LoaiMayBay (
    MaLoaiMayBay NVARCHAR(10) PRIMARY KEY,			-- Mã IATA của loại máy bay (A321, B787, ...)
    TenLoaiMayBay NVARCHAR(50) NOT NULL,			-- Tên loại máy bay (Airbus A321, Boeing 787, ...)
    SoGheToiDa INT NOT NULL							-- Số ghế tối đa, dùng để giả định số ghế khi API không cung cấp
);
GO

-- Bảng HangVe: Lưu thông tin hạng vé (thương gia, phổ thông, ...)
-- Không ánh xạ trực tiếp với API, dùng để quản lý vé
CREATE TABLE HangVe (
    MaHangVe NVARCHAR(10) PRIMARY KEY,				-- Mã hạng vé (HV01, HV02, ...)
    TenHangVe NVARCHAR(50) NOT NULL,				-- Tên hạng vé (Thương gia, Phổ thông, ...)
    TyLeGia DECIMAL(5, 2) NOT NULL					-- Tỷ lệ giá so với giá gốc (1.5 cho thương gia, 1.0 cho phổ thông)
);
GO

-- Bảng NhanVien: Lưu thông tin nhân viên
CREATE TABLE NhanVien (
    MaNhanVien NVARCHAR(10) PRIMARY KEY,			-- Mã nhân viên (NV01, NV02, ...)
    TenNhanVien NVARCHAR(50) NOT NULL,				-- Tên nhân viên
    SoDienThoai NVARCHAR(15),						-- Số điện thoại
    Email NVARCHAR(50)								-- Email
);
GO

-- Bảng KhachHang: Lưu thông tin khách hàng
CREATE TABLE KhachHang (
    MaKhachHang NVARCHAR(10) PRIMARY KEY,			-- Mã khách hàng (KH01, KH02, ...)
    TenKhachHang NVARCHAR(50) NOT NULL,				-- Tên khách hàng
    DiaChi NVARCHAR(100),							-- Địa chỉ
    GioiTinh NVARCHAR(10),							-- Giới tính (Nam, Nữ)
    SoDienThoai NVARCHAR(15),						-- Số điện thoại
    Email NVARCHAR(50)								-- Email
);
GO

-- Bảng MayBay: Lưu thông tin cụ thể về từng máy bay
-- Ánh xạ với API: MaMayBay tương ứng aircraft.registration
CREATE TABLE MayBay (
    MaMayBay NVARCHAR(10) PRIMARY KEY,														-- Mã đăng ký máy bay (VN-A123, ...)
    MaLoaiMayBay NVARCHAR(10) FOREIGN KEY REFERENCES LoaiMayBay(MaLoaiMayBay),				-- Liên kết với LoaiMayBay
    MaHangHangKhong NVARCHAR(10) FOREIGN KEY REFERENCES HangHangKhong(MaHangHangKhong)		-- Hãng sở hữu máy bay
);
GO

-- Bảng TuyenBay: Lưu thông tin tuyến bay (SGN → HAN, ...)
-- Ánh xạ với API: MaSanBayCatCanh tương ứng departure.iata, MaSanBayHaCanh tương ứng arrival.iata
CREATE TABLE TuyenBay (
    MaTuyenBay NVARCHAR(10) PRIMARY KEY,													-- Mã tuyến bay (TB01, TB02, ...)
    MaSanBayCatCanh NVARCHAR(10) FOREIGN KEY REFERENCES SanBay(MaSanBay),					-- Sân bay khởi hành
    MaSanBayHaCanh NVARCHAR(10),					-- Sân bay đến
    KhoangCach INT																			-- Khoảng cách (km), dùng để tính giá vé nếu cần
);
GO

-- Bảng ChuyenBay: Lưu thông tin chuyến bay
-- Ánh xạ với API: SoHieuChuyenBay tương ứng flight.number, TrangThai tương ứng flight_status
CREATE TABLE ChuyenBay (
    MaChuyenBay NVARCHAR(10) PRIMARY KEY,													-- Mã chuyến bay (CBVN123, ...)
    MaTuyenBay NVARCHAR(10) FOREIGN KEY REFERENCES TuyenBay(MaTuyenBay),					-- Tuyến bay
    MaMayBay NVARCHAR(10) FOREIGN KEY REFERENCES MayBay(MaMayBay),							-- Máy bay
    MaHangHangKhong NVARCHAR(10) FOREIGN KEY REFERENCES HangHangKhong(MaHangHangKhong),		-- Hãng hàng không
    TrangThai NVARCHAR(20),																	-- Trạng thái chuyến bay (Đã lên lịch, Bị trễ, Hủy)
    SoHieuChuyenBay NVARCHAR(20) NOT NULL													-- Số hiệu chuyến bay (VN123, VJ456, ...)
);
GO

-- Bảng LichBay: Lưu lịch trình cụ thể của chuyến bay
-- Ánh xạ với API: NgayGioKhoiHanh tương ứng departure.scheduled, NgayGioHaCanh tương ứng arrival.scheduled, NgayBay tương ứng flight_date
CREATE TABLE LichBay (
    MaLichBay NVARCHAR(10) PRIMARY KEY,														-- Mã lịch bay (LBVN123, ...)
    MaChuyenBay NVARCHAR(10) FOREIGN KEY REFERENCES ChuyenBay(MaChuyenBay),					-- Chuyến bay
    NgayGioKhoiHanh DATETIME NOT NULL,														-- Thời gian khởi hành
    NgayGioHaCanh DATETIME NOT NULL,														-- Thời gian hạ cánh
    NgayBay DATE NOT NULL																	-- Ngày bay
);
GO

-- Bảng VeChuyenBay: Lưu thông tin vé của chuyến bay
-- Không ánh xạ trực tiếp với API, dùng để quản lý vé
CREATE TABLE VeChuyenBay (
    MaVe NVARCHAR(10) PRIMARY KEY, -- Mã vé (VVN1231A, ...)
    MaChuyenBay NVARCHAR(10) FOREIGN KEY REFERENCES ChuyenBay(MaChuyenBay),					-- Chuyến bay
    MaHangVe NVARCHAR(10) FOREIGN KEY REFERENCES HangVe(MaHangVe),							-- Hạng vé
    SoGhe NVARCHAR(10),																		-- Mã ghế (1A, 2B, ...)
    TrangThai NVARCHAR(20),																	-- Trạng thái vé (Có sẵn, Đã đặt, Đã bán)
    GiaVND DECIMAL(10, 2),																	-- Giá vé (VND)
    GiaUSD DECIMAL(10, 2)																	-- Giá vé (USD, tùy chọn)
);
GO

-- Bảng PhieuDatVe: Lưu thông tin đặt vé
CREATE TABLE PhieuDatVe (
    MaPhieuDatVe NVARCHAR(10) PRIMARY KEY,													-- Mã phiếu đặt vé (PDV01, PDV02, ...)
    MaKhachHang NVARCHAR(10) FOREIGN KEY REFERENCES KhachHang(MaKhachHang),					-- Khách hàng
    MaVe NVARCHAR(10) FOREIGN KEY REFERENCES VeChuyenBay(MaVe),								-- Vé
    NgayDat DATE NOT NULL,																	-- Ngày đặt vé
    TrangThai NVARCHAR(20)																	-- Trạng thái đặt vé (Chờ xác nhận, Đã xác nhận, Đã hủy)
);
GO

-- Bảng HoaDon: Lưu thông tin hóa đơn
CREATE TABLE HoaDon (
    MaHoaDon NVARCHAR(10) PRIMARY KEY,														-- Mã hóa đơn (HD01, HD02, ...)
    MaPhieuDatVe NVARCHAR(10) FOREIGN KEY REFERENCES PhieuDatVe(MaPhieuDatVe),				-- Phiếu đặt vé
    NgayLapHoaDon DATE NOT NULL,															-- Ngày lập hóa đơn
    TongTienVND DECIMAL(10, 2),																-- Tổng tiền (VND)
    TongTienUSD DECIMAL(10, 2),																-- Tổng tiền (USD, tùy chọn)
    TrangThaiThanhToan NVARCHAR(20),														-- Trạng thái thanh toán (Đã thanh toán, Chưa thanh toán)
    MaNhanVien NVARCHAR(10) FOREIGN KEY REFERENCES NhanVien(MaNhanVien)						-- Nhân viên
);
GO

-- Bảng HangVeHoaDon: Quan hệ n-n giữa HangVe và HoaDon
-- Sử dụng ID làm khóa chính, MaHangVe và MaHoaDon là khóa ngoại
CREATE TABLE HangVeHoaDon (
    ID INT IDENTITY(1,1) PRIMARY KEY,														-- Khóa chính tự tăng
    MaHangVe NVARCHAR(10) FOREIGN KEY REFERENCES HangVe(MaHangVe),							-- Khóa ngoại đến HangVe
    MaHoaDon NVARCHAR(10) FOREIGN KEY REFERENCES HoaDon(MaHoaDon)							-- Khóa ngoại đến HoaDon
);
GO

-- Chèn dữ liệu mẫu cho SanBay
INSERT INTO SanBay (MaSanBay, TenSanBay, ThanhPho, QuocGia) VALUES
(N'SGN', N'Sân bay Tân Sơn Nhất', N'TP. Hồ Chí Minh', N'Việt Nam'),
(N'HAN', N'Sân bay Nội Bài', N'Hà Nội', N'Việt Nam'),
(N'DAD', N'Sân bay Đà Nẵng', N'Đà Nẵng', N'Việt Nam'),
(N'CXR', N'Sân bay Cam Ranh', N'Nha Trang', N'Việt Nam'),
(N'HPH', N'Sân bay Cát Bi', N'Hải Phòng', N'Việt Nam'),
(N'VCA', N'Sân bay Cần Thơ', N'Cần Thơ', N'Việt Nam'),
(N'HUI', N'Sân bay Phú Bài', N'Huế', N'Việt Nam'),
(N'DLI', N'Sân bay Liên Khương', N'Đà Lạt', N'Việt Nam'),
(N'PXU', N'Sân bay Pleiku', N'Pleiku', N'Việt Nam'),
(N'TBB', N'Sân bay Tuy Hòa', N'Phú Yên', N'Việt Nam');
GO

-- Chèn dữ liệu mẫu cho HangHangKhong
INSERT INTO HangHangKhong (MaHangHangKhong, TenHang) VALUES
(N'VN', N'Vietnam Airlines'),
(N'VJ', N'VietJet Air'),
(N'QH', N'Bamboo Airways'),
(N'BL', N'Jetstar Pacific');
GO

-- Chèn dữ liệu mẫu cho LoaiMayBay
INSERT INTO LoaiMayBay (MaLoaiMayBay, TenLoaiMayBay, SoGheToiDa) VALUES
(N'A321', N'Airbus A321', 180),
(N'B787', N'Boeing 787', 300),
(N'A350', N'Airbus A350', 280),
(N'B737', N'Boeing 737', 170);
GO

-- Chèn dữ liệu mẫu cho HangVe
INSERT INTO HangVe (MaHangVe, TenHangVe, TyLeGia) VALUES
(N'HV01', N'Thương gia', 1.5),
(N'HV02', N'Phổ thông', 1.0);
GO

-- Chèn dữ liệu mẫu cho NhanVien
INSERT INTO NhanVien (MaNhanVien, TenNhanVien, SoDienThoai, Email) VALUES
(N'NV01', N'Nguyễn Văn Xuân', N'0909876543', N'nvx@example.com'),
(N'NV02', N'Trần Thị Yến', N'0918765432', N'tty@example.com');
GO

-- Chèn dữ liệu mẫu cho KhachHang
INSERT INTO KhachHang (MaKhachHang, TenKhachHang, DiaChi, GioiTinh, SoDienThoai, Email) VALUES
(N'KH01', N'Nguyễn Văn An', N'123 Đường Láng, TP. Hồ Chí Minh', N'Nam', N'0901234567', N'nva@example.com'),
(N'KH02', N'Trần Thị Bình', N'456 Đường Giải Phóng, Hà Nội', N'Nữ', N'0912345678', N'ttb@example.com');
GO

-- Chèn dữ liệu mẫu cho MayBay
INSERT INTO MayBay (MaMayBay, MaLoaiMayBay, MaHangHangKhong) VALUES
(N'VN-A123', N'A321', N'VN'),
(N'VN-B787', N'B787', N'VN'),
(N'VJ-A350', N'A350', N'VJ'),
(N'QH-B737', N'B737', N'QH');
GO

-- Chèn dữ liệu mẫu cho TuyenBay
INSERT INTO TuyenBay (MaTuyenBay, MaSanBayCatCanh, MaSanBayHaCanh, KhoangCach) VALUES
(N'TBSGNHAN', N'SGN', N'HAN', 1160),
(N'TBHANDAD', N'HAN', N'DAD', 630),
(N'TBDADHUI', N'DAD', N'HUI', 640),
(N'TBHUICXR', N'HUI', N'CXR', 900);
GO

-- Chèn dữ liệu mẫu cho ChuyenBay
INSERT INTO ChuyenBay (MaChuyenBay, MaTuyenBay, MaMayBay, MaHangHangKhong, TrangThai, SoHieuChuyenBay) VALUES
(N'CBVN123', N'TBSGNHAN', N'VN-A123', N'VN', N'Đã lên lịch', N'VN123'),
(N'CBVJ456', N'TBHANDAD', N'VJ-A350', N'VJ', N'Đã lên lịch', N'VJ456'),
(N'CBQH789', N'TBDADHUI', N'QH-B737', N'QH', N'Đã lên lịch', N'QH789');
GO

-- Chèn dữ liệu mẫu cho LichBay
INSERT INTO LichBay (MaLichBay, MaChuyenBay, NgayGioKhoiHanh, NgayGioHaCanh, NgayBay) VALUES
(N'LBVN123', N'CBVN123', '2025-06-18 08:00:00', '2025-06-18 10:15:00', '2025-06-18'),
(N'LBVJ456', N'CBVJ456', '2025-06-18 09:00:00', '2025-06-18 10:30:00', '2025-06-18'),
(N'LBQH789', N'CBQH789', '2025-06-18 12:00:00', '2025-06-18 13:30:00', '2025-06-18');
GO

-- Chèn dữ liệu mẫu cho VeChuyenBay
INSERT INTO VeChuyenBay (MaVe, MaChuyenBay, MaHangVe, SoGhe, TrangThai, GiaVND, GiaUSD) VALUES
(N'VVN1231A', N'CBVN123', N'HV01', N'1A', N'Có sẵn', 5000000, 200),
(N'VVN1232B', N'CBVN123', N'HV02', N'2B', N'Có sẵn', 2500000, 100),
(N'VVJ4561A', N'CBVJ456', N'HV02', N'1A', N'Có sẵn', 2000000, 80);
GO

-- Chèn dữ liệu mẫu cho PhieuDatVe
INSERT INTO PhieuDatVe (MaPhieuDatVe, MaKhachHang, MaVe, NgayDat, TrangThai) VALUES
(N'PDV01', N'KH01', N'VVN1231A', '2025-06-17', N'Đã xác nhận'),
(N'PDV02', N'KH02', N'VVJ4561A', '2025-06-17', N'Chờ xác nhận');
GO

-- Chèn dữ liệu mẫu cho HoaDon
INSERT INTO HoaDon (MaHoaDon, MaPhieuDatVe, NgayLapHoaDon, TongTienVND, TongTienUSD, TrangThaiThanhToan, MaNhanVien) VALUES
(N'HD01', N'PDV01', '2025-06-17', 5000000, 200, N'Đã thanh toán', N'NV01'),
(N'HD02', N'PDV02', '2025-06-17', 2000000, 80, N'Chưa thanh toán', N'NV02');
GO

-- Chèn dữ liệu mẫu cho ThongKe
INSERT INTO ThongKe (MaThongKe, ThangNam, SoLuongVe, DoanhThuVND, DoanhThuUSD) VALUES
(N'TK01', N'06-2025', 50, 125000000, 5000),
(N'TK02', N'07-2025', 60, 150000000, 6000);
GO

-- Chèn dữ liệu mẫu cho HangVeHoaDon
INSERT INTO HangVeHoaDon (MaHangVe, MaHoaDon) VALUES
(N'HV01', N'HD01'),
(N'HV02', N'HD02');
GO