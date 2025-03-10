﻿using InventorySalesManagement.Core.DTO.EntityDto;
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

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // Ensure the token is validated
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

    public async Task<IActionResult> Edit(int? id)
    {
        return View(id);
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // Ensure the token is validated
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

        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

        return Json(new { success = false, errors });
    }

    [HttpDelete]
    [ValidateAntiForgeryToken] // Ensure the token is validated
    public async Task<IActionResult> Delete(int id)
    {
        if (_unitOfWork.Services == null)
        {
            return Json(new { success = false, message = "Entity set is null." });
        }

        var service = await _unitOfWork.Services
            .FindByQuery(s => s.Id == id && !s.IsDeleted)
            .FirstOrDefaultAsync();

        if (service == null)
        {
            return Json(new { success = false, message = "الخدمة غير موجودة." });
        }

        service.IsDeleted = true;
        service.DeletedAt = DateTime.Now;
        _unitOfWork.Services.Update(service);

        await _unitOfWork.SaveChangesAsync();
        return Json(new { success = true, message = "تم حذف الخدمة بنجاح." });
    }

    private bool ServiceExists(int id)
    {
        return _unitOfWork.Services.IsExist(e => e.Id == id);
    }
}
