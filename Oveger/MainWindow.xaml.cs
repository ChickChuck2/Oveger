using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oveger.XAMLS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;

namespace Oveger
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                ConfigManager Configurator = new ConfigManager();

                if (!File.Exists("itens.json"))
                {
                    File.Create("itens.json").Dispose();
                    Configurator.LoadOrCreateConfigs(ConfigManager.TypeFile.Create, this);
                }
                else
                    Configurator.LoadOrCreateConfigs(ConfigManager.TypeFile.Load, this);
                    
            }catch(Exception ex) { Console.WriteLine($"Erro ao criar itens.json {ex.Message}"); }
        }

        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG", "JPGE" };

        /// <summary>
        /// 
        /// </summary>
        /// <example>BRUSHERHEX("#ffaacc")</example>
        /// <param name="HEXFormater"></param>
        /// <returns></returns>
        private SolidColorBrush BRUSHERHEX(string HEXFormater)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFrom(HEXFormater);
        }

        Grid addItemGrid = new Grid();
        TextBlock ItemName = new TextBlock();
        Button addnewitem = new Button();


        public void ButtonSettings(string path)
        {
            addItemGrid = new Grid();
            ItemName = new TextBlock();

            addnewitem = new Button();
            addnewitem.Style = FindResource("AddButton") as Style;


            addItemGrid.Width = 70;
            addItemGrid.Height = 90;

            ItemName.Text = "Add File";
            ItemName.Width = 70;
            ItemName.Height = double.NaN;
            ItemName.Margin = new Thickness {Top = 77};
            ItemName.TextAlignment = TextAlignment.Center;

            wrappanel1.Children.Add(addItemGrid);
            addItemGrid.Children.Add(addnewitem);
            addItemGrid.Children.Add(ItemName);

            //SET IMAGE ON BUTTON

            string EXT = Path.GetExtension(path).ToUpper();
            string FileName = Path.GetFileName(path);

            ItemName.Text = FileName;

            if(ImageExtensions.Contains(EXT.ToUpper()))
            {
                ImageBrush img = new ImageBrush();

                img.ImageSource = SetImage(path);

                addnewitem.Style = setStyle(addnewitem, img);

            }else if(EXT.Contains(".exe".ToUpper()))
            {
                System.Drawing.Bitmap img = ExtractIconfromFile(path).ToBitmap();
                addnewitem.Style = setStyle(addnewitem, ConvertedExeImage(addnewitem, img));
            }
            else if (EXT.Contains(".mp4".ToUpper()))
            {
                if (!File.Exists($"ffmpeg.exe")) //sddd
                {
                    string remoteUri = "http://www.contoso.com/library/homepage/images/";
                    string fileName = "ffmpeg.exe", myStringWebResource = null;

                    // Create a new WebClient instance.
                    using (WebClient myWebClient = new WebClient())
                    {
                        myStringWebResource = remoteUri + fileName;
                        // Download the Web resource and save it into the current filesystem folder.
                        myWebClient.DownloadFile(myStringWebResource, fileName);
                    }
                }
                Console.WriteLine("MAKE THUMB");
            }

            addnewitem.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                OpenFileProcess(path);
            });

        }
        private Process OpenFileProcess(string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.Start();

            return process;
        }

        private void window1_initialized(object sender, EventArgs e)
        {
            Topmost = false;

            wrappanel1.Width = wrappanel1.Width - 50;
            wrappanel1.Height = wrappanel1.Height - 50;

            // BUTTON START SETTINGS

            addItemGrid.Width = 70;
            addItemGrid.Height = 90;

            addItemGrid.Children.Add(addnewitem);

            ItemName.Text = "Add File";
            ItemName.Width = 70;
            ItemName.Height = double.NaN;
            ItemName.Margin = new Thickness
            {
                Top = 77
            };
            ItemName.TextAlignment = TextAlignment.Center;

            addItemGrid.Children.Add(ItemName);

            addnewitem.Style = FindResource("AddButton") as Style;
            addnewitem.Click += new RoutedEventHandler((object sender1, RoutedEventArgs e1) => CreateButtonFile());
            //BUTTON END SETTINGS

            wrappanel1.Children.Add(addItemGrid);
        }

        private void CreateButtonFile()
        {
            addnewitem = new Button()
            {
                Style = FindResource("AddButton") as Style
            };

            OpenFileDialog AddItem = new OpenFileDialog();
            if (AddItem.ShowDialog() == true)
            {
                string FilePath = AddItem.FileName;
                string FileExt = Path.GetExtension(FilePath).ToUpper();

                Console.WriteLine(FilePath);
                Console.WriteLine(FileExt);

                if (ImageExtensions.Contains(FileExt))
                {
                    ImageBrush img = new ImageBrush();
                    img.ImageSource = SetImage(FilePath);

                    addnewitem.Style = setStyle(addnewitem, img);
                }
                if(FileExt == ".exe".ToUpper())
                {
                    System.Drawing.Bitmap img = ExtractIconfromFile(FilePath).ToBitmap();
                    addnewitem.Style = setStyle(addnewitem, ConvertedExeImage(addnewitem, img));
                }

                ConfigManager config = new ConfigManager();
                config.LoadOrCreateConfigs(ConfigManager.TypeFile.Save, this, AddItem.FileName);

                wrappanel1.Children.Add(addnewitem);
            }

            addnewitem.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => OpenFileProcess(AddItem.FileName));
        }
        private ImageBrush ConvertedExeImage(Button addnewitem, System.Drawing.Bitmap img)
        {
            ImageBrush ib = new ImageBrush();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(img.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            ib.ImageSource = bitmapSource;

            return ib;
        }

        private Style setStyle(Button addnewitem, object img)
        {
            addnewitem.Style = new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = ContentProperty, Value = "-"},
                    new Setter { Property = HorizontalContentAlignmentProperty, Value = HorizontalAlignment.Center },
                    new Setter { Property = VerticalAlignmentProperty, Value = VerticalAlignment.Center },
                    new Setter { Property = WidthProperty, Value = 70d},
                    new Setter { Property = HeightProperty, Value = 70d},
                    new Setter { Property = ForegroundProperty,Value = Brushes.White},
                    new Setter { Property = FontSizeProperty, Value = 30d},
                    new Setter
                    {
                        Property = TemplateProperty,
                        Value = new ControlTemplate(typeof(Button))
                        {
                            VisualTree = CreateFactory(img)
                        }
                    }
                }
            };
            return addnewitem.Style;
        }

        private BitmapFrame SetImage(string FilePath)
        {
            Uri iri = new Uri(FilePath, UriKind.Relative);
            return BitmapFrame.Create(iri);
        }
        private FrameworkElementFactory CreateFactory(object img)
        {
            // --START OF MAIN FACTORY-- //
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Border));
            factory.SetValue(Border.BackgroundProperty, img);
            factory.SetValue(Border.CornerRadiusProperty, new CornerRadius
            { TopLeft = 10, TopRight = 10, BottomLeft = 10, BottomRight = 10 });
            factory.SetValue(Border.BorderBrushProperty, BRUSHERHEX("#696969"));
            factory.SetValue(Border.BorderThicknessProperty, new Thickness
            { Top = 1, Left = 1, Right = 1, Bottom = 1, });
            // --END OF MAIN FACTORY-- //

            FrameworkElementFactory contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.MarginProperty, new Thickness
            { Top = -1, Left = -1, Right = -1, Bottom = -1 });
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            //contentFactory.SetValue(ContentPresenter.ContentProperty, "-");
            factory.AppendChild(contentFactory);

            return factory;

        }

        public System.Drawing.Icon ExtractIconfromFile(string pathEXE)
        {
            System.Drawing.Icon result = (System.Drawing.Icon)null;

            try
            {
                result = System.Drawing.Icon.ExtractAssociatedIcon(pathEXE);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO EXTRAIR ICON {ex}");
            }
            return result;
        }
    }
}
