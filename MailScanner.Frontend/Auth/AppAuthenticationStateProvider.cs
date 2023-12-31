﻿

namespace MailScanner.Frontend;

public class AppAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime jsRuntime;

    public AppAuthenticationStateProvider(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }
    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var anonymous = new ClaimsIdentity();
        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
    }
}
