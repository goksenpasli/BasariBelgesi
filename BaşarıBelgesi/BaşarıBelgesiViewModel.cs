using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using DotLiquid;
using Microsoft.Win32;
using dotTemplate = DotLiquid.Template;

namespace BaşarıBelgesi
{
    public class BaşarıBelgesiViewModel : InpcBase
    {
        public BaşarıBelgesiViewModel()
        {
            XmlDataPath = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath) + @"\Data.xml";
            Kişi = new Kişi();
            Kurum = new Kurum();
            DocumentViewModel = new DocumentViewModel();
            Kurumlar = new Kurumlar() { Kurum = DataYükle() };

            AddKişi = new RelayCommand<object>(parameter =>
            {
                Kişi kişi = new()
                {
                    Adi = Kişi.Adi,
                    Soyadi = Kişi.Soyadi,
                    BasariBelgeAciklama = Kişi.BasariBelgeAciklama,
                    BasariBelgeTipi = Kişi.BasariBelgeTipi,
                    GorevYeri = Kişi.GorevYeri,
                    Unvan = Kişi.Unvan,
                    TC = Kişi.TC
                };
                SeçiliKurum.Kişi.Add(kişi);
                Kurumlar.Serialize();
            }, parameter => SeçiliKurum is not null &&
            !string.IsNullOrWhiteSpace(Kişi.Adi) && !string.IsNullOrWhiteSpace(Kişi.TC) &&
             !string.IsNullOrWhiteSpace(Kişi.BasariBelgeAciklama) &&
              !string.IsNullOrWhiteSpace(Kişi.BasariBelgeTipi) &&
               !string.IsNullOrWhiteSpace(Kişi.GorevYeri) &&
               !string.IsNullOrWhiteSpace(Kişi.Unvan) &&
            !string.IsNullOrWhiteSpace(Kişi.Soyadi));

            AddKurum = new RelayCommand<object>(parameter =>
            {
                Kurum kurum = new()
                {
                    Adi = Kurum.Adi,
                    AmirAdi = Kurum.AmirAdi,
                    AmirSoyadi = Kurum.AmirSoyadi,
                    AmirUnvani = Kurum.AmirUnvani,
                };
                Kurumlar.Kurum.Add(kurum);
                Kurumlar.Serialize();
            }, parameter => !string.IsNullOrWhiteSpace(Kurum.Adi) && !string.IsNullOrWhiteSpace(Kurum.AmirAdi) && !string.IsNullOrWhiteSpace(Kurum.AmirUnvani));

            AddLogo = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Resim Dosyası (*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.gif;*.tif;*.tiff;*.bmp;*.dib;*.rle;)|*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.gif;*.tif;*.tiff;*.bmp;*.dib;*.rle;",
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    SeçiliKurum.Logo = SetImage(openFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
                Kurumlar.Serialize();
            }, parameter => SeçiliKurum is not null);

