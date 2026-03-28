public class RefreshToken 
{
    public Guid Id { get; set; }
    public string HashedToken { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public uint RowVersion { get; set; }

    // Навигационные свойства
    public User User { get; set; }
}
