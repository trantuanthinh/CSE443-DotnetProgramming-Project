using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Project.AppContext;
using Project.Core.Extensions;
using Project.Models;

namespace Project.Core
{
    public class BaseController : Controller
    {
        protected User CurrentUser;
        protected readonly IMapper? _mapper;
        protected readonly DataContext? _dataContext;
        protected readonly ILogger? _logger;

        public BaseController() { }

        public BaseController(
            DataContext? dataContext = null,
            IMapper? mapper = null,
            ILogger? logger = null
        )
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            CurrentUser = HttpContext.Session.GetObject<User>("CurrentUser");
            if (CurrentUser != null)
            {
                ViewBag.User = CurrentUser;
            }
            base.OnActionExecuting(context);
        }
    }
}
