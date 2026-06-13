using BaşarıBelgesi.Properties;
using DotLiquid;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PdfSharp.Xps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using dotTemplate = DotLiquid.Template;

namespace BaşarıBelgesi
{
    public class BaşarıBelgesiViewModel : InpcBase, IDataErrorInfo, ILiquidizable
    {
        public BaşarıBelgesiViewModel()
        {
            XmlDataPath = $@"{Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath)}\Data.xml";
            Settings.Default.PropertyChanged += Default_PropertyChanged;
            Kişi = new Kişi();
            Kurum = new Kurum();
            DocumentViewModel = new DocumentViewModel();
            Kurumlar = new Kurumlar() { Kurum = DataYükle() };
            Background = ResourceImageToBase64("background.jpg");
            AddKişi = new RelayCommand<object>(
                parameter =>
                {
                    Kişi kişi = new()
                    {
                        Adi = Kişi.Adi,
                        Soyadi = Kişi.Soyadi,
                        BasariBelgeAciklama = Kişi.BasariBelgeAciklama,
                        BasariBelgeTipi = Kişi.BasariBelgeTipi,
                        GorevYeri = Kişi.GorevYeri,
                        Unvan = Kişi.Unvan,
                        TC = Kişi.TC,
                        EvrakSayı = Kişi.EvrakSayı,
                        EvrakTarih = Kişi.EvrakTarih,
                        Gerekçe = Kişi.Gerekçe
                    };
                    SeçiliKurum.Kişi.Add(kişi);
                    Kurumlar.Serialize();
                },
                parameter => SeçiliKurum is not null &&
                Kişi.TcDoğrula(Kişi.TC).IsValid &&
                !string.IsNullOrWhiteSpace(Kişi.BasariBelgeAciklama) &&
                !string.IsNullOrWhiteSpace(Kişi.BasariBelgeTipi) &&
                !string.IsNullOrWhiteSpace(Kişi.GorevYeri) &&
                !string.IsNullOrWhiteSpace(Kişi.Unvan) &&
                !string.IsNullOrWhiteSpace(Kişi.Soyadi));

            AddKurum = new RelayCommand<object>(
                parameter =>
                {
                    Kurum kurum = new() { Adi = Kurum.Adi, AmirAdi = Kurum.AmirAdi, AmirSoyadi = Kurum.AmirSoyadi, AmirUnvani = Kurum.AmirUnvani, };
                    Kurumlar.Kurum.Add(kurum);
                    Kurumlar.Serialize();
                },
                parameter => !string.IsNullOrWhiteSpace(Kurum.Adi) && !string.IsNullOrWhiteSpace(Kurum.AmirAdi) && !string.IsNullOrWhiteSpace(Kurum.AmirUnvani));

            AddLogo = new RelayCommand<object>(
                parameter =>
                {
                    CommonOpenFileDialog openFileDialog = new() { Filters = { new CommonFileDialogFilter("Resim Dosyaları", "*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.gif;*.tif;*.tiff;*.bmp;*.dib;*.rle;") }, Multiselect = false };
                    if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        SeçiliKurum.Logo = SetImage(openFileDialog.FileName, ImageFormat.Png);
                    }
                    Kurumlar.Serialize();
                },
                parameter => SeçiliKurum is not null);

            AddKişiLogo = new RelayCommand<object>(
                parameter =>
                {
                    CommonOpenFileDialog openFileDialog = new() { Filters = { new CommonFileDialogFilter("Resim Dosyaları", "*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.gif;*.tif;*.tiff;*.bmp;*.dib;*.rle;") }, Multiselect = false };
                    if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        SeçiKişi.Resim = SetImage(openFileDialog.FileName, ImageFormat.Jpeg);
                    }
                    Kurumlar.Serialize();
                },
                parameter => SeçiKişi is not null);

