namespace AirlineWeb.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SanBay")]
    public partial class SanBay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SanBay()
        {
            TuyenBays = new HashSet<TuyenBay>();
            TuyenBays1 = new HashSet<TuyenBay>();
        }

        [Key]
        [StringLength(10)]
        public string MaSanBay { get; set; }

        [Required]
        [StringLength(100)]
        public string TenSanBay { get; set; }

        [StringLength(50)]
        public string ThanhPho { get; set; }

        [StringLength(50)]
        public string QuocGia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TuyenBay> TuyenBays { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TuyenBay> TuyenBays1 { get; set; }
    }
}
