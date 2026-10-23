namespace App.Client.Controls.Main;

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
        customersGrid = new DataGridView();
        titleLabel = new Label();
        mainMenu = new MenuStrip();
        settingsMenuItem = new ToolStripMenuItem();
        ((System.ComponentModel.ISupportInitialize)customersGrid).BeginInit();
        mainMenu.SuspendLayout();
        SuspendLayout();
        // 
        // customersGrid
        // 
        customersGrid.AllowUserToAddRows = false;
        customersGrid.AllowUserToDeleteRows = false;
        customersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        customersGrid.Dock = DockStyle.Fill;
        customersGrid.Location = new Point(0, 73);
        customersGrid.Name = "customersGrid";
        customersGrid.ReadOnly = true;
        customersGrid.RowHeadersVisible = false;
        customersGrid.Size = new Size(800, 377);
        customersGrid.TabIndex = 0;
        // 
        // titleLabel
        // 
        titleLabel.AutoSize = true;
        titleLabel.Dock = DockStyle.Top;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        titleLabel.Location = new Point(0, 24);
        titleLabel.Name = "titleLabel";
        titleLabel.Padding = new Padding(12);
        titleLabel.Size = new Size(200, 49);
        titleLabel.TabIndex = 1;
        titleLabel.Text = "Sample Customers";
        // 
        // mainMenu
        // 
        mainMenu.Items.AddRange(new ToolStripItem[] { settingsMenuItem });
        mainMenu.Location = new Point(0, 0);
        mainMenu.Name = "mainMenu";
        mainMenu.Size = new Size(800, 24);
        mainMenu.TabIndex = 2;
        // 
        // settingsMenuItem
        // 
        settingsMenuItem.Name = "settingsMenuItem";
        settingsMenuItem.Size = new Size(70, 20);
        settingsMenuItem.Text = "Settings...";
        settingsMenuItem.Click += settingsMenuItem_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(customersGrid);
        Controls.Add(titleLabel);
        Controls.Add(mainMenu);
        MainMenuStrip = mainMenu;
        MinimumSize = new Size(500, 300);
        Name = "MainForm";
        Text = "WiX .NET Starter — Sample Data";
        ((System.ComponentModel.ISupportInitialize)customersGrid).EndInit();
        mainMenu.ResumeLayout(false);
        mainMenu.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private DataGridView customersGrid;
    private Label titleLabel;
    private MenuStrip mainMenu;
    private ToolStripMenuItem settingsMenuItem;
}
