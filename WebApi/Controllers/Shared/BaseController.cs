using System.Security.Claims;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Shared;

[ApiController]
public class BaseController : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                throw new NotFoundException("user", "user");
            }
            
            var userClaims = identity.Claims.ToArray();
            if (!userClaims.Any())
            {
                return Guid.Empty;
            }

            return Guid.Parse(userClaims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        }
    }
}