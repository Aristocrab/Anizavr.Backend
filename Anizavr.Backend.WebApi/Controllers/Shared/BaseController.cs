using System.Security.Claims;
using Anizavr.Backend.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Anizavr.Backend.WebApi.Controllers.Shared;

[ApiController]
public class BaseController : ControllerBase
{
    protected Guid UserId => GetCurrentUserId();

    private Guid GetCurrentUserId()
    {
        if (HttpContext.User.Identity is not ClaimsIdentity identity)
        {
            throw new UnauthorizedException(
                Guid.Empty,
                "Пользователь", 
                "id");
        }

        var userClaims = identity.Claims.ToArray();
        if (!userClaims.Any())
        {
            return Guid.Empty;
        }

        return Guid.Parse(userClaims.FirstOrDefault(x => x.Type == "UserId")!.Value);
    }
}