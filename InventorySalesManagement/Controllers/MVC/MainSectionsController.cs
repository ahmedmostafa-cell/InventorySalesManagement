﻿using InventorySalesManagement.Core.DTO.EntityDto;
using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Entity.SectionsData;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InventorySalesManagement.Controllers.MVC;

[Authorize(AuthenticationSchemes = "Bearer")]
public class MainSectionsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private ApplicationUser _user;

    public MainSectionsController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
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
        return View(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false));
    }

    public async Task<IActionResult> GetMainSections()
    {
        var sections = await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false);
        return Json(sections);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _unitOfWork.MainSections == null)
        {
            return NotFound();
        }

        var mainSection = await _unitOfWork.MainSections
            .FindAsync(m => m.Id == id && m.IsDeleted == false);

        if (mainSection == null)
        {
            return NotFound();
        }

        return View(mainSection);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MainSectionDto mainSectionDto)
    {
        if (!ModelState.IsValid) return View(mainSectionDto);

        MainSection mainSection = new MainSection();

        mainSection.TitleEn = mainSectionDto.TitleEn;
        mainSection.TitleAr = mainSectionDto.TitleAr;
        mainSection.Description = mainSectionDto.Description;

         await _unitOfWork.MainSections.AddAsync(mainSection);

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.MainSections == null)
        {
            return NotFound();
        }

        var mainSection = await _unitOfWork.MainSections
            .FindAsync(m => m.Id == id && m.IsDeleted == false);

        MainSectionDto mainSectionDto = new MainSectionDto();
        mainSectionDto.TitleEn = mainSection.TitleEn;
        mainSectionDto.TitleAr = mainSection.TitleAr;
        mainSectionDto.Description = mainSection.Description;

        if (mainSection == null)
        {
            return NotFound();
        }

        return View(mainSectionDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MainSectionDto mainSectionDto)
    {

        if (!ModelState.IsValid) return View(mainSectionDto);
        try
        {
            var mainSection = await _unitOfWork.MainSections
                        .FindByQuery(
                            criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();

            mainSection.TitleEn = mainSectionDto.TitleEn;
            mainSection.TitleAr = mainSectionDto.TitleAr;
            mainSection.Description = mainSectionDto.Description;

            _unitOfWork.MainSections.Update(mainSection);

            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (_unitOfWork.MainSections == null)
        {
            return Problem("Entity set 'ApplicationContext.MainSections'  is null.");
        }
        var mainSection = await _unitOfWork.MainSections
            .FindByQuery(
                criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();

        if (mainSection != null)
        {
            mainSection.IsDeleted = true;
            mainSection.DeletedAt = DateTime.Now;
            _unitOfWork.MainSections.Update(mainSection);
        }

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool MainSectionExists(int id)
    {
        return _unitOfWork.MainSections.IsExist(e => e.Id == id);
    }
}
