using InventorySalesManagement.Core.DTO;
using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Helpers;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;
using InventorySalesManagement.BusinessLayer.Interfaces;
using InventorySalesManagement.Core.ModelView.AuthViewModel.RegisterData;
using InventorySalesManagement.Core.ModelView.AuthViewModel.LoginData;

namespace InventorySalesManagement.Controllers.API;

public class UsersController : BaseApiController
{

    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse = new();

    public UsersController(IUnitOfWork unitOfWork, IAccountService accountService)
    {
        _accountService = accountService;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("CenterRegister")]
    public async Task<ActionResult<BaseResponse>> CenterRegister(RegisterCenterVm model, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.RegisterCenterAsync(model);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;

        }
        else
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = new { result.FullName, result.PhoneNumber };
        }
        return Ok(_baseResponse);

    }


    //-------------------------------------------------------------------------------------------- login Api 
    [HttpPost("login")]
    public async Task<ActionResult<BaseResponse>> LoginAsync([FromBody] LoginModel model, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.LoginAsync(model);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.Data = model;
            return Ok(_baseResponse);
        }
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم تسجيل الدخول" : "Login Successfully";
        _baseResponse.Data = new
        {
            result.UserId,
            result.Email,
            result.FullName,
            result.Token,
            Role = result.Roles,
            result.PhoneNumber,
            result.UserType
        };
        return Ok(_baseResponse);
    }

    //-------------------------------------------------------------------------------------------- logout Api 
    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<BaseResponse>> LogoutAsync([FromHeader] string lang)
    {
        //var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var userName = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userName
        if (!string.IsNullOrEmpty(userName))
        {
            var result = await _accountService.Logout(userName);
            if (result)
            {
                _baseResponse.ErrorCode = 0;
                _baseResponse.ErrorMessage = (lang == "ar") ? "تم تسجيل الخروج بنجاح " : "Signed out successfully";
                return Ok(_baseResponse);
            }
        }
        _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
        _baseResponse.ErrorMessage = (lang == "ar") ? "هذا الحساب غير موجود " : "The User Not Exist";
        return Ok(_baseResponse);
    }


    //----------------------------------------------------------------------------------------------------- get profile
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("GetUserInfo")]
    public async Task<ActionResult<BaseResponse>> GetUserInfo([FromHeader] string lang)
    {
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        if (string.IsNullOrEmpty(userId))
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar") ? "المستخدم غير موجود" : "User not exist";
            _baseResponse.Data = null;
            return Ok(_baseResponse);
        }
        var result = await _accountService.GetUserInfo(userId);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = result;
            return Ok(_baseResponse);
        }




        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new
        {
            result.Email,
            result.FullName,
            result.PhoneNumber,
            Role = result.Roles,
            result.UserType,
            result.IsApproved,
        };
        return Ok(_baseResponse);
    }



}
