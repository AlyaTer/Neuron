using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.ML;

namespace Lab5
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

        private static string TRAIN_DATA_FILEPATH = @"D:/AI/Lab4/Lab5/Lab5/bin/Debug/dataset.csv";
        private static string MODEL_FILEPATH = @"D:/AI/Lab4/Lab5/Lab5/bin/Debug/MLModel.zip";


        private static string PATH_TO_TEST_DATA = @"D:/AI/Lab4/Lab5/Lab5/bin/Debug/testData/testDataset.csv";
        //Dataset to use for predictions 
        private const string DATA_FILEPATH = @"D:/AI/Lab4/Lab5/Lab5/bin/Debug/dataset.csv";

        // Create MLContext to be shared across the model creation workflow objects 
        // Set a random seed for repeatable/deterministic results across multiple trainings.
        private static MLContext mlContext = new MLContext(seed: 1);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fontsList = new List<string>() { "Arial", "Arial Narrow", "Calibri", "Times New Roman", "Calibri Light" };
            var fontStyles = new List<System.Drawing.FontStyle>() { new System.Drawing.FontStyle() };   //check
            var pathDataset = @"dataset.csv";
            var drawString = "ANKO";
            var backgroundColor = "ffffffff";
            var saveFiles = true;
            var filesCatalogName = "images";

            var tw = new StreamWriter(pathDataset);
            tw.WriteLine("PixelValues,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,Number");

            if (saveFiles)
            {
                var dn = new DirectoryInfo(filesCatalogName);
                if (!dn.Exists)
                {
                    dn.Create();
                }
            }

            foreach (var c in drawString)
            {
                foreach (var fontName in fontsList)
                {
                    for (int size = 10; size < 26; size++)
                    {
                        foreach (var fontStyle in fontStyles)
                        {
                            for (int angle = -13; angle < 13; angle++)
                            {
                                var res = new Bitmap(32, 32);

                                // Create font and brush.
                                Font drawFont = new Font(fontName, size, fontStyle);
                                var drawBrush = new SolidBrush(Color.Black);

                                // Create point for upper-left corner of drawing.
                                float x = 1;
                                float y = 1;

                                // Set format of string.
                                StringFormat drawFormat = new StringFormat();

                                using (var g = Graphics.FromImage(res))
                                {
                                    g.Clear(Color.White);
                                    g.RotateTransform(angle); // set up rotate
                                    g.DrawString(c.ToString(), drawFont, drawBrush, x, y, drawFormat);

                                }
                                if (saveFiles)
                                {
                                    var path = $"{filesCatalogName}\\{c}_{Guid.NewGuid()}.png";
                                    res.Save(path);
                                }

                                var handwritingRecognition = new HandwritingRecognition();

                                var datasetValue = handwritingRecognition.GetDatasetValues(res, backgroundColor);

                                tw.WriteLine($"{string.Join(",", datasetValue)},{c - 'A'}");

                            }

                        }
                    }
                }
            }
            tw.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            MLContext mlContext = new MLContext();

            // Training code used by ML.NET CLI and AutoML to generate the model
            //ModelBuilder.CreateModel();

            ITransformer mlModel = mlContext.Model.Load(GetAbsolutePath(MODEL_FILEPATH), out DataViewSchema inputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            // Create sample data to do a single prediction with it 
            ModelInput sampleData = CreateSingleDataSample(mlContext, PATH_TO_TEST_DATA);

            // Try a single prediction
            ModelOutput predictionResult = predEngine.Predict(sampleData);

            Console.WriteLine($"Single Prediction --> Actual value: {sampleData.Number} | Predicted value: {predictionResult.Prediction} | Predicted scores: [{String.Join(",", predictionResult.Score)}]");

            Console.WriteLine("=============== End of process, hit any key to finish ===============");
        }

        // Method to load single row of data to try a single prediction
        // You can change this code and create your own sample data here (Hardcoded or from any source)
        private static ModelInput CreateSingleDataSample(MLContext mlContext, string dataFilePath)
        {
            // Read dataset to get a single row for trying a prediction          
            IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                                            path: dataFilePath,
                                            hasHeader: true,
                                            separatorChar: ',',
                                            allowQuoting: true,
                                            allowSparse: false);

            // Here (ModelInput object) you could provide new test data, hardcoded or from the end-user application, instead of the row from the file.
            ModelInput sampleForPrediction = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false)
                                                                        .First();
            return sampleForPrediction;
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(MainWindow).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var tw = new StreamWriter(PATH_TO_TEST_DATA);
            tw.WriteLine("PixelValues,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,Number");
            var image1 = new Bitmap(@"D:/AI/Lab4/Lab5/Lab5/bin/Debug/testData/A_fecdc7b7-47c1-4a69-92f8-7a1f66261453.png", true);
            var handwritingRecognition = new HandwritingRecognition();
            var backgroundColor = "ffffffff";
            var datasetValue = handwritingRecognition.GetDatasetValues(image1, backgroundColor);
            tw.WriteLine($"{string.Join(",", datasetValue)},{'O' - 'A'}");
            tw.Close();
        }
    }
}
