using System;
using System.Text;

namespace ObfuTool.Obfuscation
{
    public static class StringEncryptor
    {
        public static string Encrypt(string value, int key)
        {
            var chars = value.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                key = ((key << 5) | (int)((uint)key >> 27)) ^ 0xA3A3A3;
                chars[i] = (char)(chars[i] ^ (key & 0xFFFF));
            }
            return new string(chars);
        }
        public static string Decrypt(string value, int key)
        {
            return Encrypt(value, key);
        }
    }
}
