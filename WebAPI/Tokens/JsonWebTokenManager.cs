using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/// <summary>
/// This class creates and validates Jason Web Tokens (JWT). 
/// It requires installation of the System.IdentityModel.Tokens.Jwt package. 
/// The process uses symmetric encryption where a single key is used to encrypt 
/// and decrypt information passed between applications. 
/// </summary>
namespace WebAPI.Models
{
    public class TokenManager
    {
        // key (secret) required for encryption and decryption. 
        // Held in code for demo only. Store in the Key Vault for non-demo implementations
        private static string Secret = "ALLWORKANDNOPLAYMAKESJACKADULLBOY123";

        // The method reads the token in string format and converts it into a JwtSecurityToken
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                // Convert the token (in string format) to a JWT Security Token
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

                // Bonb out if null
                if (jwtToken == null)
                {
                    return null;
                }

                // Generate parameters for use in the validation process and re-creates the key
                // using the same secret as used in the token generation step
                byte[] key = Convert.FromBase64String(Secret);
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

        // This method creates the Principal object using the token and then extracts the Identity object out of it.
        public static string ValidateToken(string token)
        {
            string username = null;
            ClaimsPrincipal principal = GetPrincipal(token);

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
    }

}