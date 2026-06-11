using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace BaşarıBelgesi
{
    public class Kurumlar : InpcBase
    {
        [XmlElement(ElementName = "Kurum")]
        public ObservableCollection<Kurum> Kurum
        {
            get;

            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(Kurum));
                }
            }
        } = [];
    }
}