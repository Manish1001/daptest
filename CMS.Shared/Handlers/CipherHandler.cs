#pragma warning disable SYSLIB0022
namespace CMS.Shared.Handlers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class CipherHandler
    {
        private const string AESKey = "!QAZ2WSX#EDC4RFV";

        public static string Decrypt(string text)
        {
            try
            {
                text = Regex.Replace(text, @"\s", "+");
                string plaintext;
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = 128;
                    rijAlg.Key = Encoding.UTF8.GetBytes(AESKey);
                    rijAlg.IV = Encoding.UTF8.GetBytes(AESKey);

                    var decrypt = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                    using (var msdDecrypt = new MemoryStream(Convert.FromBase64String(text)))
                    {
                        using (var csdDecrypt = new CryptoStream(msdDecrypt, decrypt, CryptoStreamMode.Read))
                        {
                            using (var srdDecrypt = new StreamReader(csdDecrypt))
                            {
                                plaintext = srdDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return plaintext;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Encrypt(string text)
        {
            try
            {
                byte[] encrypted;
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = 128;
                    rijAlg.Key = Encoding.UTF8.GetBytes(AESKey);
                    rijAlg.IV = Encoding.UTF8.GetBytes(AESKey);

                    var encryption = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                    using (var msdEncrypt = new MemoryStream())
                    {
                        using (var csdEncrypt = new CryptoStream(msdEncrypt, encryption, CryptoStreamMode.Write))
                        {
                            using (var swdEncrypt = new StreamWriter(csdEncrypt))
                            {
                                swdEncrypt.Write(text);
                            }

                            encrypted = msdEncrypt.ToArray();
                        }
                    }
                }

                return Convert.ToBase64String(encrypted);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GenerateHash(string password, string key)
        {
            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(Encoding.Unicode.GetBytes(password + key));
                return Convert.ToBase64String(computedHash);
            }
        }
    }
}
