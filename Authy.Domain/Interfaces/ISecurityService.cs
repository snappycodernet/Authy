using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Authy.Domain.Interfaces
{
    public interface ISecurityService
    {
        JwtSecurityToken DecodeToken(string token);
        string EncodeToken(JwtSecurityToken token);
        JwtSecurityToken GenerateToken(string issuer, string audience, string secretKey, List<Claim> claims, DateTime? expirationDate);
        string HashPasword(string password, out byte[] salt);
        bool VerifyPassword(string password, string hash, byte[] salt);
    }
}