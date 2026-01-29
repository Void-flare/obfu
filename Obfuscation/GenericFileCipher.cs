using System;
using System.IO;
using System.Threading.Tasks;

namespace ObfuTool.Obfuscation
{
    public static class GenericFileCipher
    {
        public static async Task<string> EncryptAsync(string inputPath, string outputDir, int key)
        {
            Directory.CreateDirectory(outputDir);
            var outputPath = Path.Combine(outputDir, Path.GetFileName(inputPath));
            using var src = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var dst = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
            var buf = new byte[8192];
            var k = key;
            int n;
            while ((n = await src.ReadAsync(buf, 0, buf.Length)) > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    k = ((k << 5) | (int)((uint)k >> 27)) ^ 0xA3A3A3;
                    buf[i] = (byte)(buf[i] ^ (k & 0xFF));
                }
                await dst.WriteAsync(buf, 0, n);
            }
            return outputPath;
        }
        public static Task<string> DecryptAsync(string inputPath, string outputDir, int key)
        {
            return EncryptAsync(inputPath, outputDir, key);
        }
    }
}
