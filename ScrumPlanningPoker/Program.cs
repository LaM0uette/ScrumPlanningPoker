using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;
using ScrumPlanningPoker.Components;
using ScrumPlanningPoker.Hubs;
using ScrumPlanningPoker.Services;
using ScrumPlanningPoker.Services.HubServices;
using Syncfusion.Licensing;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddServerSideBlazor();

// Singleton
builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<HubService>();

// Components
builder.Services.AddMudServices();

SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cWWBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWH5ed3RXRGRYVUF1X0A=");
builder.Services.AddSyncfusionBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// SignalR
app.MapHub<SessionRoomHub>("/session-room-hub");

app.Run();