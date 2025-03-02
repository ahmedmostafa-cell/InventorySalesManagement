using InventorySalesManagement.Core.Entity.SectionsData;

namespace InventorySalesManagement.Core.ModelView.OrdersModel;

public class OrderViewModel
{
	public IEnumerable<Service> Services { get; set; }
	public List<OrderServiceViewModel> OrderServices { get; set; }
}
