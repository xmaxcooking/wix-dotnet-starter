using App.Core;

namespace App.Core.Tests;

public class SampleDataTests
{
    [Fact]
    public void GetCustomers_ReturnsNonEmptyList()
    {
        var customers = SampleData.GetCustomers();

        Assert.NotEmpty(customers);
    }

    [Fact]
    public void GetCustomers_HasUniqueIds()
    {
        var customers = SampleData.GetCustomers();

        Assert.Equal(customers.Count, customers.Select(c => c.Id).Distinct().Count());
    }
}