            AddKişiLogo = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Resim Dosyası (*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.gif;*.tif;*.tiff;*.bmp;*.dib;*.rle;)|*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.gif;*.tif;*.tiff;*.bmp;*.dib;*.rle;",
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    SeçiKişi.Resim = SetImage(openFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                Kurumlar.Serialize();
            }, parameter => SeçiKişi is not null);

            Sakla = new RelayCommand<object>(parameter => Kurumlar.Serialize(), parameter => true);

            KişiTümünüSeç = new RelayCommand<object>(parameter =>
            {
                foreach (Kişi item in SeçiliKurum.Kişi)
                {
                    item.Seçili = TümüSeçili;
                }
            }, parameter => SeçiliKurum is not null);

            BaşarıBelgesiÇıkar = new RelayCommand<object>(parameter =>
            {
                if (parameter is DataGrid dataGrid && HasValidationErrors(dataGrid))
                {
                    _ = MessageBox.Show("Hatalı Kayıtlar Vardır.");
                }
                if (File.Exists("report.lqd"))
                {
                    IEnumerable<Kişi> data = SeçiliKurum?.Kişi.Where(z => z.Seçili);
                    Kurum kurum = SeçiliKurum;
                    if (data != null)
                    {
                        Hash günlükrapor = Hash.FromAnonymousObject(new {
                            Kişi = data,
                            Kurum = kurum,
                        });
                        string template = GenerateTemplate(günlükrapor, "report.lqd");
                        FlowDocument fd = (FlowDocument)XamlReader.Parse(template);
                        DocumentViewModel.Document = WriteXPS(fd);
                        DocumentViewModel.Başlık = "RAPOR";
                        DocumentViewer t = new()
                        {
                            Owner = Application.Current.MainWindow,
                            DataContext = DocumentViewModel
                        };
                        t.Show();
                    }
                }
            }, parameter => SeçiliKurum?.Kişi.Any(z => z.Seçili) == true);

            ŞablonGöster = new RelayCommand<object>(parameter =>
            {
                CultureInfo culture = new(CultureInfo.CurrentCulture.Name);
                string seperator = culture.TextInfo.ListSeparator;
                string metin = $"ADI{seperator}SOYADI{seperator}UNVAN{seperator}BAŞARI BELGE TİPİ{seperator}BAŞARI BELGE AÇIKLAMA{seperator}GÖREV YERİ{seperator}TC";
                WriteCsvFile(metin);
            }, parameter => true);

            ExceldenAl = new RelayCommand<object>(parameter =>
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Microsoft Excel Virgülle Ayrılmış Değerler Dosyası (*.csv)|*.csv",
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (Kişi item in CSVKişiler(openFileDialog.FileName))
                    {
                        Kişi kişi = new()
                        {
                            Adi = item.Adi,
                            Soyadi = item.Soyadi,
                            BasariBelgeAciklama = item.BasariBelgeAciklama,
                            BasariBelgeTipi = item.BasariBelgeTipi,
                            GorevYeri = item.GorevYeri,
                            Unvan = item.Unvan
                        };
                        SeçiliKurum.Kişi.Add(kişi);
                    }
                    Kurumlar.Serialize();
                }
            }, parameter => SeçiliKurum is not null);
        }

        public static string XmlDataPath { get; set; }

        public RelayCommand<object> AddKişi { get; }

        public RelayCommand<object> AddKişiLogo { get; }

        public RelayCommand<object> AddKurum { get; }

        public RelayCommand<object> AddLogo { get; }

        public RelayCommand<object> BaşarıBelgesiÇıkar { get; }

        public DocumentViewModel DocumentViewModel { get; }

        public RelayCommand<object> ExceldenAl { get; }

        public Kişi Kişi { get; set; }

        public RelayCommand<object> KişiTümünüSeç { get; }

        public Kurum Kurum { get; set; }

        public Kurumlar Kurumlar { get; set; }

        public RelayCommand<object> Sakla { get; }

        public Kişi SeçiKişi {
            get => seçiKişi;

            set {
                if (seçiKişi != value)
                {
                    seçiKişi = value;
                    OnPropertyChanged(nameof(SeçiKişi));
                }
            }
        }

        public Kurum SeçiliKurum {
            get => seçiliKurum;

            set {
                if (seçiliKurum != value)
                {
                    seçiliKurum = value;
                    OnPropertyChanged(nameof(SeçiliKurum));
                }
            }
        }

        public RelayCommand<object> ŞablonGöster { get; }

        public bool TümüSeçili {
            get => tümüSeçili; set {

                if (tümüSeçili != value)
                {
                    tümüSeçili = value;
                    OnPropertyChanged(nameof(TümüSeçili));
                }
            }
        }

        public ObservableCollection<Kurum> DataYükle()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    return null;
                }

                if (File.Exists(XmlDataPath))
                {
                    return XmlDataPath.DeSerialize<Kurumlar>().Kurum;
                }

                _ = Directory.CreateDirectory(Path.GetDirectoryName(XmlDataPath));
                return new ObservableCollection<Kurum>();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, Application.Current?.MainWindow?.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public FixedDocumentSequence WriteXPS(FlowDocument flowDocument)
        {
            Package package = Package.Open(new MemoryStream(), FileMode.Create, FileAccess.ReadWrite);
            Uri packUri = new("pack://temp.xps");
            PackageStore.RemovePackage(packUri);
            PackageStore.AddPackage(packUri, package);
            using XpsDocument xpsDocument = new(package, CompressionOption.SuperFast, packUri.ToString());
            DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
            using (XpsSerializationManager xpsSerializationManager = new(new XpsPackagingPolicy(xpsDocument), false))
            {
                xpsSerializationManager.SaveAsXaml(paginator);
            }
            return xpsDocument.GetFixedDocumentSequence();
        }

        private IEnumerable<Kişi> CSVKişiler(string dosyayolu)
        {
            string[] satırlar = File.ReadAllLines(dosyayolu, Encoding.Default);
            if (satırlar.Length == 0)
            {
                return null;
            }

            CultureInfo culture = new(CultureInfo.CurrentCulture.Name);
            return satırlar.Skip(1).Select(satır =>
            {
                string[] veri = satır.Split(culture.TextInfo.ListSeparator.ToCharArray());
                return new Kişi
                {
                    Adi = veri[0],
                    Soyadi = veri[1],
                    Unvan = veri[2],
                    BasariBelgeTipi = veri[3],
                    BasariBelgeAciklama = veri[4],
                    GorevYeri = veri[5],
                    TC = veri[6]
                };
            });
        }

        private string GenerateTemplate(Hash context, string reportpath)
        {
            using FileStream stream = new(reportpath, FileMode.Open);
            using StreamReader reader = new(stream);
            dotTemplate template = dotTemplate.Parse(reader.ReadToEnd());

            Hash docContext = context;

            return template.Render(docContext);
        }

        private bool HasValidationErrors(DataGrid dataGrid)
        {
            foreach (object item in dataGrid.Items)
            {
                ReadOnlyObservableCollection<ValidationError> validationResult = Validation.GetErrors(dataGrid.ItemContainerGenerator.ContainerFromItem(item));

                if (validationResult.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private string SetImage(string imagePath, System.Drawing.Imaging.ImageFormat format)
        {
            const int maxWidth = 240;
            using System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);
            int newWidth = image.Width;
            int newHeight = image.Height;
            if (newWidth > maxWidth)
            {
                newWidth = maxWidth;
                newHeight = (int)(image.Height * (float)maxWidth / image.Width);
            }
            using System.Drawing.Image resizedImage = new Bitmap(image, newWidth, newHeight);
            MemoryStream memoryStream = new();
            resizedImage.Save(memoryStream, format);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private void WriteCsvFile(string metin)
        {
            string dosyaismi = Path.GetTempPath() + Guid.NewGuid() + ".csv";
            File.WriteAllText(dosyaismi, metin, Encoding.Default);
            _ = MessageBox.Show("Oluşturulan Dosyada İlk Satırları Silmeden Verileri İşleyin Microsoft Excel Virgülle Ayrılmış Değerler Dosyası (*.csv) Olarak Kaydedip Yükleyin.", "Başarı Belgesi", MessageBoxButton.OK, MessageBoxImage.Information);
            _ = Process.Start(dosyaismi);
        }

        private Kişi seçiKişi;

        private Kurum seçiliKurum;

        private bool tümüSeçili;
    }
}