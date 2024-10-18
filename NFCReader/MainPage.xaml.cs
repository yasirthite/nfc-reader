using System.Diagnostics;
using Plugin.NFC;

namespace NFCReader;

public partial class MainPage : ContentPage
{

    private bool _areEventsSubscribed = false;
    
    public MainPage()
    {
        InitializeComponent();

        BindingContext = new MainViewModel();
    }

    protected override async void OnAppearing()
    {
        CrossNFC.Legacy = false;
        
        if (CrossNFC.IsSupported)
        {
            await AutoStartAsync().ConfigureAwait(false);
        }
        else
        {
            (BindingContext as MainViewModel).Message = "NFC not available";
            _messageLabel.TextColor = Colors.Red;
        }
            
        base.OnAppearing();
    }

    private async Task AutoStartAsync()
    {
        await Task.Delay(500);
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SubscribeEvents();
                CrossNFC.Current.StartListening();
            });
            
            (BindingContext as MainViewModel).Message = "Ready to Scan!";
            _messageLabel.TextColor = Colors.LimeGreen;

            if (!CrossNFC.Current.IsEnabled)
            {
                (BindingContext as MainViewModel).Message = "NFC not enabled";
                _messageLabel.TextColor = Colors.Red;
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.Message);
        }
    }

    private void CurrentOnOnMessageReceived(ITagInfo? tagInfo)
    {
        if (tagInfo is null)
        {
            Debug.WriteLine("No tag found");
            return;
        }

        // Customized serial number
        byte[]? identifier = tagInfo.Identifier;
       
        (BindingContext as MainViewModel)!.NfcTag = NFCUtils.ByteArrayToHexString(identifier, ":");
    }
    
    private void SubscribeEvents()
    {
        if (_areEventsSubscribed)
        {
            UnsubscribeEvents();
        }

        CrossNFC.Current.OnMessageReceived += CurrentOnOnMessageReceived;
        _areEventsSubscribed = true;
    }

    private void UnsubscribeEvents()
    {
        CrossNFC.Current.OnMessageReceived -= CurrentOnOnMessageReceived;
    }
}