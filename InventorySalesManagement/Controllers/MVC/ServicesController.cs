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


[Authorize]
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
        var applicationContext = await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false, include: s => s.Include(service => service.MainSection));
        return View(applicationContext);
    }

    // GET: Services/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _unitOfWork.Services == null)
        {
            return NotFound();
        }

        var service = await _unitOfWork.Services.FindAsync(m => m.Id == id && m.IsDeleted == false);
        if (service == null)
        {
            return NotFound();
        }

        return View(service);
    }

    // GET: Services/Create
    public IActionResult Create()
    {
        ViewData["MainSectionId"] = new SelectList(_unitOfWork.MainSections.FindAll(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
        return View(new Service());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Service service)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr", service.MainSectionId);
        return View(service);
    }

    // GET: Services/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.Services == null)
        {
            return NotFound();
        }

        var service = await _unitOfWork.Services.FindAsync(m => m.Id == id && m.IsDeleted == false);
        if (service == null)
        {
            return NotFound();
        }
        ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr", service.MainSectionId);
        return View(service);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Service service)
    {
        if (id != service.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.Services.Update(service);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(service.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr", service.MainSectionId);
        return View(service);
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
