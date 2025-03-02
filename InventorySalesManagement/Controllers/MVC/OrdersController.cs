using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySalesManagement.Controllers.MVC;

public class OrdersController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public OrdersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: Orders
    public async Task<IActionResult> Index()
    {
        var applicationContext =
            await _unitOfWork.Orders.FindAllAsync(s => s.IsDeleted == false );
        return View(applicationContext);
    }
}
