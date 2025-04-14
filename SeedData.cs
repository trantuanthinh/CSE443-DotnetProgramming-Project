using Microsoft.EntityFrameworkCore;
using Project.AppContext;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new DataContext(
            serviceProvider.GetRequiredService<DbContextOptions<DataContext>>()
        );

        // Kiểm tra nếu dữ liệu đã tồn tại
        if (context.Users.Any())
        {
            return; // Dữ liệu đã được seed
        }

        // Thêm dữ liệu mẫu
        // context.Users.AddRange(
        //     new User { Name = "Admin", Role = "manager" },
        //     new User { Name = "Lecturer", Role = "lecturer" }
        // );

        context.SaveChanges();
    }
}
