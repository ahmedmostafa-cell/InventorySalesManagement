using InventorySalesManagement.BusinessLayer.Interfaces;
using InventorySalesManagement.Core.Helpers;
using InventorySalesManagement.Core.ModelView.AuthViewModel.RegisterData;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySalesManagement.Controllers.MVC;

[Authorize(AuthenticationSchemes = "Bearer")]
public class AdminController : Controller
{
	private readonly IAccountService _accountService;
	private readonly IUnitOfWork _unitOfWork;

	public AdminController(IUnitOfWork unitOfWork, IAccountService accountService)
	{
		_accountService = accountService;
		_unitOfWork = unitOfWork;
	}

	public async Task<ActionResult> Index()
	{
		var allAdmins = await _unitOfWork.Users.FindAllAsync(s => s.IsAdmin == true && s.UserType == UserType.Admin);

		return View(allAdmins);
	}

	public IActionResult Create()
	{

		return View(new RegisterAdminMv());
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<ActionResult> Create(RegisterAdminMv adminModel)
	{
		if (!ModelState.IsValid) return View(adminModel);

		var result = await _accountService.RegisterAdminAsync(adminModel);

		if (!result.IsAuthenticated)
		{
			ModelState.AddModelError("", result.ArMessage);
			return View(adminModel);
		}
		else
		{
			return RedirectToAction(nameof(Index));
		}
	}
}
