using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSharpSoChiTieu.Data
{
    [Table("ct_Currency")]
    public class ct_Currency : EntityBase
    {
        [Required]
        [StringLength(10)]
        public string? Code { get; set; } // e.g., VND, USD

        [Required]
        [StringLength(100)]
        public string? Name { get; set; } // e.g., Vietnamese Dong

        [StringLength(10)]
        public string? Symbol { get; set; } // e.g., đ, $
    }
}