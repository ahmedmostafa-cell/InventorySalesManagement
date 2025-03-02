using InventorySalesManagement.Core.DTO;
using InventorySalesManagement.Core.Entity.ApplicationData;
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
public class OrderProvidersController : BaseApiController, IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse;
    private ApplicationUser _user;
    public OrderProvidersController(IUnitOfWork unitOfWork)
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

    //----------------------------------------------------------------------------------------------------
    [HttpGet("GetAllOrders")]
    public async Task<ActionResult<BaseResponse>> GetAllOrders([FromHeader] string lang, [FromQuery] GetAllOrder model)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }


        var orders = await _unitOfWork.Orders.FindByQuery(
                s =>  s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                Service = new
                {
                    s.Service.Id,
                    title = lang == "ar" ? s.Service.TitleAr : s.Service.TitleEn,
                },
            }).ToListAsync();

        if (!orders.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول علي الطلبات بنجاح "
            : "Get Orders Successfully ";
        _baseResponse.Data = orders;
        return Ok(_baseResponse);
    }

    //----------------------------------------------------------------------------------------------------
    [HttpGet("GetOrderById/{id:required:int}")]
    public async Task<ActionResult<BaseResponse>> GetOrderById([FromHeader] string lang, int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var order = await _unitOfWork.Orders.FindByQuery(
                criteria: s => s.Id == id && s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                Service = new
                {
                    s.Service.Id,
                    title = lang == "ar" ? s.Service.TitleAr : s.Service.TitleEn,
                },
            }).FirstOrDefaultAsync();

        if (order == null)
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.Data = order;
        return Ok(_baseResponse);
    }
}
