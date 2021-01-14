using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class UserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //pegar o contexto atual
            var resultContext = await next();

            //verificar se o usuario esta autenticado
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            //pegar os dados do usuario atual
            var userId = resultContext.HttpContext.User.GetUserId();

            //pedir acesso ao repositorio para atualizar um campo
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}
