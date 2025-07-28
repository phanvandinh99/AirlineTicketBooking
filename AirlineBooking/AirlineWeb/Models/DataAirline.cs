using System.Data.Entity;

namespace AirlineWeb.Models
{
    public partial class DataAirline : DbContext
    {
        public DataAirline()
            : base("name=DataAirline")
        {
        }

        public virtual DbSet<ChuyenBay> ChuyenBay { get; set; }
        public virtual DbSet<HangHangKhong> HangHangKhong { get; set; }
        public virtual DbSet<HangVe> HangVe { get; set; }
        public virtual DbSet<HangVeHoaDon> HangVeHoaDon { get; set; }
        public virtual DbSet<HoaDon> HoaDon { get; set; }
        public virtual DbSet<KhachHang> KhachHang { get; set; }
        public virtual DbSet<LichBay> LichBay { get; set; }
        public virtual DbSet<LoaiMayBay> LoaiMayBay { get; set; }
        public virtual DbSet<MayBay> MayBay { get; set; }
        public virtual DbSet<NhanVien> NhanVien { get; set; }
        public virtual DbSet<PhieuDatVe> PhieuDatVe { get; set; }
        public virtual DbSet<SanBay> SanBay { get; set; }
        public virtual DbSet<ThongKe> ThongKe { get; set; }
        public virtual DbSet<TuyenBay> TuyenBay { get; set; }
        public virtual DbSet<VeChuyenBay> VeChuyenBay { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HangVe>()
                .Property(e => e.TyLeGia)
                .HasPrecision(5, 2);

            modelBuilder.Entity<HoaDon>()
                .Property(e => e.TongTienVND)
                .HasPrecision(10, 2);

            modelBuilder.Entity<HoaDon>()
                .Property(e => e.TongTienUSD)
                .HasPrecision(10, 2);

            modelBuilder.Entity<SanBay>()
                .HasMany(e => e.TuyenBay)
                .WithOptional(e => e.SanBay)
                .HasForeignKey(e => e.MaSanBayCatCanh);

            modelBuilder.Entity<SanBay>()
                .HasMany(e => e.TuyenBays1)
                .WithOptional(e => e.SanBay1)
                .HasForeignKey(e => e.MaSanBayHaCanh);

            modelBuilder.Entity<ThongKe>()
                .Property(e => e.DoanhThuVND)
                .HasPrecision(15, 2);

            modelBuilder.Entity<ThongKe>()
                .Property(e => e.DoanhThuUSD)
                .HasPrecision(15, 2);

            modelBuilder.Entity<VeChuyenBay>()
                .Property(e => e.GiaVND)
                .HasPrecision(10, 2);

            modelBuilder.Entity<VeChuyenBay>()
                .Property(e => e.GiaUSD)
                .HasPrecision(10, 2);
        }
    }
}
