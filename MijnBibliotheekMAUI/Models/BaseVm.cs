using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MijnBibliotheekMAUI.Models;
/// Basis ViewModel met INotifyPropertyChanged-implementatie
public class BaseVm : INotifyPropertyChanged
{
    bool _isBusy;
    string _error = "";

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string Error
    {
        get => _error;
        set
        {
            if (SetProperty(ref _error, value))
                OnPropertyChanged(nameof(HasError)); 
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(Error); 
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;
        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
