namespace ErSoftDev.Framework.Jwt
{
    public interface IJwtService
    {
        Task<TokenResponse> Generate(TokenRequest tokenRequest);
    }
}