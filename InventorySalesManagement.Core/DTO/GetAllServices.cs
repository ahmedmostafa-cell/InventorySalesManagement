namespace InventorySalesManagement.Core.DTO;

public class GetAllServices
{
    public string ServiceProviderName { get; set; } = null;
    public int? MainSectionId { get; set; } = null;
    public int? StartPrice { get; set; } = null;
    public string SearchName { get; set; } = null;
    public int? EndPrice { get; set; } = null;

}
