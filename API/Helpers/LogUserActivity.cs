using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter //: ControllerBase 
    {
        //private readonly IUserRepository _userRepository;
        //private readonly RequestDelegate _next;

        //public LogUserActivity(IUserRepository userRepository, RequestDelegate next)
        //{
        //    _userRepository = userRepository;
        //    _next = next;
        //}

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            //if user is not Authenticated, just return
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId(); // get user id // in the past i used to get username, but inside user repository we get bloatware like photos 
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>(); //get repo
            var user = await repo.GetUserByIdAsync(userId); //get real user obj from repo

            //edit current user Last Active prop
            user.LastActive = DateTime.Now;

            await repo.SaveAllAsync(); //save changes on repo / db
        }


        //public async Task InvokeAsync(HttpContext context)
        //{
        //    await _next(context);

        //    var username = User.GetUsername(); // get username from jtw
        //    var user = await _userRepository.GetUserByUsernameAsync(username); //get real user obj from db / repo


        //    user.LastActive = DateTime.Now;
        //    await _userRepository.SaveAllAsync();

        //}
    }
}
