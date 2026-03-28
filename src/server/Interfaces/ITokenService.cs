namespace server.Interfaces
{
    public interface ITokenService
    {
        public string GenerateJwtToken(JwtUser user);
        public Task<string> GenerateRefreshTokenAsync(Guid userId);
        public Task<(string jwtToken, string refreshToken)?> RefreshAsync(string rawToken);
        public Task<bool> RevokeAsync(string rawToken);
        public string HashToken(string token);
    }
}