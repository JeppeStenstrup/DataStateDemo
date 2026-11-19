using DataStateDemo.Common;
using DataStateDemo.Models;
using DataStateDemo.Services;
using DataStateDemo.State;
using Microsoft.AspNetCore.Components;

namespace DataStateDemo.Components.Pages;

public partial class Home : ComponentBase
{
    [Inject]
    public IMemberService MemberService { get; set; } = default!;

    protected DataState<List<MemberItem>> membersState = DataState<List<MemberItem>>.None();
    protected int selectedDelayMs = 2500;
    protected bool isActionInProgress;

    protected void OnDelayChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var delay))
        {
            selectedDelayMs = delay;
        }
    }

    protected async Task LoadDataAsync()
    {
        if (isActionInProgress) return;
        isActionInProgress = true;

        try
        {
            membersState = await membersState.Initialize(
                () => MemberService.GetMembersAsync(selectedDelayMs),
                preserveStale: false);
        }
        finally
        {
            isActionInProgress = false;
        }
    }

    protected async Task RefreshWithStaleDataAsync()
    {
        if (isActionInProgress) return;
        isActionInProgress = true;

        try
        {
            membersState = await membersState.Initialize(
                () => MemberService.GetMembersAsync(selectedDelayMs),
                preserveStale: true);
        }
        finally
        {
            isActionInProgress = false;
        }
    }

    protected async Task SimulateErrorAsync()
    {
        if (isActionInProgress) return;
        isActionInProgress = true;

        try
        {
            membersState = await membersState.Initialize(
                () => MemberService.GetFailingAsync(selectedDelayMs),
                preserveStale: false);
        }
        finally
        {
            isActionInProgress = false;
        }
    }

    protected async Task SimulateEmptyAsync()
    {
        if (isActionInProgress) return;
        isActionInProgress = true;

        try
        {
            membersState = await membersState.Initialize(
                () => MemberService.GetEmptyListAsync(selectedDelayMs),
                preserveStale: false);
        }
        finally
        {
            isActionInProgress = false;
        }
    }

    protected async Task HandleRetryAsync() =>
        await LoadDataAsync();

    protected void ResetState() =>
        membersState = DataState<List<MemberItem>>.None();
}
