using System.Text.RegularExpressions;

namespace Project.Utils
{
    // DONE
    public class SharedService
    {
        public string ImageToBase64(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            // Convert IFormFile to a byte array
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();

            // Convert byte array to Base64 string
            return Convert.ToBase64String(bytes);
        }
    }
}
