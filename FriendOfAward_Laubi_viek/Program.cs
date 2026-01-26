using FriendOfAward_Laubi_viek.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<AuthServiceSimple>();
builder.Services.AddSingleton<QrServiceToken>();

// QR‑Liste als Singleton
builder.Services.AddSingleton<Queue<string>>(
    new Queue<string>(Enumerable.Range(0, 100)
        .Select(_ => Guid.NewGuid().ToString("N")))
);

var app = builder.Build();


app.UseStaticFiles();
app.UseAntiforgery();
app.UseSession();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
