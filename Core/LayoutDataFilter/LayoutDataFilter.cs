using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Project.Interfaces;
using Project.Utils;

namespace Project.Core.Filters
{
    public class LayoutDataFilter : IAsyncActionFilter
    {
        private readonly IUserService _userService;

        public LayoutDataFilter(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            if (context.Controller is Controller controller)
            {
                var user = controller.User;
                if (user.Identity != null && user.Identity.IsAuthenticated)
                {
                    var userList = await _userService.GetManagers();
                    if (user.HasClaim(ClaimTypes.Role, UserType.Manager.ToString()))
                    {
                        userList = await _userService.GetLecturers();
                    }

                    controller.ViewData["UserConversations"] = userList;
                }
            }
            await next();
        }
    }
}
