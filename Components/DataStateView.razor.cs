using DataStateDemo.State;
using Microsoft.AspNetCore.Components;

namespace DataStateDemo.Components;

public partial class DataStateView<T> : ComponentBase
{
    [Parameter, EditorRequired]
    public DataState<T> State { get; set; } = default;

    [Parameter]
    public string LoadingMessage { get; set; } = "Loading data...";

    [Parameter]
    public EventCallback OnRetry { get; set; }

    [Parameter]
    public Func<T, bool>? IsEmptyPredicate { get; set; }

    [Parameter]
    public RenderFragment? UninitializedContent { get; set; }

    [Parameter]
    public RenderFragment<T?>? InitializedContent { get; set; }

    [Parameter]
    public RenderFragment<DataState<T>.Error>? ErrorContent { get; set; }

    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    [Parameter]
    public RenderFragment<T>? SuccessContent { get; set; }
}
