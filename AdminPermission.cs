using System;
using System.Linq;
using System.Threading.Tasks;
using bimeh_back.Components.Extensions;
using bimeh_back.Components.Response;
using bimeh_back.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace bimeh_back.Components.Filters
{
    public class AdminPermission : ActionFilterAttribute
    {
        private readonly string _permission;

        public AdminPermission(string permission)
        {
            _permission = permission;
        }

        // public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        // {
        //     var admin = (Admin) ((ControllerExtension) context.Controller).AuthenticatedUser;
        //
        //     if (!admin.Role.RolePermissions
        //         .Any(x => x.Permission.Code.Equals(_permission, StringComparison.CurrentCultureIgnoreCase)))
        //     {
        //         context.Result = ResponseFormat.PermissionDeniedMsg("شما به این قسمت دسترسی ندارید.");
        //         return;
        //     }
        //
        //     await next();
        // }
    }
}