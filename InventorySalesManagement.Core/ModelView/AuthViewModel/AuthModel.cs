using InventorySalesManagement.Core.Helpers;

namespace InventorySalesManagement.Core.ModelView.AuthViewModel;

public class AuthModel
{
    public bool IsAuthenticated { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string FullName { get; set; }
    public bool Status { get; set; }
    public bool IsUser { get; set; } = false;
    public bool IsAdmin { get; set; } = false;

    public int ErrorCode { get; set; }
    public UserType UserType { get; set; }
    public bool IsApproved { get; set; }
    public string Message { get; set; }
    public string ArMessage { get; set; }
    public string PhoneNumber { get; set; }
}