using InventorySalesManagement.BusinessLayer.Interfaces;
using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.ModelView.AuthViewModel.LoginData;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel loginUser)
    {
        if (!ModelState.IsValid)
        {
            return View(loginUser);
        }
        var result = await _accountService.LoginAsync(loginUser);
        if (!result.IsAuthenticated)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(loginUser);
        }
        var user = await _accountService.GetUserByPhoneNumber(loginUser.PhoneNumber);

        if (user.IsAdmin)
        {
            await _signInManager.SignInAsync(user, loginUser.IsPersist);
            return RedirectToAction("Index", "Dashboard");
        }
        ModelState.AddModelError(string.Empty, "لا تملك الصلاحية اللازمه للدخول");
        return View(loginUser);
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
