namespace MailScanner.Frontend;
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    public CustomAuthenticationStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _httpClient.GetFromJsonAsync<TokenResponse>("api/token");

        if (token?.AccessToken != null)
        {
            // User is authenticated
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, token.Username),
                new Claim(ClaimTypes.Role, token.Role)
            }, "BearerToken");

            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        else
        {
            // User is not authenticated
            var identity = new ClaimsIdentity();

            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
    }
}