namespace AirlineWeb.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MayBay")]
    public partial class MayBay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MayBay()
        {
            ChuyenBay = new HashSet<ChuyenBay>();
        }

        [Key]
        [StringLength(10)]
        public string MaMayBay { get; set; }

        [StringLength(10)]
        public string MaLoaiMayBay { get; set; }

        [StringLength(10)]
        public string MaHangHangKhong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChuyenBay> ChuyenBay { get; set; }

        public virtual HangHangKhong HangHangKhong { get; set; }

        public virtual LoaiMayBay LoaiMayBay { get; set; }
    }
}
