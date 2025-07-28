namespace AirlineWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LichBay")]
    public partial class LichBay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LichBay()
        {
            VeChuyenBay = new HashSet<VeChuyenBay>();
        }

        [Key]
        public int MaLichBay { get; set; }

        public int? MaChuyenBay { get; set; }

        public DateTime NgayGioKhoiHanh { get; set; }

        public DateTime NgayGioHaCanh { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayBay { get; set; }

        public virtual ChuyenBay ChuyenBay { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VeChuyenBay> VeChuyenBay { get; set; }
    }
}
