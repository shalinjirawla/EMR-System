using System;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.Encryption
{
    public interface IAbdmEncryptionService
    {
        /// <summary>
        /// Generates a new Diffie-Hellman Key Pair for exchanging data.
        /// </summary>
        Task<AbdmKeyPair> GenerateKeyPairAsync();

        /// <summary>
        /// Encrypts plain text using the remote public key, local private key, and a random nonce.
        /// Returns the Base64 encoded cipher text and the Base64 nonce.
        /// </summary>
        Task<AbdmEncryptedData> EncryptAsync(string plainText, string remotePublicKeyBase64, string localPrivateKeyBase64);

        /// <summary>
        /// Decrypts the cipher text using the remote public key, local private key, and the provided nonce.
        /// </summary>
        Task<string> DecryptAsync(string cipherTextBase64, string nonceBase64, string remotePublicKeyBase64, string localPrivateKeyBase64);
    }

    public class AbdmKeyPair
    {
        public string PrivateKeyBase64 { get; set; }
        public string PublicKeyBase64 { get; set; }
    }

    public class AbdmEncryptedData
    {
        public string CipherTextBase64 { get; set; }
        public string NonceBase64 { get; set; }
    }
}
