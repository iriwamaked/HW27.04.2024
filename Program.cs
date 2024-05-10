using ASP1.Data.Context;
using ASP1.Data.Dal;
using ASP1.MiddleWare;
using ASP1.Services.Hash;
using ASP1.Services.Kdf;
using ASP1.Services.Upload;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//��������� ������ - ���� ��������� �� ���������� build.Services
//builder.Services.AddSingleton<IHashService, Md5HashService>();
/*��� �������� DIP �������������� "������" �� ����������� �� ��������������. ���� ����������
 ����� ������ � ����� �����: "���� ���� ����� �� �������� �� IHashService, �� ������ ���� Md5HashService"*/
builder.Services.AddSingleton<IHashService, ShaHashService>();
//
builder.Services.AddSingleton<IKdfService, Pdkdf1Service>();



//�������� �������� �����, � ����� �������� ���� ������������
builder.Services.AddDbContext<DataContext>(options=>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MsSql") ),
        ServiceLifetime.Singleton
   );

builder.Services.AddSingleton<DataAccessor>();

builder.Services.AddSingleton<IUploadService, UploadServiceV1>();
//��������� Http-������
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    //������� ��������� - ����� ���������������, �.�. ���� � ������� 10 ������ �� ������ �� ������ � ������, ������ ����������
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//��������� Http-������
app.UseSession();

//��� MiddleWare ��� ��������������  ����� ������
app.UseSessionAuth();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
