using Newtonsoft.Json;

namespace CSharpSoChiTieu.Models
{
    public static class SessionExtensions
    {
        // Lưu đối tượng vào session dưới dạng JSON
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            var jsonString = JsonConvert.SerializeObject(value);
            session.SetString(key, jsonString); // Lưu trữ chuỗi JSON
        }

        // Lấy đối tượng từ session và chuyển ngược lại thành kiểu đã chỉ định
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default : JsonConvert.DeserializeObject<T>(value); // Deserialize chuỗi JSON thành đối tượng
        }
    }
}
