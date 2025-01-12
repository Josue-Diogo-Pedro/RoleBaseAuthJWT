using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        
        var roles = await _roleManager.Roles.Select(role => new RoleResponseDTO{
            Id = role.Id,
            Name = role.Name,
            TotalUsers = _userManager.GetUsersInRoleAsync(role.Name!).Result.Count
        }).ToListAsync();

        return Ok(roles);
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

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole([FromBody]RoleAssignDTO roleAssignDTO){
        
        var user = await _userManager.FindByIdAsync(roleAssignDTO.UserId);

        if(user is null)
            return NotFound("User not found.");

        var role = await _roleManager.FindByIdAsync(roleAssignDTO.RoleId);

        if(role is null)
            return NotFound("Role not found");
        
        var result = await _userManager.AddToRoleAsync(user, role.Name!);

        if(result.Succeeded)
            return Ok(new {message = "Role assigned successfully"});

        var error = result.Errors.FirstOrDefault();

        return BadRequest(error!.Description);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id){

        var role = await _roleManager.FindByIdAsync(id);

        if(role is null)
            return NotFound("Role not found");

        var result = await _roleManager.DeleteAsync(role);

        if(result.Succeeded)
            return Ok(new {message = "Role deleted successfully"});

        return BadRequest("Role deletion failed.");
    }
    
}