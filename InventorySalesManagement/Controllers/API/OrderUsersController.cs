using InventorySalesManagement.Core.DTO;
using InventorySalesManagement.Core.DTO.EntityDto;
using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Entity.OrderData;
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
public class OrderUsersController : BaseApiController, IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ApplicationUser _user;

    public OrderUsersController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _baseResponse = new BaseResponse();
        _httpContextAccessor = httpContextAccessor;
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

    //--------------------------------------------------------------------------------------------------------
    // Create order 
    [HttpPost("AddOrder")]
    public async Task<ActionResult<BaseResponse>> AddOrder([FromHeader] string lang, [FromBody] OrderDto orderDto)
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
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new
            {
                message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
            };
            return Ok(_baseResponse);
        }

      
      
        var order = new Order
        {
            CreatedOn = DateTime.Now,
            IsDeleted = false,
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم اضافة الطلب بنجاح "
            : "Add Order Successfully ";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------------
    // Remove order
    [HttpPost("RemoveOrder/{orderId:required:int}")]
    public async Task<ActionResult<BaseResponse>> RemoveOrder([FromHeader] string lang, int orderId)
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
                criteria: s => s.Id == orderId &&
                               s.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheOrderNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الطلب غير موجود "
                : "The Order Not Exist ";
            return Ok(_baseResponse);
        }

        _unitOfWork.Orders.Delete(order);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم حذف الطلب بنجاح "
            : "delete Order Successfully ";
        return Ok(_baseResponse);
    }
}
