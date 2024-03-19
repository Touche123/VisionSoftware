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

namespace MainApp
{
    public class MainWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

		public event PropertyChangedEventHandler? PropertyChanged;
		private ObservableCollection<BitmapImage> _thumbnails;
		public ObservableCollection<BitmapImage> Thumbnails
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

		public MainWindowViewModel()
        {
            Ncc = new NCC();
            LoadCommand = new DelegateCommand<string>(Ncc.LoadExecute);
            TrainCommand = new DelegateCommand(Ncc.TrainTemplate);
            SearchCommand = new DelegateCommand(Ncc.MatchSearch);
			Thumbnails = new ObservableCollection<BitmapImage>();
			SelectedImages = new ObservableCollection<object>();
			LoadImagesFromDirectory("C:\\dev\\c#\\Vision Software\\Application\\bin\\Debug\\net6.0-windows\\assets\\images\\Cognex Block Images");
			AllocConsole();

            //Test test = new Test();

            //DNNWindowViewModel dNNWindowViewModel = new DNNWindowViewModel();
            //dNNWindowViewModel.Run();
        }

		

		private void LoadImagesFromDirectory(string directoryPath)
		{
			if (Directory.Exists(directoryPath))
			{
				string[] imageFiles = Directory.GetFiles(directoryPath, "*.bmp"); // Change the file extension as needed

				foreach (string imagePath in imageFiles)
				{
					BitmapImage thumbnail = CreateThumbnail(imagePath, 200, 150); // Create a thumbnail
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
