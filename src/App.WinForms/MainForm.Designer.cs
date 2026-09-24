namespace App.WinForms;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        customersGrid = new DataGridView();
        titleLabel = new Label();
        ((System.ComponentModel.ISupportInitialize)customersGrid).BeginInit();
        SuspendLayout();
        //
        // titleLabel
        //
        titleLabel.AutoSize = true;
        titleLabel.Dock = DockStyle.Top;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        titleLabel.Padding = new Padding(12);
        titleLabel.Text = "Sample Customers";
        //
        // customersGrid
        //
        customersGrid.AllowUserToAddRows = false;
        customersGrid.AllowUserToDeleteRows = false;
        customersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        customersGrid.Dock = DockStyle.Fill;
        customersGrid.ReadOnly = true;
        customersGrid.RowHeadersVisible = false;
        customersGrid.Name = "customersGrid";
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(customersGrid);
        Controls.Add(titleLabel);
        MinimumSize = new Size(500, 300);
        Text = "WiX .NET Starter — Sample Data";
        ((System.ComponentModel.ISupportInitialize)customersGrid).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private DataGridView customersGrid;
    private Label titleLabel;
}
