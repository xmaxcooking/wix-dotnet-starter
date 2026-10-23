using App.Client.Controls.Settings;
using App.Infrastructure;

namespace App.Client.Controls.Main;

public partial class MainForm : Form
{
    private readonly Func<SettingsForm> _settingsFormFactory;

    /// <summary>For the WinForms visual Designer only.</summary>
    public MainForm()
    {
        _settingsFormFactory = createSettingsForm;
        InitializeComponent();
        customersGrid.DataSource = createDefaultDbContext().Customers.ToList();
    }

    public MainForm(Func<SettingsForm> settingsFormFactory, IAppDbContext dbContext)
    {
        _settingsFormFactory = settingsFormFactory;
        InitializeComponent();
        customersGrid.DataSource = dbContext.Customers.ToList();
    }

    private static SettingsForm createSettingsForm() => new();

    private static IAppDbContext createDefaultDbContext() => new FakeDbContext();

    private void settingsMenuItem_Click(object sender, EventArgs e) => _settingsFormFactory().ShowDialog(this);
}