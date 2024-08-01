using ImpersonationDemo.Models;
using System.Security.Principal;

namespace ImpersonationDemo.Middleware
{
    public class WindowAuthenticationMiddleware
    {
        private readonly RequestDelegate _nextRequestDelegate;

        public WindowAuthenticationMiddleware(RequestDelegate requestDelegate)
        {
            _nextRequestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext httpContext, AuthenticationData authenticationData)
        {
            if (httpContext != null)
            {
                if (httpContext.User != null && httpContext.User.Identity != null)
                {
                    var userIdentity = (WindowsIdentity?)httpContext.User.Identity;

                    if (userIdentity != null) 
                    {
                        authenticationData.WindowsIdentity = userIdentity.Clone();
                    }
                }

                await _nextRequestDelegate(httpContext);
            }
        }
    }
}
