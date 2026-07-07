using System.Threading.Tasks;

namespace EMRSystem.Abdm.Sessions
{
    public interface IAbdmSessionManager
    {
        Task<string> GetAccessTokenAsync();
    }
}
