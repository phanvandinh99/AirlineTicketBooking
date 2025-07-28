namespace AirlineWeb.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("VeChuyenBay")]
    public partial class VeChuyenBay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VeChuyenBay()
        {
            PhieuDatVe = new HashSet<PhieuDatVe>();
        }

        [Key]
        public int MaVe { get; set; }

        public int? MaLichBay { get; set; }

        [StringLength(10)]
        public string MaHangVe { get; set; }

        [StringLength(10)]
        public string SoGhe { get; set; }

        public int? TrangThai { get; set; }

        public decimal? GiaVND { get; set; }

        public virtual HangVe HangVe { get; set; }

        public virtual LichBay LichBay { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhieuDatVe> PhieuDatVe { get; set; }
    }
}
