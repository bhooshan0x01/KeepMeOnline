using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using KeepMeOnline.ViewModel;

namespace KeepMeOnline.Views;

public partial class AboutDialog : Window
{
    public AboutDialog()
    {
        InitializeComponent();
        DataContext = new AboutViewModel();
    }

    private void OnOkClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
