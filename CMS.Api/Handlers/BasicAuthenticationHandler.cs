namespace CMS.Api.Handlers
{
    // ReSharper disable StyleCop.SA1402
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Text.Encodings.Web;
    using Domain.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Options;
    using Shared.Enums;
    using Shared.Handlers;

    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
    }

    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly IUserTokenService userTokenServices;

        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserTokenService userTokenServices)
            : base(options, logger, encoder, clock)
        {
            this.userTokenServices = userTokenServices;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!this.Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            string authorizationHeader = this.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            if (!authorizationHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            var token = authorizationHeader.Substring("bearer".Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            try
            {
                return await this.ValidateTokenAsync(token);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        private async Task<AuthenticateResult> ValidateTokenAsync(string token)
        {
            var userToken = await this.userTokenServices.FindAsync(token);
            if (userToken.UserTokenId == 0)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            if (DateTimeHandler.Difference(userToken.ExpiryDate, DateTime.Now, CalenderType.Minutes) > 0)
            {
                await this.userTokenServices.DeleteExpiredTokensAsync(userToken.UserAccountId);
                return AuthenticateResult.Fail("Expired");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, userToken.RoleName),
                new Claim(ClaimTypes.NameIdentifier, userToken.UserAccountId.ToString()),
                new Claim(ClaimTypes.Name, userToken.Username),
                new Claim(ClaimTypes.GivenName, userToken.FullName),
                new Claim("UserId", userToken.UserId.ToString()),
                new Claim("RoleId", userToken.RoleId.ToString())
            };
            var identity = new ClaimsIdentity(claims, this.Scheme.Name);
            var principal = new GenericPrincipal(identity, new[] { userToken.RoleName });
            var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}