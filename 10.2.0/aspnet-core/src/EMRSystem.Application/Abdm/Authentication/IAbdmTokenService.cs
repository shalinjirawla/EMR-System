using System.Threading.Tasks;

namespace EMRSystem.Abdm.Authentication
{
    public interface IAbdmTokenService
    {
        /// <summary>
        /// Fetches the access token required for ABDM API calls.
        /// Caches the token until its expiration.
        /// </summary>
        Task<string> GetAccessTokenAsync();
    }
}
