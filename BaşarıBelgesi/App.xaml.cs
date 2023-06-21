using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace BaşarıBelgesi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewKeyDownEvent, new KeyEventHandler(KeyDown));
        }

        private static void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox { AcceptsReturn: false })
            {
                MoveToNextUiElement(e);
            }
        }

        private static void MoveToNextUiElement(RoutedEventArgs e)
        {
            TraversalRequest request = new(FocusNavigationDirection.Next);
            if (!(Keyboard.FocusedElement is not UIElement elementWithFocus) && elementWithFocus.MoveFocus(request))
            {
                e.Handled = true;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if !DEBUG
        Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
#endif
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _ = MessageBox.Show(e.Exception.Message);
            e.Handled = true;
        }
    }
}