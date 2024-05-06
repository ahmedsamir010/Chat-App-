namespace Chat.API.Controllers
{
    [ServiceFilter(typeof(logUserActivity))]
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize] 
    public class BaseController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
    }
}
