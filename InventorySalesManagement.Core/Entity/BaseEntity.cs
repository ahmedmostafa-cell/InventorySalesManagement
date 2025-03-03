using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventorySalesManagement.Core.Entity;

public class BaseEntity
{
    public int Id { get; set; }
    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsUpdated { get; set; } = false;
    [Display(Name = "تاريخ أخر تحديث  ")]
    [JsonIgnore]
    public DateTime? UpdatedAt { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
    [JsonIgnore]
    public DateTime? DeletedAt { get; set; } = null;
}
