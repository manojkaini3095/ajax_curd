using ajax_curd.Context;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureKeyVault;

string GetKeyVaultEndpoint() => "https://studentkeyvault.vault.azure.net/";

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = GetKeyVaultEndpoint();
if (!string.IsNullOrEmpty(keyVaultEndpoint))
{
    var azureServiceTokenProvider = new AzureServiceTokenProvider();
    var keyVaultClient = new KeyVaultClient(
        new KeyVaultClient.AuthenticationCallback(
            azureServiceTokenProvider.KeyVaultTokenCallback));
    builder.Configuration.AddAzureKeyVault(
        keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
}

var connection = builder.Configuration["StudentDatabase"];
var triggerUri = builder.Configuration["TriggerUri"];
builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseSqlServer(connection));

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
