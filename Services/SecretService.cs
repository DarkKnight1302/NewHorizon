using NewHorizon.Services.Interfaces;

namespace NewHorizon.Services
{
    public class SecretService : ISecretService
    {
        private readonly IConfiguration _configuration;

        public SecretService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetSecretValue(string key)
        {
            return _configuration[key];
        }
    }
}
