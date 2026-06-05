using System.Security.Cryptography;
using System.Text;
using HRMS.Application.Interfaces.Security;

namespace HRMS.Web.Security
{
    public class IdProtector : IIdProtector
    {
        private readonly byte[] _key;

        public IdProtector(IConfiguration configuration)
        {
            var key = configuration["Encryption:Key"];
            if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
                throw new Exception("Encryption key must be at least 32 characters.");

            _key = Encoding.UTF8.GetBytes(key);
        }

        public string Protect(string value)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(value);
            var cipherBytes = encryptor.TransformFinalBlock(
                plainBytes, 0, plainBytes.Length);

            var result = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

            // Base64  URL-safe Base64
            return Convert.ToBase64String(result)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        public string Unprotect(string value)
        {
            // URL-safe Base64  normal Base64
            var base64 = value
                .Replace("-", "+")
                .Replace("_", "/");

            // Restore padding
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            var fullBytes = Convert.FromBase64String(base64);

            using var aes = Aes.Create();
            aes.Key = _key;

            var iv = new byte[16];
            var cipherBytes = new byte[fullBytes.Length - 16];

            Buffer.BlockCopy(fullBytes, 0, iv, 0, 16);
            Buffer.BlockCopy(fullBytes, 16, cipherBytes, 0, cipherBytes.Length);

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();

            var plainBytes = decryptor.TransformFinalBlock(
                cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }

    }
}
