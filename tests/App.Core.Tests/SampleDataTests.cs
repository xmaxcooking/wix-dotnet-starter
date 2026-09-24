using App.Core;

namespace App.Core.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    public void GetCustomers_ReturnsNonEmptyList()
    {
        var customers = SampleData.GetCustomers();

        Assert.AreNotEqual(0, customers.Count);
    }

    [TestMethod]
    public void GetCustomers_HasUniqueIds()
    {
        var customers = SampleData.GetCustomers();

        Assert.AreEqual(customers.Count, customers.Select(c => c.Id).Distinct().Count());
    }
}
