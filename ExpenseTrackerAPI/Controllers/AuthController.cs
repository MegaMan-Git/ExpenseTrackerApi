using ExpenseTracker.Application.Dtos;
using ExpenseTracker.Application.Dtos.Auth;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.DataLayer.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpenseTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    public class AuthController : ControllerBase
    {
        #region Dependency Injection
        private readonly AppDbContext _context;
        private readonly IAuthInfoRepository _authRepository;
        private readonly IConfiguration _configuration;
        public AuthController
            (AppDbContext context
            ,IAuthInfoRepository authInfoRepository
            , IConfiguration configuration)
        {
            _context = context;
            _authRepository = authInfoRepository;
            _configuration = configuration;
        }
        #endregion


        #region Action Register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var NewUser = await _authRepository.RegisterUser(registerDto);
            
            string Token = GenerateToken(NewUser);

            return Ok(Token);
        }
        #endregion

        #region Action Login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var UserDto = await _authRepository.LoginUser(loginDto);

            string Token =  GenerateToken(UserDto);

            return Ok(Token);
        }
        #endregion

        #region (jwt)منطق ساخت توکن
        private string GenerateToken(UserDto userDto)
        {
            //گرفتن کلید مخفی
            var SecurityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes
                (
                    _configuration["Jwt:SecretKey"] ?? "shhhhhhh you know?")
                );
            
            //امضای دیجیتال توکن
            var SigningCredentials = 
                new SigningCredentials(SecurityKey,SecurityAlgorithms.HmacSha256);

            //و مقداردهی  Claim ساخت یک نمونه از 
            var ClaimsForToken = new List<Claim>();
            ClaimsForToken.Add(new Claim(ClaimTypes.NameIdentifier,userDto.Id.ToString()));
            ClaimsForToken.Add(new Claim(ClaimTypes.Email, userDto.Email));
            ClaimsForToken.Add(new Claim(ClaimTypes.Name, userDto.FullName));

            //ایجاد توکن امنیتی
            var JwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                //اطلاعات کاربر
                ClaimsForToken,
                //تاریخ شروع اعتبار
                DateTime.UtcNow,
                // انقضا 3 ساعت بعد
                DateTime.UtcNow.AddHours(3),
                //امضای دیجیتال توکن
                SigningCredentials
                );

            string JwtToken = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);

            return JwtToken;
        }
        #endregion
    }
}
