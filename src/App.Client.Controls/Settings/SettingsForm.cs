using App.Abstractions.FileSystem;
using App.Updates.Feed;
using App.Updates.Settings;
using Microsoft.Extensions.Logging.Abstractions;

namespace App.Client.Controls.Settings;

public partial class SettingsForm : Form
{
    private readonly IUpdateSettingsStore _updateSettings;

    /// <summary>For the WinForms visual Designer only.</summary>
    public SettingsForm()
    {
        _updateSettings = createDefaultUpdateSettings();
        InitializeComponent();
        applyChannel(_updateSettings.GetChannel());
    }

    public SettingsForm(IUpdateSettingsStore updateSettings)
    {
        _updateSettings = updateSettings;
        InitializeComponent();
        applyChannel(updateSettings.GetChannel());
    }

    private static IUpdateSettingsStore createDefaultUpdateSettings()
    {
        var fileSystem = new FileSystemIO();
        var logger = NullLogger<UpdateSettings>.Instance;
        return new UpdateSettings(fileSystem, logger);
    }

    private void applyChannel(UpdateChannel channel)
    {
        if (channel == UpdateChannel.Beta)
        {
            betaRadioButton.Checked = true;
        }
        else
        {
            stableRadioButton.Checked = true;
        }
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        _updateSettings.SetChannel(betaRadioButton.Checked ? UpdateChannel.Beta : UpdateChannel.Stable);
        DialogResult = DialogResult.OK;
        Close();
    }
}