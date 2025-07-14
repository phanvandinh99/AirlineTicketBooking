namespace AirlineWeb.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LoaiMayBay")]
    public partial class LoaiMayBay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LoaiMayBay()
        {
            MayBays = new HashSet<MayBay>();
        }

        [Key]
        [StringLength(10)]
        public string MaLoaiMayBay { get; set; }

        [Required]
        [StringLength(50)]
        public string TenLoaiMayBay { get; set; }

        public int SoGheToiDa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MayBay> MayBays { get; set; }
    }
}
