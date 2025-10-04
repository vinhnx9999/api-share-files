using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace VinhSharingFiles.APIs.Extensions;

public static class JwtSecurityExtensions
{
    public static Task TokenJwtValidatedAsync(this TokenValidatedContext context)
    {
        if (context.SecurityToken is JwtSecurityToken accessToken)
        {
            if (context?.Principal?.Identity is ClaimsIdentity identity)
            {
                identity.AddClaim(new Claim("id_token", accessToken.RawData));
                return Task.CompletedTask;
            }
        }
        else
        {
            var strToken = context.SecurityToken;
            var rawData = ((Microsoft.IdentityModel.JsonWebTokens.JsonWebToken)strToken).EncodedToken;
            Console.WriteLine(strToken.ToString());
            if (context?.Principal?.Identity is ClaimsIdentity identity)
            {
                identity.AddClaim(new Claim("id_token", rawData));
                return Task.CompletedTask;
            }
        }

        return Task.CompletedTask;
    }
}
