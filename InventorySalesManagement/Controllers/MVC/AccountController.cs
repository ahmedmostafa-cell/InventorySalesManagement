using InventorySalesManagement.BusinessLayer.Interfaces;
using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.ModelView.AuthViewModel.LoginData;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;

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
        // SeedQuran.SeedUser(_unitOfWork, _userManager).Wait();
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
            HttpOnly = true, // Prevents JavaScript access (more secure)
            Secure = true,   // Ensures cookie is sent over HTTPS
            Expires = DateTime.UtcNow.AddHours(2) // Token expiry
        });
        var user = await _accountService.GetUserByPhoneNumber(model.PhoneNumber);
        if (user.IsAdmin)
        {
            await _signInManager.SignInAsync(user, model.IsPersist);
            return Json(new { success = true, result.Token });
        }
        if (!result.IsAuthenticated)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return Json(new { success = false, message = "Invalid username or password." });
        }

        
        ModelState.AddModelError(string.Empty, "لا تملك الصلاحية اللازمه للدخول");
        return Json(new { success = false, message = "unauthorized." });
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (User.Identity != null)
        {
            var userName = User.Identity.Name;
            await _accountService.Logout(userName);
        }

        await _signInManager.SignOutAsync();//expires cookie
        return RedirectToAction("Login");
    }
}
