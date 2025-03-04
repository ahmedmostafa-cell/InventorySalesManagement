using System.ComponentModel.DataAnnotations;

namespace InventorySalesManagement.Core.ModelView.AuthViewModel.LoginData;

public class LoginModel
{

    [Required(ErrorMessage = "يجب أدخال اسم المستخدم ")]
    [Display(Name = "User Name")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "يجب أدخال كلمة السر ")]
    [Display(Name = "Password")]
    public string Password { get; set; }
    public string DeviceToken { get; set; }

    public bool IsPersist { get; set; }
}
