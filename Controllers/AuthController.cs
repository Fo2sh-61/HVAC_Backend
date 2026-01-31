using Backend.DTO.Auth;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {  
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var user = new ApplicationUser
            {
                Email = registerDTO.Email,
                FirstName = registerDTO.FullName.Split(' ')[0],
                LastName = registerDTO.FullName.Split(' ').Length > 1 ? registerDTO.FullName.Split(' ')[1] : "",
                UserName = registerDTO.UserName,
                Address=registerDTO.Address,
                PhoneNumber=registerDTO.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                var roleExists = await _roleManager.RoleExistsAsync(registerDTO.Role);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(registerDTO.Role));
                }
                await _userManager.AddToRoleAsync(user, registerDTO.Role);
            }
            return Ok(new
            {
                Message = "User registered successfully",
                User = user
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm]LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid Email or Password");
            bool isPasswordValid=await _userManager.CheckPasswordAsync(user,model.Password);
            if (!isPasswordValid)
                return Unauthorized("Invalid Email or Password");
            var roles=await _userManager.GetRolesAsync(user);
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName)
            };
            foreach (var role in roles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }
            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken (
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claim,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );
            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            });
        }

        [Authorize]
        [HttpGet("CurrentUser")]
        public async Task<IActionResult>GetCurrentUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(
            new
            {
                Id=user.Id,
                FullName = user.FirstName + " " + user.LastName,
                Email = user.Email,
                Roles = roles
            });
        }
    }
}
