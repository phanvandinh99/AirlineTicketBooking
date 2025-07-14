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
            HoaDons = new HashSet<HoaDon>();
        }

        [Key]
        [StringLength(10)]
        public string MaPhieuDatVe { get; set; }

        [StringLength(10)]
        public string MaKhachHang { get; set; }

        [StringLength(10)]
        public string MaVe { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayDat { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HoaDon> HoaDons { get; set; }

        public virtual KhachHang KhachHang { get; set; }

        public virtual VeChuyenBay VeChuyenBay { get; set; }
    }
}
