// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Security;

/// <summary>
/// Checks if user is in `Administrators` role
/// </summary>
public class CheckAdministratorsRoleRequirement
    : AuthorizationHandler<CheckAdministratorsRoleRequirement>, IAuthorizationRequirement
{
    /// <summary>
    /// Makes a decision if authorization is allowed based on a specific requirement.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CheckAdministratorsRoleRequirement requirement)
    {
        if (context.User.IsInRole("Administrators"))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
