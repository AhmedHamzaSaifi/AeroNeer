using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Aeroneer.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        CompanyBox.Focus();
        Keyboard.Focus(CompanyBox);
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || e.OriginalSource is not TextBox)
        {
            return;
        }

        e.Handled = true;
        var request = new TraversalRequest(FocusNavigationDirection.Next);
        ((UIElement)e.OriginalSource).MoveFocus(request);
    }
}
