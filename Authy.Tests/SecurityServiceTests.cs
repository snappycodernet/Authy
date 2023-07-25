using Authy.Data.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Authy.Tests
{
    [TestClass]
    public class SecurityServiceTests
    {
        [TestMethod]
        public void TestPasswordHashingWorks()
        {
            var svc = new SecurityService();
            var pwd = "donutholesaregood123";

            var hash = svc.HashPasword(pwd, out byte[] salt);

            var isEqual = svc.VerifyPassword(pwd, hash, salt);
            var isNotEqual = svc.VerifyPassword("h43ihot54", hash, salt);

            Assert.IsTrue(isEqual);
            Assert.IsFalse(isNotEqual);
        }

        [TestMethod]
        public void TestGenerateTokenWorks()
        {
            var svc = new SecurityService();
            var iss = "https://localhost:5000";
            var aud = "https://localhost:3000";
            var secret = "some super secret key";

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, "test@test.com"),
                new Claim(ClaimTypes.GivenName, "Fred"),
                new Claim(ClaimTypes.Surname, "Fredrickson"),
                new Claim(ClaimTypes.Sid, "289"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "user"),
                new Claim(ClaimTypes.Role, "developer"),
            };

            var token = svc.GenerateToken(iss, aud, secret, claims, null);

            Assert.IsNotNull(token);

            var decodedToken = svc.DecodeToken(token);

            Assert.IsTrue(decodedToken.Claims.Any());

            var givenNameClaim = decodedToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);

            Assert.IsNotNull(givenNameClaim);
            Assert.AreEqual("Fred", givenNameClaim.Value);
        }
    }
}