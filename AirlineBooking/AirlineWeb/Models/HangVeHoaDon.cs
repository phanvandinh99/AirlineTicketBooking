namespace AirlineWeb.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HangVeHoaDon")]
    public partial class HangVeHoaDon
    {
        public int ID { get; set; }

        [StringLength(10)]
        public string MaHangVe { get; set; }

        public int? MaHoaDon { get; set; }

        public virtual HangVe HangVe { get; set; }

        public virtual HoaDon HoaDon { get; set; }
    }
}
