public record GoogleUserInfo
{
    public string GoogleId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public bool EmailVerified { get; set; }
    public string Picture { get; set; }
}