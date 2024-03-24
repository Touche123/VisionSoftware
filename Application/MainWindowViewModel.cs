using Microsoft.Win32;
using OneOf.Types;
using OpenCvSharp;
using OpenCvSharp.Features2D;
using OpenCvSharp.Flann;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Tensorflow;
using System.ComponentModel;
using System.Collections.ObjectModel;
using static Tensorflow.GraphTransferInfo.Types;
using OpenCvSharp.XFeatures2D;

namespace MainApp
{
	public class Thumbnail : BindableBase
	{
		private string _imagePath;
		private BitmapImage _image;

		public event PropertyChangedEventHandler? PropertyChanged;

		public string ImagePath
		{
			get { return _imagePath; }
			set 
			{ 
				_imagePath = value;
				OnPropertyChanged(nameof(ImagePath));
			}
		}
		public BitmapImage Image
		{
			get { return _image; }
			set 
			{ 
				_image = value;
				OnPropertyChanged(nameof(Image));
			}
		}
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

    public class MainWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

		public event PropertyChangedEventHandler? PropertyChanged;
		private ObservableCollection<Thumbnail> _thumbnails;
		public ObservableCollection<Thumbnail> Thumbnails
		{
			get { return _thumbnails; }
			set
			{
				_thumbnails = value;
				OnPropertyChanged(nameof(Thumbnails));
			}
		}

		private ObservableCollection<object> _selectedImages;
		public ObservableCollection<object> SelectedImages
		{
			get { return _selectedImages; }
			set
			{
				_selectedImages = value;
				OnPropertyChanged(nameof(SelectedImages));
                if (_selectedImages.Count != 0)
    				Ncc.LoadDestination(SelectedImages[0].ToString());
			}
		}

        public NCC Ncc { get; set; }
		private Mat destination;
		private double canvasWidth;
		private double canvasHeight;
		private double zoomViewboxWidth;
		private double zoomViewboxHeight;
		UserControl _toolContent;
		InspectModel _inspectModel;

		public UserControl ToolContent
		{
			get { return _toolContent; }
			set 
			{ 
				_toolContent = value;
				OnPropertyChanged(nameof(ToolContent));
			}
		}

		Mat Template { get; set; }
		/// <summary>
		/// Image to be processed
		/// </summary>
		public Mat Destination
		{
			get { return destination; }
			set
			{
				destination = value;
				CanvasWidth = destination.Width;
				CanvasHeight = destination.Height;
				ZoomViewboxWidth = destination.Width;
				ZoomViewboxHeight = destination.Height;

				OnPropertyChanged(nameof(Destination));
			}
		}

		public void MenuSelectCommandExecute(string context)
		{
			switch (context)
			{
				case "image":
					ToolContent = new UserControl_Image();
					break;
				case "inspect":
					ToolContent = new UserControl_Inspect(_inspectModel);
					break;

				default:
					break;
			}
		}
		public double CanvasWidth
		{
			get { return canvasWidth; }
			set
			{
				canvasWidth = value;
				OnPropertyChanged(nameof(CanvasWidth));
			}
		}

		public double CanvasHeight
		{
			get { return canvasHeight; }
			set
			{
				canvasHeight = value;
				OnPropertyChanged(nameof(CanvasHeight));
			}
		}

		public double ZoomViewboxWidth
		{
			get { return zoomViewboxWidth; }
			set
			{
				if (zoomViewboxWidth != value)
				{
					zoomViewboxWidth = value;
					OnPropertyChanged(nameof(ZoomViewboxWidth));
				}
			}
		}

		public double ZoomViewboxHeight
		{
			get { return zoomViewboxHeight; }
			set
			{
				if (zoomViewboxHeight != value)
				{
					zoomViewboxHeight = value;
					OnPropertyChanged(nameof(ZoomViewboxHeight));
				}
			}
		}

		public void OnMouseWheel(int delta)
		{

			double zoomIncrement = 10 * (ZoomViewboxWidth + ZoomViewboxHeight) / 200;

			UpdateViewBox(delta > 0 ? zoomIncrement : -zoomIncrement);
		}

