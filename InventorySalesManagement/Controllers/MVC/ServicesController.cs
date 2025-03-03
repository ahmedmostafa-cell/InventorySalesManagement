using InventorySalesManagement.Core.DTO.EntityDto;
using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Entity.SectionsData;
using InventorySalesManagement.Core.Helpers;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySalesManagement.Controllers.MVC;


[Authorize(AuthenticationSchemes = "Bearer")]
public class ServicesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private ApplicationUser _user;

    public ServicesController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;

    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = _userManager.GetUserId(User);
        _user = _unitOfWork.Users.Find(s => s.Id == userId);
    }

    //---------------------------------------------------------------------------------------------

    // GET: Services
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetServices()
    {
        var services = await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false,
            include: s => s.Include(service => service.MainSection));

        var data = services.Select(s => new
        {
            s.Id,
            s.TitleAr,
            MainSection = s.MainSection?.TitleAr,
            s.Qty,
            ProductType = s.ProductType == ProductType.Stored ? "مخزني" : "خدمي"
        });

        return Json(new { data });
    }


    // GET: Services/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound(); // Return a 404 error if no ID is provided
        }

        return View(id); // Pass ID to the view
    }


    [HttpGet]
    public async Task<IActionResult> GetServiceDetails(int id)
    {
        var service = await _unitOfWork.Services.FindAsync(m => m.Id == id && m.IsDeleted == false);

        if (service == null)
        {
            return NotFound();
        }

        return Json(new
        {
            id = service.Id,
            titleAr = service.TitleAr,
            titleEn = service.TitleEn,
            description = service.Description,
            price = service.Price,
            qty = service.Qty,
            productType = service.ProductType.ToString(),
            mainSectionTitle = service.MainSectionId
        });
    }

    // GET: Services/Create
    public IActionResult Create()
    {
        ViewData["MainSectionId"] = new SelectList(_unitOfWork.MainSections.FindAll(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
        return View(new Service());
    }


    [HttpPost]
    public async Task<IActionResult> Create(ServiceDto serviceDto)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "يرجى ملء جميع الحقول المطلوبة." });
        }
        Service service = new Service();
        try
        {
            // Save to database (example)
            service.MainSectionId = serviceDto.MainSectionId;
            service.Description = serviceDto.Description;
            service.ProductType = serviceDto.ProductType;
            service.TitleAr = serviceDto.TitleAr;
            service.TitleEn = serviceDto.TitleEn;
            service.Price = serviceDto.Price;
            service.Qty = serviceDto.Qty;
            service.IsDeleted = false;
            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "تم إنشاء الخدمة بنجاح!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "حدث خطأ أثناء الحفظ: " + ex.Message });
        }
    }

    // GET: Services/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        return View(id);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, ServiceDto service)
    {

        Service updatedService = await _unitOfWork.Services.GetByIdAsync(id);
        if (ModelState.IsValid)
        {
            try
            {
                updatedService.MainSectionId = service.MainSectionId;
                updatedService.Description = service.Description;
                updatedService.ProductType = service.ProductType;
                updatedService.TitleAr = service.TitleAr;
                updatedService.TitleEn = service.TitleEn;
                updatedService.Price = service.Price;
                updatedService.Qty = service.Qty;
                updatedService.IsDeleted = false;
                _unitOfWork.Services.Update(updatedService);
                await _unitOfWork.SaveChangesAsync();

                return Json(new { success = true, message = "تم تحديث الخدمة بنجاح!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(updatedService.Id))
                {
                    return Json(new { success = false, message = "Service not found." });
                }
                else
                {
                    return Json(new { success = false, message = "حدث خطأ أثناء حفظ البيانات." });
                }
            }
        }

        // If model validation fails, return errors
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return Json(new { success = false, errors });
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (_unitOfWork.Services == null)
        {
            return Problem("Entity set 'ApplicationContext.MainSections'  is null.");
        }
        var services = await _unitOfWork.Services
            .FindByQuery(
                criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();
        if (services != null)
        {
            services.IsDeleted = true;
            services.DeletedAt = DateTime.Now;
            _unitOfWork.Services.Update(services);
        }

        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ServiceExists(int id)
    {
        return _unitOfWork.Services.IsExist(e => e.Id == id);
    }

    #region  AJAX

    // check user type 
    public async Task<IActionResult> CheckUserType(string userId)
    {
        if (userId == null)
        {
            return Json(new { Error = true, Message = "يجب تحديد مقدم الخدمة أولا  " });
        }

        var user = await _unitOfWork.Users.FindAsync(m => m.Id == userId);
        if (user == null)
        {
            return Json(new { Error = true, Message = "يجب تحديد مقدم الخدمة أولا  " });
        }

        return user.UserType switch
        {
            UserType.Admin => Json(new { Error = false, type = 1, Message = "مقدم الخدمة مركز" }),
            UserType.User => Json(new { Error = false, type = 2, Message = "مقدم الخدمة مستقل" }),
            _ => Json(new { Error = true, Message = "يجب تحديد مقدم الخدمة أولا  " })
        };
    }

    #endregion
}
