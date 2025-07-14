namespace AirlineWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HoaDon")]
    public partial class HoaDon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HoaDon()
        {
            HangVeHoaDons = new HashSet<HangVeHoaDon>();
        }

        [Key]
        [StringLength(10)]
        public string MaHoaDon { get; set; }

        [StringLength(10)]
        public string MaPhieuDatVe { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayLapHoaDon { get; set; }

        public decimal? TongTienVND { get; set; }

        public decimal? TongTienUSD { get; set; }

        [StringLength(20)]
        public string TrangThaiThanhToan { get; set; }

        [StringLength(10)]
        public string MaNhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HangVeHoaDon> HangVeHoaDons { get; set; }

        public virtual NhanVien NhanVien { get; set; }

        public virtual PhieuDatVe PhieuDatVe { get; set; }
    }
}
