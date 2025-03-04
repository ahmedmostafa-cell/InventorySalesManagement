using InventorySalesManagement.BusinessLayer.Interfaces;
using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Helpers;
using InventorySalesManagement.Core.ModelView.AuthViewModel.LoginData;
using InventorySalesManagement.Core.ModelView.AuthViewModel.RegisterData;
using InventorySalesManagement.Core.ModelView.AuthViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventorySalesManagement.BusinessLayer.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Jwt _jwt;

    public AccountService(UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IOptions<Jwt> jwt)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _jwt = jwt.Value;
    }

    public async Task<List<ApplicationUser>> GetAllUsers()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<ApplicationUser> GetUserById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return null;
        return user;
    }

    public async Task<ApplicationUser> GetUserByPhoneNumber(string phoneNumber)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        return user;
    }

    public async Task<ApplicationUser> GetUserByEmail(string email)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email);
        return user;
    }

    public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
    {
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return user;
        }
        else
        {
            return null;
        }
    }

    public async Task<AuthModel> LoginAsync(LoginModel model)
    {
        var userByPhone = await _userManager.FindByNameAsync(model.PhoneNumber);
        var userByEmail = await _userManager.FindByEmailAsync(model.PhoneNumber);

        // Prioritize userByPhone if found, otherwise use userByEmail
        var user = userByPhone ?? userByEmail;

        if (user is null)
        {
            return new AuthModel
            {
                Message = "Your username does not exist!",
                ArMessage = "اسم المستخدم غير مسجل",
                ErrorCode = (int)Errors.ThisPhoneNumberNotExist
            };
        }

        if (!await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return new AuthModel
            {
                Message = "Password is incorrect!",
                ArMessage = "كلمة المرور غير صحيحة",
                ErrorCode = (int)Errors.TheUsernameOrPasswordIsIncorrect
            };
        }

        return new AuthModel
        {
            UserId = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FullName = user.FullName,
            IsAuthenticated = true,
            UserType = user.UserType,
            IsAdmin = user.IsAdmin,
            IsApproved = user.IsApproved,
            Token = new JwtSecurityTokenHandler().WriteToken(GenerateJwtToken(user).Result),
            Message = "Login successful",
            ArMessage = "تم تسجيل الدخول بنجاح"
        };
    }


    public async Task<bool> Logout(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null)
            return false;
        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<AuthModel> GetUserInfo(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new AuthModel { ErrorCode = (int)Errors.TheUserNotExistOrDeleted, Message = "User not found!", ArMessage = "المستخدم غير موجود" };

        var rolesList = await _userManager.GetRolesAsync(user);
        var result = new AuthModel
        {
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FullName = user.FullName,
            IsAuthenticated = true,
            UserType = user.UserType,
            IsAdmin = user.IsAdmin,
            IsApproved = user.IsApproved,
        };
        return result;
    }

	public async Task<AuthModel> RegisterAdminAsync(RegisterAdminMv model)
	{
		if (await _userManager.FindByEmailAsync(model.Email) is not null)
			return new AuthModel { Message = "this email is already Exist!", ArMessage = "هذا البريد الالكتروني مستخدم من قبل", ErrorCode = (int)Errors.ThisEmailAlreadyExist };

		if (await Task.Run(() => _userManager.Users.Any(item => item.PhoneNumber == model.PhoneNumber)))
			return new AuthModel { Message = "this phone number is already Exist!", ArMessage = "هذا الرقم المحمول مستخدم من قبل", ErrorCode = (int)Errors.ThisPhoneNumberAlreadyExist };


        var user = new ApplicationUser
        {
            FullName = model.FullName,
            UserName = model.PhoneNumber,
            NormalizedUserName = model.PhoneNumber,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            Status = true,
            PhoneNumberConfirmed = true,
            UserType = UserType.Admin,
            IsAdmin = true,
            IsApproved = true
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors =
                result.Errors.Aggregate(string.Empty, (current, error) => current + $"{error.Description},");
            return new AuthModel { Message = errors, ArMessage = errors, ErrorCode = (int)Errors.ErrorWithCreateAccount };
        }
        else
        {
            return new AuthModel { Message = "Registered successfully", ArMessage = "تم تسجيل الدخول بنجاح" };
        }
    }

	#region create and validate JWT token

	private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user, int? time = null)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim("roles", role)).ToList();
        var userType = user.UserType.ToString();

        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id),
                new Claim("Name", user.FullName),
                new Claim("userType",userType),
                (user.IsAdmin) ? new Claim("isAdmin", "true") : new Claim("isAdmin", "false"),
            }
            .Union(userClaims)
            .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: (time != null) ? DateTime.Now.AddHours((double)time) : DateTime.Now.AddDays(_jwt.DurationInDays),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }


    public string ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            if (token == null)
                return null;
            if (token.StartsWith("Bearer "))
                token = token.Replace("Bearer ", "");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var accountId = jwtToken.Claims.First(x => x.Type == "uid").Value;

            return accountId;
        }
        catch
        {
            return null;
        }
    }

    #endregion create and validate JWT token

}
