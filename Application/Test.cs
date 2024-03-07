using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.Utils;
using Tensorflow.NumPy;

using System.IO;
using static Tensorflow.Keras.Engine.InputSpec;
using System.Drawing;
using System.Drawing.Drawing2D;
using Tensorflow.Keras.Datasets;
using SciSharp.Models.ObjectDetection;
using SciSharp.Models;

namespace MainApp
{
    internal class Test
    {
        tensorflow tf = new tensorflow();
        YoloConfig cfg;

        float accuracy_test = 0f;
        public float MIN_SCORE = 0.5f;
        
        YoloDataset trainingData, testingData;

        public Test()
        {
            cfg = new YoloConfig("YOLOv3");
            (trainingData, testingData) = PrepareData();
            Train();
            TestImage();
        }

        public void Train()
        {
            // using wizard to train model
            var wizard = new ModelWizard();
            var task = wizard.AddObjectDetectionTask<YOLOv3>(new TaskOptions
            {
                InputShape = (28, 28, 1),
                NumberOfClass = 10,
            });
            task.SetModelArgs(cfg);

            task.Train(new YoloTrainingOptions
            {
                TrainingData = trainingData,
                TestingData = testingData
            });
        }
        public void TestImage()
        {
            var wizard = new ModelWizard();
            var task = wizard.AddObjectDetectionTask<YOLOv3>(new TaskOptions
            {
                ModelPath = @"./assets/yolov3.h5"
            });
            task.SetModelArgs(cfg);
            var result = task.Test(new TestingOptions
            {

            });
            accuracy_test = result.Accuracy;
        }

        public (YoloDataset, YoloDataset) PrepareData()
        {
            string dataDir = Path.Combine("YOLOv3", "data");
            Directory.CreateDirectory(dataDir);

            var trainset = new YoloDataset("train", cfg);
            var testset = new YoloDataset("test", cfg);
            return (trainset, testset);
        }

    }
}
