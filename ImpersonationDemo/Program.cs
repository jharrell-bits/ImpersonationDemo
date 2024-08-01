using ImpersonationDemo.Middleware;
using ImpersonationDemo.Models;
using Microsoft.AspNetCore.Authentication.Negotiate;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages();

// add default HttpClient
builder.Services.AddHttpClient();

// add WindowsAuthentication / Ignore SSL Cert Errors HttpClient
builder.Services.AddHttpClient("WindowsAuthenticationClient")
    .ConfigurePrimaryHttpMessageHandler(
        () => new HttpClientHandler() { 
            UseDefaultCredentials = true,
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) => { return true; }
        }
    );

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationData>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseMiddleware<WindowAuthenticationMiddleware>();

app.Run();
