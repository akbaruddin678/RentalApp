using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class RentalsPage : ContentPage
{
    public RentalsPage(RentalsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RentalsViewModel vm)
            vm.LoadRentalsCommand.Execute(null);
    }

    private void OnIncomingTabClicked(object sender, EventArgs e)
    {
        IncomingView.IsVisible = true;
        OutgoingView.IsVisible = false;
    }

    private void OnOutgoingTabClicked(object sender, EventArgs e)
    {
        IncomingView.IsVisible = false;
        OutgoingView.IsVisible = true;
    }
}
