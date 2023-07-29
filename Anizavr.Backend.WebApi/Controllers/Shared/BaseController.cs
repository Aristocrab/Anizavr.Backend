using System.Security.Claims;
using Anizavr.Backend.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Anizavr.Backend.WebApi.Controllers.Shared;

[ApiController]
public class BaseController : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                throw new NotFoundException("Пользователь", "id", Guid.Empty.ToString());
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