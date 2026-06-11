using DotLiquid;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace BaşarıBelgesi
{
    public class Kurum : InpcBase, ILiquidizable
    {
        [XmlAttribute(AttributeName = "Adi")]
        public string Adi
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(Adi));
                }
            }
        }

        [XmlAttribute(AttributeName = "AmirAdi")]
        public string AmirAdi
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(AmirAdi));
                }
            }
        }

        [XmlAttribute(AttributeName = "AmirSoyadi")]
        public string AmirSoyadi
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(AmirSoyadi));
                }
            }
        }

        [XmlAttribute(AttributeName = "AmirUnvani")]
        public string AmirUnvani
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(AmirUnvani));
                }
            }
        }

        [XmlElement(ElementName = "Kişi")]
        public ObservableCollection<Kişi> Kişi
        {
            get;

            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(Kişi));
                }
            }
        } = [];

        [XmlAttribute(AttributeName = "Logo")]
        public string Logo
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(Logo));
                }
            }
        }

        public object ToLiquid() => new { Adi, AmirAdi, AmirSoyadi, AmirUnvani, Logo, };
    }
}