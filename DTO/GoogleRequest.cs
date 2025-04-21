using Project.Utils;

namespace Project.DTO
{
    public class GoogleRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public LoginType LoginType { get; set; } = LoginType.Google;
    }
}
