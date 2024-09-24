using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Fantasy.Frontend.AuthenticationProviders;

public class AuthenticationProviderTest : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var anonimous = new ClaimsIdentity();
        var user = new ClaimsIdentity(authenticationType: "test");
        var admin = new ClaimsIdentity(new List<Claim>
    {
        new Claim("FirstName", "Jorge"),
        new Claim("LastName", "Lasso"),
        new Claim(ClaimTypes.Name, "jorgelasso_1992@hotmail.com")
        new Claim(ClaimTypes.Role, "Admin")
    },
   authenticationType: "test");

        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(admin)));
    }
}