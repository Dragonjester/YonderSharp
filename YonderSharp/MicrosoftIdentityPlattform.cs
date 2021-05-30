using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace YonderSharp
{
    //TODO: What happend to implementation against interface?
    /// <summary>
    /// Helper-class for the Microsoft identity Plattform
    /// https://docs.microsoft.com/de-de/azure/active-directory/develop/
    /// </summary>
    public class MicrosoftIdentityPlattform
    {
        private ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
               "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration",
               new OpenIdConnectConfigurationRetriever());

        private OpenIdConnectConfiguration config;
        private JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        private Dictionary<string, Guid> validatedGuidCach = new Dictionary<string, Guid>();


        /// <param name="token">JWT that might or might not represent a valid user</param>
        /// <returns>The Guid of the user. If the JWT is invalid: Guid.Empty</returns>
        public Guid GetValidatedUserId(string token)
        {
            JwtSecurityToken unvalidatedToken = new JwtSecurityToken(token);

            if (unvalidatedToken.ValidTo > DateTime.UtcNow)
            {
                if (config == null)
                {
                    config = configManager.GetConfigurationAsync().Result;
                }

                TokenValidationParameters validationParameters = new TokenValidationParameters
                {
                    //decode the JWT to see what these values should be
                    ValidAudience = unvalidatedToken.Audiences.First(),
                    ValidIssuer = unvalidatedToken.Issuer,
                    IssuerSigningKeys = config.SigningKeys, //2. .NET Core equivalent is "IssuerSigningKeys" and "SigningKeys"
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true
                };

                SecurityToken validatedToken;
                handler.ValidateToken(token, validationParameters, out validatedToken);

                string id = ((JwtSecurityToken)validatedToken).Claims.First(y => y.Type.Equals("oid", StringComparison.OrdinalIgnoreCase)).Value;

                validatedGuidCach.Add(token, Guid.Parse(id));

                return validatedGuidCach[token];
            }

            validatedGuidCach.Remove(token);

            return Guid.Empty;
        }
    }
}