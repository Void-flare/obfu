using System.Windows.Forms;

namespace ObfuTool
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox inputPath;
        private Button browseInput;
        private TextBox outputPath;
        private Button browseOutput;
        private CheckBox renameCheck;
        private CheckBox stringEncryptCheck;
        private CheckBox controlFlowCheck;
        private Label passwordLabel;
        private TextBox passwordBox;
        private Button obfuscateBtn;
        private Button deobfuscateBtn;
        private TextBox logBox;
        private OpenFileDialog openFileDialog;
        private FolderBrowserDialog folderBrowserDialog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            inputPath = new TextBox();
            browseInput = new Button();
            outputPath = new TextBox();
            browseOutput = new Button();
            renameCheck = new CheckBox();
            stringEncryptCheck = new CheckBox();
            obfuscateBtn = new Button();
            deobfuscateBtn = new Button();
            logBox = new TextBox();
            openFileDialog = new OpenFileDialog();
            folderBrowserDialog = new FolderBrowserDialog();
            controlFlowCheck = new CheckBox();
            passwordLabel = new Label();
            passwordBox = new TextBox();
            SuspendLayout();
            Text = "ObfuTool";
            MinimumSize = new System.Drawing.Size(720, 480);
            inputPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputPath.Location = new System.Drawing.Point(12, 12);
            inputPath.Size = new System.Drawing.Size(600, 23);
            browseInput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            browseInput.Location = new System.Drawing.Point(618, 12);
            browseInput.Size = new System.Drawing.Size(84, 23);
            browseInput.Text = "Input";
            browseInput.Click += BrowseInput_Click;
            outputPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            outputPath.Location = new System.Drawing.Point(12, 48);
            outputPath.Size = new System.Drawing.Size(600, 23);
            browseOutput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            browseOutput.Location = new System.Drawing.Point(618, 48);
            browseOutput.Size = new System.Drawing.Size(84, 23);
            browseOutput.Text = "Output";
            browseOutput.Click += BrowseOutput_Click;
            renameCheck.Location = new System.Drawing.Point(12, 88);
            renameCheck.Size = new System.Drawing.Size(220, 24);
            renameCheck.Text = "Rename symbols";
            renameCheck.Checked = true;
            stringEncryptCheck.Location = new System.Drawing.Point(238, 88);
            stringEncryptCheck.Size = new System.Drawing.Size(220, 24);
            stringEncryptCheck.Text = "Encrypt strings";
            stringEncryptCheck.Checked = true;
            controlFlowCheck.Location = new System.Drawing.Point(464, 88);
            controlFlowCheck.Size = new System.Drawing.Size(180, 24);
            controlFlowCheck.Text = "Control-flow junk";
            controlFlowCheck.Checked = true;
            passwordLabel.Location = new System.Drawing.Point(12, 124);
            passwordLabel.Size = new System.Drawing.Size(80, 24);
            passwordLabel.Text = "Password";
            passwordBox.Location = new System.Drawing.Point(98, 124);
            passwordBox.Size = new System.Drawing.Size(240, 23);
            obfuscateBtn.Location = new System.Drawing.Point(352, 124);
            obfuscateBtn.Size = new System.Drawing.Size(140, 30);
            obfuscateBtn.Text = "Obfuscate";
            obfuscateBtn.Click += ObfuscateBtn_Click;
            deobfuscateBtn.Location = new System.Drawing.Point(498, 124);
            deobfuscateBtn.Size = new System.Drawing.Size(140, 30);
            deobfuscateBtn.Text = "Deobfuscate";
            deobfuscateBtn.Click += DeobfuscateBtn_Click;
            logBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logBox.Location = new System.Drawing.Point(12, 170);
            logBox.Size = new System.Drawing.Size(690, 260);
            logBox.Multiline = true;
            logBox.ScrollBars = ScrollBars.Vertical;
            logBox.ReadOnly = true;
            Controls.Add(inputPath);
            Controls.Add(browseInput);
            Controls.Add(outputPath);
            Controls.Add(browseOutput);
            Controls.Add(renameCheck);
            Controls.Add(stringEncryptCheck);
            Controls.Add(controlFlowCheck);
            Controls.Add(passwordLabel);
            Controls.Add(passwordBox);
            Controls.Add(obfuscateBtn);
            Controls.Add(deobfuscateBtn);
            Controls.Add(logBox);
            ResumeLayout(false);
        }
    }
}
