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

		public UserControl_Inspect_Add_Pattern(UserControl_Inspect userControlInspect, Mat destination, InspectModel inspectModel)
        {
            InitializeComponent();
            Destination = destination;

            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(new OpenCvSharp.Point(0, 0), new OpenCvSharp.Size(100, 100));
            Mat tmp = Destination;
            Cv2.Rectangle(tmp, rect, new Scalar(255, 255, 255));
            Destination = tmp;
        }
    }
}
