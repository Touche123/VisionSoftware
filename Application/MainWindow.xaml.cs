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
        private System.Windows.Point _startPoint;
        private bool _isDragging;
        private bool _isResizing;
        private const double HandleSize = 40; // Size of the resize handles
        public MainWindow()
        {
            InitializeComponent();
            InitializeRectangle();
        }

        private void InitializeRectangle()
        {
            // Set initial position and size of the rectangle
            Canvas.SetLeft(ManipulatableRectangle, 100);
            Canvas.SetTop(ManipulatableRectangle, 100);
            ManipulatableRectangle.Width = 200;
            ManipulatableRectangle.Height = 100;

            // Position the resize handle at the bottom-right corner of the rectangle
            PositionResizeHandle();
        }

        private void PositionResizeHandle()
        {
            double left = Canvas.GetLeft(ManipulatableRectangle);
            double top = Canvas.GetTop(ManipulatableRectangle);
            Canvas.SetLeft(HandleBottomRight, left + ManipulatableRectangle.Width - HandleSize / 2);
            Canvas.SetTop(HandleBottomRight, top + ManipulatableRectangle.Height - HandleSize / 2);
        }
        public void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox listBox && listBox.DataContext is MainWindowViewModel viewModel)
			{
				viewModel.SelectedImages.Clear();
				foreach (var item in listBox.SelectedItems)
				{
					viewModel.SelectedImages.Add(item);
					viewModel.InspectService.Destination?.Dispose();
					var thumbnail = (Thumbnail)viewModel.SelectedImages[0];
                    viewModel.InspectService.Destination = new Mat(thumbnail.ImagePath);
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

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if click is on a corner for resizing
            if (IsMouseOverResizeHandle(e.GetPosition(OverlayCanvas)))
            {
                _isResizing = true;
            }
            else
            {
                _isDragging = true;
                _startPoint = e.GetPosition(OverlayCanvas);
            }

            Mouse.Capture(ManipulatableRectangle);
        }

        // Example of using MouseDown event to add an ellipse
        private void OverlayCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if click is on a corner for resizing
            if (IsMouseOverResizeHandle(e.GetPosition(OverlayCanvas)))
            {
                _isResizing = true;
            }
            else
            {
                _isDragging = true;
                _startPoint = e.GetPosition(OverlayCanvas);
            }

            Mouse.Capture(ManipulatableRectangle);
        }

        private void OverlayCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                // Move the rectangle
                System.Windows.Point currentPosition = e.GetPosition(OverlayCanvas);
                double offsetX = currentPosition.X - _startPoint.X;
                double offsetY = currentPosition.Y - _startPoint.Y;

                double newLeft = Canvas.GetLeft(ManipulatableRectangle) + offsetX;
                double newTop = Canvas.GetTop(ManipulatableRectangle) + offsetY;

                Canvas.SetLeft(ManipulatableRectangle, newLeft);
                Canvas.SetTop(ManipulatableRectangle, newTop);
                PositionResizeHandle(); // Update handle position
                _startPoint = currentPosition;
            }
            else if (_isResizing)
            {
                // Resize the rectangle
                System.Windows.Point currentPosition = e.GetPosition(OverlayCanvas);
                double newWidth = currentPosition.X - Canvas.GetLeft(ManipulatableRectangle);
                double newHeight = currentPosition.Y - Canvas.GetTop(ManipulatableRectangle);

                if (newWidth > 0) ManipulatableRectangle.Width = newWidth;
                if (newHeight > 0) ManipulatableRectangle.Height = newHeight;

                PositionResizeHandle(); // Update handle position
            }
        }
        private void OverlayCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _isResizing = false;
            Mouse.Capture(null);
        }

        private bool IsMouseOverResizeHandle(System.Windows.Point mousePosition)
        {
            double left = Canvas.GetLeft(ManipulatableRectangle);
            double top = Canvas.GetTop(ManipulatableRectangle);
            double right = left + ManipulatableRectangle.Width;
            double bottom = top + ManipulatableRectangle.Height;

            // Check if mouse is near the rectangle corners (resize handles)
            return (mousePosition.X >= right - HandleSize && mousePosition.X <= right &&
                    mousePosition.Y >= bottom - HandleSize && mousePosition.Y <= bottom);
        }

        // Resize handle events
        private void Handle_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.SizeNWSE; // Change cursor to indicate resizing
        }

        private void Handle_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow; // Reset cursor when leaving the handle
        }

        private void Handle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isResizing = true; // Start resizing
            Mouse.Capture(HandleBottomRight); // Capture mouse to the handle
        }
    }
}
