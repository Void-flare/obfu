namespace ObfuTool.Obfuscation
{
    public class ObfuscationOptions
    {
        public bool Rename { get; set; }
        public bool EncryptStrings { get; set; }
        public bool ControlFlow { get; set; }
        public string Password { get; set; }
    }
}
