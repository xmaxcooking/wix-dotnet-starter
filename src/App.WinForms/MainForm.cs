using App.Core;

namespace App.WinForms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        customersGrid.DataSource = SampleData.GetCustomers();
    }
}
