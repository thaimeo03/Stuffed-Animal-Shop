namespace Stuffed_Animal_Shop.Utilities
{
    public class RandomCustom
    {
        public string RandomString(int length)
        {
            // Chuỗi chứa tất cả các ký tự mà bạn muốn sử dụng để tạo xâu
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // Sử dụng Random để chọn ngẫu nhiên các ký tự từ chuỗi chars
            Random random = new Random();

            // Tạo xâu ngẫu nhiên bằng cách chọn ngẫu nhiên các ký tự từ chuỗi chars
            string randomString = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomString;
        }
    }
}
