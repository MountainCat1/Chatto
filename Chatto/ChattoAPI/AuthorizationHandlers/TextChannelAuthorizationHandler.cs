using System.Security.Claims;
using Chatto.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace Chatto.AuthorizationHandlers;

public class TextChannelAuthorizationHandler : AuthorizationHandler<IsAMemberRequirement, TextChannel>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        IsAMemberRequirement requirement,
        TextChannel resource)
    {

        var claim = context.User.FindFirst(u => u.Type == ClaimTypes.NameIdentifier);

        if (resource.Users.Select(user => user.AccountId).Contains(int.Parse(claim.Value)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class IsAMemberRequirement : IAuthorizationRequirement
{
}