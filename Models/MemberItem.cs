namespace DataStateDemo.Models;

public sealed record MemberItem(
    Guid Id,
    int Number,
    string Name,
    string Email,
    string Role,
    string Status,
    DateTime JoinedDate);
