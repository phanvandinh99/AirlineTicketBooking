namespace AirlineWeb.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ChuyenBay")]
    public partial class ChuyenBay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChuyenBay()
        {
            LichBays = new HashSet<LichBay>();
            VeChuyenBays = new HashSet<VeChuyenBay>();
        }

        [Key]
        [StringLength(10)]
        public string MaChuyenBay { get; set; }

        [StringLength(10)]
        public string MaTuyenBay { get; set; }

        [StringLength(10)]
        public string MaMayBay { get; set; }

        [StringLength(10)]
        public string MaHangHangKhong { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        [Required]
        [StringLength(20)]
        public string SoHieuChuyenBay { get; set; }

        public virtual HangHangKhong HangHangKhong { get; set; }

        public virtual MayBay MayBay { get; set; }

        public virtual TuyenBay TuyenBay { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichBay> LichBays { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VeChuyenBay> VeChuyenBays { get; set; }
    }
}
