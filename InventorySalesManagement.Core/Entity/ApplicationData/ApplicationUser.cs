using InventorySalesManagement.Core.Helpers;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Net;

namespace InventorySalesManagement.Core.Entity.ApplicationData;

public class ApplicationUser : IdentityUser
{

    public bool IsAdmin { get; set; } = false;//if true, user is admin
    public bool Status { get; set; } = true;//true=active,false=deactive
    public bool IsApproved { get; set; } = false;// this is for admin approval

    //-------------------------------------------------------------------
    public UserType UserType { get; set; } // 0 = Admin,1 = NormalUser ,2 = Center, 3 = freelancer
    public string FullName { get; set; }
    public string Description { get; set; } // only for center and freelancer
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public string DeviceToken { get; set; }

}
