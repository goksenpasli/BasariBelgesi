using System.ComponentModel;
using System.Windows.Documents;

namespace BaşarıBelgesi
{
    public class DocumentViewModel 
    {
        public string Başlık { get; set; }

        public FixedDocumentSequence Document { get; set; }
    }
}