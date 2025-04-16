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

            #region user
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
            #endregion

            #region category
            var categories = new List<string>
            {
                "Measuring Instruments",
                "Chemical Handling Instruments",
                "Safety Instruments",
            };

            var categoriesList = categories
                .Select(category => new Category
                {
                    Id = Guid.NewGuid(),
                    Name = category,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                })
                .ToList();
            #endregion

            #region item
            var images = new List<string> { };

            var items = new List<(string name, Guid categoryId, Category category, int quantity)>
            {
                ("Volumetric Flask", categoriesList[0].Id, categoriesList[0], 10),
                ("Electronic Balance", categoriesList[0].Id, categoriesList[0], 2),
                ("Flat-bottom Flask", categoriesList[1].Id, categoriesList[1], 3),
                ("Test Tubes", categoriesList[1].Id, categoriesList[1], 20),
                ("Safety Goggles", categoriesList[2].Id, categoriesList[2], 50),
                ("Lab Gloves", categoriesList[2].Id, categoriesList[2], 50),
            };

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
            var itemsList = items
                .Select(item =>
                {
                    string image = null;
                    var imagePath = Path.Combine(directoryPath, $"{item.name}.jpg");
                    if (File.Exists(imagePath))
                    {
                        byte[] imageBytes = File.ReadAllBytes(imagePath);
                        string base64String = Convert.ToBase64String(imageBytes);
                        image = base64String;
                    }

                    return new Item
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        Name = item.name,
                        CategoryId = item.categoryId,
                        Category = item.category,
                        Quantity = item.quantity,
                        Image = image,
                    };
                })
                .ToList();
            #endregion

            context.Users.AddRange(userList);
            context.Categories.AddRange(categoriesList);
            context.Items.AddRange(itemsList);
            context.SaveChanges();
        }
    }
}
