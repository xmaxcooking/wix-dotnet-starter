using App.Core.Models;

namespace App.Core;

/// <summary>
/// Placeholder in-memory data set, standing in for a real data source (database, API, file, ...).
/// </summary>
public static class SampleData
{
    public static IReadOnlyList<Customer> GetCustomers() =>
    [
        new(1, "Ava Nguyen", "ava.nguyen@example.com", "Switzerland", new DateOnly(2023, 3, 14)),
        new(2, "Liam Fischer", "liam.fischer@example.com", "Germany", new DateOnly(2023, 6, 2)),
        new(3, "Mia Rossi", "mia.rossi@example.com", "Italy", new DateOnly(2024, 1, 21)),
        new(4, "Noah Dubois", "noah.dubois@example.com", "France", new DateOnly(2024, 5, 9)),
        new(5, "Emma Keller", "emma.keller@example.com", "Austria", new DateOnly(2024, 11, 30)),
    ];
}
