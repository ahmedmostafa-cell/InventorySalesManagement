using InventorySalesManagement.Extensions;
using InventorySalesManagement.Middleware;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMvc(o => o.EnableEndpointRouting = false);
builder.Services.AddDistributedMemoryCache();

// api Services
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true); // validation Error Api
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// context && json services && IBaseRepository && IUnitOfWork Service
builder.Services.AddContextServices(builder.Configuration);

// Services [IAccountService, IPhotoHandling, AddAutoMapper, Hangfire ,
// Session , SignalR ,[ INotificationService, FcmNotificationSetting, FcmSender,ApnSender ]  ]
builder.Services.AddApplicationServices(builder.Configuration);

// Identity services && JWT
builder.Services.AddIdentityServices(builder.Configuration);

// Swagger Service
builder.Services.AddSwaggerDocumentation();




// cookies services
builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = $"/Account/Login";
	options.LogoutPath = $"/Account/Login";
	options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
		policy =>
		{
			policy.AllowAnyOrigin();
			policy.AllowAnyMethod();
			policy.AllowAnyHeader();
		});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseMiddleware<ExceptionMiddleware>();
	app.UseExceptionHandler("/ErrorsMvc/Index/{0}");
}
app.UseSwaggerDocumentation();
app.UseStatusCodePagesWithReExecute("/Errors/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.UseApplicationMiddleware();

app.MapControllers();
app.UseMvc(routes =>
{
	routes.MapRoute(
		name: "default",
		template: "{controller=Account}/{action=Login}/{id?}");
});
app.Run();
