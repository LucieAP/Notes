public class GetUserResponse 
{
    public Guid Id { get; set; }
    public required string Name { get; set; } 
    public required string Email { get; set; } 
    public string? Picture { get; set; }
}