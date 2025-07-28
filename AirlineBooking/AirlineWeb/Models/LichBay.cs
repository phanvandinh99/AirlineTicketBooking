namespace AirlineWeb.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LichBay")]
    public partial class LichBay
    {
        [Key]
        public int MaLichBay { get; set; }

        public int? MaChuyenBay { get; set; }

        public DateTime NgayGioKhoiHanh { get; set; }

        public DateTime NgayGioHaCanh { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayBay { get; set; }

        public virtual ChuyenBay ChuyenBay { get; set; }
    }
}
