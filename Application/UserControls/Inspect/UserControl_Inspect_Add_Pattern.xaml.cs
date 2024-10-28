using OpenCvSharp;
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
using static Tensorflow.GraphTransferInfo.Types;

namespace MainApp.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl_Inspect_Add_Pattern.xaml
    /// </summary>
    public partial class UserControl_Inspect_Add_Pattern : UserControl
    {
		public Mat Destination
		{
			get { return (Mat)GetValue(AddPatternProperty); }
			set { SetValue(AddPatternProperty, value); }
		}

		public static readonly DependencyProperty AddPatternProperty =
		DependencyProperty.Register("Destination", typeof(Mat), typeof(UserControl_Inspect_Add_Pattern));
		private readonly InspectService _inspectService;
        private UserControl_Inspect _userControlInspect;
        public UserControl_Inspect_Add_Pattern(UserControl_Inspect userControlInspect, Mat destination)
        {
            InitializeComponent();
			_inspectService = ServiceLocator.ResolveSingleton<InspectService>();
            _inspectService.RectangleVisible = true;
			Destination = destination;
            _userControlInspect = userControlInspect;

            //OpenCvSharp.Rect rect = new OpenCvSharp.Rect(new OpenCvSharp.Point(0, 0), new OpenCvSharp.Size(100, 100));
            //Mat tmp = Destination;
            //Cv2.Rectangle(tmp, rect, new Scalar(255, 255, 255));
            //Destination = tmp;
        }

        public Mat CropRotatedRect(Mat sourceImage, RotatedRect roi)
        {
            // 1. Get the bounding rectangle of the rotated rect to handle out-of-bounds cases
            OpenCvSharp.Rect boundingRect = roi.BoundingRect();
            boundingRect = boundingRect & new OpenCvSharp.Rect(0, 0, sourceImage.Width, sourceImage.Height); // Clip to image bounds

            // 2. Define a matrix for the rotation and apply it to extract the ROI
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(roi.Center, roi.Angle, 1.0);

            // 3. Rotate the entire image to align the rotated rectangle with the axes
            Mat rotatedImage = new Mat();
            Cv2.WarpAffine(sourceImage, rotatedImage, rotationMatrix, sourceImage.Size());

            // 4. Crop the aligned bounding rectangle from the rotated image
            Mat extractedRoi = new Mat(rotatedImage, boundingRect);

            // 5. Resize to the size of the original rotated rect (optional)
            Mat finalRoi = new Mat();
            Cv2.Resize(extractedRoi, finalRoi, (OpenCvSharp.Size)roi.Size);

            return finalRoi;
        }
        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            _inspectService.RectangleVisible = false;

            PatternTool patternTool = new PatternTool();
            patternTool.Template = CropRotatedRect(_inspectService.Destination, _inspectService.InspectModel.Roi);

            _inspectService.InspectModel.Tools.Add(patternTool);
            _userControlInspect.ContentControl.Content = new UserControl_Inspect_Hierarchy(_userControlInspect);
            
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            _inspectService.RectangleVisible = false;
            _userControlInspect.ContentControl.Content = new UserControl_Inspect_Hierarchy(_userControlInspect);
        }
    }
}
