using System;
using System.Linq;
using System.Security.Cryptography;
using Mono.Cecil;

namespace ObfuTool.Obfuscation
{
    public static class KeyDerivation
    {
        public static int DeriveKeyInt32(string password, ModuleDefinition module)
        {
            var saltSrc = module.Assembly.Name.Name + module.Mvid.ToString();
            var salt = System.Text.Encoding.UTF8.GetBytes(saltSrc);
            if (string.IsNullOrEmpty(password)) password = module.Assembly.Name.Name;
            using var kdf = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var bytes = kdf.GetBytes(4);
            return BitConverter.ToInt32(bytes, 0);
        }
        public static int DeriveKeyInt32FromFile(string password, string filePath)
        {
            var fi = new System.IO.FileInfo(filePath);
            var saltSrc = fi.Name + fi.Length.ToString();
            var salt = System.Text.Encoding.UTF8.GetBytes(saltSrc);
            if (string.IsNullOrEmpty(password)) password = fi.Name;
            using var kdf = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var bytes = kdf.GetBytes(4);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
