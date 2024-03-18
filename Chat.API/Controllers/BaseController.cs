namespace Chat.API.Controllers
{
    [ServiceFilter(typeof(logUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize] 
    public class BaseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
