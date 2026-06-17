using System.Windows;
using System.Windows.Data;

namespace BaşarıBelgesi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BaşarıBelgesiViewModel dc = DataContext as BaşarıBelgesiViewModel;
            _ = dc?.Cvs = TryFindResource("Kurumlar") as CollectionViewSource;
        }
    }
}