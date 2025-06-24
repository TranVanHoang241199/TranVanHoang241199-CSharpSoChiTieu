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
        public string currency { get; set; } = null;

        // Filter theo loại giao dịch
        public string? Type { get; set; }

        // Filter theo khoảng tiền
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }

        // Filter theo khoảng ngày
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Filter nhanh theo thời gian
        public string? QuickDate { get; set; }
    }
}
