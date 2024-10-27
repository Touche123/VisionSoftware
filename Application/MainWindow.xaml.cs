using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static Tensorflow.GraphTransferInfo.Types;

namespace MainApp
{
    public interface IService
    {

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
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
					viewModel.Destination?.Dispose();
					//viewModel.Destination = Cv2.ImRead(viewModel.SelectedImages[0].ToString(), ImreadModes.Grayscale);
					var thumbnail = (Thumbnail)viewModel.SelectedImages[0];
					
                    viewModel.Destination = new Mat(thumbnail.ImagePath);
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
