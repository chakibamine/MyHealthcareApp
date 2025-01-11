using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyHealthcareApp.Models;
using MyHealthcareApp.Services;

namespace MyHealthcareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;

        public AuthController(
            MyHealthcareAppContext context, 
            IConfiguration configuration,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email and password are required");
            }

            // Try to find patient first
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Email == request.Email);

            if (patient != null)
            {
                try 
                {
                    var isValidPassword = _passwordHasher.VerifyPassword(request.Password, patient.Password);
                    if (isValidPassword)
                    {
                        var token = GenerateJwtToken(patient.Id.ToString(), "Patient");
                        return Ok(new
                        {
                            Token = token,
                            Role = "Patient",
                            UserId = patient.Id,
                            Name = patient.Nom,
                            Email = patient.Email
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Password verification error: {ex.Message}");
                }
            }

            // Try to find doctor
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Email == request.Email);

            if (doctor != null)
            {
                try 
                {
                    var isValidPassword = _passwordHasher.VerifyPassword(request.Password, doctor.Password);
                    if (isValidPassword)
                    {
                        var token = GenerateJwtToken(doctor.Id.ToString(), "Doctor");
                        return Ok(new
                        {
                            Token = token,
                            Role = "Doctor",
                            UserId = doctor.Id,
                            Name = doctor.Nom,
                            Email = doctor.Email
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Password verification error: {ex.Message}");
                }
            }

            return Unauthorized("Invalid credentials");
        }

        [HttpPost("test-hash")]
        public ActionResult<object> TestHash([FromBody] LoginRequest request)
        {
            var hashedPassword = _passwordHasher.HashPassword(request.Password);
            var isValid = _passwordHasher.VerifyPassword(request.Password, hashedPassword);
            
            return Ok(new
            {
                originalPassword = request.Password,
                hashedPassword = hashedPassword,
                isValid = isValid
            });
        }

        private string GenerateJwtToken(string userId, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourDefaultSecretKey123!"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}