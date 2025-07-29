namespace AirlineWeb.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("KhachHang")]
    public partial class KhachHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhachHang()
        {
            PhieuDatVe = new HashSet<PhieuDatVe>();
        }

        [Key]
        [StringLength(20)] // Phù hợp với database NVARCHAR(20)
        public string MaKhachHang { get; set; }

        [Required]
        [StringLength(20)] // Phù hợp với database VARCHAR(20)
        public string MatKhau { get; set; }

        [Required]
        [StringLength(50)] // Phù hợp với database NVARCHAR(50)
        public string TenKhachHang { get; set; }

        [StringLength(100)] // Phù hợp với database NVARCHAR(100)
        public string DiaChi { get; set; }

        [StringLength(10)] // Phù hợp với database NVARCHAR(10)
        public string GioiTinh { get; set; }

        [StringLength(15)] // Phù hợp với database NVARCHAR(15)
        public string SoDienThoai { get; set; }

        [StringLength(50)] // Phù hợp với database NVARCHAR(50)
        [EmailAddress] // Thêm validation email
        public string Email { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhieuDatVe> PhieuDatVe { get; set; }
    }
}
