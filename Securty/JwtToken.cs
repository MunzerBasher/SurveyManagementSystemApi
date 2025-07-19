using System.Text.Json;

namespace SurveyManagementSystemApi.Securty
{
    public class JwtToken : IJwtToken
    {
        private readonly IOptions<Jwt> _jwt;

        public JwtToken(IOptions<Jwt> jwt)
        {
            _jwt = jwt;
        }

        

        public string GenerateToken(UserIdentity user, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            Claim[] claims = [
                new (JwtRegisteredClaimNames.Sub, user.Id ),
                new (JwtRegisteredClaimNames.Email,user.Email! ),
                new (JwtRegisteredClaimNames.GivenName, user.FirstName ),
                new (JwtRegisteredClaimNames.FamilyName,user.LastName ),
                new (JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString() ),
                new(nameof(roles), JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray),
                new(nameof(permissions), JsonSerializer.Serialize(permissions), JsonClaimValueTypes.JsonArray)
                ];
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var expiresIn = 30;

            var token = new JwtSecurityToken(
                issuer: _jwt.Value.Issuer,
                audience: _jwt.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresIn),
                signingCredentials: signingCredentials
            );
            var Jwttoken = new JwtSecurityTokenHandler().WriteToken(token);
            return Jwttoken;
        }

        public string? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.Key));

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = false,

                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
                return userId;

            }
            catch 
            { 
                return null;
            }

        }


    }
}