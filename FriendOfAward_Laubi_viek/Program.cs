using FriendOfAward_Laubi_viek;
using FriendOfAward_Laubi_viek.Components;
using Microsoft.AspNetCore.Components.Authorization;
using QRCoder;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// SERVICES
// -------------------------

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<AuthServiceSimple>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// QR‑Liste als Singleton
builder.Services.AddSingleton<Queue<string>>(
    new Queue<string>(Enumerable.Range(0, 100)
        .Select(_ => Guid.NewGuid().ToString("N")))
);

var app = builder.Build();

// -------------------------
// PIPELINE
// -------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseStaticFiles();
app.UseAntiforgery();
app.UseSession();

// Razor Components (App.razor)
app.MapRazorComponents<App.App>()
    .AddInteractiveServerRenderMode();

// QR‑API
app.MapGet("/api/qr/next", (Queue<string> qrList) =>
{
    if (qrList.Count == 0)
        return Results.NotFound("Keine QR-Codes mehr");

    var token = qrList.Dequeue();

    var qrGen = new QRCodeGenerator();
    var data = qrGen.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
    var qr = new PngByteQRCode(data);
    var bytes = qr.GetGraphic(20);

    string base64 = Convert.ToBase64String(bytes);

    return Results.Text(base64);
});

app.Run();