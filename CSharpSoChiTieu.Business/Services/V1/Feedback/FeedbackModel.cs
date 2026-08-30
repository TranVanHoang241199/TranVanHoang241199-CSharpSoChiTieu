using CSharpSoChiTieu.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSoChiTieu.Business.Services
{
    // Model dùng để nhận dữ liệu từ Form gửi lên (API / Form submit)
    public class FeedbackCreateModel
    {
        public string Type { get; set; } // "Feedback" hoặc "Bug"
        public string Priority { get; set; } // "Low", "Medium", "High"
        public string Title { get; set; }
        public string Description { get; set; }
        public List<IFormFile>? Attachments { get; set; } // File ảnh gửi từ Form
    }

    // Model DTO hiển thị danh sách lên View
    public class FeedbackViewModel : EntityBase
    {
        public string Type { get; set; }
        public string Priority { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImgUrl { get; set; }
        public bool IsResolved { get; set; }

        // Thuộc tính hiển thị trạng thái động cho View
        public string Status => IsResolved ? "Resolved" : "Pending";
    }
}
