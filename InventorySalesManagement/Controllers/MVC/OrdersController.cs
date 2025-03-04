using InventorySalesManagement.Core.Entity.OrderData;
using InventorySalesManagement.Core.Entity.OrderServiceData;
using InventorySalesManagement.Core.ModelView.OrdersModel;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySalesManagement.Controllers.MVC;

[Authorize(AuthenticationSchemes = "Bearer")]
public class OrdersController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public OrdersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var applicationContext =
            await _unitOfWork.Orders.FindAllAsync(s => s.IsDeleted == false );

        return View(applicationContext);
    }

    public async Task<IActionResult> Create()
    {
        var model = new OrderViewModel
        {
            Services = await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false, include: s => s.Include(service => service.MainSection))
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult Create([FromBody] OrderViewModel model)
    {
        if (model.OrderServices == null || model.OrderServices.Count == 0)
        {
            return BadRequest("لا يوجد خدمات مضافة للطلب.");
        }

        float totalOrderPrice = model.OrderServices.Sum(os => os.Quantity * os.Price);
        var order = new Order
        {
            OrderNumber = "ORD-" + new Random().Next(1000, 9999),
            CreatedOn = DateTime.Now,
            Total = totalOrderPrice,
            OrderServices = model.OrderServices.Select(os => new OrderService
            {
                ServiceId = os.ServiceId,
                Quantity = os.Quantity,
                Price = os.Price
            }).ToList()
        };

        _unitOfWork.Orders.Add(order);

        _unitOfWork.SaveChanges();

        return Json(new { success = true });
    }

    [HttpGet]
    public IActionResult GetInvoiceDetails(int id)
    {
        var invoiceDetails = _unitOfWork.Orders 
            .GetById(id);
          
        return Json(invoiceDetails);
    }

    [HttpPost]
    public async Task<JsonResult> Details([FromBody] string invoiceId)
    {
        if (string.IsNullOrWhiteSpace(invoiceId))
        {
            return Json(new { success = false, message = "لا يوجد فواتير." });
        }

        var orderDetails = await _unitOfWork.Orders.FindAllAsync(
            s => s.Id == int.Parse(invoiceId),
            include: s => s.Include(order => order.OrderServices)
                           .ThenInclude(orderService => orderService.Service) 
        );

        if (orderDetails == null || !orderDetails.Any())
        {
            return Json(new { success = false, message = "لم يتم العثور على الفاتورة." });
        }

        var order = orderDetails.FirstOrDefault(); 

        var response = new
        {
            success = true,
            message = "تم جلب البيانات بنجاح!",
            orderNumber = order.OrderNumber,
            createdOn = order.CreatedOn,
            total = order.Total,
            orderServices = order.OrderServices.Select(os => new
            {
                id = os.Id,
                serviceId = os.ServiceId,
                serviceName = os.Service?.TitleAr, 
                quantity = os.Quantity,
                unitPrice = os.Price,
                totalPrice = os.Quantity * os.Price
            }).ToList()
        };

        return Json(response);
    }

}

