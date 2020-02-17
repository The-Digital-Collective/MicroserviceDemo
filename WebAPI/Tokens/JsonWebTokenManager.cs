using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/// <summary>
/// This class validates the passe in Jason Web Tokens (JWT). To do this it decrypts token 
/// using the secret, which is common between the calling and called applications (symmetric encryption). 
/// To support JWT processing the application requires installation of the System.IdentityModel.Tokens.Jwt NuGet package. 
/// </summary>
namespace WebAPI.Models
{
    public class TokenManager
    {
        // This method creates the Principal object using the token and then extracts the Identity object out of it.
        public static string ValidateToken(string token, string secret)
        {
            string username = null;
            ClaimsPrincipal principal = GetPrincipal(token, secret);

            if (principal == null)
            {
                return null;
            }

            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            Claim usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim.Value;
            return username;
        }

        // key (secret) required for encryption and decryption. 
        // The method reads the token in string format and converts it into a JwtSecurityToken
        public static ClaimsPrincipal GetPrincipal(string token, string secret)
        {
            try
            {
                // Convert the token (in string format) to a JWT Security Token
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

                // Bomb out if null
                if (jwtToken == null)
                {
                    return null;
                }

                // Generates parameters for use in the validation process and re-creates the key
                // using the same secret as used in the token generation step
                byte[] key = Convert.FromBase64String(secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);

                return principal;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

}