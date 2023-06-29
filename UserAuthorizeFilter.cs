using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using bimeh_back.Components.Extensions;
using bimeh_back.Components.Response;
using bimeh_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;


namespace bimeh_back.Components.Filters
{
    public class UserAuthorizeFilter : ActionFilterAttribute
    {
        private readonly AppDbContext _dbContext;

        public UserAuthorizeFilter(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var authorizedUser = context.HttpContext.User;
            if (!authorizedUser.Identity.IsAuthenticated) {
                await next();
                return;
            }

            if (!await TokenExists(context.HttpContext.Request.Headers["Authorization"].ToString())) {
                context.Result = ResponseFormat.NotAuthMsg();
                return;
            }

            var user = await GetAuthenticatedUser(authorizedUser);
            if (user == null) {
                context.Result = ResponseFormat.NotAuthMsg();
                return;
            }

            switch (CheckUserAccount(authorizedUser, user)) {
                case 0:
                    context.Result = ResponseFormat.PermissionDeniedMsg("حساب کاربری شما غیرفعال شده است.");
                    return;
            }

            var routePolicy = GetRoutePolicy(context.ActionDescriptor.EndpointMetadata);

            if (CheckRoutePolicy(authorizedUser, routePolicy)) {
                if (context.Result == null) {
                    ((ControllerExtension) context.Controller).AuthenticatedUser = user;
                    await next();
                }
            }
            else {
                context.Result = null;
                ((ControllerExtension) context.Controller).AuthenticatedUser = user;
                await next();
            }
        }

        private async Task<object> GetAuthenticatedUser(ClaimsPrincipal authorizedUser)
        {
            if (authorizedUser.IsInRole(Policies.Admin)) {
                return await _dbContext.Admins
                    .Include(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                    .ThenInclude(x => x.Permission)
                    .FirstOrDefaultAsync(x =>
                        x.Id == long.Parse(authorizedUser.FindFirstValue("id")));
            }

            if (authorizedUser.IsInRole(Policies.User)) {
                return await _dbContext.Users.FirstOrDefaultAsync(x =>
                    x.Id == long.Parse(authorizedUser.FindFirstValue("id")));
            }

            return null;
        }

        private int CheckUserAccount(ClaimsPrincipal authorizedUser, object user)
        {
            /*if (authorizedUser.IsInRole(Policies.Admin)) {
                return ((Admin) user).Active ? 1 : 0;
            }*/

            if (authorizedUser.IsInRole(Policies.User)) {
                return ((User) user).Active ? 1 : 0;
            }

            return 1;
        }

        private async Task<bool> TokenExists(string accessToken)
        {
            return await _dbContext.Tokens.AnyAsync(token => token.AccessToken == accessToken);
        }

        private bool CheckRoutePolicy(ClaimsPrincipal authorizedUser, string routePolicy)
        {
            return routePolicy != null && authorizedUser.IsInRole(routePolicy);
        }

        private string GetRoutePolicy(IEnumerable<object> endPointMetadata)
        {
            var authorizeAttributes = endPointMetadata.OfType<AuthorizeAttribute>().ToList();
            if (authorizeAttributes.Count == 0) return null;
            var authorizeAttribute = authorizeAttributes[0];
            return authorizeAttribute.Policy;
        }
    }
}