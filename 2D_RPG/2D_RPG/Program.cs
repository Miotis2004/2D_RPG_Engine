using _2D_RPG.Components;
using _2D_RPG.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<EditorStateService>();
builder.Services.AddSingleton<TileMapService>();
builder.Services.AddSingleton<MapObjectService>();
builder.Services.AddSingleton<ProjectValidationService>();
builder.Services.AddSingleton<AssetCatalogService>();
builder.Services.AddSingleton<RuntimeEngineService>();
builder.Services.AddSingleton<CombatService>();
builder.Services.AddSingleton<AnimationService>();
builder.Services.AddSingleton<MenuService>();
builder.Services.AddSingleton<ProjectPersistenceService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
