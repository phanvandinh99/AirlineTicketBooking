## I. Môi trường phát triển
   * Công cụ: Visual Studio 2022, Microsoft SQL Server
   * Framework: ASP.NET MVC 5
   * Thư viện: EintityFramework, Razor, Twilio (code xác thực SMS), Jquery...

## I. Cài đặt

### 1. Môi trường, công cụ
  1.1 SQL Server 2018: [Link](https://go.microsoft.com/fwlink/?linkid=866662)

  1.2 SQL Server Management Studio (SSMS): [Link](https://aka.ms/ssmsfullsetup)

  1.3 Cài đặt Visual Studio:  [Link](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Enterprise&rel=16)

### 2. Tạo mở cơ sở dữ liệu
  * coppy 2 file dữ liệu từ thư mục `Database` là QuanLyNhaHang.mdf và QuanLyNhaHang_log.ldf coppy và paste vào thư mục DATA SQL: `C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA`
  * Mở SQL Server Management Studio (SSMS), chuột phải vào Database chọn Attach.. chọn Add file QuanLyNhaHang.mdf trong thư mục data vừa coppy ở bước trên. Chọn ok để hoàn thành

### 3. Mở sourcecode bằng Visual Studio và build   
  * Mở sourcecode bằng cách open Project từ Visual Studio, sau đó chọn file `DoAn.sln` trong thư mục DoAn

### 4. Thay đổi cấu hình Web.config
  * Mở SQL Server Management Studio: 
    * coppy tên ServerName

  * Mở sourcecode bằng Visual Studio:
    * Tạo kết nối với cơ sở dữ liệu local bằng cách thay đổi file `Web.config`: 
      * Data Source = `Tên ServerName của SQL Server`
      * Initial Catalog =`Tên cơ sở dữ liệu` => `QuuanLyNhaHang`

### 5. Chạy project
  * Click chuột phải vào Solution cảu project trong Visual Studio, chọn Build Solution 

### 5. Chạy project và sử dụng
  * Trong Visual Studio, nhấn `Ctrl + F5` để chạy

## II. Các chức năng chính của hệ thống

### Các tác nhân tham gia vào hệ thống
  * Nhân viên: là tác nhân tham gia quản lý bán hàng, quản lý hoá đơn của hệ thống
  * Nhân viên kho: quản lý kho, nhập xuất nguyên liệu, quản lý ngyên liệu... 
  * Khách hàng: sử dụng hệ thống để xem thông tin món ăn, chi tiết gọi món và đặt bàn
  * Admin: quản lý thông tin tài khoản hệ thống

  * Nhân viên:
    * Đăng nhập
    * Quản lý tầng
    * Quản lý bàn
    * Quản lý thông tin nhân viên phục vụ
    * Quản lý danh mục món ăn, thức uống
    * Quản lý yêu cầu gọi món khách hàng
    * Quản lý hoá đơn:
      * Cập nhật tên và số điện thoại theo khách hàng
      * Thêm món ăn vào hoá đơn (gọi món, huỷ món, cập nhật thêm hoặc bớt số lượng món ăn)
      * Xoá hoá đơn (Chỉ được xoá với trường hợp khách chưa gọi món nào)
      * chuyển bàn hoặc gộp bàn
      * Thanh toán hoá đơn
    * Tìm kiếm
    * Đặt bàn

  * Nhân viên kho
    * Đăng nhập
    * Quản lý nguyên liệu
    * Quản lý nhập nguyên liệu
    * Quản lý xuất/hoàn trả nguyên liệu. (Khi món ăn được gọi xẽ xuất nguyên liệu tương ứng với món đó. Trường hợp huỷ món sẽ tạo phiếu hoàn trả để trả nguyên liệu vào kho)
    * Quản lý tìm kiếm
    * Thống kê (ngày, tháng, năm, quý...)

  * Admin
    * Đăng nhập
    * Quản lý thông tin tài khoản nhân viên, nhân viên kho

  * Khách hàng
    * đăng nhập theo số điện thoại đã đặt bàn
    * Xem thông tin chi tiết món ăn
    * Xem món ăn đã đặt theo số điện thoại trong hoá đơn
    * yêu cầu đặt bàn

###### Nếu có thắc mắc về sourcecode và cách sử dụng có thể phản hồi email ""