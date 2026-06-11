using System.Windows.Documents;

namespace BaşarıBelgesi
{
    public class DocumentViewModel
    {
        public string Başlık { get; set; }

        public IDocumentPaginatorSource Document { get; set; }
    }
}