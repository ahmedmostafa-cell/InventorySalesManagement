using InventorySalesManagement.BusinessLayer.Interfaces;
using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.ModelView.AuthViewModel.LoginData;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventorySalesManagement.Controllers.MVC;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IAccountService accountService, SignInManager<ApplicationUser> signInManager)
    {
        this._signInManager = signInManager;
        _accountService = accountService;
        _unitOfWork = unitOfWork;
        _userManager = userManager;

    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid input." });
        }

        var result = await _accountService.LoginAsync(model);

        Response.Cookies.Append("AuthToken", result.Token, new CookieOptions
        {
            HttpOnly = true, 
            Secure = true,   
            Expires = DateTime.UtcNow.AddHours(2)
        });


        return Json(new { success = true, result.Token });

    }

}
