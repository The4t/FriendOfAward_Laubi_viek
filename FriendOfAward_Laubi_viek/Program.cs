using FriendOfAward_Laubi_viek;
using FriendOfAward_Laubi_viek.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddSingleton<AuthServiceSimple>();
builder.Services.AddSingleton<QrServiceToken>();


builder.Services.AddSingleton<DbWrapperMySqlV2>(DbWrapperMySqlV2.Wrapper);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseSession();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
