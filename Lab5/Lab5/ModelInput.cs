using Microsoft.ML.Data;

namespace Lab5
{
    public class ModelInput
    {
        [ColumnName("PixelValues"), LoadColumn(0, 63)]
        [VectorType(64)]
        public float[] PixelValues;


        [ColumnName("Number"), LoadColumn(64)]
        public float Number { get; set; }

    }
}
