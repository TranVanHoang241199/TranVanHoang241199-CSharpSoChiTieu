
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
        public string SearchValue { get; set; }


        // Trạng thái order

        //public int Status { get; set; } = 0;

        //Product

        /// <summary>
        /// 
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SupplierID { get; set; }
    }
}
