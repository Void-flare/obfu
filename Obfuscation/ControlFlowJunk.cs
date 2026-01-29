using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ObfuTool.Obfuscation
{
    public static class ControlFlowJunk
    {
        public static void Inject(ModuleDefinition module, int key)
        {
            var rnd = new Random(key);
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.Instructions.Count > 10))
                {
                    var il = method.Body.GetILProcessor();
                    for (int i = 0; i < method.Body.Instructions.Count; i += Math.Max(5, rnd.Next(7, 15)))
                    {
                        var target = method.Body.Instructions[i];
                        var k1 = rnd.Next();
                        var k2 = k1;
                        var i0 = il.Create(OpCodes.Ldc_I4, k1);
                        var i1 = il.Create(OpCodes.Ldc_I4, k2);
                        var i2 = il.Create(OpCodes.Xor);
                        var br = il.Create(OpCodes.Brtrue, target);
                        il.InsertBefore(target, br);
                        il.InsertBefore(br, i2);
                        il.InsertBefore(i2, i1);
                        il.InsertBefore(i1, i0);
                    }
                    method.Body.MaxStackSize += 8;
                }
            }
        }
    }
}
