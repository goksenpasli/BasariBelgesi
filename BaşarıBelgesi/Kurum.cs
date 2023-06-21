using System.Collections.ObjectModel;
using System.Xml.Serialization;
using DotLiquid;

namespace BaşarıBelgesi
{
    public class Kurum : InpcBase, ILiquidizable
    {
        [XmlAttribute(AttributeName = "Adi")]
        public string Adi {
            get => adi; set {

                if (adi != value)
                {
                    adi = value;
                    OnPropertyChanged(nameof(Adi));
                }
            }
        }

        [XmlAttribute(AttributeName = "AmirAdi")]
        public string AmirAdi {
            get => amirAdi; set {

                if (amirAdi != value)
                {
                    amirAdi = value;
                    OnPropertyChanged(nameof(AmirAdi));
                }
            }
        }

        [XmlAttribute(AttributeName = "AmirSoyadi")]
        public string AmirSoyadi {
            get => amirSoyadi; set {

                if (amirSoyadi != value)
                {
                    amirSoyadi = value;
                    OnPropertyChanged(nameof(AmirSoyadi));
                }
            }
        }

        [XmlAttribute(AttributeName = "AmirUnvani")]
        public string AmirUnvani {
            get => amirUnvani; set {

                if (amirUnvani != value)
                {
                    amirUnvani = value;
                    OnPropertyChanged(nameof(AmirUnvani));
                }
            }
        }

        [XmlElement(ElementName = "Kişi")]
        public ObservableCollection<Kişi> Kişi {
            get => kişi;

            set {
                if (kişi != value)
                {
                    kişi = value;
                    OnPropertyChanged(nameof(Kişi));
                }
            }
        }

        [XmlAttribute(AttributeName = "Logo")]
        public string Logo {
            get => logo; set {

                if (logo != value)
                {
                    logo = value;
                    OnPropertyChanged(nameof(Logo));
                }
            }
        }

        public object ToLiquid()
        {
            return new {
                Adi,
                AmirAdi,
                AmirSoyadi,
                AmirUnvani,
                Logo,
            };
        }

        private string adi;

        private string amirAdi;

        private string amirSoyadi;

        private string amirUnvani;

        private ObservableCollection<Kişi> kişi = new();

        private string logo;
    }
}