namespace MusicMan.Mobile;

public partial class MainPage : ContentPage
{
    private readonly Services.CollectionApiClient _api;

    public MainPage(Services.CollectionApiClient api)
    {
        InitializeComponent();
        _api = api;
    }

    private async void Submit_Clicked(object sender, EventArgs e)
    {
        var text = BarcodeEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            await DisplayAlert("Missing barcode", "Please enter a barcode.", "OK");
            return;
        }

        StatusLabel.Text = $"Submitting {text}...";
        try
        {
            var ok = await _api.AddByBarcodeAsync(text);
            StatusLabel.Text = ok ? $"Added by barcode: {text}" : $"Failed to add: {text}";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error: {ex.Message}";
        }
    }
}
