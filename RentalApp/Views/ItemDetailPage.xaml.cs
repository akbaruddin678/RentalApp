using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class ItemDetailPage : ContentPage
{
    public ItemDetailPage(ItemDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnViewReviewsClicked(object sender, EventArgs e)
    {
        if (BindingContext is ItemDetailViewModel vm && vm.Item is not null)
            await Shell.Current.GoToAsync($"ReviewsPage?itemId={vm.Item.Id}");
    }
}
