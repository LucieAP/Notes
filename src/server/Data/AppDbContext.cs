
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) {}

    // Сущности БД
    public DbSet<User> Users { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<NoteGroup> NoteGroups { get; set; }
    public DbSet<TaskGroup> TaskGroups { get; set; }
    public DbSet<RecipeGroup> RecipeGroups { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeStep> RecipeSteps { get; set; }
    public DbSet<IngredientGroup> IngredientGroups { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(u => u.GoogleId).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Note>().Property(n => n.Content).HasColumnType("jsonb");
    }
}