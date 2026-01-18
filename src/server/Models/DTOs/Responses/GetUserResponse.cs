public record GetUserResponse 
{
    public Guid Id { get; set; }
    public required string Name { get; set; } 
    public required string Email { get; set; } 
    public string? Picture { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime LastLoginAt { get; set; }
}