using DataStateDemo.Common;
using DataStateDemo.Models;

namespace DataStateDemo.Services;

public sealed class MemberService : IMemberService
{
    private static readonly List<MemberItem> SampleItems =
    [
        new(Guid.Parse("11111111-1111-1111-1111-111111111111"), 101, "Freja Møller", "freja.moller@example.com", "Chairperson", "Active", DateTime.UtcNow.AddYears(-3)),
        new(Guid.Parse("22222222-2222-2222-2222-222222222222"), 102, "Lars Holm", "lars.holm@example.com", "Treasurer", "Active", DateTime.UtcNow.AddYears(-2)),
        new(Guid.Parse("33333333-3333-3333-3333-333333333333"), 103, "Astrid Lindberg", "astrid.lindberg@example.com", "Secretary", "Active", DateTime.UtcNow.AddYears(-1)),
        new(Guid.Parse("44444444-4444-4444-4444-444444444444"), 104, "Christian Bay", "christian.bay@example.com", "Member", "Active", DateTime.UtcNow.AddMonths(-6)),
        new(Guid.Parse("55555555-5555-5555-5555-555555555555"), 105, "Sofie Poulsen", "sofie.poulsen@example.com", "Member", "Passive", DateTime.UtcNow.AddMonths(-2))
    ];

    public async Task<Result<List<MemberItem>>> GetMembersAsync(int delayMs, CancellationToken cancellationToken = default)
    {
        await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);
        return Result<List<MemberItem>>.Success([.. SampleItems]);
    }

    public async Task<Result<List<MemberItem>>> GetEmptyListAsync(int delayMs, CancellationToken cancellationToken = default)
    {
        await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);
        return Result<List<MemberItem>>.Success([]);
    }

    public async Task<Result<List<MemberItem>>> GetFailingAsync(int delayMs, CancellationToken cancellationToken = default)
    {
        await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);
        return Result<List<MemberItem>>.Failure("Database.ConnectionTimeout", "PostgreSQL connection pool exhausted. Failed to acquire connection within timeout.");
    }
}
