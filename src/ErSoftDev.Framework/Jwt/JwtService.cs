using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ErSoftDev.Framework.Jwt
{
    public class JwtService : IJwtService, IScopedDependency
    {
        private readonly IOptions<AppSetting> _optionsAppSetting;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public JwtService(IOptions<AppSetting> optionsAppSetting, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _optionsAppSetting = optionsAppSetting;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<TokenResponse> Generate(TokenRequest tokenRequest)
        {
            var secretKey = Encoding.UTF8.GetBytes(HighSecurity.JwtSecretKey);
            var encryptKey = Encoding.UTF8.GetBytes(HighSecurity.JwtEncryptKey);

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptKey),
                SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var tokenExpiry = DateTime.Now.AddSeconds(_optionsAppSetting.Value.Jwt.TokenExpirySecond);

            var descriptor = new SecurityTokenDescriptor()
            {
                Issuer = _optionsAppSetting.Value.Jwt.Issuer,
                Audience = _optionsAppSetting.Value.Jwt.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = tokenExpiry,
                SigningCredentials = signingCredentials,// Header and Payload coding
                //EncryptingCredentials = encryptingCredentials,//Header and Payload coding
                Subject = getClaims(tokenRequest.Subject)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            var generatedToken = tokenHandler.WriteToken(securityToken);

            return new TokenResponse()
            {
                Token = generatedToken,
                TokenExpiry = tokenExpiry
            };
        }

        private ClaimsIdentity getClaims(IEnumerable<Claim> subject)
        {
            var claimList = subject.ToList();
            return new ClaimsIdentity(claimList);
        }
    }
}