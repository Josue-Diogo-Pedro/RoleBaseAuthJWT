using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateRoleDTO
{
    [Required(ErrorMessage = "Role Name is required.")]
    public string RoleName { get; set; } = null!;
}