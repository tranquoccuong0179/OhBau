using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using OhBau.Model.Entity;
using OhBau.Model.Enum;

namespace OhBau.Model.Utils
{
    public class JwtUtil
    {
        private JwtUtil()
        {
        }

        public static string GenerateJwtToken(Account account, Tuple<string, Guid> guidClaim)
        {
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

            byte[] keyBytes = Convert.FromHexString("0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F00");
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, account.Phone.ToString()),
                new Claim("role", account.Role),
            };

            if (guidClaim != null)
            {
                claims.Add(new Claim(guidClaim.Item1, guidClaim.Item2.ToString()));
            }

            var expires = (account.Role.Equals(RoleEnum.FATHER.GetDescriptionFromEnum()) ||
                           account.Role.Equals(RoleEnum.MOTHER.GetDescriptionFromEnum()))
                ? DateTime.Now.AddDays(15)
                : DateTime.Now.AddDays(30);

            var token = new JwtSecurityToken(
                issuer: "OhBau",
                audience: null,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expires,
                signingCredentials: credentials
                );
            return jwtHandler.WriteToken(token);
        }

        //public static string GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[32];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomNumber);
        //    }
        //    return Convert.ToBase64String(randomNumber);
        //}
    }
}
