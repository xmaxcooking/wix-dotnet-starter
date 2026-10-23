using App.Infrastructure;

var sampleBundleVariable = args.Length > 0 ? args[0] : string.Empty;

var customerCount = new FakeDbContext().Customers.Count();
Console.WriteLine($"App.Installer.Tool: SampleBundleVariable='{sampleBundleVariable}', " +
    $"App.Domain reports {customerCount} sample customers.");

return 0;