using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ObfuTool.Obfuscation
{
    public class Deobfuscator
    {
        public async Task<DeobfuscationResult> ProcessAsync(string inputAssemblyPath, string outputDir, string mapPath, string password)
        {
            var map = JsonSerializer.Deserialize<Dictionary<string, string>>(await File.ReadAllTextAsync(mapPath)) ?? new Dictionary<string, string>();
            var reverse = map.ToDictionary(kv => kv.Value, kv => kv.Key);
            ModuleDefinition module = null;
            try
            {
                module = ModuleDefinition.ReadModule(inputAssemblyPath, new ReaderParameters { ReadSymbols = false });
            }
            catch
            {
            }
            if (module == null)
            {
                var keyFile = KeyDerivation.DeriveKeyInt32FromFile(password, inputAssemblyPath);
                Directory.CreateDirectory(outputDir);
                var outFile = await GenericFileCipher.DecryptAsync(inputAssemblyPath, outputDir, keyFile);
                return new DeobfuscationResult { OutputAssemblyPath = outFile };
            }
            foreach (var type in module.Types.ToList())
            {
                var obfTypeFull = type.FullName;
                foreach (var m in type.Methods)
                {
                    if (reverse.TryGetValue(m.Name, out var origMFull) && origMFull.StartsWith(obfTypeFull + "::"))
                    {
                        m.Name = origMFull.Substring(obfTypeFull.Length + 2);
                    }
                }
                foreach (var f in type.Fields)
                {
                    if (reverse.TryGetValue(f.Name, out var origFFull) && origFFull.StartsWith(obfTypeFull + "::"))
                    {
                        f.Name = origFFull.Substring(obfTypeFull.Length + 2);
                    }
                }
                foreach (var p in type.Properties)
                {
                    if (reverse.TryGetValue(p.Name, out var origPFull) && origPFull.StartsWith(obfTypeFull + "::"))
                    {
                        p.Name = origPFull.Substring(obfTypeFull.Length + 2);
                    }
                }
                if (reverse.TryGetValue(type.Name, out var origTypeFull))
                {
                    var origTypeName = origTypeFull.Split('.').Last();
                    type.Name = origTypeName;
                }
            }
            var runtimeType = module.Types.FirstOrDefault(t => t.Namespace == "Runtime" && t.Name == "Strings");
            MethodReference decryptRef = null;
            if (runtimeType != null)
            {
                decryptRef = runtimeType.Methods.FirstOrDefault(x => x.Name == "Decrypt");
            }
            if (decryptRef != null)
            {
                var key = KeyDerivation.DeriveKeyInt32(password, module);
                foreach (var type in module.Types)
                {
                    foreach (var method in type.Methods.Where(m => m.HasBody))
                    {
                        var il = method.Body.GetILProcessor();
                        for (int i = 0; i < method.Body.Instructions.Count - 2; i++)
                        {
                            var ins0 = method.Body.Instructions[i];
                            var ins1 = method.Body.Instructions[i + 1];
                            var ins2 = method.Body.Instructions[i + 2];
                            if (ins0.OpCode == OpCodes.Ldstr && ins1.OpCode == OpCodes.Ldc_I4 && ins2.OpCode == OpCodes.Call && ins2.Operand is MethodReference mr && mr.Name == "Decrypt")
                            {
                                var enc = ins0.Operand as string;
                                var dec = StringEncryptor.Decrypt(enc ?? "", key);
                                ins0.Operand = dec;
                                method.Body.Instructions.RemoveAt(i + 2);
                                method.Body.Instructions.RemoveAt(i + 1);
                            }
                        }
                    }
                }
                module.Types.Remove(runtimeType);
            }
            Directory.CreateDirectory(outputDir);
            var outputPath = Path.Combine(outputDir, Path.GetFileName(inputAssemblyPath));
            module.Write(outputPath, new WriterParameters { WriteSymbols = false });
            return new DeobfuscationResult { OutputAssemblyPath = outputPath };
        }
    }

    public class DeobfuscationResult
    {
        public string OutputAssemblyPath { get; set; }
    }
}
