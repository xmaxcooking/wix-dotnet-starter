namespace App.Client.Controls.Settings;

partial class SettingsForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        channelLabel = new Label();
        stableRadioButton = new RadioButton();
        betaRadioButton = new RadioButton();
        noteLabel = new Label();
        okButton = new Button();
        cancelButton = new Button();
        SuspendLayout();
        //
        // channelLabel
        //
        channelLabel.AutoSize = true;
        channelLabel.Location = new Point(12, 12);
        channelLabel.Text = "Update channel:";
        //
        // stableRadioButton
        //
        stableRadioButton.AutoSize = true;
        stableRadioButton.Location = new Point(12, 36);
        stableRadioButton.Text = "Stable";
        stableRadioButton.Checked = true;
        //
        // betaRadioButton
        //
        betaRadioButton.AutoSize = true;
        betaRadioButton.Location = new Point(12, 60);
        betaRadioButton.Text = "Beta";
        //
        // noteLabel
        //
        noteLabel.AutoSize = true;
        noteLabel.ForeColor = SystemColors.GrayText;
        noteLabel.Location = new Point(12, 92);
        noteLabel.MaximumSize = new Size(300, 0);
        noteLabel.Text = "Takes effect on next launch - the update check only runs at startup.";
        //
        // okButton
        //
        okButton.DialogResult = DialogResult.OK;
        okButton.Location = new Point(156, 136);
        okButton.Text = "OK";
        okButton.Click += okButton_Click;
        //
        // cancelButton
        //
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(237, 136);
        cancelButton.Text = "Cancel";
        //
        // SettingsForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AcceptButton = okButton;
        CancelButton = cancelButton;
        ClientSize = new Size(324, 171);
        Controls.Add(channelLabel);
        Controls.Add(stableRadioButton);
        Controls.Add(betaRadioButton);
        Controls.Add(noteLabel);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Settings";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label channelLabel;
    private RadioButton stableRadioButton;
    private RadioButton betaRadioButton;
    private Label noteLabel;
    private Button okButton;
    private Button cancelButton;
}
