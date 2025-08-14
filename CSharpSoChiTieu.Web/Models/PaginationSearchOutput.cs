using CSharpSoChiTieu.Business.Services;

namespace CSharpSoChiTieu.Web.Models
{
    /// <summary>
    /// Lớp cở sở dùng để biểu diễn kết quả tìm kiếm dưới dạng phân trang
    /// </summary>
    public abstract class PaginationSearchOutput
    {
        /// <summary>
        /// Trang được hiển thị
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

        /// <summary>
        /// Số dòng dữ liệu tìm được 
        /// </summary>
        public int RowCount { get; set; }


        /// <summary>
        /// Số trang
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int p = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    p += 1;
                return p;
            }
        }

    }


    public class CategorySearchOutput : PaginationSearchOutput
    {
        public List<CategoryViewModel>? Data { get; set; }
    }

    public class IncomeExpenseSearchOutput : PaginationSearchOutput
    {
        public List<IncomeExpenseViewModel>? Data { get; set; }
    }

    public class HistorySearchOutput : PaginationSearchOutput
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }

        public List<IncomeExpenseViewModel>? Data { get; set; }
        public List<IEGroupViewModel>? Groups { get; set; }
    }
}
