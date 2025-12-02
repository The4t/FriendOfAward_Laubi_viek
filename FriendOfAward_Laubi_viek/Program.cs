using FriendOfAward_Laubi_viek.Components;
using QRCoder;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


var qrList = new Queue<string>(
    Enumerable.Range(0, 100).Select(_ => Guid.NewGuid().ToString("N"))
);

app.MapGet("/api/qr/next", () =>
{
    if (qrList.Count == 0)
        return Results.NotFound("Keine QR-Codes mehr");

    var token = qrList.Dequeue();

    // QR erstellen
    var qrGen = new QRCodeGenerator();
    var data = qrGen.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
    var qr = new PngByteQRCode(data);
    var bytes = qr.GetGraphic(20);

    string base64 = Convert.ToBase64String(bytes);

    return Results.Text(base64);
});