using App.Core;

// Demonstrates using an ordinary .NET 10 library (App.Core - the same one App.WinForms uses)
// during install. A net472 MSI custom action can't reference it directly; this project runs
// as a Burn ExePackage instead, so it's a plain .NET 10 console app.
//
// args[0] is the bundle's overridable SampleBundleVariable (see Bundle.wxs), the same value
// installer/App.Actions/Actions.cs reads via the MSI's SAMPLEPROPERTY - one bundle variable,
// two different ways of consuming it during the same install.

var sampleBundleVariable = args.Length > 0 ? args[0] : string.Empty;

var customers = SampleData.GetCustomers();
Console.WriteLine($"App.InstallTool: SampleBundleVariable='{sampleBundleVariable}', " +
    $"App.Core reports {customers.Count} sample customers.");

return 0;
