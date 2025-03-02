using InventorySalesManagement.BusinessLayer.Interfaces;
using InventorySalesManagement.Core.Helpers;
using InventorySalesManagement.Core.ModelView.AuthViewModel.RegisterData;
using InventorySalesManagement.Core.ModelView.AuthViewModel.UpdateData;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySalesManagement.Controllers.MVC;

[Authorize]
public class AdminController : Controller
{
	private readonly IAccountService _accountService;
	private readonly IUnitOfWork _unitOfWork;


	public AdminController(IUnitOfWork unitOfWork, IAccountService accountService)
	{
		_accountService = accountService;
		_unitOfWork = unitOfWork;
	}

	// GET: AdminController
	public async Task<ActionResult> Index()
	{
		var allAdmins = await _unitOfWork.Users.FindAllAsync(s => s.IsAdmin == true && s.UserType == UserType.Admin);

		return View(allAdmins);
	}
	//------------------------------------------------------------------------------------------------------- UsersList
	// GET: AdminController
	public async Task<ActionResult> IndexUsers()
	{
		var allUsers = await _unitOfWork.Users.FindAllAsync(s => s.IsAdmin == false && s.UserType == UserType.User && s.Status == true);

		return View(allUsers);
	}
	public async Task<ActionResult> IndexUsersDeleted()
	{
		var allUsers = await _unitOfWork.Users.FindAllAsync(s => s.IsAdmin == false && s.UserType == UserType.User && s.Status == false);

		return View(allUsers);
	}

	public async Task<ActionResult> IndexServiceProviderDeleted()
	{
		var allUsers = await _unitOfWork.Users.FindAllAsync(s => s.IsAdmin == false && (s.UserType == UserType.User));

		return View(allUsers);
	}



	//------------------------------------------------------------------------------------------------------- Create
	// GET: AdminController/Create
	public IActionResult Create()
	{

		return View(new RegisterAdminMv());
	}

	// POST: AdminController/Create
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

	//-------------------------------------------------------------------------------------------------------  Edit
	// GET: AdminController/Edit/userId
	public async Task<IActionResult> Edit(string id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var admin = await _unitOfWork.Users.FindAsync(s => s.IsAdmin && s.Id == id);
		if (admin == null)
		{
			return NotFound();
		}
		UpdateAdminMv updateAdmin = new()
		{
			UserId = admin.Id,
			FullName = admin.FullName,
			Email = admin.Email,
		};

		return View(updateAdmin);
	}

	// POST: AdminController/Edit/userId
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(string id, UpdateAdminMv adminModel)
	{
		var admin = await _unitOfWork.Users.FindAsync(s => s.IsAdmin && s.Id == adminModel.UserId, isNoTracking: true);
		if (admin == null)
		{
			return NotFound();
		}

		if (!ModelState.IsValid) return View(adminModel);
		var result = await _accountService.UpdateAdminProfile(adminModel.UserId, adminModel);

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



	//-------------------------------------------------------------------------------------------------------  EditUser
	// GET: AdminController/Edit/userId
	public async Task<IActionResult> EditUser(string id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var user = await _unitOfWork.Users.FindAsync(s => s.Id == id);
		if (user == null)
		{
			return NotFound();
		}
		UpdateAdminMv updateAdmin = new()
		{
			UserId = user.Id,
			FullName = user.FullName,
			Email = user.Email,
			PhoneNumber = user.PhoneNumber,

		};

		return View(updateAdmin);
	}

	// POST: AdminController/Edit/userId
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EditUser(string id, UpdateAdminMv adminModel)
	{
		var admin = await _unitOfWork.Users.FindAsync(s => s.Id == adminModel.UserId, isNoTracking: true);
		if (admin == null)
		{
			return NotFound();
		}

		if (!ModelState.IsValid) return View(adminModel);
		var result = await _accountService.UpdateAdminProfile(adminModel.UserId, adminModel);

		if (!result.IsAuthenticated)
		{
			ModelState.AddModelError("", result.ArMessage);
			return View(adminModel);
		}
		else
		{
			return RedirectToAction(nameof(IndexUsers));
		}
	}

	//------------------------------------------------------------------------------------------------------- Details

	// GET: AdminController/Details/userId
	public async Task<IActionResult> Details(string id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var admin = await _unitOfWork.Users.FindAsync(s => s.IsAdmin && s.Id == id);
		if (admin == null)
		{
			return NotFound();
		}
		return View(admin);
	}


	//------------------------------------------------------------------------------------------------------- DetailsUsers

	// GET: AdminController/Details/userId
	public async Task<IActionResult> DetailsUsers(string id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var user = await _unitOfWork.Users.FindAsync(s => s.Id == id);
		if (user == null)
		{
			return NotFound();
		}
		return View(user);
	}
}
