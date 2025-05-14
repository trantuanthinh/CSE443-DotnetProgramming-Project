using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Project.AppContext;

namespace Project.Core
{
    public class BaseController : Controller
    {
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
            base.OnActionExecuting(context);
        }
    }
}
