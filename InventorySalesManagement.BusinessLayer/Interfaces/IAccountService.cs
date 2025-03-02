using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.ModelView.AuthViewModel;
using InventorySalesManagement.Core.ModelView.AuthViewModel.LoginData;
using InventorySalesManagement.Core.ModelView.AuthViewModel.RegisterData;
using InventorySalesManagement.Core.ModelView.AuthViewModel.UpdateData;

namespace InventorySalesManagement.BusinessLayer.Interfaces;

public interface IAccountService
{
    Task<List<ApplicationUser>> GetAllUsers();
    Task<ApplicationUser> GetUserById(string userId);
    Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
    Task<AuthModel> RegisterCenterAsync(RegisterCenterVm model);
	Task<AuthModel> RegisterAdminAsync(RegisterAdminMv model);
	Task<AuthModel> UpdateAdminProfile(string userId, UpdateAdminMv updateUser);

	Task<AuthModel> LoginAsync(LoginModel model);
    Task<bool> Logout(string userName);
    Task<AuthModel> GetUserInfo(string userId);

    string ValidateJwtToken(string token);
    Task<ApplicationUser> GetUserByPhoneNumber(string phoneNumber);

}
