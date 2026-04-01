using System.ComponentModel.DataAnnotations;

public record ReorderItemsRequest
{
    [Required]
    [MinLength(1)]
    public List<Guid> OrderedIds { get; init; } = [];
}
