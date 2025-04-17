using System.Net;

namespace API_HotelManagement.common
{
    public class OperationResult
    {
        public OperationResult(HttpStatusCode status, string message)
        {
            Status = status;
            Message = message;
        }

        public OperationResult(string message)
        {
            Message = message;
        }

        public OperationResult()
        {
        }

        //[JsonProperty(Order = 1)]
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;

        public string Message { get; set; } = "Success";
        //public long TotalTime { get; set; } = 0;
    }

    #region login
    public class OperationResultAuth : OperationResult
    {
        public OperationResultAuth(string accessToken) : base(HttpStatusCode.OK, "Success")
        {
            AccessToken = accessToken;
        }

        public OperationResultAuth(HttpStatusCode status, string accessToken) : base(status, "OK")
        {
            AccessToken = accessToken;
        }

        public OperationResultAuth(HttpStatusCode status, string accessToken, string message) : base(status, message)
        {
            AccessToken = accessToken;
        }

        //[JsonProperty(Order = 2)] // cho nằm vt cột 3
        public string AccessToken { get; set; }
    }
    #endregion login


    #region Get

    public class OperationResult<T> : OperationResult
    {
        public OperationResult(T data) : base(HttpStatusCode.OK, "Success")
        {
            Data = data;
        }

        public OperationResult(HttpStatusCode status, T data) : base(status, "OK")
        {
            Data = data;
        }

        public OperationResult(HttpStatusCode status, T data, string message) : base(status, message)
        {
            Data = data;
        }

        public T Data { get; set; }
    }

    public class OperationResultList<T> : OperationResult
    {
        public OperationResultList(List<T> data) : base(HttpStatusCode.OK, "Success")
        {
            Data = data;
        }

        public List<T> Data { get; set; }
    }

    public class OperationResultPagination<T> : OperationResult
    {
        public OperationResultPagination(List<T> data, long totalItems, int currentPage, int pageSize) : base(HttpStatusCode.OK, "Success")
        {
            Meta = new PaginationMeta
            {
                TotalItems = totalItems,
                CurrentPage = currentPage,
                PageSize = pageSize
            };
            Data = data;
        }

        public PaginationMeta Meta { get; set; }
        public List<T> Data { get; set; }
    }

    public class PaginationMeta
    {
        /// <summary>
        /// tổng số phần tử 
        /// </summary>
        public long TotalItems { get; set; }
        /// <summary>
        /// số lượng trang dữ liệu mà bạn có
        /// (được tính dựa vào totalItems và PageSize)
        /// </summary>
        public int TotalPage
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int p = (int)(TotalItems / PageSize);
                if (TotalItems % PageSize > 0)
                    p += 1;
                return p;
            }
        }
        /// <summary>
        /// trang dữ liệu hiện tại mà bạn đang yêu cầu hoặc đang xem (Nhập vào)
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// kích thước của mỗi trang dữ liệu (Nhập vào)
        /// </summary>
        public int PageSize { get; set; }
    }

    #endregion Get

    #region Post

    public class OperationResultObject<T> : OperationResult
    {
        public OperationResultObject(T data) : base(HttpStatusCode.OK, "Success")
        {
            Data = data;
        }

        public OperationResultObject(T data, string message) : base(HttpStatusCode.OK, message)
        {
            Data = data;
        }

        public OperationResultObject(T data, string message, HttpStatusCode status) : base(status, message)
        {
            Data = data;
        }

        public T Data { get; set; }
    }

    #endregion Post

    #region Update

    public class OperationResultUpdate : OperationResult
    {
        public OperationResultUpdate(Guid id) : base(HttpStatusCode.OK, "Success")
        {
            Data = new OperationResultUpdateModel { Id = id };
        }

        public OperationResultUpdate(Guid id, string message) : base(HttpStatusCode.OK, message)
        {
            Data = new OperationResultUpdateModel { Id = id };
        }

        public OperationResultUpdate(HttpStatusCode status, string message, Guid id) : base(status, message)
        {
            Data = new OperationResultUpdateModel { Id = id };
        }

        public OperationResultUpdateModel Data { get; set; }
    }

    public class OperationResultUpdateMulti : OperationResult
    {
        public OperationResultUpdateMulti(List<OperationResultUpdate> data) : base(HttpStatusCode.OK, "Success")
        {
            Data = data;
        }

        public List<OperationResultUpdate> Data { get; set; }
    }

    public class OperationResultUpdateModel
    {
        public Guid Id { get; set; }
    }

    #endregion Update

    #region Delete

    public class OperationResultDelete : OperationResult
    {
        public OperationResultDelete(Guid id, string name) : base(HttpStatusCode.OK, "Success")
        {
            Data = new OperationResultDeleteModel { Id = id, Name = name };
        }

        public OperationResultDelete(HttpStatusCode status, string message, Guid id, string name) : base(status, message)
        {
            Data = new OperationResultDeleteModel { Id = id, Name = name };
        }

        public OperationResultDeleteModel Data { get; set; }
    }

    public class OperationResultDeleteMulti : OperationResult
    {
        public OperationResultDeleteMulti(List<OperationResultDelete> data) : base(HttpStatusCode.OK, "Success")
        {
            Data = data;
        }

        public List<OperationResultDelete> Data { get; set; }
    }

    public class OperationResultDeleteModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }

    #endregion Delete

    #region Error

    public class OperationResultError : OperationResult
    {
        public OperationResultError(HttpStatusCode status, string message, List<Dictionary<string, string>> errorDetail = null) : base(status, message)
        {
            ErrorDetail = errorDetail;
        }

        public List<Dictionary<string, string>> ErrorDetail { get; set; }
    }

    #endregion Error
}
