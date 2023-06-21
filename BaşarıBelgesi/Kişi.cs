using System.Globalization;
using System.Windows.Media;
using System.Xml.Serialization;
using DotLiquid;

namespace BaşarıBelgesi
{
    public class Kişi : InpcBase, ILiquidizable
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

        [XmlAttribute(AttributeName = "BasariBelgeAciklama")]
        public string BasariBelgeAciklama {
            get => basariBelgeAciklama; set {

                if (basariBelgeAciklama != value)
                {
                    basariBelgeAciklama = value;
                    OnPropertyChanged(nameof(BasariBelgeAciklama));
                }
            }
        }

        [XmlAttribute(AttributeName = "BasariBelgeTipi")]
        public string BasariBelgeTipi {
            get => basariBelgeTipi; set {

                if (basariBelgeTipi != value)
                {
                    basariBelgeTipi = value;
                    OnPropertyChanged(nameof(BasariBelgeTipi));
                }
            }
        }

        [XmlAttribute(AttributeName = "GorevYeri")]
        public string GorevYeri {
            get => gorevYeri; set {

                if (gorevYeri != value)
                {
                    gorevYeri = value;
                    OnPropertyChanged(nameof(GorevYeri));
                }
            }
        }

        [XmlAttribute(AttributeName = "Resim")]
        public string Resim {
            get => resim;

            set {
                if (resim != value)
                {
                    resim = value;
                    OnPropertyChanged(nameof(Resim));
                }
            }
        }

        [XmlIgnore]
        public bool Seçili {
            get => seçili; set {

                if (seçili != value)
                {
                    seçili = value;
                    OnPropertyChanged(nameof(Seçili));
                }
            }
        }

        [XmlAttribute(AttributeName = "Soyadi")]
        public string Soyadi {
            get => soyadi; set {

                if (soyadi != value)
                {
                    soyadi = value;
                    OnPropertyChanged(nameof(Soyadi));
                }
            }
        }

        [XmlAttribute(AttributeName = "Tc")]
        public string TC {
            get => tC;

            set {
                if (tC != value)
                {
                    tC = value;
                    OnPropertyChanged(nameof(TC));
                }
            }
        }

        [XmlAttribute(AttributeName = "Unvan")]
        public string Unvan {
            get => unvan; set {

                if (unvan != value)
                {
                    unvan = value;
                    OnPropertyChanged(nameof(Unvan));
                }
            }
        }

        public object ToLiquid()
        {
            return new {
                Adi,
                Soyadi,
                BasariBelgeAciklama,
                BasariBelgeTipi,
                GorevYeri,
                Resim,
                Unvan,
                TC,
            };
        }

        private string adi;

        private string basariBelgeAciklama;

        private string basariBelgeTipi;

        private string gorevYeri;

        private string resim;

        private bool seçili;

        private string soyadi;

        private string tC;

        private string unvan;
    }
}