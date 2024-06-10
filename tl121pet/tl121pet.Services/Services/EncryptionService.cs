using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using tl121pet.Entities.Infrastructure;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly EncryptionSettings _settings;

        public EncryptionService(IOptions<EncryptionSettings> options)
        {
            _settings = options.Value;
        }

        public string Encrypt(string plainText)
        {
            byte[] key = Encoding.UTF8.GetBytes(_settings.Key);
            byte[] iv = Encoding.UTF8.GetBytes(_settings.IV);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            byte[] key = Encoding.UTF8.GetBytes(_settings.Key);
            byte[] iv = Encoding.UTF8.GetBytes(_settings.IV);
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
