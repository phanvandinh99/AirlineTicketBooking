using System.Data.Entity;

namespace AirlineWeb.Models
{
    public partial class DataAirline : DbContext
    {
        public DataAirline()
            : base("name=DataAirline")
        {
        }

        public virtual DbSet<ChuyenBay> ChuyenBays { get; set; }
        public virtual DbSet<HangHangKhong> HangHangKhongs { get; set; }
        public virtual DbSet<HangVe> HangVes { get; set; }
        public virtual DbSet<HangVeHoaDon> HangVeHoaDons { get; set; }
        public virtual DbSet<HoaDon> HoaDons { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<LichBay> LichBays { get; set; }
        public virtual DbSet<LoaiMayBay> LoaiMayBays { get; set; }
        public virtual DbSet<MayBay> MayBays { get; set; }
        public virtual DbSet<NhanVien> NhanViens { get; set; }
        public virtual DbSet<PhieuDatVe> PhieuDatVes { get; set; }
        public virtual DbSet<SanBay> SanBays { get; set; }
        public virtual DbSet<ThongKe> ThongKes { get; set; }
        public virtual DbSet<TuyenBay> TuyenBays { get; set; }
        public virtual DbSet<VeChuyenBay> VeChuyenBays { get; set; }

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
                .HasMany(e => e.TuyenBays)
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
