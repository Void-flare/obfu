# ObfuTool

ObfuTool is a powerful, lightweight, and open-source obfuscation and encryption tool for .NET assemblies and generic files. It provides a robust set of features to protect your intellectual property properly.

## Features

### For .NET Assemblies (exe/dll)
- **Symbol Renaming**: Renames classes, methods, fields, and properties to unrecognizable names to confuse reverse engineers.
- **String Encryption**: Encrypts string literals using a rolling XOR algorithm with runtime decryption.
- **Control Flow Obfuscation**: Injects opaque predicates and junk code to break decompilers and confuse analysis.
- **Metadata Stripping**: Removes unnecessary metadata and attributes (e.g., debug info, custom attributes) to reduce file size and information leakage.
- **Runtime Injection**: Minimalist runtime code is injected directly into the assembly, requiring no external dependencies.

### For All Files (Generic)
- **Generic File Encryption**: If a file is not a valid .NET assembly (or if you choose), it is encrypted using a strong symmetric key derived from your password.
- **Password Protection**: Uses PBKDF2 for key derivation to ensure high security against brute-force attacks.

### General
- **Portable**: Distributed as a single executable or a portable ZIP package.
- **User-Friendly UI**: Clean, responsive Windows Forms interface.
- **Fast**: Optimized for speed using Mono.Cecil.

## Usage

1. **Launch ObfuTool**: Run `ObfuTool.exe`.
2. **Select Input**: Click "Eingabe" (Input) to select your target file (.exe, .dll, or any other file).
3. **Select Output**: Click "Ausgabe" (Output) to choose where the protected file should be saved.
4. **Configure Options**:
   - **Rename symbols**: Enable to rename types and members.
   - **Encrypt strings**: Enable to encrypt string literals.
   - **Control-flow junk**: Enable to inject junk code.
   - **Password**: Enter a password for key derivation (optional but recommended for stronger encryption).
5. **Obfuscate**: Click "Obfuskieren" to process the file. A `.map.json` file will be created alongside the output (keep this safe if you plan to deobfuscate later!).
6. **Deobfuscate**: Click "Deobfuskieren" to restore the original file. You must provide the correct password and have the `.map.json` file available (for assemblies).

## Building from Source

Requirements:
- .NET 8.0 SDK

Steps:
1. Clone the repository.
2. Open the solution in Visual Studio or VS Code.
3. Run the portable build script:
   ```cmd
   MakePortable.bat
   ```
   This will create a `dist` folder containing the portable executable and ZIP package.

## Disclaimer

This tool is intended for educational and legitimate protection purposes only. The author is not responsible for any misuse of this software.

## License

MIT License
