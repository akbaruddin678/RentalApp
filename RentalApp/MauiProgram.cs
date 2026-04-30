using Microsoft.Extensions.Logging;
using RentalApp.ViewModels;
using RentalApp.Database.Data;
using RentalApp.Database.Data.Repositories;
using RentalApp.Views;
using RentalApp.Services;

namespace RentalApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Database
        builder.Services.AddDbContext<AppDbContext>();

        // ── Existing StarterApp Services ──
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // ── Repositories ──
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<IRentalRepository, RentalRepository>();
        builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        // ── New Services ──
        builder.Services.AddScoped<IRentalService, RentalService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<ILocationService, LocationService>();
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<IApiService, ApiService>(sp =>
            new ApiService(sp.GetRequiredService<IHttpClientFactory>().CreateClient()));

        // ── Existing Shell & App ──
        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

        // ── Existing Pages & ViewModels ──
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<UserListViewModel>();
        builder.Services.AddTransient<UserListPage>();
        builder.Services.AddTransient<UserDetailPage>();
        builder.Services.AddTransient<UserDetailViewModel>();
        builder.Services.AddSingleton<TempViewModel>();
        builder.Services.AddTransient<TempPage>();

        // ── New RentalApp Pages & ViewModels ──
        builder.Services.AddTransient<ItemsListViewModel>();
        builder.Services.AddTransient<ItemsListPage>();
        builder.Services.AddTransient<ItemDetailViewModel>();
        builder.Services.AddTransient<ItemDetailPage>();
        builder.Services.AddTransient<CreateItemViewModel>();
        builder.Services.AddTransient<CreateItemPage>();
        builder.Services.AddTransient<NearbyItemsViewModel>();
        builder.Services.AddTransient<NearbyItemsPage>();
        builder.Services.AddTransient<RentalsViewModel>();
        builder.Services.AddTransient<RentalsPage>();
        builder.Services.AddTransient<ReviewsViewModel>();
        builder.Services.AddTransient<ReviewsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
