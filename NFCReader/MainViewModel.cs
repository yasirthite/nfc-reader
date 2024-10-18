using CommunityToolkit.Mvvm.ComponentModel;

namespace NFCReader;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] 
    private string _nfcTag;

    [ObservableProperty] 
    private string _message;
}