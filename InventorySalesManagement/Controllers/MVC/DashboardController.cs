using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Helpers;
using InventorySalesManagement.Core.ModelView.MV;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InventorySalesManagement.Controllers.MVC;

[Authorize]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private ApplicationUser _user;

    public DashboardController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = _userManager.GetUserId(User);
        _user = _unitOfWork.Users.Find(s => s.Id == userId);
    }

    public IActionResult CheckUser()
    {
        if (_user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        return _user.IsAdmin ? RedirectToAction("Index", "Dashboard") : RedirectToAction("Login", "Account");
    }

    public async Task<IActionResult> Index()
    {
        var total = await _unitOfWork.Orders.FindByQuery(s => s.IsDeleted == false ).SumAsync(s => s.Total);
        var data = new DashboardCounts()
        {
            Centers = await _unitOfWork.Users.CountAsync(s => s.UserType == UserType.Admin && s.IsApproved == true),
            FreeAgents = await _unitOfWork.Users.CountAsync(s => s.UserType == UserType.User && s.IsApproved == true),
            AllSections = await _unitOfWork.MainSections.CountAsync(s => s.IsDeleted == false),
            AllServices = await _unitOfWork.Services.CountAsync(s => s.IsDeleted == false),
            AllOrders = await _unitOfWork.Orders.CountAsync(s => s.IsDeleted == false),
            TotalAmount = total,
        };
        return View(data);
    }


}
