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
-- Bảng HangHangKhong: Lưu thông tin hãng hàng không (Vietnam Airlines, VietJet, ...)
-- Ánh xạ với API: MaHangHangKhong tương ứng airline.iata, TenHang tương ứng airline.name
CREATE TABLE HangHangKhong (
    MaHangHangKhong NVARCHAR(10) PRIMARY KEY,		-- Mã IATA của hãng (VN, VJ, ...)
    TenHang NVARCHAR(100) NOT NULL					-- Tên hãng hàng không (Vietnam Airlines, VietJet Air, ...)
);
GO
INSERT INTO HangHangKhong(MaHangHangKhong, TenHang) VALUES
('VN', N'Vietnam Airlines'),
('VJ', N'Vietjet Air'),
('QH', N'Bamboo Airways'),
('BL', N'Pacific Airline'),
('VU', N'Vietravel Airlines'),
('OV', N'VASCO');
GO

-- Bảng LoaiMayBay: Lưu thông tin loại máy bay (A321, B787, ...)
-- Ánh xạ với API: MaLoaiMayBay tương ứng aircraft.iata
CREATE TABLE LoaiMayBay (
    MaLoaiMayBay NVARCHAR(10) PRIMARY KEY,			-- Mã IATA của loại máy bay (A321, B787, ...)
    TenLoaiMayBay NVARCHAR(50) NOT NULL,			-- Tên loại máy bay (Airbus A321, Boeing 787, ...)
    SoGheToiDa INT NOT NULL							-- Số ghế tối đa, dùng để giả định số ghế khi API không cung cấp
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

-- Bảng HangVe: Lưu thông tin hạng vé (thương gia, phổ thông, ...)
-- Không ánh xạ trực tiếp với API, dùng để quản lý vé
CREATE TABLE HangVe (
    MaHangVe NVARCHAR(10) PRIMARY KEY,				-- Mã hạng vé (HV01, HV02, ...)
    TenHangVe NVARCHAR(50) NOT NULL,				-- Tên hạng vé (Thương gia, Phổ thông, ...)
    TyLeGia DECIMAL(5, 2) NOT NULL					-- Tỷ lệ giá so với giá gốc (1.5 cho thương gia, 1.0 cho phổ thông)
);
GO
INSERT INTO HangVe (MaHangVe, TenHangVe, TyLeGia) VALUES
('HV01', 'Thương gia', 2.00),
('HV02', 'Phổ thông đặc biệt', 1.50),
('HV03', 'Phổ thông', 1.00),
('HV04', 'SkyBoss', 1.80),
('HV05', 'Bamboo First Class', 2.50),
('HV06', 'Bamboo Plus', 1.30),
('HV07', 'Promo', 0.80),
('HV08', 'Starter Max', 1.20),
('HV09', 'Starter Plus', 1.10);
Go

-- Bảng NhanVien: Lưu thông tin nhân viên
CREATE TABLE NhanVien (
    MaNhanVien NVARCHAR(10) PRIMARY KEY,			-- Mã nhân viên (NV01, NV02, ...)
    TenNhanVien NVARCHAR(50) NOT NULL,				-- Tên nhân viên
    SoDienThoai NVARCHAR(15),						-- Số điện thoại
    Email NVARCHAR(50)								-- Email
);
GO
-- Chèn dữ liệu mẫu cho NhanVien
INSERT INTO NhanVien (MaNhanVien, TenNhanVien, SoDienThoai, Email) VALUES
(N'Admin', N'Nhân Viên Quản Trị', N'0909876543', N'admin@gmail.com'),
(N'NVVNAirLine', N'VietNamAirline', N'0918765432', N'airline@gmail.com');
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
-- Chèn dữ liệu mẫu cho KhachHang
INSERT INTO KhachHang (MaKhachHang, TenKhachHang, DiaChi, GioiTinh, SoDienThoai, Email) VALUES
(N'KH01', N'Nguyễn Văn An', N'123 Đường Láng, TP. Hồ Chí Minh', N'Nam', N'0901234567', N'nva@gmail.com'),
(N'KH02', N'Trần Thị Bình', N'456 Đường Giải Phóng, Hà Nội', N'Nữ', N'0912345678', N'ttb@gmail.com');
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
    MaSanBayHaCanh NVARCHAR(10) FOREIGN KEY REFERENCES SanBay(MaSanBay),					-- Sân bay đến
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

-- Bảng ThongKe: Lưu thông tin thống kê doanh thu và số vé
CREATE TABLE ThongKe (
    MaThongKe NVARCHAR(10) PRIMARY KEY,														-- Mã thống kê (TK01, TK02, ...)
    ThangNam NVARCHAR(7) NOT NULL,															-- Tháng và năm (MM-YYYY, ví dụ: 06-2025)
    SoLuongVe INT NOT NULL,																	-- Số lượng vé bán
    DoanhThuVND DECIMAL(15, 2),																-- Doanh thu (VND)
    DoanhThuUSD DECIMAL(15, 2)																-- Doanh thu (USD, tùy chọn)
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