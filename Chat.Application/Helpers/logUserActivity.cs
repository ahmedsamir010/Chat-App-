using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
namespace Chat.Application.Helpers
{
    public class logUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var resultContext=await next();
            if (!resultContext!.HttpContext.User.Identity!.IsAuthenticated)
            {
                return;
            }
            var userId = resultContext?.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var repo = resultContext?.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo!.GetUserByIdAsync(userId!);
              user!.LastActive = DateTime.Now;
          await repo.UpdateUser(user);
        }   
    }
}
