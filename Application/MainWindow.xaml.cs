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
        private bool _isRotating;
        private const double HandleSize = 10; // Size of the resize handles
        private double _rotationAngle = 0; // Current rotation angle

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
            PositionHandles();
        }

        private void PositionHandles()
        {
            double centerX = Canvas.GetLeft(ManipulatableRectangle) + ManipulatableRectangle.Width / 2;
            double centerY = Canvas.GetTop(ManipulatableRectangle) + ManipulatableRectangle.Height / 2;
            double halfWidth = ManipulatableRectangle.Width / 2;
            double halfHeight = ManipulatableRectangle.Height / 2;

            // Calculate the positions based on rotation
            double cos = Math.Cos(_rotationAngle * Math.PI / 180);
            double sin = Math.Sin(_rotationAngle * Math.PI / 180);

            // Handle positions before rotation
            double topLeftX = -halfWidth;
            double topLeftY = -halfHeight;

            double topRightX = halfWidth;
            double topRightY = -halfHeight;

            double bottomLeftX = -halfWidth;
            double bottomLeftY = halfHeight;

            double bottomRightX = halfWidth;
            double bottomRightY = halfHeight;

            // Apply rotation
            //HandleTopLeft.Visibility = Visibility.Visible;
            Canvas.SetLeft(HandleTopLeft, centerX + (topLeftX * cos - topLeftY * sin) - 5);
            Canvas.SetTop(HandleTopLeft, centerY + (topLeftX * sin + topLeftY * cos) - 5);

            //HandleTopRight.Visibility = Visibility.Visible;
            Canvas.SetLeft(HandleTopRight, centerX + (topRightX * cos - topRightY * sin) - 5);
            Canvas.SetTop(HandleTopRight, centerY + (topRightX * sin + topRightY * cos) - 5);

            //HandleBottomLeft.Visibility = Visibility.Visible;
            Canvas.SetLeft(HandleBottomLeft, centerX + (bottomLeftX * cos - bottomLeftY * sin) - 5);
            Canvas.SetTop(HandleBottomLeft, centerY + (bottomLeftX * sin + bottomLeftY * cos) - 5);

            //HandleBottomRight.Visibility = Visibility.Visible;
            Canvas.SetLeft(HandleBottomRight, centerX + (bottomRightX * cos - bottomRightY * sin) - 5);
            Canvas.SetTop(HandleBottomRight, centerY + (bottomRightX * sin + bottomRightY * cos) - 5);

            // Position Rotation Handle
            Canvas.SetLeft(RotationHandle, centerX - 10);
            Canvas.SetTop(RotationHandle, centerY - 20);
        }

        private void RotateRectangle(double angle)
        {
            // Set the rotation to happen around the center of the rectangle itself
            ManipulatableRectangle.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

            // Apply rotation
            RotateTransform rotateTransform = new RotateTransform(angle);
            ManipulatableRectangle.RenderTransform = rotateTransform;
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
            if (e.ButtonState == MouseButtonState.Pressed && !_isDragging && !_isResizing && !_isRotating)
            {
                _startPoint = e.GetPosition(OverlayCanvas);
                _isDragging = true;
            }
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
                PositionHandles(); // Update handle positions
                _startPoint = currentPosition;
            }
            else if (_isResizing)
            {
                // Resize the rectangle based on the handle being dragged
                System.Windows.Point currentPosition = e.GetPosition(OverlayCanvas);
                Rectangle handle = (Rectangle)Mouse.Captured;

                if (handle == HandleTopLeft)
                {
                    double newWidth = Canvas.GetLeft(ManipulatableRectangle) + ManipulatableRectangle.Width - currentPosition.X;
                    double newHeight = Canvas.GetTop(ManipulatableRectangle) + ManipulatableRectangle.Height - currentPosition.Y;

                    if (newWidth > 0) ManipulatableRectangle.Width = newWidth;
                    if (newHeight > 0) ManipulatableRectangle.Height = newHeight;

                    Canvas.SetLeft(ManipulatableRectangle, currentPosition.X);
                    Canvas.SetTop(ManipulatableRectangle, currentPosition.Y);
                }
                else if (handle == HandleTopRight)
                {
                    double newWidth = currentPosition.X - Canvas.GetLeft(ManipulatableRectangle);
                    double newHeight = Canvas.GetTop(ManipulatableRectangle) + ManipulatableRectangle.Height - currentPosition.Y;

                    if (newWidth > 0) ManipulatableRectangle.Width = newWidth;
                    if (newHeight > 0) ManipulatableRectangle.Height = newHeight;

                    Canvas.SetTop(ManipulatableRectangle, currentPosition.Y);
                }
                else if (handle == HandleBottomLeft)
                {
                    double newWidth = Canvas.GetLeft(ManipulatableRectangle) + ManipulatableRectangle.Width - currentPosition.X;
                    double newHeight = currentPosition.Y - Canvas.GetTop(ManipulatableRectangle);

                    if (newWidth > 0) ManipulatableRectangle.Width = newWidth;
                    if (newHeight > 0) ManipulatableRectangle.Height = newHeight;

                    Canvas.SetLeft(ManipulatableRectangle, currentPosition.X);
                }
                else if (handle == HandleBottomRight)
                {
                    double newWidth = currentPosition.X - Canvas.GetLeft(ManipulatableRectangle);
                    double newHeight = currentPosition.Y - Canvas.GetTop(ManipulatableRectangle);

                    if (newWidth > 0) ManipulatableRectangle.Width = newWidth;
                    if (newHeight > 0) ManipulatableRectangle.Height = newHeight;
                }

                PositionHandles(); // Update handle positions after resizing
            }
        }
        private void OverlayCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _isResizing = false;
            _isRotating = false;
            Mouse.Capture(null);
        }

        private bool IsMouseOverResizeHandle(System.Windows.Point mousePosition)
        {
            return HandleTopLeft.IsMouseOver || HandleTopRight.IsMouseOver || HandleBottomLeft.IsMouseOver || HandleBottomRight.IsMouseOver;
        }
        private bool IsMouseOverHandle(System.Windows.Point mousePosition, Rectangle handle)
        {
            double left = Canvas.GetLeft(handle);
            double top = Canvas.GetTop(handle);
            return (mousePosition.X >= left && mousePosition.X <= left + HandleSize &&
                    mousePosition.Y >= top && mousePosition.Y <= top + HandleSize);
        }

        // Resize handle events
        private void Handle_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Rectangle)
            {
                Cursor = Cursors.SizeAll; // Change cursor to indicate resizing
            }
        }

        private void Handle_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow; // Reset cursor when leaving the handle
        }

        private void Handle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isResizing = true; // Start resizing
            Mouse.Capture((Rectangle)sender); // Capture mouse to the handle
        }

        // Rotation handle events
        private void RotationHandle_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand; // Change cursor to indicate rotation

            if (_isRotating)
            {
                System.Windows.Point currentMousePosition = e.GetPosition(OverlayCanvas);
                double centerX = Canvas.GetLeft(ManipulatableRectangle) + ManipulatableRectangle.Width / 2;
                double centerY = Canvas.GetTop(ManipulatableRectangle) + ManipulatableRectangle.Height / 2;

                // Calculate angle based on the difference from the previous mouse position
                double previousAngle = Math.Atan2(_startPoint.Y - centerY, _startPoint.X - centerX) * (180 / Math.PI);
                double currentAngle = Math.Atan2(currentMousePosition.Y - centerY, currentMousePosition.X - centerX) * (180 / Math.PI);
                _rotationAngle += currentAngle - previousAngle;

                // Apply rotation to rectangle
                RotateRectangle(_rotationAngle);

                // Update position for next mouse move event
                _startPoint = currentMousePosition;

                PositionHandles(); // Update handle positions after rotation
            }
        }

        private void RotationHandle_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow; // Reset
        }

        private void RotationHandle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isRotating = true; // Start rotating
            _startPoint = e.GetPosition(OverlayCanvas);
            Mouse.Capture(RotationHandle); // Capture mouse to the rotation handle
        }
    }
}
