using RentalApp.ViewModels;

namespace RentalApp;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        // Existing routes
        Routing.RegisterRoute(nameof(Views.MainPage), typeof(Views.MainPage));
        Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
        Routing.RegisterRoute(nameof(Views.RegisterPage), typeof(Views.RegisterPage));
        Routing.RegisterRoute(nameof(Views.UserListPage), typeof(Views.UserListPage));
        Routing.RegisterRoute(nameof(Views.UserDetailPage), typeof(Views.UserDetailPage));
        Routing.RegisterRoute(nameof(Views.TempPage), typeof(Views.TempPage));
        // New routes
        Routing.RegisterRoute("ItemsListPage", typeof(Views.ItemsListPage));
        Routing.RegisterRoute("ItemDetailPage", typeof(Views.ItemDetailPage));
        Routing.RegisterRoute("CreateItemPage", typeof(Views.CreateItemPage));
        Routing.RegisterRoute("NearbyItemsPage", typeof(Views.NearbyItemsPage));
        Routing.RegisterRoute("RentalsPage", typeof(Views.RentalsPage));
        Routing.RegisterRoute("ReviewsPage", typeof(Views.ReviewsPage));
    }
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = _serviceProvider.GetService<AppShell>();
        if (shell is null) throw new InvalidOperationException("AppShell could not be resolved.");
        return new Window(shell);
    }
}
