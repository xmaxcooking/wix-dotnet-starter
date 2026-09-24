namespace App.Core.Models;

public sealed record Customer(int Id, string Name, string Email, string Country, DateOnly SignedUpOn);
