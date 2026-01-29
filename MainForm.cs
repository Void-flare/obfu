using System;
using System.IO;
using System.Windows.Forms;
using ObfuTool.Obfuscation;

namespace ObfuTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void BrowseInput_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "All files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                inputPath.Text = openFileDialog.FileName;
                if (string.IsNullOrWhiteSpace(outputPath.Text))
                {
                    outputPath.Text = Path.GetDirectoryName(openFileDialog.FileName) ?? "";
                }
            }
        }

        private void BrowseOutput_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                outputPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private async void ObfuscateBtn_Click(object sender, EventArgs e)
        {
            logBox.Text = "";
            var inPath = inputPath.Text.Trim();
            var outDir = outputPath.Text.Trim();
            if (!File.Exists(inPath)) { logBox.Text = "Input file not found"; return; }
            if (!Directory.Exists(outDir)) { Directory.CreateDirectory(outDir); }
            obfuscateBtn.Enabled = false;
            deobfuscateBtn.Enabled = false;
            var opts = new ObfuscationOptions
            {
                Rename = renameCheck.Checked,
                EncryptStrings = stringEncryptCheck.Checked,
                ControlFlow = controlFlowCheck.Checked,
                Password = passwordBox.Text?.Trim() ?? ""
            };
            try
            {
                var obfuscator = new Obfuscator();
                var result = await obfuscator.ProcessAsync(inPath, outDir, opts);
                logBox.AppendText("Success!" + Environment.NewLine);
                logBox.AppendText("Output: " + result.OutputAssemblyPath + Environment.NewLine);
                logBox.AppendText("Mapping: " + result.MapPath + Environment.NewLine);
                MessageBox.Show("Operation completed successfully!", "ObfuTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                logBox.Text = "Error: " + ex.Message + Environment.NewLine + ex.StackTrace;
                MessageBox.Show("Error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                obfuscateBtn.Enabled = true;
                deobfuscateBtn.Enabled = true;
            }
        }

        private async void DeobfuscateBtn_Click(object sender, EventArgs e)
        {
            logBox.Text = "";
            var inPath = inputPath.Text.Trim();
            var outDir = outputPath.Text.Trim();
            if (!File.Exists(inPath)) { logBox.Text = "Input file not found"; return; }
            if (!Directory.Exists(outDir)) { Directory.CreateDirectory(outDir); }
            var mapPath = Path.ChangeExtension(inPath, ".map.json");
            // If map file is missing, we might be in generic file mode or it was deleted. 
            // We proceed, passing a dummy path or handling it in Deobfuscator.
            // But Deobfuscator currently expects mapPath to exist for reading.
            // Let's create a temporary empty map if it doesn't exist, or better, 
            // modify Deobfuscator to accept optional map path.
            // However, to keep it simple without changing interface too much:
            if (!File.Exists(mapPath)) 
            {
                 // Create a dummy map file if it doesn't exist so Deobfuscator doesn't crash on ReadAllText
                 // This allows GenericFileCipher fallback to work for files without map
                 mapPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
                 await File.WriteAllTextAsync(mapPath, "{}");
            }
            
            obfuscateBtn.Enabled = false;
            deobfuscateBtn.Enabled = false;
            try
            {
                var deobfuscator = new Deobfuscator();
                var result = await deobfuscator.ProcessAsync(inPath, outDir, mapPath, passwordBox.Text?.Trim() ?? "");
                logBox.AppendText("Success!" + Environment.NewLine);
                logBox.AppendText("Output: " + result.OutputAssemblyPath + Environment.NewLine);
                MessageBox.Show("Operation completed successfully!", "ObfuTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                logBox.Text = "Error: " + ex.Message + Environment.NewLine + ex.StackTrace;
                MessageBox.Show("Error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                obfuscateBtn.Enabled = true;
                deobfuscateBtn.Enabled = true;
            }
        }
    }
}
