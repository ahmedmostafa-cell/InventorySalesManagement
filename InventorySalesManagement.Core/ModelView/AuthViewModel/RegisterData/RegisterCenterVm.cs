using System.ComponentModel.DataAnnotations;

namespace InventorySalesManagement.Core.ModelView.AuthViewModel.RegisterData;

public class RegisterCenterVm
{
    [Required, StringLength(50), MinLength(5)]
    public string FullName { get; set; }

    [Required, StringLength(128)]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "البريد الالكتروني غير صحيح")]
    public string Email { get; set; }


    [Required, StringLength(256)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "رقم الهاتف")]
    [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
    [Required]
    public string PhoneNumber { get; set; }

}
