using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Helpers
{
    public static class Emails
    {

        public static void SendEmailConfirmation(string username,byte[] key)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name, username),
                }),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };
                var token = tokenHandler.CreateToken(securityDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("email", "password"),
                    EnableSsl = true,
                };

                smtpClient.Send("email", username, "subject", tokenString);
            }
            catch
            {

            }
            
        }
        
        public static string DecodeConfirmation(string username,byte[] key)
        {

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var claims = handler.ValidateToken(username, validations, out var tokenSecure);
                return claims.Identity.Name;

            }
            catch
            {
                return null;
            }
            
        }
    }
}
