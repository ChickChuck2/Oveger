using Microsoft.Win32;
using NReco.VideoConverter;
using Oveger.XAMLS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Oveger
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        VariablesClasses variables = new VariablesClasses();

        public MainWindow()
        {
            InitializeComponent();
        }

        [Flags]
        public enum Modifiers
        {
            NoMod = 0x0000,
            Alt = 0x0001,
            Ctrl = 0x0002,
            Shift = 0x0004,
            Win = 0x0008
        }

        public readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG", "JPGE" };

        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        private async Task DownloadFile(string URL, string fileName)
        {
            DownloadProgress progress = new DownloadProgress();
            progress.Show();
            progress.SetText(fileName);

            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
            myWebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler((object sender, DownloadProgressChangedEventArgs e )=>
            {
                Application.Current.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new Action(() =>  {try {progress.progressDownload.Value = e.ProgressPercentage;} catch { progress.Show(); }; }));
            });
            myWebClient.DownloadFileCompleted += new AsyncCompletedEventHandler((object sender, AsyncCompletedEventArgs e) =>
            {
                Application.Current.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new Action(() =>
                    {
                        progress.Background = new SolidColorBrush(Colors.Green);
                        progress.Opacity = 0.75f;
                        progress.setSeconds(5);
                    }));
            });

            Uri uriS = new Uri(URL);
            await Task.Run(() => myWebClient.DownloadFileAsync(uriS, fileName));
        }

        private void CreateAddButton()
        {
            Grid addItemGrid = new Grid();
            TextBlock ItemName = new TextBlock();
            Button addnewitem = new Button();

            Loaded += Window1_Loaded;

            if (expandCBox.IsChecked.Value)
            {
                Console.WriteLine("CHECKED");
            }
            else
            {
                Console.WriteLine("UNCHECKED");
            }
            wrappanel1.Width = wrappanel1.Width - 50;
            wrappanel1.Height = wrappanel1.Height - 50;

            // BUTTON START SETTINGS
            addItemGrid.Width = 70;
            addItemGrid.Height = 90;

            addItemGrid.Children.Add(addnewitem);

            ItemName.Text = "Add File";
            ItemName.Width = 70;
            ItemName.Height = double.NaN;
            ItemName.Margin = new Thickness { Top = 77 };
            ItemName.TextAlignment = TextAlignment.Center;

            addItemGrid.Children.Add(ItemName);

            addnewitem.Style = FindResource("AddButton") as Style;
            addnewitem.Click += new RoutedEventHandler((object sender1, RoutedEventArgs e1) => CreateButtonFile());
            //BUTTON END SETTINGS

            wrappanel1.Children.Add(addItemGrid);
        }
        private void window1_initialized(object sender, EventArgs e)
        {
            CreateAddButton();
            ConfigManager.verifyPaths(true);
            ConfigManager.LoadOrCreate(this);
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            var source = PresentationSource.FromVisual(this) as HwndSource;
            if (source == null)
                throw new Exception("Could not create hWnd source from window.");
            source.AddHook(WndProc);

            RegisterHotKey(new WindowInteropHelper(this).Handle, 2, (int)Modifiers.Ctrl | (int)Modifiers.Alt, (int)System.Windows.Forms.Keys.S);
            
            TaskbarInitialize();
        }

        private void TaskbarInitialize()
        {
            IntPtr icon = Properties.Resources.icon.GetHicon();
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = "Oveger";
            notifyIcon.Icon = System.Drawing.Icon.FromHandle(icon);
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler((object sender, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    if (!IsVisible) Show(); else Hide();
                else Close();
            });
            notifyIcon.Visible = true;
        }
        private void CreateButtonFile()
        {
            OpenFileDialog AddItem = new OpenFileDialog();
            AddItem.Filter = "All files|*.*" +
                "|images|*.png;*.jpeg;*.jpg" +
                "|exe|*.exe" +
                "|gif|*.gif";
            if (AddItem.ShowDialog() == true)
            {
                string path = AddItem.FileName;
                SetConfig(path);
                ConfigManager.Save(path); //SAVE PATH
            }
        }

        public void SetConfig(string path)
        {
            if (File.Exists(path))
            {
                string EXT = Path.GetExtension(path).ToUpper();
                string FileName = Path.GetFileName(path);

                Grid addItemGrid = new Grid() { Width=70, Height=90};
                Button addnewitem = new Button() { Style = FindResource("AddButton") as Style};
                TextBlock ItemName = new TextBlock(){Text=FileName,Width=70,Height=double.NaN,Margin=new Thickness{Top=77},TextAlignment=TextAlignment.Center};

                addItemGrid.Children.Add(addnewitem);
                addItemGrid.Children.Add(ItemName);

                wrappanel1.Children.Add(addItemGrid);

                ImageBrush img = new ImageBrush() { Stretch=Stretch.UniformToFill};

                if (ImageExtensions.Contains(EXT) || EXT.Contains(".gif".ToUpper()))
                    setButtonConfig(TypeofGet.Icon, img, addnewitem, path);
                else if (EXT.Contains(".exe".ToUpper()))
                {
                    img = ConvertedExeImage(ExtractIconfromFile(path).ToBitmap());
                    addnewitem.Style = setStyle(addnewitem, img);
                }
                else if (EXT.Contains(".mp4".ToUpper()))
                {
                    FFMpegConverter ffMpeg = new FFMpegConverter();
                    string thumbDirectory = @"thumbnails";
                    string thumbnail = FileName + ".jpg";
                    string thumbFileDir = Path.Combine(thumbDirectory, thumbnail);
                    if (!Directory.Exists("thumbnails"))
                        Directory.CreateDirectory("thumbnails");
                    if (!File.Exists(Path.Combine(thumbDirectory, thumbnail)))
                        ffMpeg.GetVideoThumbnail(path, @"thumbnails\" + thumbnail);
                    img.ImageSource = SetImage(thumbFileDir);
                    addnewitem.Style = setStyle(addnewitem, img);
                }
                else if (EXT.Contains(".txt".ToUpper()))
                    setButtonConfig(TypeofGet.IconEXT,img, addnewitem, path);
                else
                    setButtonConfig(TypeofGet.IconEXT, img, addnewitem, path);

                addnewitem.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => OpenFileProcess(path));
                addnewitem.MouseRightButtonDown += new MouseButtonEventHandler((object sender, MouseButtonEventArgs e) => PropertyRighClick(path,addItemGrid));
            }
            else ConfigManager.RemoveifNotExist(); //REMOVE PATH
        }

        private enum TypeofGet
        {
            IconEXT,
            Icon
        }
        private void setButtonConfig(TypeofGet type,ImageBrush img, Button addnewitem, string path)
        {
            if(type == TypeofGet.IconEXT)
            {
                img.ImageSource = IconManager.FindIconForFilename(path, false);
                addnewitem.Style = setStyle(addnewitem, img);
            }
            else
            {
                img.ImageSource = SetImage(path);
                addnewitem.Style = setStyle(addnewitem, img);
            }
        }

        private ImageSource SetImage(string FilePath)
        {
            Uri iri = new Uri(FilePath, UriKind.Relative);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = iri;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        private void PropertyRighClick(string path, Grid gridToDelete, string oldPath = null)
        {
            RightButtonClick right = new RightButtonClick();
            oldPath = path;

            Closing += new CancelEventHandler((object sender, CancelEventArgs e) => right.Close());

            TextBlock textblock = new TextBlock();
            right.open.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => OpenFileProcess(path));
            right.rename.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => RenameLabel(textblock));
            right.changepath.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => ChangePath(oldPath));
            right.DeleteButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => DeleteFile(path, gridToDelete));
            right.openfolderpath.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => OpenFilePath(path));
            right.property.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => OpenProperty(path));

            int entry = 0;
            Point MouseP = new Point { X = GetMousePosition().X, Y = GetMousePosition().Y };

            right.Left = MouseP.X - 50;
            right.Top = MouseP.Y + 30;
            right.MouseEnter += new MouseEventHandler((object sender1, MouseEventArgs e1) => entry = entry + 1);
            right.MouseLeave += new MouseEventHandler((object sender1, MouseEventArgs e1) => { entry = entry + 1; if (entry > 1) right.Hide(); entry = 0; });

            right.Show();
        }

        private void RenameLabel(TextBlock textblockvalue)
        {
            Console.WriteLine("RENAMED");
        }
        private void ChangePath(string oldPath)
        {
            string newPath = "";
            ChangePathForm changePath = new ChangePathForm();
            changePath.oldPathBox.Text = oldPath;
            changePath.confirmbutton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                newPath = changePath.newPathBox.Text;
                if(!Directory.Exists(newPath+@"\.."))
                    Directory.CreateDirectory(newPath+@"\..");
                File.Move(oldPath, newPath);
                ConfigManager.ChangePath(oldPath, newPath);
                changePath.Close();
                wrappanel1.Children.Clear();
                CreateAddButton();
                ConfigManager.LoadOrCreate(this);
                Show();
            });
            Hide();
            changePath.Show();
        }
        private void DeleteFile(string path, Grid grid)
        {
            Console.WriteLine(grid.Name);
            Console.WriteLine(path);
            MessageBoxResult dialog =  MessageBox.Show(this, $"Deseja deletar {Path.GetFileName(path)} permanentemente?", "Confirmação", MessageBoxButton.YesNo);
            if (dialog == MessageBoxResult.Yes)
            {
                try
                {
                    wrappanel1.Children.Remove(grid);
                    File.Delete(path);
                    if (path.Contains(".mp4"))
                        File.Delete(Path.Combine("thumbnails", Path.GetFileName(path)+".jpg"));
                    ConfigManager.verifyPaths(false);
                }catch(Exception ex) {Console.WriteLine(ex);}
            }
        }
        private Process OpenFilePath(string path)
        {
            Process process = new Process();
            string argument = "/select, \"" + path + "\"";
            process.StartInfo.FileName = "explorer.exe";
            process.StartInfo.Arguments = argument;
            process.Start();
            Hide();
            return process;
        }
        private void OpenProperty(string path)
        {
            variables.OpenPropertyDialog(path);
            Hide();
        }
        private Process OpenFileProcess(string path)
        {
            Hide();
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = path;
                process.Start();
                return process;
            }
            catch (Win32Exception W32E)
            {
                MessageBox.Show($"{W32E.Message}");
                return null;
            }
        }

        private ImageBrush ConvertedExeImage(System.Drawing.Bitmap img)
        {
            ImageBrush ib = new ImageBrush();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(img.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            ib.ImageSource = bitmapSource;

            return ib;
        }

        private Style setStyle(Button addnewitem, object img)
        {
            DataTrigger tg = new DataTrigger()
            {
                Binding = new Binding("mousehover"),
                Value = 1
            };
            tg.Setters.Add(new Setter()
            {
                Property = Button.BackgroundProperty,
                Value = Brushes.Green
            });

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

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0312)
            {
                Show();
            }
            return IntPtr.Zero;
        }
    }
}