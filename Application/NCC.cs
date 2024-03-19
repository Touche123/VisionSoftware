using Microsoft.Win32;
using OneOf.Types;
using OpenCvSharp;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow.Keras.Engine;
using static Tensorflow.GraphTransferInfo.Types;

namespace MainApp
{
	public class Model
	{
		public List<ModelData> Data = new();
	}

	public class ModelData
	{
		public int PyramidLevel;
		public List<PointInfo> Points = new();
	}

	public class NCC : BindableBase
	{
		/// <summary>
		/// Image used to create template
		/// </summary>
		public Mat Template
		{
			get { return _template; }
			set
			{
				_template = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// Image to be processed
		/// </summary>
		public Mat Destination
		{
			get { return _destination; }
			set
			{
				_destination = value;
				RaisePropertyChanged();
			}
		}

		private Mat _template;
		private Mat _destination;
		private Model _model;
		private readonly List<Point> _results = new();
		private readonly List<PointInfo> _pointData = new();

		public void TrainTemplate()
		{
			_model = new Model();

			Mat src = new();
			Mat output = new();
			Mat gx = new();
			Mat gy = new();
			Mat magnitude = new();
			Mat direction = new();

			_pointData.Clear();

			Cv2.CvtColor(_template, src, ColorConversionCodes.RGB2GRAY);

			Cv2.Canny(src, output, 100, 100 * 3);
			Cv2.FindContours(output, out var contours, out var hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxNone);

			Cv2.Sobel(src, gx, MatType.CV_64F, 1, 0, 3);
			Cv2.Sobel(src, gy, MatType.CV_64F, 0, 1, 3);

			Cv2.CartToPolar(gx, gy, magnitude, direction);

			var sum = new Point2d(0, 0);

			for (int p = 4; p > 0; p--)
			{
				for (int i = 0, m = contours.Length; i < m; i++)
				{
					for (int j = 0, n = contours[i].Length; j < n; j++)
					{
						var cur = contours[i][j];
						var fdx = gx.At<double>(cur.Y, cur.X, 0); // dx
						var fdy = gy.At<double>(cur.Y, cur.X, 0); // dy
						var der = new Point2d(fdx, fdy); // (dx,dy)
						var mag = magnitude.At<double>(cur.Y, cur.X, 0); // √(dx²+dy²)
						var dir = direction.At<double>(cur.Y, cur.X, 0); // atan2(dy,dx)
						_pointData.Add(new PointInfo
						{
							Point = cur,
							Derivative = der,
							Direction = dir,
							Magnitude = mag == 0 ? 0 : 1 / mag,
						});
						sum += cur;
					}
				}

				/// update Center and Offset in PointInfo
				var center = new Point2d(sum.X / _pointData.Count, sum.Y / _pointData.Count);
				foreach (var item in _pointData)
				{
					item.Update(center);
				}

				_model.Data.Add(new ModelData { PyramidLevel = p, Points = _pointData });

				Cv2.PyrDown(src, src);
			}

			///// overlay display origin image, edge(green) and center point(red)
			//Cv2.DrawContours(template, new[] { results.Select(_ => _.Point) }, -1, Scalar.LightGreen, 2);
			////Cv2.DrawContours(template, contours, -1, Scalar.LightGreen, 2);
			//Cv2.Circle(template, center.ToPoint(), 2, Scalar.Red, -1);

			/// update UI
			//RaisePropertyChanged(nameof(Template));

		}

		/// <summary>
		/// NCC to find template
		/// </summary>
		public void MatchSearch()
		{
			Stopwatch stopwatch = new();
			Trace.TraceInformation("NCC matching start");
			stopwatch.Start();

			/// convert to gray image

			Mat src = new();
			Mat gx = new();
			Mat gy = new();
			Mat direction = new();
			Mat magnitude = new();
			double resultScore = 0;

			for (int p = 4; p > 0; p--)
			{
				Cv2.CvtColor(_destination, src, ColorConversionCodes.RGB2GRAY);

				/// use the sobel filter on the source image which returns the gradients in the X (Gx) and Y (Gy) direction.
				Cv2.Sobel(src, gx, MatType.CV_64F, 1, 0, 3);
				Cv2.Sobel(src, gy, MatType.CV_64F, 0, 1, 3);

				/// compute the magnitude and direction
				Cv2.CartToPolar(gx, gy, magnitude, direction);

				var minScore = 0.7;
				var greediness = 0.8;

				/// ncc match search
				long noOfCordinates = _pointData.Count;
				double normMinScore = minScore / noOfCordinates; // normalized min score
				double normGreediness = (1 - greediness * minScore) / (1 - greediness) / noOfCordinates;
				double partialScore = 0;

				Point center = new();

				for (int i = 0, h = src.Height; i < h; i++)
				{
					for (int j = 0, w = src.Width; j < w; j++)
					{
						double partialSum = 0;
						for (var m = 0; m < noOfCordinates; m++)
						{
							var item = _pointData[m];
							var curX = (int)(j + item.Offset.X);
							var curY = (int)(i + item.Offset.Y);
							var iTx = item.Derivative.X;
							var iTy = item.Derivative.Y;
							if (curX < 0 || curY < 0 || curY > src.Height - 1 || curX > src.Width - 1)
								continue;

							var iSx = gx.At<double>(curY, curX, 0);
							var iSy = gy.At<double>(curY, curX, 0);

							if ((iSx != 0 || iSy != 0) && (iTx != 0 || iTy != 0))
							{
								var mag = magnitude.At<double>(curY, curX, 0);
								var matGradMag = mag == 0 ? 0 : 1 / mag; // 1/√(dx²+dy²)
								partialSum += ((iSx * iTx) + (iSy * iTy)) * (item.Magnitude * matGradMag);
							}

							var sumOfCoords = m + 1;
							partialScore = partialSum / sumOfCoords;
							/// check termination criteria
							/// if partial score score is less than the score than needed to make the required score at that position
							/// break serching at that coordinate.
							if (partialScore < Math.Min((minScore - 1) + normGreediness * sumOfCoords, normMinScore * sumOfCoords))
								break;
						}
						if (partialScore > resultScore)
						{
							resultScore = partialScore;
							center.X = j * 8;
							center.Y = i * 8;

							_results.Add(center);
						}
					}
				}

				Cv2.PyrDown(src, src);
			}


			/// overlay display origin image, edge(green) and center point(red)
			//Cv2.DrawContours(destination, new[] { results.Select(_ => _.Offset.ToPoint() * 8) }, -1, Scalar.LightGreen, 2, offset: center);
			//Cv2.Circle(destination, center, 5, Scalar.Red, -1);
			Trace.TraceInformation($"NCC matching score {resultScore}. time: {stopwatch.Elapsed.TotalMilliseconds} ms");
			//RaisePropertyChanged(nameof(Destination));
			stopwatch.Stop();
		}

		/// <summary>
		/// Load template and destination image
		/// </summary>
		/// <param name="i"></param>
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
						Template = new Mat(file);
					}
					else
					{
						Destination?.Dispose();
						Destination = new Mat(file);
					}
				}
			}
		}

		public void LoadDestination(string path)
		{
			Destination?.Dispose();
			Destination = new Mat(path);
		}
	}
}
