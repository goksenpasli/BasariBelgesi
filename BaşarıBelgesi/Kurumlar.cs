using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace BaşarıBelgesi
{
    public class Kurumlar : InpcBase
    {
        [XmlElement(ElementName = "Kurum")]
        public ObservableCollection<Kurum> Kurum {
            get => kurum;

            set {
                if (kurum != value)
                {
                    kurum = value;
                    OnPropertyChanged(nameof(Kurum));
                }
            }
        }

        private ObservableCollection<Kurum> kurum = new();
    }
}