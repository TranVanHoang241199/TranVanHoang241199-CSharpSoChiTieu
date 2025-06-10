
namespace CSharpSoChiTieu.Models
{
    public class PaginationSearchInput
    {
        /// <summary>
        /// Trang cần hiển thị
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Số dòng trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gía trị tìm kiếm 
        /// </summary>
        public string? SearchValue { get; set; }
    }


    public class PaginationHistorySearchInput
    {
        /// <summary>
        /// Trang cần hiển thị
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Số dòng trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gía trị tìm kiếm 
        /// </summary>
        public string? SearchValue { get; set; }

        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
    }
}
