extern alias BCCrypto;
using Abp.Dependency;
using BCCrypto::Org.BouncyCastle.Asn1.X9;
using BCCrypto::Org.BouncyCastle.Crypto;
using BCCrypto::Org.BouncyCastle.Crypto.Agreement;
using BCCrypto::Org.BouncyCastle.Crypto.Digests;
using BCCrypto::Org.BouncyCastle.Crypto.Engines;
using BCCrypto::Org.BouncyCastle.Crypto.Generators;
using BCCrypto::Org.BouncyCastle.Crypto.Modes;
using BCCrypto::Org.BouncyCastle.Crypto.Parameters;
using BCCrypto::Org.BouncyCastle.Math.EC;
using BCCrypto::Org.BouncyCastle.Security;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.Encryption
{
    public class AbdmEncryptionService : IAbdmEncryptionService, ITransientDependency
    {
        private static X9ECParameters GetCurveParams()
        {
            var p = BCCrypto::Org.BouncyCastle.Crypto.EC.CustomNamedCurves.GetByName("curve25519");
            if (p == null) p = BCCrypto::Org.BouncyCastle.Crypto.EC.CustomNamedCurves.GetByName("Curve25519");
            if (p == null) p = ECNamedCurveTable.GetByName("curve25519");
            if (p == null) p = ECNamedCurveTable.GetByName("Curve25519");
            
            // Fallback to prime256v1 (secp256r1) if Curve25519 is missing in this BC version, 
            // just to prevent a hard crash, though ABDM prefers Curve25519.
            if (p == null) p = ECNamedCurveTable.GetByName("prime256v1");
            
            if (p == null) throw new InvalidOperationException("No suitable EC curve found!");
            return p;
        }

        private static readonly X9ECParameters CurveParams = GetCurveParams();
        private static readonly ECDomainParameters DomainParams = new ECDomainParameters(CurveParams.Curve, CurveParams.G, CurveParams.N, CurveParams.H, CurveParams.GetSeed());
        private static readonly SecureRandom SecureRandom = new SecureRandom();

        public Task<AbdmKeyPair> GenerateKeyPairAsync()
        {
            var keyGenParams = new ECKeyGenerationParameters(DomainParams, SecureRandom);
            var generator = new ECKeyPairGenerator();
            generator.Init(keyGenParams);
            var keyPair = generator.GenerateKeyPair();

            var privateKey = (ECPrivateKeyParameters)keyPair.Private;
            var publicKey = (ECPublicKeyParameters)keyPair.Public;

            return Task.FromResult(new AbdmKeyPair
            {
                PrivateKeyBase64 = Convert.ToBase64String(privateKey.D.ToByteArrayUnsigned()),
                PublicKeyBase64 = Convert.ToBase64String(publicKey.Q.GetEncoded(false))
            });
        }

        public Task<AbdmEncryptedData> EncryptAsync(string plainText, string remotePublicKeyBase64, string localPrivateKeyBase64)
        {
            var sharedSecret = GenerateSharedSecret(localPrivateKeyBase64, remotePublicKeyBase64);
            var hkdfKey = GenerateHkdfKey(sharedSecret);

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var nonce = new byte[12];
            SecureRandom.NextBytes(nonce);

            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(hkdfKey), 128, nonce, null);
            cipher.Init(true, parameters);

            var cipherText = new byte[cipher.GetOutputSize(plainBytes.Length)];
            var len = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, cipherText, 0);
            cipher.DoFinal(cipherText, len);

            return Task.FromResult(new AbdmEncryptedData
            {
                CipherTextBase64 = Convert.ToBase64String(cipherText),
                NonceBase64 = Convert.ToBase64String(nonce)
            });
        }

        public Task<string> DecryptAsync(string cipherTextBase64, string nonceBase64, string remotePublicKeyBase64, string localPrivateKeyBase64)
        {
            var sharedSecret = GenerateSharedSecret(localPrivateKeyBase64, remotePublicKeyBase64);
            var hkdfKey = GenerateHkdfKey(sharedSecret);

            var cipherBytes = Convert.FromBase64String(cipherTextBase64);
            var nonce = Convert.FromBase64String(nonceBase64);

            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(hkdfKey), 128, nonce, null);
            cipher.Init(false, parameters);

            var plainTextBytes = new byte[cipher.GetOutputSize(cipherBytes.Length)];
            var len = cipher.ProcessBytes(cipherBytes, 0, cipherBytes.Length, plainTextBytes, 0);
            cipher.DoFinal(plainTextBytes, len);

            return Task.FromResult(Encoding.UTF8.GetString(plainTextBytes));
        }

        private byte[] GenerateSharedSecret(string localPrivateKeyBase64, string remotePublicKeyBase64)
        {
            var localPrivateKeyBytes = Convert.FromBase64String(localPrivateKeyBase64);
            var localPrivateKey = new ECPrivateKeyParameters(new BCCrypto::Org.BouncyCastle.Math.BigInteger(1, localPrivateKeyBytes), DomainParams);

            var remotePublicKeyBytes = Convert.FromBase64String(remotePublicKeyBase64);
            var remotePublicKey = new ECPublicKeyParameters(DomainParams.Curve.DecodePoint(remotePublicKeyBytes), DomainParams);

            var agreement = new ECDHBasicAgreement();
            agreement.Init(localPrivateKey);
            return agreement.CalculateAgreement(remotePublicKey).ToByteArrayUnsigned();
        }

        private byte[] GenerateHkdfKey(byte[] sharedSecret)
        {
            var hkdf = new HkdfBytesGenerator(new Sha256Digest());
            var info = Encoding.UTF8.GetBytes("abdm-encryption");
            var hkdfParams = new HkdfParameters(sharedSecret, new byte[0], info);
            hkdf.Init(hkdfParams);

            var result = new byte[32];
            hkdf.GenerateBytes(result, 0, result.Length);
            return result;
        }
    }
}
