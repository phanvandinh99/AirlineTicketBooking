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
            ChuyenBays = new HashSet<ChuyenBay>();
        }

        [Key]
        [StringLength(10)]
        public string MaTuyenBay { get; set; }

        [StringLength(10)]
        public string MaSanBayCatCanh { get; set; }

        [StringLength(10)]
        public string MaSanBayHaCanh { get; set; }

        public int? KhoangCach { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChuyenBay> ChuyenBays { get; set; }

        public virtual SanBay SanBay { get; set; }

        public virtual SanBay SanBay1 { get; set; }
    }
}