		private void UpdateViewBox(double newValue)
		{
			if ((ZoomViewboxWidth >= 0) && ZoomViewboxHeight >= 0)
			{
				ZoomViewboxWidth += newValue;
				ZoomViewboxHeight += newValue;
			}
		}

		public MainWindowViewModel()
        {
			
            Ncc = new NCC();
			_inspectModel = new InspectModel();
			//LoadCommand = new DelegateCommand<string>(Ncc.LoadExecute);
			LoadCommand = new DelegateCommand<string>(LoadExecute);
			TrainCommand = new DelegateCommand(Ncc.TrainTemplate);
            //SearchCommand = new DelegateCommand(Ncc.MatchSearch);
			SearchCommand = new DelegateCommand(SurfTest);
			Thumbnails = new ObservableCollection<Thumbnail>();
			SelectedImages = new ObservableCollection<object>();
			MenuSelectCommand = new DelegateCommand<string>(MenuSelectCommandExecute);
			LoadImagesFromDirectory("C:\\dev\\c#\\Vision Software\\Application\\bin\\Debug\\net6.0-windows\\assets\\images\\Cognex Block Images");
			AllocConsole();

            //Test test = new Test();

            //DNNWindowViewModel dNNWindowViewModel = new DNNWindowViewModel();
            //dNNWindowViewModel.Run();
        }

        void SurfTest()
        {
			// Convert the template image to grayscale if it's not already
			if (Template.Channels() > 1)
			{
				Cv2.CvtColor(Template, Template, ColorConversionCodes.BGR2GRAY);
			}

			var surf = SURF.Create(100);
			var keypointsTemplate = new KeyPoint[0];
			var keypointsDestination = new KeyPoint[0];
			Mat descriptorsTemplate = new Mat(), descriptorsDestination = new Mat();

			surf.DetectAndCompute(Template, null, out keypointsTemplate, descriptorsTemplate);
			surf.DetectAndCompute(Destination, null, out keypointsDestination, descriptorsDestination);

			// Match descriptors using FLANN matcher
			var matcher = new FlannBasedMatcher();
			var knnMatches = matcher.KnnMatch(descriptorsTemplate, descriptorsDestination, 2);

			// Filter matches using ratio test
			List<DMatch> goodMatches = new List<DMatch>();
			float ratioThresh = 0.7f;
			foreach(var match in knnMatches)
			{
				if (match[0].Distance < ratioThresh * match[1].Distance)
				{
					goodMatches.Add(match[0]);
				}
			}

			// Refine keypoints to subpixel accuracy
			var criteria = new TermCriteria(CriteriaTypes.MaxIter, 30, 0.01);
			var subpixelSize = new Size(5, 5); // Adjust according to the patch size
			foreach (var keypoint in keypointsTemplate)
			{
				Cv2.CornerSubPix(Template, new[] { keypoint.Pt }, subpixelSize, new Size(-1, -1), criteria);
			}

			foreach (var keypoint in keypointsDestination)
			{
				Cv2.CornerSubPix(Destination, new[] { keypoint.Pt }, subpixelSize, new Size(-1, -1), criteria);
			}

			// Estimate affine transformation using RANSAC
			var pointsTemplate = keypointsTemplate.Select(kp => kp.Pt).ToArray();
			var pointsLarger = keypointsDestination.Select(kp => kp.Pt).ToArray();
			var affineTransform = Cv2.GetAffineTransform(pointsTemplate, pointsLarger);

			// Apply the affine transformation to the template image
			Mat transformedTemplate = new Mat();
			Cv2.WarpAffine(Template, transformedTemplate, affineTransform, Destination.Size());

			// Draw correspondences on the images
			var matchesImage = new Mat();
			Cv2.DrawMatches(Template, keypointsTemplate, Destination, keypointsDestination, goodMatches, matchesImage);


			// Display the result
			Cv2.ImShow("Correspondences", matchesImage);
			Cv2.ImShow("Transformed Template", transformedTemplate);
			Cv2.WaitKey(0);

		}

