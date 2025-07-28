namespace AirlineWeb.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TuyenBay")]
    public partial class TuyenBay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TuyenBay()
        {
            ChuyenBay = new HashSet<ChuyenBay>();
        }

        [Key]
        public int MaTuyenBay { get; set; }

        [StringLength(10)]
        public string MaSanBayCatCanh { get; set; }

        [StringLength(10)]
        public string MaSanBayHaCanh { get; set; }

        public int? KhoangCach { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChuyenBay> ChuyenBay { get; set; }

        public virtual SanBay SanBay { get; set; }

        public virtual SanBay SanBay1 { get; set; }
    }
}
