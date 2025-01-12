using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleResponseDTO>>> GetRoles(){
        
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO createRoleDTO){
        if(string.IsNullOrEmpty(createRoleDTO.RoleName))
            return BadRequest("Role name is required");
        
        var roleExist = await _roleManager.RoleExistsAsync(createRoleDTO.RoleName);

        if(roleExist)
            return BadRequest("Role alredy exist");
        
        var roleResult = await _roleManager.CreateAsync(new IdentityRole(createRoleDTO.RoleName));

        if(roleResult.Succeeded)
            return Ok(new {message = "Role created successfully"});

        return BadRequest("Role creation failed");
    }


}