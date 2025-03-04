using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.ModelView.AuthViewModel;
using InventorySalesManagement.Core.ModelView.AuthViewModel.LoginData;
using InventorySalesManagement.Core.ModelView.AuthViewModel.RegisterData;

namespace InventorySalesManagement.BusinessLayer.Interfaces;

public interface IAccountService
{
    Task<List<ApplicationUser>> GetAllUsers();
    Task<ApplicationUser> GetUserById(string userId);
	Task<AuthModel> RegisterAdminAsync(RegisterAdminMv model);
	Task<AuthModel> LoginAsync(LoginModel model);
    Task<AuthModel> GetUserInfo(string userId);
    string ValidateJwtToken(string token);
    Task<ApplicationUser> GetUserByPhoneNumber(string phoneNumber);

}
