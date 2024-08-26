using Microsoft.Win32;
using NReco.VideoConverter;
using Oveger.Scripts;
using Oveger.XAMLS;
using Shell32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG", "JPGE" };

        public static RoutedCommand MyCommand = new RoutedCommand();

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [Flags]
        public enum Modifiers { NoMod = 0x0000, Alt = 0x0001, Ctrl = 0x0002, Shift = 0x0004, Win = 0x0008 }
        private enum TypeofGet { GetIconEXT, GetIcon }

        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        readonly System.Windows.Forms.ContextMenuStrip context = new System.Windows.Forms.ContextMenuStrip();
        readonly System.Windows.Forms.ToolStripMenuItem close = new System.Windows.Forms.ToolStripMenuItem { Text = "Fechar" };
        readonly System.Windows.Forms.ToolStripMenuItem show = new System.Windows.Forms.ToolStripMenuItem { Text = "Mostrar" };
        readonly System.Windows.Forms.ToolStripMenuItem StartWithWindows = new System.Windows.Forms.ToolStripMenuItem { Text = "Iniciar com Windows" };
        readonly System.Windows.Forms.ToolStripMenuItem changehotkey = new System.Windows.Forms.ToolStripMenuItem { Text = "Atalhos e ajuda" };
		readonly System.Windows.Forms.ToolStripMenuItem groupsTray = new System.Windows.Forms.ToolStripMenuItem() {Text = "Grupos" };

        public MainWindow() => InitializeComponent();


        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Hide();
            inprogram = false;
        }

        private void Window1_Closed(object sender, EventArgs e) => notifyIcon.Visible = false;



		private bool _isMoving;
		private Point? _buttonPosition;
		private double deltaX;
		private double deltaY;
		private TranslateTransform _currentTT;

		private void CreateAddButton()
        {

            Grid addItemGrid = new Grid() { Width=70, Height=148 };
            TextBlock ItemName = new TextBlock()
            {
                Text = "Adicionar Arquivo",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness { Bottom = -1, Left = -1, Right = -1, Top = 77 },
                TextAlignment = TextAlignment.Center
            };
            Button addnewitem = new Button()
            { Style = FindResource("AddButton") as Style, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness { Bottom = -1, Left = -1, Right = -1, Top = 10 } };

            addItemGrid.Children.Add(addnewitem);
            addItemGrid.Children.Add(ItemName);

            addnewitem.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => CreateButtonFile());

            wrappanel1.Children.Add(addItemGrid);
        }
        private void Window1_initialized(object sender, EventArgs e)
        {
            MyCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            //CreateAddButton();
        }

        private void Window1_LocationChanged(object sender, EventArgs e)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
            System.Windows.Forms.Screen curr = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            wrappanel1.Width = curr.Bounds.Width;
            wrappanel1.Height = curr.Bounds.Height;
            Window1.UpdateLayout();
        }
        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(PresentationSource.FromVisual(this) is HwndSource source))
                throw new Exception("Could not create hWnd source from window.");
            source.AddHook(WndProc);

            ConfigManager.LoadOrCreate(this);
            ConfigManager.VerifyPaths(true);
            ReloadButtons();
            RegisterHotKey(new WindowInteropHelper(this).Handle, 2, (int)ConfigManager.GetMODKey(0) | (int)ConfigManager.GetMODKey(1), (int)ConfigManager.GetKey());
            TaskbarInitialize();
            Window1.WindowState = WindowState.Maximized;
            Hide();
            Window1.WindowState = WindowState.Normal;

            ConfigGroups();
		}

        void ConfigGroups()
        {
			Expander customGroup = new() {IsExpanded = true, Header="BOSTA", Width = 360, Height = 150 };
			StackPanel customStack = new();
			Button test1 = new Button() {Content = "test1" };
			TextBlock test2 = new TextBlock() { Text = "test2" };

			customStack.Children.Add(test1);
			customStack.Children.Add(test2);
			customGroup.Content = customStack;

			wrappanel1.Children.Add(customGroup);
		}

        public async Task VerifyPaths(Action action)
        {
            while(true)
            {
                action();
                Task task = Task.Delay(TimeSpan.FromMinutes(30));

                try { await task; }
                catch{ return; }
            }
        }
        private void TaskbarInitialize()
        {
            close.Click += new EventHandler((object sender, EventArgs e) => Process.GetCurrentProcess().Kill());
            show.Click += new EventHandler((object sender, EventArgs e) => Show());
            StartWithWindows.Checked = ConfigManager.GetBool();
			StartWithWindows.Click += new EventHandler((object sender, EventArgs e) =>
            {
                StartWithWindows.Checked = !StartWithWindows.Checked;
                RegKeyRegister.SetStartup(StartWithWindows.Checked);
                ConfigManager.Save(StartWithWindows.Checked, null);
            });
            changehotkey.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Helpandshortcut helpandshortcut = new Helpandshortcut();
                helpandshortcut.Show();
            });
            groupsTray.Click += new EventHandler((object sender, EventArgs e) =>
            {
                groupsWindow GW = new groupsWindow();
                GW.Show();
            });

            context.Items.Add(show);
            context.Items.Add(StartWithWindows);
            context.Items.Add(changehotkey);
            context.Items.Add(groupsTray);
            context.Items.Add(close);

            IntPtr icon = Properties.Resources.icon.GetHicon();
            notifyIcon = new System.Windows.Forms.NotifyIcon { Text = "Oveger", ContextMenuStrip = context, Icon = System.Drawing.Icon.FromHandle(icon), Visible = true };
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler((object sender, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    Show();
            });
        }
        private void CreateButtonFile()
        {
            OpenFileDialog AddItem = new OpenFileDialog { Multiselect = true, DereferenceLinks = false, Filter = "All files|*.*" + "|images|*.png;*.jpeg;*.jpg" + "|exe|*.exe" + "|gif|*.gif" };
            if (AddItem.ShowDialog() == true)
            {
                string[] paths = AddItem.FileNames;

                foreach(string path in paths)
                {
                    SetConfig(path);
                    ConfigManager.Save(StartWithWindows.Checked, path);
                }
            }
        }
        
        public void SetConfig(string path)
        {
            string EXT = Path.GetExtension(path).ToUpper();
            string FileName = Path.GetFileName(path);
            Grid addItemGrid = new Grid() { Width = 70, Height = 148 };
            Button addnewitem = new Button() { Style = FindResource("AddButton") as Style, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness { Bottom = -1, Left = -1, Right = -1, Top = 10 } };
            TextBlock ItemName = new TextBlock() { Text = ConfigManager.GetLabelName(path, FileName), Margin = new Thickness { Bottom = -1, Left = -1, Right = -1, Top = 77 }, TextAlignment = TextAlignment.Center, TextWrapping = TextWrapping.Wrap };

            addItemGrid.Children.Add(addnewitem);
            addItemGrid.Children.Add(ItemName);
            wrappanel1.Children.Add(addItemGrid);

            ImageBrush img = new ImageBrush() { Stretch = Stretch.UniformToFill };
            if (ImageExtensions.Contains(EXT) || EXT.Contains(".gif".ToUpper()))
                SetButtonConfig(TypeofGet.GetIcon, img, addnewitem, path);
            else if (EXT.Contains(".exe".ToUpper()) || EXT.Contains(".url".ToUpper()))
            {
                System.Drawing.Bitmap Extracted = ExtractIconfromFile(path).ToBitmap();
                img = ConvertBitmapToImageBrush(Extracted);
                addnewitem.Style = SetStyle(addnewitem, img);
                Extracted.Dispose();
            }
            else if (EXT.Contains(".mp4".ToUpper()) || EXT.Contains(".mkv".ToUpper()) || EXT.Contains(".webp".ToUpper()))
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
                addnewitem.Style = SetStyle(addnewitem, img);
            }
            else
                SetButtonConfig(TypeofGet.GetIconEXT, img, addnewitem, path);

            addnewitem.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                if (!_isMoving)
                    OpenFileProcess(path);
			});
            addnewitem.MouseRightButtonDown += new MouseButtonEventHandler((object sender, MouseButtonEventArgs e) => PropertyRighClick(path, addItemGrid, ItemName));
        }

        private void SetButtonConfig(TypeofGet type,ImageBrush img, Button addnewitem, string path)
        {
            if(type == TypeofGet.GetIconEXT)
                img.ImageSource = IconManager.FindIconForFilename(path, true);
            else
                img.ImageSource = SetImage(path);
            addnewitem.Style = SetStyle(addnewitem, img);
		}

        private ImageSource SetImage(string FilePath)
        {
            Uri iri = new Uri(FilePath, UriKind.Relative);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = iri;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
        

        private void PropertyRighClick(string path, Grid gridToDelete, TextBlock textblock, string oldPath = null)
        {
            RightButtonClick right = new RightButtonClick();
            oldPath = path;

            Closing += new CancelEventHandler((object sender, CancelEventArgs e) => right.Close());
            right.open.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { OpenFileProcess(path);right.Close(); });
            right.rename.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { RenameLabel(path, textblock);right.Close(); });
            right.changepath.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { ChangePath(oldPath); right.Close(); });
            right.DeleteButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { DeleteFile(path, gridToDelete); right.Close(); });
            right.DeleteinProgram.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { DeleteInProgram(path, gridToDelete); right.Close(); });
            right.openfolderpath.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { OpenFilePath(path); right.Close(); });
            right.property.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { OpenProperty(path); right.Close(); });

            int entry = 0;
            Point MouseP = new Point { X = VariablesClasses.Mouse.GetMousePosition().X, Y = VariablesClasses.Mouse.GetMousePosition().Y };

            right.Left = MouseP.X - 50;
            right.Top = MouseP.Y + 30;
            right.MouseEnter += new MouseEventHandler((object sender1, MouseEventArgs e1) => entry++);
            right.MouseLeave += new MouseEventHandler((object sender1, MouseEventArgs e1) => { entry++; if (entry > 1) right.Hide(); entry = 0; });

            right.Show();
        }

        private void RenameLabel(string path,TextBlock textblock)
        {
            Hide();
            LocalRenameWindow localRename = new LocalRenameWindow();
            localRename.Show();
            localRename.ConfirmButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                string Filename = localRename.renamedbox.Text;
                textblock.Text = Filename;
                localRename.Close();
                if(ConfigManager.RenameLabel(path, Filename) > 0)
                    ReloadButtons();
                Show();
            });
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
            MessageBoxResult dialog =  MessageBox.Show(this, $"Deseja deletar {Path.GetFileName(path)} permanentemente?", "Confirmação", MessageBoxButton.YesNo);
            if (dialog == MessageBoxResult.Yes)
            {
                try
                {
                    wrappanel1.Children.Remove(grid);
                    File.Delete(path);
                    if (path.Contains(".mp4"))
                        File.Delete(Path.Combine("thumbnails", Path.GetFileName(path)+".jpg"));
                    ConfigManager.VerifyPaths(false);
                }catch(Exception ex) {Console.WriteLine(ex);}
            }
        }
        private void DeleteInProgram(string path, Grid grid)
        {
            wrappanel1.Children.Remove(grid);
            if (path.Contains(".mp4"))
                File.Delete(Path.Combine("thumbnails", Path.GetFileName(path) + ".jpg"));
            ConfigManager.Remove(path);
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
            VariablesClasses.OpenPropertyDialog(path);
            Hide();
        }
        private Process OpenFileProcess(string path)
        {
            Hide();
            inprogram = false;
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

        private ImageBrush ConvertBitmapToImageBrush(System.Drawing.Bitmap img)
        {
            ImageBrush ib = new ImageBrush();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(img.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            ib.ImageSource = bitmapSource;

            return ib;
        }

        private Style SetStyle(Button addnewitem, ImageBrush img)
        {
            DataTrigger tg = new DataTrigger()
            {
                Binding = new Binding("mousehover"),
                Value = 1
            };
            tg.Setters.Add(new Setter()
            {
                Property = BackgroundProperty,
                Value = Brushes.Green
            });

            Trigger trigger = new Trigger
            {
                Property = IsMouseOverProperty,
                Value = true
            };

			// Create a setter to change the background color
			Setter[] setter =
			[
				new Setter()
				{
                    Property = HeightProperty,
                    Value = 90d
                },
                new Setter ()
                {
                    Property = WidthProperty,
                    Value = 90d
                },
                new Setter()
                {
                    Property = OpacityProperty,
                    Value = 0.9
                }
            ];

            //trigger.Setters.Add(setter[0]);
            //trigger.Setters.Add(setter[1]);
            trigger.Setters.Add(setter[2]);

            var style = new Style(typeof(Button))
            {
                Setters =
                {
                    //new Setter { Property = ContentProperty, Value = "-"},
                    new Setter { Property = HorizontalContentAlignmentProperty, Value = HorizontalAlignment.Center },
                    new Setter { Property = VerticalAlignmentProperty, Value = VerticalAlignment.Center },
                    new Setter { Property = WidthProperty, Value = 70d},
                    new Setter { Property = HeightProperty, Value = 70d},
                    new Setter { Property = FontSizeProperty, Value = 30d},
					new Setter { Property = OpacityProperty, Value = 0.7 },

					new Setter
                    {
                        Property = TemplateProperty,
                        Value = new ControlTemplate(typeof(Button))
                        {
                            VisualTree = CreateFactory(img)
                        }
                    }
                },
                Triggers = 
                {
                    trigger
                }
            };
            return style;
        }

        private FrameworkElementFactory CreateFactory(ImageBrush img)
        {
            // --START OF MAIN FACTORY-- //
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Border));
            factory.SetValue(Border.BackgroundProperty, img);
            factory.SetValue(Border.CornerRadiusProperty, new CornerRadius { TopLeft = 10, TopRight = 10, BottomLeft = 10, BottomRight = 10 });
            factory.SetValue(Border.BorderBrushProperty, BRUSHERHEX("#696969"));
            factory.SetValue(Border.BorderThicknessProperty, new Thickness { Top = 1, Left = 1, Right = 1, Bottom = 1, });
            // --END OF MAIN FACTORY-- //

            FrameworkElementFactory contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.MarginProperty, new Thickness { Top = -1, Left = -1, Right = -1, Bottom = -1 });
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            //contentFactory.SetValue(ContentPresenter.ContentProperty, "-");
            factory.AppendChild(contentFactory);
            return factory;
        }

        public System.Drawing.Icon ExtractIconfromFile(string path)
        {
            Console.WriteLine($"EXTRACTING FROM {path}");
            System.Drawing.Icon result = System.Drawing.Icon.ExtractAssociatedIcon(path);
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
        bool inprogram = false;
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0312)
            {
                if (inprogram)
                    Hide();
                else
                    Show();
                inprogram = !inprogram;
            }
            return IntPtr.Zero;
        }
        private void ReloadButtons()
        {
            wrappanel1.Children.Clear();
            CreateAddButton();
            ConfigManager.LoadOrCreate(this);
        }
	}
}