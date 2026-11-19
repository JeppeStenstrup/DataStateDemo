namespace DataStateDemo.Common;

public sealed record Error(string Code, string Description)
{
    public static Error Failure(string code, string description) => new(code, description);
    public static Error Timeout(string code, string description) => new(code, description);
    public static Error NotFound(string code, string description) => new(code, description);
}
