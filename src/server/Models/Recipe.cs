using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using server.Interfaces;

public class Recipe : IUpdatableItem, ISoftDeletable
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Заголовок обязателен для заполнения.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Заголовок должен содержать от 1 до 100 символов.")]
    public required string Title { get; set; }
    [StringLength(5000, ErrorMessage = "Описание не может превышать 5000 символов.")]
    public string? Description { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public bool IsTrashed { get; set; }
    public bool IsDeleted { get; set; } 
    public DateTime? DeletedAt { get; set; }

    // Внешние ключи 
    [ForeignKey("User")]
    [Required(ErrorMessage = "Идентификатор создателя обязателен для заполнения.")]
    public Guid CreatedBy { get; set; }
    public Guid? RecipeGroupId { get; set; }

    // Навигационные свойства
    public User User { get; set; }
    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public ICollection<IngredientGroup> IngredientGroups { get; set; } = new List<IngredientGroup>();
    public ICollection<RecipeStep> RecipeSteps { get; set; } = new List<RecipeStep>();
    public RecipeGroup? RecipeGroup { get; set; }
}