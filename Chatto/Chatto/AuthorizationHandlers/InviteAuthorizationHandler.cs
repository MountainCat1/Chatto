using System.Security.Claims;
using Chatto.Infrastructure;
using Chatto.Services;
using Microsoft.AspNetCore.Authorization;

namespace Chatto.AuthorizationHandlers;

public class InviteAuthorizationHandler : AuthorizationHandler<IsTargetRequirement, TextChannelInvite>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        IsTargetRequirement requirement,
        TextChannelInvite resource)
    {

        var claim = context.User.FindFirst(u => u.Type == ClaimTypes.NameIdentifier);
        
        //if(resource.TargetAccountId == int.Parse(claim.Value))
        {
            context.Succeed(requirement);
        }
    }
}

public class IsTargetRequirement : IAuthorizationRequirement
{
}