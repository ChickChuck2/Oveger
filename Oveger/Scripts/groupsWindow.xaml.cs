using Oveger.XAMLS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Oveger.Scripts
{
	/// <summary>
	/// Lógica interna para groupsWindow.xaml
	/// </summary>
	public partial class groupsWindow : Window
	{
		public bool otherMode = false;
		public string pathToAdd = "";
		public MainWindow mainWindow;
		public static RoutedCommand MyCommand = new RoutedCommand();
		public groupsWindow()
		{
			InitializeComponent();
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			StartLabels();
			MyCommand.InputGestures.Add(new KeyGesture(Key.Escape));
		}

		private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e) => Close();

		private void confirmButton_Click(object sender, RoutedEventArgs e)
		{
			var groups = groupsBlock.Text.Split(';');
			ConfigManager.AddGroups(groups);
			groupsMngr.Children.Clear();
			StartLabels();
			Close();
        }


		void StartLabels()
		{
			var groups = ConfigManager.GetGroups();
			bool yn = false;
			var black = new SolidColorBrush(new Color() { R =31, G = 31, B = 31,A = 100, });
			var white = new SolidColorBrush(new Color() { R =255, G = 255, B = 255,A = 100, });
			foreach (var group in groups)
			{
				yn = !yn;
				Label label = new Label() { Style = new() { Setters = { new Setter { Property = BackgroundProperty, Value = (yn) ? black : white } } } };
				label.Content = group;
				label.MouseDown += new((sender, e) =>
				{
					Console.WriteLine($"othmd = {otherMode}");
					if (otherMode)
					{
						var v = MessageBox.Show("Tem certeza que deseja excluir essa categoria?", "Excluir grupo?", MessageBoxButton.YesNo);
						if (v == MessageBoxResult.Yes)
						{
							ConfigManager.RemoveGroup(label.Content.ToString());
							groupsMngr.Children.Remove(label);
							mainWindow.Reload();
						}
					}
					else
					{
						ConfigManager.AddPathOnGroup(pathToAdd, group);
						mainWindow.Reload();
						Close();
					}
				});
				groupsMngr.Children.Add(label);
			}
		}
	}
}