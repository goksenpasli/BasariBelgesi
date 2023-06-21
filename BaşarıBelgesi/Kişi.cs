using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Serialization;
using DotLiquid;

namespace BaşarıBelgesi
{
    public class Kişi : InpcBase, ILiquidizable, IDataErrorInfo
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

        public string Error => string.Empty;

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

        public string this[string columnName] => columnName switch
        {
            "TC" when !TcDoğrula(TC).IsValid => TcDoğrula(TC).ErrorContent.ToString(),
            _ => null
        };

        public ValidationResult TcDoğrula(object value)
        {
            int tekler = 0;
            int ciftler = 0;
            if (value is not string tcKimlikNo)
            {
                return new ValidationResult(false, "TC Kimlik No Boş Olamaz.");
            }

            if (tcKimlikNo.Length != 11)
            {
                return new ValidationResult(false, "TC Kimlik No 11 Hanelidir.");
            }

            if (!tcKimlikNo.All(char.IsNumber))
            {
                return new ValidationResult(false, "TC Kimlik No Sadece Rakamlardan Oluşur.");
            }

            if (tcKimlikNo.Substring(0, 1) == "0")
            {
                return new ValidationResult(false, "TC Kimlik İlk Hanesi 0 Olamaz.");
            }

            for (int i = 0; i < 9; i += 2)
            {
                tekler += int.Parse(tcKimlikNo[i].ToString());
            }

            for (int i = 1; i < 8; i += 2)
            {
                ciftler += int.Parse(tcKimlikNo[i].ToString());
            }

            string k10 = (((tekler * 7) - ciftler) % 10).ToString();
            string k11 = ((tekler + ciftler + int.Parse(tcKimlikNo[9].ToString())) % 10).ToString();
            return k10 == tcKimlikNo[9].ToString() && k11 == tcKimlikNo[10].ToString()
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "Girilen TC Kimlik No Yanlıştır.");
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