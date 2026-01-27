using System.ComponentModel.DataAnnotations;

public record UpdateIngredientRequest
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Название должно содержать от 1 до 100 символов.")]
    public string? Name { get; set; } 
    
    [Range(0.001, float.MaxValue, ErrorMessage = "Количество должно быть положительным числом.")]
    public float? Quantity { get; set; } 
    public Unit? Unit { get; set; }

    [StringLength(500, ErrorMessage = "Примечание не может превышать 500 символов.")]
    public string? Note { get; set; } 
}