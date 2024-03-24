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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

		public void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox listBox && listBox.DataContext is MainWindowViewModel viewModel)
			{
				viewModel.SelectedImages.Clear();
				foreach (var item in listBox.SelectedItems)
				{
					viewModel.SelectedImages.Add(item);
					//viewModel.Ncc.Destination = viewModel.SelectedImages[0];
				}
			}
		}

		private void ZoomViewbox_OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (DataContext is MainWindowViewModel viewModel)
			{
				viewModel.OnMouseWheel(e.Delta);
			}
		}

		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = true;
		}
	}
}
