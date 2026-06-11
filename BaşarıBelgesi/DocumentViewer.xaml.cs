using System.Windows;

namespace BaşarıBelgesi
{
    /// <summary>
    /// Interaction logic for DocumentViewer.xaml
    /// </summary>
    public partial class DocumentViewer : Window
    {
        public DocumentViewer() { InitializeComponent(); }

        public RelayCommand<object> OpenFile { get; }
    }
}