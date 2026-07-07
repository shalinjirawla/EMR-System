using System.Threading.Tasks;

namespace EMRSystem.Abdm.Encryption
{
    public interface IEncryptionService
    {
        Task<string> EncryptAsync(string plainText);
    }
}
