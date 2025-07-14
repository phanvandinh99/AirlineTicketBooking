namespace AirlineWeb.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ThongKe")]
    public partial class ThongKe
    {
        [Key]
        [StringLength(10)]
        public string MaThongKe { get; set; }

        [Required]
        [StringLength(7)]
        public string ThangNam { get; set; }

        public int SoLuongVe { get; set; }

        public decimal? DoanhThuVND { get; set; }

        public decimal? DoanhThuUSD { get; set; }
    }
}
