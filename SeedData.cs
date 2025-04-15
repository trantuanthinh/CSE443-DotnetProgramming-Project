using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Project.AppContext;
using Project.Models;
using Project.Utils;

namespace Project
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new DataContext(
                serviceProvider.GetRequiredService<DbContextOptions<DataContext>>()
            );

            if (context.Users.Any())
            {
                return;
            }

            string password = "quametmoi";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var users = new List<(
                string Username,
                string Name,
                string Email,
                int PhoneNumber,
                UserType Role
            )>
            {
                (
                    "nguyenxuancuong",
                    "Nguyen Xuan Cuong",
                    "conan246817@gmail.com",
                    123456789,
                    UserType.Lecturer
                ),
                (
                    "tranvantai",
                    "Tran Van Tai",
                    "tranvantai@gmail.com",
                    987654321,
                    UserType.Lecturer
                ),
                ("nguyenvana", "Nguyen Van A", "nguyenvana@gmail.com", 123789456, UserType.Manager),
                ("tranthib", "Tran Thi B", "tranthib@gmail.com", 987321654, UserType.Manager),
            };

            var userList = users
                .Select(user => new User
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Username = user.Username,
                    Name = user.Name,
                    Email = user.Email,
                    Password = hashedPassword,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    LoginType = LoginType.Standard,
                })
                .ToList();

            context.Users.AddRange(userList);
            context.SaveChanges();
        }
    }
}
