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
    public class Obfuscator
    {
        public async Task<ObfuscationResult> ProcessAsync(string inputAssemblyPath, string outputDir, ObfuscationOptions options)
        {
            var map = new Dictionary<string, string>();
            ModuleDefinition module = null;
            try
            {
                var readerParams = new ReaderParameters { ReadSymbols = false };
                module = ModuleDefinition.ReadModule(inputAssemblyPath, readerParams);
            }
            catch
            {
            }
            if (module == null)
            {
                var keyFile = KeyDerivation.DeriveKeyInt32FromFile(options.Password, inputAssemblyPath);
                var outFile = await GenericFileCipher.EncryptAsync(inputAssemblyPath, outputDir, keyFile);
                var mapPathFile = Path.ChangeExtension(outFile, ".map.json");
                await File.WriteAllTextAsync(mapPathFile, "{}");
                return new ObfuscationResult { OutputAssemblyPath = outFile, MapPath = mapPathFile };
            }
            TypeDefinition runtimeType = null;
            MethodDefinition decryptMethod = null;
            if (options.EncryptStrings)
            {
                runtimeType = InjectRuntime(module);
                decryptMethod = runtimeType.Methods.First(x => x.Name == "Decrypt");
            }
            var nameGen = new NameGenerator();
            if (options.Rename)
            {
                foreach (var type in module.Types.Where(t => !t.IsSpecialName && !t.IsInterface && !t.IsEnum))
                {
                    if (!type.Namespace.StartsWith("System"))
                    {
                        var newName = nameGen.NextType();
                        map[type.FullName] = newName;
                        type.Name = newName;
                        foreach (var m in type.Methods.Where(m => !m.IsRuntime && !m.IsConstructor && !m.IsPInvokeImpl && !m.IsSpecialName))
                        {
                            var mn = nameGen.NextMethod();
                            map[type.FullName + "::" + m.Name] = mn;
                            m.Name = mn;
                        }
                        foreach (var f in type.Fields.Where(f => !f.IsSpecialName))
                        {
                            var fn = nameGen.NextField();
                            map[type.FullName + "::" + f.Name] = fn;
                            f.Name = fn;
                        }
                        foreach (var p in type.Properties.Where(p => !p.IsSpecialName))
                        {
                            var pn = nameGen.NextProperty();
                            map[type.FullName + "::" + p.Name] = pn;
                            p.Name = pn;
                        }
                    }
                }
            }
            if (options.EncryptStrings)
            {
                var key = KeyDerivation.DeriveKeyInt32(options.Password, module);
                foreach (var type in module.Types)
                {
                    foreach (var method in type.Methods.Where(m => m.HasBody))
                    {
                        var il = method.Body.GetILProcessor();
                        for (int i = 0; i < method.Body.Instructions.Count; i++)
                        {
                            var ins = method.Body.Instructions[i];
                            if (ins.OpCode == OpCodes.Ldstr && ins.Operand is string s)
                            {
                                var enc = StringEncryptor.Encrypt(s, key);
                                method.Body.Instructions[i].Operand = enc;
                                var ldc = il.Create(OpCodes.Ldc_I4, key);
                                var call = il.Create(OpCodes.Call, decryptMethod);
                                method.Body.Instructions.Insert(i + 1, ldc);
                                method.Body.Instructions.Insert(i + 2, call);
                                i += 2;
                            }
                        }
                    }
                }
            }
            foreach (var ca in module.Assembly.CustomAttributes.ToArray()) module.Assembly.CustomAttributes.Remove(ca);
            foreach (var ca in module.CustomAttributes.ToArray()) module.CustomAttributes.Remove(ca);
            foreach (var type in module.Types)
            {
                foreach (var ca in type.CustomAttributes.ToArray()) type.CustomAttributes.Remove(ca);
                foreach (var m in type.Methods) foreach (var ca in m.CustomAttributes.ToArray()) m.CustomAttributes.Remove(ca);
                foreach (var f in type.Fields) foreach (var ca in f.CustomAttributes.ToArray()) f.CustomAttributes.Remove(ca);
                foreach (var p in type.Properties) foreach (var ca in p.CustomAttributes.ToArray()) p.CustomAttributes.Remove(ca);
            }
            if (options.ControlFlow)
            {
                ControlFlowJunk.Inject(module, KeyDerivation.DeriveKeyInt32(options.Password, module));
            }
            Directory.CreateDirectory(outputDir);
            var outputPath = Path.Combine(outputDir, Path.GetFileName(inputAssemblyPath));
            module.Write(outputPath, new WriterParameters { WriteSymbols = false });
            var mapPath = Path.ChangeExtension(outputPath, ".map.json");
            var json = JsonSerializer.Serialize(map);
            await File.WriteAllTextAsync(mapPath, json);
            return new ObfuscationResult { OutputAssemblyPath = outputPath, MapPath = mapPath };
        }

        private TypeDefinition InjectRuntime(ModuleDefinition module)
        {
            var type = new TypeDefinition("Runtime", "Strings", TypeAttributes.Public | TypeAttributes.Class, module.TypeSystem.Object);
            module.Types.Add(type);
            var method = new MethodDefinition("Decrypt", MethodAttributes.Public | MethodAttributes.Static, module.TypeSystem.String);
            var par = new ParameterDefinition("value", ParameterAttributes.None, module.TypeSystem.String);
            method.Parameters.Add(par);
            var par2 = new ParameterDefinition("k", ParameterAttributes.None, module.TypeSystem.Int32);
            method.Parameters.Add(par2);
            type.Methods.Add(method);
            var il = method.Body.GetILProcessor();
            method.Body.InitLocals = true;
            var arr = new VariableDefinition(new ArrayType(module.TypeSystem.Char));
            var idx = new VariableDefinition(module.TypeSystem.Int32);
            method.Body.Variables.Add(arr);
            method.Body.Variables.Add(idx);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, module.ImportReference(typeof(string).GetMethod("ToCharArray", Type.EmptyTypes)));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_1);
            var loopBody = il.Create(OpCodes.Ldloc_0);
            var loopCheck = il.Create(OpCodes.Ldloc_1);
            var loopEnd = il.Create(OpCodes.Nop);
            il.Append(loopCheck);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldlen);
            il.Emit(OpCodes.Conv_I4);
            il.Emit(OpCodes.Bge, loopEnd);
            il.Append(loopBody);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldelema, module.TypeSystem.Char);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldobj, module.TypeSystem.Char);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Xor);
            il.Emit(OpCodes.Conv_U2);
            il.Emit(OpCodes.Stobj, module.TypeSystem.Char);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Br, loopCheck);
            il.Append(loopEnd);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Newobj, module.ImportReference(typeof(string).GetConstructor(new[] { typeof(char[]) })));
            il.Emit(OpCodes.Ret);
            return type;
        }
    }

    public class ObfuscationResult
    {
        public string OutputAssemblyPath { get; set; }
        public string MapPath { get; set; }
    }
}
