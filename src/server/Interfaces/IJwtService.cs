namespace server.Interfaces
{
    public interface IJwtService
    {
        public string GenerateJwtToken(JwtUser user);
    }
}