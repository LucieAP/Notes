namespace server.Interfaces.Services
{
    public interface IJwtService
    {
        public string GenerateJwtToken(JwtUser user);
    }
}