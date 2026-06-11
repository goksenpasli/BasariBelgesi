using DotLiquid;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace BaşarıBelgesi
{
    public class Kişi : InpcBase, ILiquidizable, IDataErrorInfo
    {
        public Kişi() { PropertyChanged += Kişi_PropertyChanged; }

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

        [XmlAttribute(AttributeName = "BasariBelgeAciklama")]
        public string BasariBelgeAciklama
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(BasariBelgeAciklama));
                }
            }
        }

        [XmlAttribute(AttributeName = "BasariBelgeTipi")]
        public string BasariBelgeTipi
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(BasariBelgeTipi));
                    OnPropertyChanged(nameof(BasariBelgeAciklama));
                }
            }
        }

        public string Error => string.Empty;

        [XmlAttribute(AttributeName = "EvrakSayı")]
        public int EvrakSayı
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(EvrakSayı));
                }
            }
        }

        [XmlAttribute(AttributeName = "EvrakTarih")]
        public DateTime EvrakTarih
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(EvrakTarih));
                }
            }
        } = DateTime.Today;

        [XmlAttribute(AttributeName = "Gerekçe")]
        public string Gerekçe
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(Gerekçe));
                }
            }
        } = "Kamu kaynağında önemli ölçüde tasarruf sağlanması Sunulan hizmetlerin etkinlik ve kalitesinin yükseltilmesi";

        [XmlAttribute(AttributeName = "GorevYeri")]
        public string GorevYeri
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(GorevYeri));
                }
            }
        }

        [XmlAttribute(AttributeName = "Resim")]
        public string Resim
        {
            get;

            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(Resim));
                }
            }
        }

        [XmlIgnore]
        public bool Seçili
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(Seçili));
                }
            }
        }

        [XmlAttribute(AttributeName = "Soyadi")]
        public string Soyadi
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(Soyadi));
                }
            }
        }

        [XmlAttribute(AttributeName = "Tc")]
        public string TC
        {
            get;

            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(TC));
                }
            }
        }

        [XmlAttribute(AttributeName = "Unvan")]
        public string Unvan
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
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
            return k10 == tcKimlikNo[9].ToString() && k11 == tcKimlikNo[10].ToString() ? ValidationResult.ValidResult : new ValidationResult(false, "Girilen TC Kimlik No Yanlıştır.");
        }

        public object ToLiquid() => new { Adi, Soyadi, BasariBelgeAciklama, BasariBelgeTipi, GorevYeri, Resim, Unvan, TC, Gerekçe, EvrakSayı, EvrakTarih };

        private void Kişi_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BasariBelgeTipi))
            {
                BasariBelgeAciklama = $"Görevli olduğunuz kurumda üstün görev ve sorumluluk anlayışıyla görevinizi ifa etmeniz, kendi sorumluluklarınızdaki iş ve işlemleri titizlikle takip ederek, kamu hizmetlerinin hızlı ve vatandaş memnuniyetini önde tutarak yürütülmesi yönündeki başarılı çalışmalarınızdan dolayı sizi 657 sayılı Devlet Memurları Kanunu’ nun 122. Maddesi uyarınca {BasariBelgeTipi} ile taltif eder, başarılı çalışmalarınızın devamını dilerim.";
            }
        }
    }
}