		static int RefineKeypoint(Mat image, KeyPoint keypoint)
		{
			// Perform subpixel refinement of keypoints using cornerSubPix
			var points = new[] { keypoint.Pt };
			Cv2.CornerSubPix(image, points, new Size(5, 5), new Size(-1, -1), new TermCriteria(CriteriaTypes.MaxIter | CriteriaTypes.Eps, 30, 0.01));
			return 0; // As CornerSubPix directly modifies the keypoints, there's no need to return anything
		}

		public void LoadExecute(string i)
		{
			OpenFileDialog dialog = new()
			{
				InitialDirectory = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "assets"),
				Filter = "All|*.*|jpg|*.jpg|png|*.png|bmp|*.bmp"
			};
			if (dialog.ShowDialog().Value == true)
			{
				if (dialog.CheckFileExists)
				{
					string file = dialog.FileName;
					if (i.ToLower().Contains('t'))
					{

						Template?.Dispose();
						Template = Cv2.ImRead(file, ImreadModes.Grayscale);
						Template = new Mat(file);
					}
					else
					{
						Destination?.Dispose();
						Destination = Cv2.ImRead(file, ImreadModes.Grayscale);
						//Destination = new Mat(file);
					}
				}
			}
		}


		private void LoadImagesFromDirectory(string directoryPath)
		{
			if (Directory.Exists(directoryPath))
			{
				string[] imageFiles = Directory.GetFiles(directoryPath, "*.bmp"); // Change the file extension as needed

				foreach (string imagePath in imageFiles)
				{
					Thumbnail thumbnail = new Thumbnail();
					thumbnail.ImagePath = Path.GetFileName(imagePath);
					thumbnail.Image = CreateThumbnail(imagePath, 200, 150); // Create a thumbnail
					//BitmapImage thumbnail = CreateThumbnail(imagePath, 200, 150); // Create a thumbnail
					Thumbnails.Add(thumbnail);
				}
			}
		}

		private BitmapImage CreateThumbnail(string imagePath, int width, int height)
		{
			using var image = new System.Drawing.Bitmap(imagePath);
			var thumbnail = image.GetThumbnailImage(width, height, null, IntPtr.Zero);
			using var ms = new MemoryStream();
			thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Save the thumbnail in memory stream
			var bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
			bitmapImage.EndInit();
			return bitmapImage;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ICommand LoadCommand { get; private set; }
        public ICommand TrainCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
		public ICommand MenuSelectCommand { get; private set; }
        
    }

    public class PointInfo
    {
        /// <summary>
        /// Point of edge 
        /// </summary>
        /// <remarks>
        /// (x,y)
        /// </remarks>
        public Point Point;

        /// <summary>
        /// Center of edge 
        /// </summary>
        /// <remarks>
        /// (x0,y0)
        /// </remarks>
        public Point2d Center { get; private set; }

        /// <summary>
        /// Point-Center 
        /// </summary>
        /// <remarks>
        /// (x-x0,y-y0)
        /// </remarks>
        public Point2d Offset { get; private set; }

        /// <summary>
        /// Derivative at Point
        /// </summary>
        /// <remarks>
        /// (dx,dy)
        /// </remarks>
        public Point2d Derivative;

        /// <summary>
        /// Magnitude at Point
        /// </summary>
        /// <remarks>
        /// 1/√(dx²+dy²)
        /// </remarks>
        public double Magnitude;

        /// <summary>
        /// Direction at Point
        /// </summary>
        /// <remarks>
        /// atan2(dy,dx) (not currently in use)
        /// </remarks>
        public double Direction;

        /// <summary>
        /// Calc Offset with Point by center
        /// </summary>
        /// <param name="center"></param>
        public void Update(Point2d center)
        {
            Center = center;
            Offset = Point - center;
        }

    }
}
