namespace AirlineWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PhieuDatVe")]
    public partial class PhieuDatVe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PhieuDatVe()
        {
            HoaDon = new HashSet<HoaDon>();
        }

        [Key]
        public int MaPhieuDatVe { get; set; }

        [StringLength(20)] // Phù hợp với database NVARCHAR(20)
        public string MaKhachHang { get; set; }

        public int? MaVe { get; set; }

        [Required]
        [StringLength(100)] // Phù hợp với database NVARCHAR(100)
        public string HoTenHanhKhach { get; set; }

        [Required]
        [StringLength(20)] // Phù hợp với database VARCHAR(20)
        public string NgaySinh { get; set; }

        [StringLength(20)] // Phù hợp với database VARCHAR(20)
        public string CanCuoc { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayDat { get; set; }

        public int? TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HoaDon> HoaDon { get; set; }

        public virtual KhachHang KhachHang { get; set; }

        public virtual VeChuyenBay VeChuyenBay { get; set; }
    }
}
