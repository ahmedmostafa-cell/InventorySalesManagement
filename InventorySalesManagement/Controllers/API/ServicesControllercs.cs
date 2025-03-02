using InventorySalesManagement.Core.DTO;
using InventorySalesManagement.Core.DTO.EntityDto;
using InventorySalesManagement.Core.DTO.Pagination;
using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Entity.SectionsData;
using InventorySalesManagement.Core.Helpers;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace InventorySalesManagement.Controllers.API;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ServicesController : BaseApiController, IActionFilter
{
    private readonly BaseResponse _baseResponse;
    private readonly IUnitOfWork _unitOfWork;
    private ApplicationUser _user;

    public ServicesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _baseResponse = new BaseResponse();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        if (string.IsNullOrEmpty(accessToken))
            return;

        var userId = User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var user = _unitOfWork.Users.FindByQuery(s => s.Id == userId)
            .FirstOrDefault();
        _user = user;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
    //---------------------------------------------------------------------------------------------------

    [HttpGet("ServicesForUser")]
    public async Task<ActionResult<BaseResponse>> ServicesForUser([FromHeader] string lang, [FromQuery] PaginationParameters parameters, [FromQuery] GetAllServices getAll)
    {


        var services = await _unitOfWork.Services.FindByQuery(
                         criteria: s => s.IsDeleted == false &&
                                        (string.IsNullOrEmpty(getAll.SearchName) ||
                                         (s.TitleAr ?? "").Contains(getAll.SearchName) ||
                                         (s.TitleEn ?? "").Contains(getAll.SearchName)) &&
                                        (getAll.MainSectionId == null || s.MainSectionId == getAll.MainSectionId),
                         orderBy: s => s.OrderByDescending(o => o.CreatedAt), // ✅ Corrected OrderBy
                         skip: (parameters.PageNumber - 1) * parameters.PageSize,
                         take: parameters.PageSize)
                     .Select(s => new
                     {
                         s.Id,
                         title = lang == "ar" ? s.TitleAr : s.TitleEn,
                         s.Description,
                         s.Price,
                         s.Qty,
                         s.ProductType,
                         MainSection = new
                         {
                             s.MainSection.Id,
                             title = lang == "ar" ? s.MainSection.TitleAr : s.MainSection.TitleEn,
                             s.MainSection.ImgUrl
                         }
                     })
                     .ToListAsync();



        if (!services.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد خدمات " : "Services Not Found";
            return Ok(_baseResponse);
        }

        var servicesCount = await _unitOfWork.Services.CountAsync(criteria: s => s.IsDeleted == false &&
            (getAll.SearchName == null || s.TitleAr.Contains(getAll.SearchName) || s.TitleEn.Contains(getAll.SearchName)) &&
            (getAll.MainSectionId == null || s.MainSectionId == getAll.MainSectionId));

        var pageCount = servicesCount / parameters.PageSize;
        if (servicesCount % parameters.PageSize > 0)
        {
            pageCount++;
        }

        if (pageCount == 0)
        {
            pageCount = 1;
        }

        PaginationResponse paginationResponse = new()
        {
            CurrentPage = parameters.PageNumber,
            TotalItems = servicesCount,
            TotalPages = pageCount,
            Items = services
        };

        _baseResponse.Data = paginationResponse;
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول على البيانات بنجاح"
            : "The Data Has Been Retrieved Successfully";

        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    [HttpGet("ServiceDetails/{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> ServiceDetails([FromHeader] string lang, int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var service = await _unitOfWork.Services.FindByQuery(
                s => s.IsDeleted == false && s.Id == id)
            .Select(s => new
            {
                s.Id,
                s.TitleAr,
                s.TitleEn,
                s.Description,
                s.Price,
                s.Qty,
                s.ProductType,
                MainSection = new
                {
                    s.MainSection.Id,
                    title = lang == "ar" ? s.MainSection.TitleAr : s.MainSection.TitleEn,
                    s.MainSection.ImgUrl
                }
            }).FirstOrDefaultAsync();

        if (service == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد خدمات " : "Services Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول على البيانات بنجاح"
            : "The Data Has Been Retrieved Successfully";
        _baseResponse.Data = service;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    [HttpPost("AddService")]
    public async Task<ActionResult<BaseResponse>> AddService([FromHeader] string lang, [FromForm] ServiceDto serviceDto)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

     

        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = lang == "ar" ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new
            {
                message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
            };
            return Ok(_baseResponse);
        }

        var mainSection = await _unitOfWork.MainSections.FindByQuery(s => s.Id == serviceDto.MainSectionId)
            .FirstOrDefaultAsync();
        if (mainSection == null)
        {
            _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "القسم الرئيسي غير موجود "
                : "The Main Section Not Exist ";
            return Ok(_baseResponse);
        }

        var service = new Service
        {
            TitleAr = serviceDto.TitleAr,
            TitleEn = serviceDto.TitleEn,
            Description = serviceDto.Description,
            Price = serviceDto.Price,
            MainSectionId = serviceDto.MainSectionId,
            IsDeleted = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            ProductType = serviceDto.ProductType,
        };
        try
        {
            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();
        }
        catch
        {
            _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "خطأ في اضافة الخدمة "
                : "Error In Add Service ";
            return Ok(_baseResponse);
        }



        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم اضافة الخدمة بنجاح"
            : "The Service Has Been Added Successfully";

        return Ok(_baseResponse);
    }

    //-----------------------------------------------------------------------------------------------------

    [HttpPut("UpdateService/{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> UpdateService([FromHeader] string lang, int id,
        [FromForm] ServiceDto serviceDto)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

       

        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = lang == "ar" ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new
            {
                message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
            };
            return Ok(_baseResponse);
        }

        var service = await _unitOfWork.Services.FindByQuery(s =>
                s.Id == id && s.IsDeleted == false)
            .FirstOrDefaultAsync();
        if (service == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ServiceNotFound;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "الخدمة غير موجودة "
                : "The Service Not Exist ";
            return Ok(_baseResponse);
        }

        var mainSection = await _unitOfWork.MainSections.FindByQuery(s => s.Id == serviceDto.MainSectionId)
            .FirstOrDefaultAsync();
        if (mainSection == null)
        {
            _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "القسم الرئيسي غير موجود "
                : "The Main Section Not Exist ";
            return Ok(_baseResponse);
        }

        service.TitleAr = serviceDto.TitleAr;
        service.TitleEn = serviceDto.TitleEn;
        service.Description = serviceDto.Description;
        service.Price = serviceDto.Price;
        service.MainSectionId = serviceDto.MainSectionId;
        service.UpdatedAt = DateTime.Now;
        service.IsDeleted = false;
        service.ProductType = serviceDto.ProductType;
        _unitOfWork.Services.Update(service);
        await _unitOfWork.SaveChangesAsync();


        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم اضافة الخدمة بنجاح"
            : "The Service Has Been Added Successfully";
        /* _baseResponse.Data = newData;*/

        return Ok(_baseResponse);
    }

    //-----------------------------------------------------------------------------------------------------

    [HttpDelete("DeleteService/{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> DeleteService([FromHeader] string lang, int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        if (_user.UserType is UserType.Admin)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب ليس مزود خدمة "
                : "The User Not Provider ";
            return Ok(_baseResponse);
        }
        if (_user.UserType is UserType.User)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب ليس مزود خدمة "
                : "The User Not Provider ";
            return Ok(_baseResponse);
        }

        var service = await _unitOfWork.Services.FindByQuery(s =>
                s.Id == id && s.IsDeleted == false)
            .FirstOrDefaultAsync();
        if (service == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ServiceNotFound;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "الخدمة غير موجودة "
                : "The Service Not Exist ";
            return Ok(_baseResponse);
        }

        service.IsDeleted = true;
        service.UpdatedAt = DateTime.Now;
        service.DeletedAt = DateTime.Now;

        _unitOfWork.Services.Update(service);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم حذف الخدمة بنجاح"
            : "The Service Has Been Deleted Successfully";
        _baseResponse.Data = new { };

        return Ok(_baseResponse);
    }
}
