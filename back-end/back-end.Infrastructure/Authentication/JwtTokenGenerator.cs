using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


using System.Security.Claims;
using System.Text;
using backend.Application.Common.Interfaces.Authentication;
using backend.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Options;
using backend.Domain.Entities;
namespace backend.Infrastructure.Authentication;



public class JwtTokenGenerator : IJwtTokenGenerator
{

    private readonly JwtSettings _jwtSettings;
    private readonly IDateTimeProvider _dateTimeProivder;

    public JwtTokenGenerator(IDateTimeProvider dateTimeProivder, IOptions<JwtSettings> jwtOptions)
    {
        _dateTimeProivder = dateTimeProivder;
        _jwtSettings = jwtOptions.Value;
    }

    public string GenerateToken(User user)
    {
    
        var signingCredentials =  new SigningCredentials(

            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                SecurityAlgorithms.HmacSha256
        );

       var claims =  new []
       {
           new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
           new Claim(JwtRegisteredClaimNames.FamilyName, user.FullName),
           new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
       };

       var securityToken  =  new JwtSecurityToken(
        issuer: _jwtSettings.issuer,
        audience: _jwtSettings.Audience,
        expires: _dateTimeProivder.UtcNow.AddMinutes(_jwtSettings.ExpryMinutes),
        claims : claims,
        signingCredentials : signingCredentials
        );


        return new JwtSecurityTokenHandler().WriteToken(securityToken);
        
    }
}