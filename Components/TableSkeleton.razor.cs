using DataStateDemo.State;
using Microsoft.AspNetCore.Components;

namespace DataStateDemo.Components;

public partial class TableSkeleton<TItem> : ComponentBase
{
    [Parameter, EditorRequired]
    public DataState<List<TItem>> State { get; set; } = default;

    [Parameter]
    public int Columns { get; set; } = 5;

    [Parameter]
    public int Rows { get; set; } = 5;

    [Parameter]
    public string TableClass { get; set; } = "members-table";

    [Parameter]
    public RenderFragment? TableHeader { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment<TItem> RowTemplate { get; set; } = default!;

    [Parameter]
    public EventCallback OnRetry { get; set; }

    [Parameter]
    public EventCallback OnLoad { get; set; }

    [Parameter]
    public string UninitializedTitle { get; set; } = "Directory not loaded";

    [Parameter]
    public string UninitializedSubtitle { get; set; } = "Click \"Load Directory\" above to fetch records with simulated Task.Delay.";

    [Parameter]
    public string LoadButtonText { get; set; } = "Load Directory";

    [Parameter]
    public string EmptyTitle { get; set; } = "No members found";

    [Parameter]
    public string EmptySubtitle { get; set; } = "There are currently no active members returned by the query.";

    [Parameter]
    public RenderFragment? UninitializedContent { get; set; }

    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    [Parameter]
    public RenderFragment<DataState<List<TItem>>.Error>? ErrorContent { get; set; }
}