            SetBackgroundImage = new RelayCommand<object>(
                parameter =>
                {
                    CommonOpenFileDialog openFileDialog = new() { Filters = { new CommonFileDialogFilter("Resim Dosyaları", "*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.gif;*.tif;*.tiff;*.bmp;*.dib;*.rle;") }, Multiselect = false };
                    if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        Background = LoadImageToBase64(openFileDialog.FileName);
                    }
                },
                parameter => true);

            OpenDataBaseFolder = new RelayCommand<object>(parameter => Process.Start("explorer.exe", $"/select,\"{XmlDataPath}\""), parameter => true);

            SetDefaultImage = new RelayCommand<object>(parameter => Background = ResourceImageToBase64("background.jpg"), parameter => true);

            Sakla = new RelayCommand<object>(parameter => Kurumlar.Serialize(), parameter => true);

            SetBaşarı = new RelayCommand<object>(
                parameter =>
                {
                    if (parameter is Kişi kişi)
                    {
                        kişi.BasariBelgeAciklama = $"Görevli olduğunuz kurumda üstün görev ve sorumluluk anlayışıyla görevinizi ifa etmeniz, kendi sorumluluklarınızdaki iş ve işlemleri titizlikle takip ederek, kamu hizmetlerinin hızlı ve vatandaş memnuniyetini önde tutarak yürütülmesi yönündeki başarılı çalışmalarınızdan dolayı sizi 657 sayılı Devlet Memurları Kanunu’nun 122. Maddesi uyarınca {kişi.BasariBelgeTipi} ile taltif eder, başarılı çalışmalarınızın devamını dilerim.";
                    }
                },
                parameter => true);

            KişiTümünüSeç = new RelayCommand<object>(
                parameter =>
                {
                    foreach (Kişi item in SeçiliKurum.Kişi)
                    {
                        item.Seçili = TümüSeçili;
                    }
                },
                parameter => SeçiliKurum is not null);

            BaşarıBelgesiÇıkar = new RelayCommand<object>(
                parameter =>
                {
                    if (parameter is DataGrid dataGrid && HasValidationErrors(dataGrid))
                    {
                        TaskDialog dialog = new()
                        {
                            OwnerWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle,
                            Caption = Application.Current.MainWindow.Title,
                            Icon = TaskDialogStandardIcon.Warning,
                            Text = "Hatalı Kayıtlar Vardır.",
                            InstructionText = "TC Nolarda Hatalı TC Var."
                        };
                        _ = dialog.Show();
                        return;
                    }
                    if (File.Exists("report.lqd"))
                    {
                        IEnumerable<Kişi> data = SeçiliKurum?.Kişi.Where(z => z.Seçili);
                        Kurum kurum = SeçiliKurum;
                        if (data != null)
                        {
                            Hash günlükrapor = Hash.FromAnonymousObject(new { Kişi = data, Kurum = kurum, Settings.Default.GövdeYazıTipi, Settings.Default.GövdeYazıTipiSize, Settings.Default.GövdeYazıStyle, Background });
                            string template = GenerateTemplate(günlükrapor, "report.lqd");
                            IDocumentPaginatorSource fd = (FixedDocument)XamlReader.Parse(template);
                            DocumentViewModel.Document = fd;
                            DocumentViewModel.Başlık = "RAPOR";
                            DocumentViewer t = new() { Owner = Application.Current.MainWindow, DataContext = DocumentViewModel };
                            data = null;
                            günlükrapor = null;
                            fd = null;
                            template = null;
                            t.Show();
                            GC.Collect();
                        }
                    }
                },
                parameter => SeçiliKurum?.Kişi.Any(z => z.Seçili) == true);

            PdfBaşarıBelgesiÇıkar = new RelayCommand<object>(
                parameter =>
                {
                    if (parameter is DataGrid dataGrid && HasValidationErrors(dataGrid))
                    {
                        TaskDialog dialog = new()
                        {
                            OwnerWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle,
                            Caption = Application.Current.MainWindow.Title,
                            Icon = TaskDialogStandardIcon.Warning,
                            Text = "Hatalı Kayıtlar Vardır.",
                            InstructionText = "TC Nolarda Hatalı TC Var."
                        };
                        _ = dialog.Show();
                        return;
                    }
                    if (File.Exists("report.lqd"))
                    {
                        IEnumerable<Kişi> data = SeçiliKurum?.Kişi.Where(z => z.Seçili);
                        Kurum kurum = SeçiliKurum;
                        if (data != null)
                        {
                            Hash günlükrapor = Hash.FromAnonymousObject(new { Kişi = data, Kurum = kurum, Settings.Default.GövdeYazıTipi, Settings.Default.GövdeYazıTipiSize, Settings.Default.GövdeYazıStyle, Background });
                            string template = GenerateTemplate(günlükrapor, "report.lqd");
                            IDocumentPaginatorSource fd = (FixedDocument)XamlReader.Parse(template);
                            DocumentViewModel.Document = fd;
                            DocumentViewModel.Başlık = "RAPOR";
                            string xpsfilepath = $"{Path.GetTempPath()}{Guid.NewGuid()}.xps";
                            SaveToXps(fd, xpsfilepath);
                            string pdfPath = $"{Path.GetTempPath()}{Guid.NewGuid()}.pdf";
                            XpsConverter.Convert(xpsfilepath, pdfPath, 0);
                            if (File.Exists(pdfPath))
                            {
                                _ = Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
                                File.Delete(xpsfilepath);
                            }
                            data = null;
                            günlükrapor = null;
                            fd = null;
                            template = null;
                            GC.Collect();
                        }
                    }
                },
                parameter => SeçiliKurum?.Kişi.Any(z => z.Seçili) == true);

            ŞablonGöster = new RelayCommand<object>(
                parameter =>
                {
                    CultureInfo culture = new(CultureInfo.CurrentCulture.Name);
                    string seperator = culture.TextInfo.ListSeparator;
                    string metin = $"ADI{seperator}SOYADI{seperator}UNVAN{seperator}BAŞARI BELGE TİPİ{seperator}BAŞARI BELGE AÇIKLAMA{seperator}GÖREV YERİ{seperator}TC{seperator}TARİH{seperator}SAYI{seperator}GEREKÇE";
                    WriteCsvFile(metin);
                },
                parameter => true);

            ResimKaydet = new RelayCommand<object>(
                parameter =>
                {
                    if (parameter is Kişi kişi && !string.IsNullOrWhiteSpace(kişi.Resim))
                    {
                        SaveFileDialog saveFileDialog = new() { Filter = "JPEG Resim Dosyası (*.jpg)|*.jpg", FileName = $"{kişi.Adi}_{kişi.Soyadi}_Resim.jpg" };
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            byte[] imageBytes = Convert.FromBase64String(kişi.Resim);
                            File.WriteAllBytes(saveFileDialog.FileName, imageBytes);
                        }
                    }
                },
                parameter => parameter is Kişi kişi && !string.IsNullOrWhiteSpace(kişi.Resim));

            ExceldenAl = new RelayCommand<object>(
                parameter =>
                {
                    OpenFileDialog openFileDialog = new() { Filter = "Microsoft Excel Virgülle Ayrılmış Değerler Dosyası (*.csv)|*.csv", Multiselect = false };
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
                                Unvan = item.Unvan,
                                TC = item.TC,
                                Gerekçe = item.Gerekçe,
                                EvrakTarih = item.EvrakTarih,
                                EvrakSayı = item.EvrakSayı,
                            };
                            SeçiliKurum.Kişi.Add(kişi);
                        }
                        Kurumlar.Serialize();
                    }
                },
                parameter => SeçiliKurum is not null);
        }

        public static string XmlDataPath { get; set; }

        public RelayCommand<object> AddKişi { get; }

        public RelayCommand<object> AddKişiLogo { get; }

        public RelayCommand<object> AddKurum { get; }

        public RelayCommand<object> AddLogo { get; }

        public string Background
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }

        public RelayCommand<object> BaşarıBelgesiÇıkar { get; }

        public DocumentViewModel DocumentViewModel { get; }

        public string Error => string.Empty;

        public RelayCommand<object> ExceldenAl { get; }

        public IEnumerable<int> FontSize { get; } = Enumerable.Range(8, 25);

        public Kişi Kişi { get; set; }

        public RelayCommand<object> KişiTümünüSeç { get; }

        public Kurum Kurum { get; set; }

        public Kurumlar Kurumlar { get; set; }

        public RelayCommand<object> OpenDataBaseFolder { get; }

        public RelayCommand<object> PdfBaşarıBelgesiÇıkar { get; }

        public RelayCommand<object> ResimKaydet { get; }

        public RelayCommand<object> Sakla { get; }

        public Kişi SeçiKişi
        {
            get;

            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(SeçiKişi));
                }
            }
        }

        public Kurum SeçiliKurum
        {
            get;

            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(SeçiliKurum));
                }
            }
        }

        public RelayCommand<object> SetBackgroundImage { get; }

        public RelayCommand<object> SetBaşarı { get; }

        public RelayCommand<object> SetDefaultImage { get; }

        public RelayCommand<object> ŞablonGöster { get; }

        public bool TümüSeçili
        {
            get;
            set
            {

                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(TümüSeçili));
                }
            }
        }

        public string this[string columnName] => columnName switch
        {
            "SeçiliKurum" when SeçiliKurum is null => "Lütfen Bir Kurum Seçiniz.",
            _ => null
        };

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
                return [];
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, Application.Current?.MainWindow?.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public object ToLiquid() => new { Background };

        private static string LoadImageToBase64(string filepath)
        {
            byte[] bytes = File.ReadAllBytes(filepath);
            using MemoryStream ms = new(bytes);
            return Convert.ToBase64String(ms.ToArray());
        }

        private static string ResourceImageToBase64(string resourceKey)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return string.Empty;
            }
            Uri uri = new(resourceKey, UriKind.Relative);
            using Stream stream = Application.GetResourceStream(uri).Stream;
            if (stream is not null)
            {
                using MemoryStream ms = new();
                stream.CopyTo(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
            return string.Empty;
        }

        private IEnumerable<Kişi> CSVKişiler(string dosyayolu)
        {
            string[] satırlar = File.ReadAllLines(dosyayolu, Encoding.Default);
            if (satırlar.Length == 0)
            {
                return null;
            }

            CultureInfo culture = new(CultureInfo.CurrentCulture.Name);
            return satırlar.Skip(1)
            .Select(
                satır =>
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
                        TC = veri[6],
                        EvrakTarih = DateTime.TryParse(veri[7], out DateTime tarih) ? tarih : DateTime.Now,
                        EvrakSayı = int.TryParse(veri[8], out int sayı) ? sayı : 0,
                        Gerekçe = veri[9]
                    };
                });
        }

        private void Default_PropertyChanged(object sender, PropertyChangedEventArgs e) => Settings.Default.Save();

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

        private void SaveToXps(IDocumentPaginatorSource document, string filePath)
        {
            using XpsDocument xpsDocument = new(filePath, FileAccess.ReadWrite);

            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

            writer.Write(document.DocumentPaginator);
        }

        private string SetImage(string imagePath, ImageFormat format)
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
            string dosyaismi = $"{Path.GetTempPath()}{Guid.NewGuid()}.csv";
            File.WriteAllText(dosyaismi, metin, Encoding.Default);
            TaskDialog dialog = new()
            {
                OwnerWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle,
                Caption = Application.Current.MainWindow.Title,
                Icon = TaskDialogStandardIcon.Information,
                Text = "Excel Dosyasını Doldurun.",
                InstructionText = "Oluşturulan Dosyada İlk Satırları Silmeden Formatı Değiştirmeden Verileri İşleyin Microsoft Excel Virgülle Ayrılmış Değerler Dosyası (*.csv) Olarak Kaydedip Yükleyin."
            };
            _ = dialog.Show();
            _ = Process.Start(dosyaismi);
        }
    }
}