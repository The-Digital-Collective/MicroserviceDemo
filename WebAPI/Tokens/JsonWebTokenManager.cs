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
        // HMACSHA256 generated secret key required for encryption and decryption
        // To do: store in the Key Vault for Azure implementation
        private static string Secret = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";

        // Token genaration method where a token is created and a string version of it returned
        public static string GenerateToken(string username)
        {
            /// <summary>
            /// Token genaration method where a token is created and a string version of it returned.
            /// Creates the descriptor object, which represents the main content of the JWT, 
            /// such as the claims, the expiration date and the signing information. 
            /// The token is created and a string version of it is returned. In this example
            /// only the username claim is added but the list of claim types that can be added is 
            /// large. 
            /// It is also possible to add custom data to the token post creation by accessing the JWT
            /// payload property with code like: token.Payload["NicksFavouriteFood"] = "Huel";
            /// </summary>

            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, username)}),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

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