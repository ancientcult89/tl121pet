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
            byte[] key, iv;
            PrepareKeyAndIV(out key, out iv);

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
            byte[] key, iv;
            PrepareKeyAndIV(out key, out iv);

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

        private void PrepareKeyAndIV(out byte[] key, out byte[] iv)
        {
            key = Convert.FromBase64String(_settings.Key);
            iv = Encoding.UTF8.GetBytes(_settings.IV);
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            {
                throw new ArgumentException("Key must be 16, 24, or 32 bytes in length.");
            }

            if (iv.Length != 16)
            {
                throw new ArgumentException("IV must be 16 bytes in length.");
            }
        }
    }
}