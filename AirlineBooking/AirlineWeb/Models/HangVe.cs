namespace AirlineWeb.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HangVe")]
    public partial class HangVe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HangVe()
        {
            HangVeHoaDons = new HashSet<HangVeHoaDon>();
            VeChuyenBays = new HashSet<VeChuyenBay>();
        }

        [Key]
        [StringLength(10)]
        public string MaHangVe { get; set; }

        [Required]
        [StringLength(50)]
        public string TenHangVe { get; set; }

        public decimal TyLeGia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HangVeHoaDon> HangVeHoaDons { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VeChuyenBay> VeChuyenBays { get; set; }
    }
}
