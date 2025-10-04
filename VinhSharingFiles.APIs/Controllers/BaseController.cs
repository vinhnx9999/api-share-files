using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VinhSharingFiles.APIs.Utilities;

namespace VinhSharingFiles.APIs.Controllers;

[Route("api/[controller]")]
[EnableCors("AllowPolicy")]
[ApiController]
[Authorize]
public class BaseController(IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    protected readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    
    protected string GetClaimUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value ?? string.Empty;
    }

    protected int GetUserId()
    {
        var accessToken = GetTokenFromContext();
        if (accessToken == null) return 0;

        var claimUserId = GetClaimUserId();
        var claimUId = claimUserId.ConvertingHex2Int(0);
        if (claimUId > 0) return claimUId;

        var claimIds = GetTokenClaim(accessToken, ClaimTypes.Sid);
        foreach (var str in (claimIds ?? "").Split(";"))
        {
            var userId = str.ConvertingHex2Int(0);
            if (userId > 0) return userId;
        }

        return 0;
    }

    private static string? GetTokenClaim(string token, string claimName)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

            // Use the code to get the claim value.
            var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;

            return claimValue;

        }
        catch (Exception)
        {
            // Handle token validation or reading errors.
            return null;
        }
    }

    private string GetTokenFromContext()
    {
        var _httpContext = _httpContextAccessor.HttpContext ?? throw new Exception("HttpContext is null");

        try
        {
            var curClaims = _httpContext.User.Claims;
            if (curClaims.Any())
            {
                var accessTokenClaim = curClaims.FirstOrDefault(c => c.Type == "id_token");
                return accessTokenClaim is null ? "" : accessTokenClaim.Value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetTokenFromContext] Error: {ex.Message}");
        }

        return "";
    }
}
