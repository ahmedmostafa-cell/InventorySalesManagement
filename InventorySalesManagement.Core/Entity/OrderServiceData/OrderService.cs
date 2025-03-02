using InventorySalesManagement.Core.Entity.OrderData;
using InventorySalesManagement.Core.Entity.SectionsData;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InventorySalesManagement.Core.Entity.OrderServiceData;

public class OrderService
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order Order { get; set; }

    [ForeignKey("Service")]
    public int ServiceId { get; set; }
    public Service Service { get; set; }

    [Required]
    [Display(Name = "الكمية")]
    public int Quantity { get; set; }

    [Required]
    [Display(Name = "السعر")]
    public float Price { get; set; }
}

