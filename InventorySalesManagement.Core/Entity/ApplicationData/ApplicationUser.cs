using InventorySalesManagement.Core.Helpers;
using Microsoft.AspNetCore.Identity;

namespace InventorySalesManagement.Core.Entity.ApplicationData;

public class ApplicationUser : IdentityUser
{
    public bool IsAdmin { get; set; } = false;
    public bool Status { get; set; } = true;
    public bool IsApproved { get; set; } = false;
    public UserType UserType { get; set; } 
    public string FullName { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;

}
