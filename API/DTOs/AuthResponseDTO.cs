namespace API.DTOs;

public class AuthResponseDTO
{
    public string? Token { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }        
    public string? Menssage { get; set; }
}