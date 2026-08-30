using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSharpSoChiTieu.Data
{
    [Table("ct_Feedback")]
    public class ct_Feedback : EntityBase
    {
        [StringLength(50)]
        public string Type { get; set; } // "Feedback" (Góp ý) hoặc "Bug" (Báo lỗi)

        [StringLength(20)]
        public string Priority { get; set; } // "Low", "Medium", "High"

        [StringLength(255)]
        public string Title { get; set; } // Tiêu đề

        public string Description { get; set; } // Nội dung chi tiết

        public string? ImgUrl { get; set; } // Chuỗi chứa các đường dẫn ảnh đính kèm (phân cách bằng dấu phẩy ',' hoặc mã hóa JSON array)

        public bool IsResolved { get; set; } = false; // Trạng thái đã xử lý sự cố hay chưa
    }
}
