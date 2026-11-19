using DataStateDemo.Common;
using DataStateDemo.Models;

namespace DataStateDemo.Services;

public interface IMemberService
{
    Task<Result<List<MemberItem>>> GetMembersAsync(int delayMs, CancellationToken cancellationToken = default);
    Task<Result<List<MemberItem>>> GetEmptyListAsync(int delayMs, CancellationToken cancellationToken = default);
    Task<Result<List<MemberItem>>> GetFailingAsync(int delayMs, CancellationToken cancellationToken = default);
}
