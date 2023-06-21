using System.Diagnostics;
using System.IO;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace BaşarıBelgesi
{
    public class DocumentViewModel
    {
        public DocumentViewModel()
        {
            OpenFile = new RelayCommand<object>(parameter =>
            {
                string file = Path.GetTempPath() + "output.xps";
                using XpsDocument xpsDoc = new(file, FileAccess.Write);
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                writer.Write(Document);
                _ = Process.Start(file);
            }, parameter => Document is not null);
        }

        public string Başlık { get; set; }

        public FixedDocumentSequence Document { get; set; }

        public RelayCommand<object> OpenFile { get; }
    }
}