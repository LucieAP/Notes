public record ToggleCheckboxResponse
{
    public Guid Id { get; set; }
    public bool CheckBox { get; set; }
    public DateTime LastModifiedAt { get; set; }
}