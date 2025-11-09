namespace MijnBibliotheekModels.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }
    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
