using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace bimeh_back.Components.Filters
{
    /*public class Logger
    {
      
            private readonly AppDbContext _dbContext;
            private readonly HttpContext _httpContext;

            public Logger(HttpContext httpContext,AppDbContext dbContext)
            {
                _dbContext = dbContext;
                _httpContext = httpContext;
            }
            
            public  async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
               // _dbContext.disableLogger();

            }
            

    }*/
    
    public class RequestLoggingMiddleware
    {
        
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        
        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }
        
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                _logger.LogInformation(
                    "Request {method} {url} => {statusCode}",
                    context.Request?.Method,
                    context.Request?.Path.Value,
                    context.Response?.StatusCode);
            }
        }
        
    }
    
    
}