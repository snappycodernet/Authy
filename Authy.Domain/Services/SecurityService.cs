using Authy.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Domain.Services
{
    public class SecurityService : ISecurityService
    {
        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        private readonly byte[] _privateKey = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }; // NOTE: You should use a private-key that's a LOT longer than just 4 bytes.
        private readonly TimeSpan _passwordResetExpiry = TimeSpan.FromMinutes(60);
        private const byte _version = 1; // Increment this whenever the structure of the message changes.

        public string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);
            return Convert.ToHexString(hash);
        }

        public bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);

            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }

        public JwtSecurityToken GenerateToken(string issuer, string audience, string secretKey, List<Claim> claims, DateTime? expirationDate)
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signinCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims ?? new List<Claim>(),
                notBefore: DateTime.UtcNow,
                expires: expirationDate ?? DateTime.UtcNow.AddDays(14),
                signingCredentials: signinCredentials
            );

            return token;
        }

        public string EncodeToken(JwtSecurityToken token)
        {
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        public JwtSecurityToken DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            return jwtSecurityToken;
        }

        public string CreatePasswordResetHmacCode(long userId, string privateKey)
        {
            byte[] message = Enumerable.Empty<byte>()
                .Append(_version)
                .Concat(BitConverter.GetBytes(userId))
                .Concat(BitConverter.GetBytes(DateTime.UtcNow.ToBinary()))
                .ToArray();

            using (HMACSHA256 hmacSha256 = new HMACSHA256(key: Encoding.ASCII.GetBytes(privateKey)))
            {
                byte[] hash = hmacSha256.ComputeHash(buffer: message, offset: 0, count: message.Length);

                byte[] outputMessage = message.Concat(hash).ToArray();
                string outputCodeB64 = Convert.ToBase64String(outputMessage);
                string outputCode = outputCodeB64.Replace('+', '-').Replace('/', '_');
                return outputCode;
            }
        }

        public bool VerifyPasswordResetHmacCode(string codeBase64Url, string privateKey, out long? userId)
        {
            userId = null;
            string base64 = codeBase64Url.Replace('-', '+').Replace('_', '/');
            byte[] message = Convert.FromBase64String(base64);

            byte version = message[0];
            if (version < _version) return false;

            userId = BitConverter.ToInt64(message, startIndex: 1); // Reads bytes message[1,2,3,4]
            Int64 createdUtcBinary = BitConverter.ToInt64(message, startIndex: 1 + sizeof(Int64)); // Reads bytes message[5,6,7,8,9,10,11,12]

            DateTime createdUtc = DateTime.FromBinary(createdUtcBinary);
            if (createdUtc.Add(_passwordResetExpiry) < DateTime.UtcNow) return false;

            const Int32 _messageLength = 1 + sizeof(Int64) + sizeof(Int64); // 1 + 4 + 8 == 13

            using (HMACSHA256 hmacSha256 = new HMACSHA256(key: Encoding.ASCII.GetBytes(privateKey)))
            {
                Byte[] hash = hmacSha256.ComputeHash(message, offset: 0, count: _messageLength);

                Byte[] messageHash = message.Skip(_messageLength).ToArray();
                return Enumerable.SequenceEqual(hash, messageHash);
            }
        }
    }
}
