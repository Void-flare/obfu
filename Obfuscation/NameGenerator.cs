using System;

namespace ObfuTool.Obfuscation
{
    public class NameGenerator
    {
        private int t = 0;
        private int m = 0;
        private int f = 0;
        private int p = 0;
        public string NextType()
        {
            t++;
            return "T" + t.ToString("X");
        }
        public string NextMethod()
        {
            m++;
            return "M" + m.ToString("X");
        }
        public string NextField()
        {
            f++;
            return "F" + f.ToString("X");
        }
        public string NextProperty()
        {
            p++;
            return "P" + p.ToString("X");
        }
    }
}
