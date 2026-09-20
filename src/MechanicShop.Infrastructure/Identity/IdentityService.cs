using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Infrastructure.Identity.Policy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
namespace MechanicShop.Infrastructure.Data.Interceptors;

public class IdentityService(
    UserManager<AppUser> userManager,
    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService
) : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public async Task<Result<AppUserDto>> AuthenticationAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(user is null)
        {
            return Error.NotFound("User_Not_Found",$"User with email {UtilityService.MaskEmail(email)} not found.");
        }
        if (!user.EmailConfirmed)
        {
            return Error.Conflict("Email_Not_Confirmed",$"email '{UtilityService.MaskEmail(email)}' not confirmed");
        }
        if(!await _userManager.CheckPasswordAsync(user, password))
        {
            return  Error.Conflict("Invalid_Login_Attempt","Email / Password are incorrect");
        }
        return new AppUserDto(user.Id,user.Email!,await _userManager.GetRolesAsync(user),await _userManager.GetClaimsAsync(user));

    }

    public async Task<bool> AuthorizeAsync(string userId, string? policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if(user is null)
        {
            return false;
        }
        var principal =  await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal,policyName!);
        return result.Succeeded;
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if(user is null)
        {
            return Error.NotFound("User_Not_Found",$"User with id {userId} not found.");
        }
        return new AppUserDto(user.Id,user.Email!,await _userManager.GetRolesAsync(user),await _userManager.GetClaimsAsync(user));

    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.UserName;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if(user is null)
        {
            return false;
        }
        return await _userManager.IsInRoleAsync(user,role);
    }
}