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
            HangVeHoaDon = new HashSet<HangVeHoaDon>();
        }

        [Key]
        public int MaHoaDon { get; set; }

        public int? MaPhieuDatVe { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayLapHoaDon { get; set; }

        public decimal? TraTruoc { get; set; }

        public decimal? TongTien { get; set; }

        public int? TrangThaiThanhToan { get; set; }

        [StringLength(10)]
        public string MaNhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HangVeHoaDon> HangVeHoaDon { get; set; }

        public virtual NhanVien NhanVien { get; set; }

        public virtual PhieuDatVe PhieuDatVe { get; set; }
    }
}
