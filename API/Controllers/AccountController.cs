using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AccountController(
                            UserManager<AppUser> userManager, 
                            RoleManager<IdentityRole> roleManager, 
                            IConfiguration configuration) 
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDetailDTO>>> GetUsers(){

        var users = await _userManager.Users.Select(user => new UserDetailDTO{
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = _userManager.GetRolesAsync(user).Result.ToArray()
        }).ToListAsync();

        return Ok(users);
    }

    [HttpGet("details")]
    public async Task<ActionResult<UserDetailDTO>> GetUserDetail(){

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(currentUserId!);

        if(user is null)
            return NotFound(new AuthResponseDTO{
                IsSuccess = false,
                Message = "User not found"
            });

        return Ok(new UserDetailDTO{
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = [..await _userManager.GetRolesAsync(user)],
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            AccessFailedCount = user.AccessFailedCount
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(RegisterDTO registerDTO){

        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new AppUser{
            Email = registerDTO.Email,
            FullName = registerDTO.FullName,
            UserName = registerDTO.Email
        };

        var result = await _userManager.CreateAsync(user, registerDTO.Password);

        if(!result.Succeeded)
            return BadRequest(result.Errors);

        if(registerDTO.Roles is null)
            await _userManager.AddToRoleAsync(user, "User");
        else
            foreach(var role in registerDTO.Roles)
                await _userManager.AddToRoleAsync(user, role);

        return Ok(new AuthResponseDTO{
            IsSuccess = true,
            Message = "Account created successfully"
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDTO){

        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(loginDTO.Email);

        if(user is null)
            return Unauthorized(new AuthResponseDTO{
                IsSuccess = false,
                Message = "User not found with this email"
            });

        var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

        if(!result)
            return Unauthorized(new AuthResponseDTO{
                IsSuccess = false,
                Message = "Email or password is incorrect"
            });

        var token = generateToken(user);

        return Ok(new AuthResponseDTO{
            Token = token,
            IsSuccess = true,
            Message = "Login success"
        });
    }


    private string generateToken(AppUser user){
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII
            .GetBytes(_configuration["JWTSettings:securityKey"]!);
        
        var roles = _userManager.GetRolesAsync(user).Result;

        List<Claim> claims = [
            new (JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new (JwtRegisteredClaimNames.Name, user.FullName ?? ""),
            new (JwtRegisteredClaimNames.NameId, user.Id ?? ""),
            new (JwtRegisteredClaimNames.Aud,
                _configuration["JWTSettings:ValidAudience"]!),
            new (JwtRegisteredClaimNames.Iss, _configuration["JWTSettings:ValidIssuer"]!)
        ];

        foreach(var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}