using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accord.Imaging;
using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics;
using Accord.Statistics.Analysis;

namespace Diploma
{
    public partial class Form1 : Form
    {
        private bool Colour_Mode;
        Bitmap Container_Original_512;
        Bitmap Watermark_Original_128;
        Bitmap Container_Reconstructed_512;
        Bitmap Watermark_Reconstructed_128;
        Bitmap Watermark_Difference;
        Bitmap Watermark_Difference_Filtered;
        Bitmap Container_Difference;
        double[,] Key_Eigenvectors_Grayscale;
        double[,] Key_Eigenvectors_Colourful_R;
        double[,] Key_Eigenvectors_Colourful_G;
        double[,] Key_Eigenvectors_Colourful_B;
        double[] Key2_Singularvalues;
        string Key_Eigenvectors_Grayscale_Read;
        string Key_Eigenvectors_Colourful_R_Read;
        string Key_Eigenvectors_Colourful_G_Read;
        string Key_Eigenvectors_Colourful_B_Read;

        string Key2_Singularvalues_Read;
        int Component_Number;
        double TMP_MEAN = 0;

        // функція для знаходження PSNR
        private double ComparePSNR(Bitmap originalBitmap, Bitmap processedBitmap)
        {
            int rows2 = 0, col2 = 0;

            Colour_Mode = radioButtonGrayscale.Checked;
            col2 = originalBitmap.Width;
            if (Colour_Mode == true)
                rows2 = originalBitmap.Height;
            else
                rows2 = originalBitmap.Height * 3;

            double MSE = 0;
            double PSNR = 0;

            int originalPixel = 0;
            int processedPixel = 0;

            byte originalPixelR = 0;
            byte originalPixelG = 0;
            byte originalPixelB = 0;
            byte processedPixelR = 0;
            byte processedPixelG = 0;
            byte processedPixelB = 0;

            //чорнобілі
            if (Colour_Mode == true)
            {
                for (int i = 0; i < rows2; i++)
                    for (int j = 0; j < col2; j++)
                    {
                        processedPixel = processedBitmap.GetPixel(i, j).ToArgb() & 0x000000ff;
                        originalPixel = originalBitmap.GetPixel(i, j).ToArgb() & 0x000000ff;
                        MSE += Math.Pow((Math.Abs(processedPixel - originalPixel)), 2);
                    }
                MSE = MSE / (rows2 * col2);
                PSNR = 10 * Math.Log10(255 * 255 / MSE);
            }
            else
            //кольорові
            {
                for (int i = 0; i < rows2 / 3; i++)
                    for (int j = 0; j < col2; j++)
                    {
                        processedPixelR = processedBitmap.GetPixel(i, j).R;
                        processedPixelG = processedBitmap.GetPixel(i, j).G;
                        processedPixelB = processedBitmap.GetPixel(i, j).B;

                        originalPixelR = originalBitmap.GetPixel(i, j).R;
                        originalPixelG = originalBitmap.GetPixel(i, j).G;
                        originalPixelB = originalBitmap.GetPixel(i, j).B;

                        MSE += Math.Pow((Math.Abs(processedPixelR - originalPixelR)), 2);
                        MSE += Math.Pow((Math.Abs(processedPixelG - originalPixelG)), 2);
                        MSE += Math.Pow((Math.Abs(processedPixelB - originalPixelB)), 2);
                    }
                MSE = MSE / (rows2 * col2);
                PSNR = 10 * Math.Log10(255 * 255 / MSE);
            }
            return PSNR;
        }
        public Form1()
        {
            InitializeComponent();
        }

        // кнопка для завантаження оригінального зображення без знаку
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBoxOriginalImage.Image = System.Drawing.Image.FromFile(openFileDialog1.FileName);
                Container_Original_512 = new Bitmap(openFileDialog1.FileName);
            }
        }

        // кнопка для завантаження знаку
        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBoxOriginalWatermark.Image = System.Drawing.Image.FromFile(openFileDialog1.FileName);
                Watermark_Original_128 = new Bitmap(openFileDialog1.FileName);
            }
            label5.Show();
            textBox4.Show();
            buttonEncrypt.Show();
        }

        // кнопка для шифрування
        private void button3_Click(object sender, EventArgs e)
        {
            Colour_Mode = radioButtonGrayscale.Checked;
            //для чорнобілої версії
            if (Colour_Mode == true)
            {                 
                // зчитування пікселів контейнера (лени)
                StringBuilder String_With_Container_Original_Pixels = new StringBuilder();
                for (int x = 0; x < Container_Original_512.Height; x++)
                    for (int y = 0; y < Container_Original_512.Width; y++)
                        String_With_Container_Original_Pixels.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).ToArgb() & 0x000000ff));

                // зчитування пікселів ЦВЗ (бабуїна)
                StringBuilder String_With_WaterMark_Original_Pixels = new StringBuilder();
                for (int i = 0; i < Watermark_Original_128.Height; i++)
                    for (int j = 0; j < Watermark_Original_128.Width; j++)
                        String_With_WaterMark_Original_Pixels.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).ToArgb() & 0x000000ff));

                var Matrix_With_Container_Original_Pixels = new double[Container_Original_512.Height, Container_Original_512.Width];
                int counter = 0;
                for (int i = 0; i < Container_Original_512.Height; i++)
                    for (int j = 0; j < Container_Original_512.Width; j++)
                    {
                        Matrix_With_Container_Original_Pixels[i, j] = String_With_Container_Original_Pixels[counter];
                        counter++;
                    }

                // конвертація основної матриці контейнера 512х512 в 16-ти стовпцеву (16х16384)
                int frame_dimension = 4;
                int columns = frame_dimension * frame_dimension; //4*4
                int rows = (Container_Original_512.Width / frame_dimension) * (Container_Original_512.Height / frame_dimension); //512/4 * 512/4
                var Matrix_With_Container_Original_Pixels_Converted = new double[rows, columns];

                int k1 = 0, l1 = 0, k_max = Container_Original_512.Height, l_max = Container_Original_512.Width;

                int number_of_squares = k_max / frame_dimension;//512/4
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < columns; j++)
                    {
                        //Matrix_Main[i, j] = (double)((byte)LenaOriginal512.GetPixel(((i * frame) / (int)(Math.Sqrt(rows * col))) * frame + j / frame, (i % ((int)(Math.Sqrt(rows * col)) / frame)) * frame + j % frame).ToArgb() & 0x000000ff);
                        k1 = j / frame_dimension + (i / number_of_squares) * frame_dimension;
                        l1 = j % frame_dimension + (i % number_of_squares) * frame_dimension;
                        Matrix_With_Container_Original_Pixels_Converted[i, j] = Matrix_With_Container_Original_Pixels[k1, l1];
                    }

                /*var pca = new PrincipalComponentAnalysis(Matrix_With_Container_Original_Pixels_Converted);
                pca.Compute();*/

                // центрування конвертованої матриці       !!!! тут можлива помилка
                double[] Column_Mean = Matrix_With_Container_Original_Pixels_Converted.Mean(0); // 0-середнє значення по всім колонкам / 1 - по всім рядкам
                TMP_MEAN = Column_Mean[(Component_Number = Int32.Parse(textBox4.Text)) - 1];
                textBox5.Text = Convert.ToString(Column_Mean[(Component_Number = Int32.Parse(textBox4.Text)) - 1]);
                //int tmp_tmp_avg = (Convert.ToInt32(Column_Mean[15]) + 114) / 2;
                //Column_Mean[15] = tmp_tmp_avg;
                //Column_Mean[15] = 114.0; якщо ми хочемо так зробити тоді нам в оригінальне зображення після приведення матриці до 16-ти стовпцевого типу записати ЦВЗ

                var Matrix_With_Container_Original_Pixels_Converted_Centered = Matrix_With_Container_Original_Pixels_Converted.Subtract(Column_Mean, 0); // відняти від значень стовпців середнє значення кожного стовпця

                StreamWriter outfile_Container_Centered_Pixels = new StreamWriter(@"..\\..\\..\\Container_Centered_Pixels.txt");
                for (int i = 0; i < Matrix_With_Container_Original_Pixels_Converted_Centered.GetLength(0); i++)
                {
                    for (int j = 0; j < Matrix_With_Container_Original_Pixels_Converted_Centered.GetLength(1); j++)
                    {
                        outfile_Container_Centered_Pixels.Write("{0} ", Matrix_With_Container_Original_Pixels_Converted_Centered[i, j]);
                    }
                    outfile_Container_Centered_Pixels.Write("{0}", Environment.NewLine);
                }
                outfile_Container_Centered_Pixels.Close();
                //var Matrix_With_Container_Original_Pixels_Converted_Centered = Matrix_With_Container_Original_Pixels_Converted.Subtract(128);
                // SVD
                SingularValueDecomposition svd = new SingularValueDecomposition(Matrix_With_Container_Original_Pixels_Converted_Centered);
                var eigenvectors = svd.RightSingularVectors;
                var Singularvalues = svd.Diagonal;
                Key_Eigenvectors_Grayscale = eigenvectors;
                //Key2_Singularvalues = Singularvalues; // не використовується
                
                //double[] eigenvalues = Singularvalues.Pow(2);// не задіюється
                //eigenvalues = eigenvalues.Divide(Matrix_With_Container_Original_Pixels_Converted.GetLength(0)-1); //не задіюється
                
                // пошук середнього для пікселів бабуїна
                int tmp_sum = 0, tmp_avg = 0;
                for (int i = 0; i < String_With_WaterMark_Original_Pixels.Length; i++)
                    tmp_sum += Convert.ToInt32(String_With_WaterMark_Original_Pixels[i]);
                tmp_avg = tmp_sum / String_With_WaterMark_Original_Pixels.Length; //114
                textBox8.Text = Convert.ToString(tmp_avg);
                //tmp_avg = 128;
                //Column_Mean[15] = tmp_avg;
                // заміна 16-ої компоненти на центровані значення(пікселів) бабуїна
                var Matrix_Main_Components = Matrix_With_Container_Original_Pixels_Converted_Centered.MultiplyByTranspose(eigenvectors.Transpose()); // матриця з головними компонентами               
                for (int i = 0; i < Matrix_Main_Components.GetLength(0); i++)
                {
                    Matrix_Main_Components[i, (Component_Number = Int32.Parse(textBox4.Text))-1] = Convert.ToInt32(String_With_WaterMark_Original_Pixels[i]) - tmp_avg; // поява артефактів
                    //Matrix_Main_Components[i, 15] = 0;
                }
               
                // реконструкція конвертованої матриці  
                var Back_To_Matrix_With_Container_Original_Pixels_Converted_Centered = Matrix_Main_Components.DotWithTransposed(eigenvectors);
                double[,] Matrix_With_Container_Original_Pixels_Reconstructed = Back_To_Matrix_With_Container_Original_Pixels_Converted_Centered.Add(Column_Mean, 0); // поява артефактів
                                
                // повна реконструкція матриці
                var Matrix_With_Container_Original_Pixels_Reconstructed_Fully = new int[Container_Original_512.Height, Container_Original_512.Width];
                int k2 = 0, l2 = 0;
                for (int i = 0; i < Matrix_With_Container_Original_Pixels_Reconstructed.GetLength(0); i++)
                    for (int j = 0; j < Matrix_With_Container_Original_Pixels_Reconstructed.GetLength(1); j++)
                    {
                        k2 = j / frame_dimension + (i / number_of_squares) * frame_dimension;
                        l2 = j % frame_dimension + (i % number_of_squares) * frame_dimension;
                        Matrix_With_Container_Original_Pixels_Reconstructed_Fully[k2, l2] = Convert.ToInt32(Matrix_With_Container_Original_Pixels_Reconstructed[i, j]);   
                    }

                // збереження чорнобілого зображення після певних перетворень
                int tmp = 0;
                for (int i = 0; i < Container_Original_512.Width; i++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        String_With_Container_Original_Pixels[tmp] = (char)(Matrix_With_Container_Original_Pixels_Reconstructed_Fully[i, j]+0.5);
                        //String_With_Container_Original_Pixels[tmp] = Convert.ToChar(Matrix_With_Container_Original_Pixels_Reconstructed_Fully[i, j]);
                        tmp++;
                    }

                int p = 0;
                Container_Reconstructed_512 = new Bitmap(Container_Original_512.Height, Container_Original_512.Width);
                int processedPixel;
                for (int i = 0; i < Container_Original_512.Width; i++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        processedPixel = Convert.ToInt32(String_With_Container_Original_Pixels[p]);
                        if (processedPixel < 0) processedPixel = 0;
                        if (processedPixel > 255) processedPixel = 255;
                        Container_Reconstructed_512.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixel), Convert.ToByte(processedPixel), Convert.ToByte(processedPixel)));
                        p++;
                    }
                Container_Reconstructed_512.Save("..\\..\\..\\Lena_Reconstructed.bmp");
                StringBuilder psnr = new StringBuilder();
                psnr.Append(Convert.ToString(ComparePSNR(Container_Original_512, Container_Reconstructed_512)));
                textBox1.Text = psnr.ToString();
                buttonWriteKey1.Show();
                label6.Show();
                textBox5.Show(); textBox8.Show();
            }
            else
            {
                // ШИФРУВАННЯ КОЛЬОРОВЕ
                // НОВИЙ ВАРІАНТ
                // зчитування пікселів контейнера (лени)
                StringBuilder String_With_Lena_Pixels_Colourful_R = new StringBuilder();
                StringBuilder String_With_Lena_Pixels_Colourful_G = new StringBuilder();
                StringBuilder String_With_Lena_Pixels_Colourful_B = new StringBuilder();
                for (int x = 0; x < Container_Original_512.Width; x++)
                    for (int y = 0; y < Container_Original_512.Height; y++)
                    {
                        String_With_Lena_Pixels_Colourful_R.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).R));
                        String_With_Lena_Pixels_Colourful_G.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).G));
                        String_With_Lena_Pixels_Colourful_B.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).B));
                    }

                // зчитування пікселів ЦВЗ (бабуїна)
                StringBuilder String_With_WaterMark_Pixels_Colourful_R = new StringBuilder();
                StringBuilder String_With_WaterMark_Pixels_Colourful_G = new StringBuilder();
                StringBuilder String_With_WaterMark_Pixels_Colourful_B = new StringBuilder();
                for (int i = 0; i < Watermark_Original_128.Height; i++)
                    for (int j = 0; j < Watermark_Original_128.Width; j++)
                    {
                        String_With_WaterMark_Pixels_Colourful_R.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).R));
                        String_With_WaterMark_Pixels_Colourful_G.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).G));
                        String_With_WaterMark_Pixels_Colourful_B.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).B));
                    }

                // запис пікселів із стрінгів в масиви
                var Matrix_Main_Container_R = new double[Container_Original_512.Height, Container_Original_512.Width];
                var Matrix_Main_Container_G = new double[Container_Original_512.Height, Container_Original_512.Width];
                var Matrix_Main_Container_B = new double[Container_Original_512.Height, Container_Original_512.Width];

                int counter_tmp = 0;
                for (int i = 0; i < Container_Original_512.Height; i++)
                    for (int j = 0; j < Container_Original_512.Width; j++)
                    {
                        Matrix_Main_Container_R[i, j] = String_With_Lena_Pixels_Colourful_R[counter_tmp];
                        Matrix_Main_Container_G[i, j] = String_With_Lena_Pixels_Colourful_G[counter_tmp];
                        Matrix_Main_Container_B[i, j] = String_With_Lena_Pixels_Colourful_B[counter_tmp];
                        counter_tmp++;
                    }

                // перетворення матриць R G B в 16-ти стовпцеві
                int frame_dimension = 4;
                int columns = frame_dimension * frame_dimension; //4*4
                int rows = (Container_Original_512.Width / frame_dimension) * (Container_Original_512.Height / frame_dimension); //512/4 * 512/4
                var Matrix_With_Container_Original_Pixels_Converted_R = new double[rows, columns];
                var Matrix_With_Container_Original_Pixels_Converted_G = new double[rows, columns];
                var Matrix_With_Container_Original_Pixels_Converted_B = new double[rows, columns];
                int k1 = 0, l1 = 0, k_max = Container_Original_512.Height, l_max = Container_Original_512.Width;
                int number_of_squares = k_max / frame_dimension;//512/4
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < columns; j++)
                    {
                        //Matrix_Main[i, j] = (double)((byte)LenaOriginal512.GetPixel(((i * frame) / (int)(Math.Sqrt(rows * col))) * frame + j / frame, (i % ((int)(Math.Sqrt(rows * col)) / frame)) * frame + j % frame).ToArgb() & 0x000000ff);
                        k1 = j / frame_dimension + (i / number_of_squares) * frame_dimension;
                        l1 = j % frame_dimension + (i % number_of_squares) * frame_dimension;
                        Matrix_With_Container_Original_Pixels_Converted_R[i, j] = Matrix_Main_Container_R[k1, l1];
                        Matrix_With_Container_Original_Pixels_Converted_G[i, j] = Matrix_Main_Container_G[k1, l1];
                        Matrix_With_Container_Original_Pixels_Converted_B[i, j] = Matrix_Main_Container_B[k1, l1];
                    }

                // центрування конвертованих матриць R G B                       !!!!!!!!!тут помилка
                double[] Column_Mean_R = Matrix_With_Container_Original_Pixels_Converted_R.Mean(0);
                double[] Column_Mean_G = Matrix_With_Container_Original_Pixels_Converted_G.Mean(0);
                double[] Column_Mean_B = Matrix_With_Container_Original_Pixels_Converted_B.Mean(0);
                textBox5.Text = Convert.ToString(Column_Mean_R[(Component_Number = Int32.Parse(textBox4.Text)) - 1]);
                textBox6.Text = Convert.ToString(Column_Mean_G[(Component_Number = Int32.Parse(textBox4.Text)) - 1]);
                textBox7.Text = Convert.ToString(Column_Mean_B[(Component_Number = Int32.Parse(textBox4.Text)) - 1]);
                /*
                // пошук середнього для пікселів ЦВЗ R G B (бабуїна)
                int tmp_sum_R = 0, tmp_sum_G = 0, tmp_sum_B = 0;
                int tmp_avg_R = 0, tmp_avg_G = 0, tmp_avg_B = 0;
                for (int i = 0; i < String_With_WaterMark_Pixels_Colourful_R.Length; i++)
                {
                    tmp_sum_R += Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_R[i]);
                    tmp_sum_G += Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_G[i]);
                    tmp_sum_B += Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_B[i]);
                }
                tmp_avg_R = tmp_sum_R / String_With_WaterMark_Pixels_Colourful_R.Length;
                tmp_avg_G = tmp_sum_G / String_With_WaterMark_Pixels_Colourful_G.Length;
                tmp_avg_B = tmp_sum_B / String_With_WaterMark_Pixels_Colourful_B.Length;

                textBox8.Text = Convert.ToString(tmp_avg_R);
                textBox9.Text = Convert.ToString(tmp_avg_G);
                textBox10.Text = Convert.ToString(tmp_avg_B);

                int tmp_tmp_avg_R = (Convert.ToInt32(Column_Mean_R[15]) + tmp_avg_R) / 2;
                int tmp_tmp_avg_G = (Convert.ToInt32(Column_Mean_G[15]) + tmp_avg_G) / 2;
                int tmp_tmp_avg_B = (Convert.ToInt32(Column_Mean_B[15]) + tmp_avg_B) / 2;

                Column_Mean_R[15] = tmp_tmp_avg_R;
                Column_Mean_G[15] = tmp_tmp_avg_G;
                Column_Mean_B[15] = tmp_tmp_avg_B;
                */
                var Matrix_Main_Converted_Centered_R = Matrix_With_Container_Original_Pixels_Converted_R.Subtract(Column_Mean_R, 0);
                var Matrix_Main_Converted_Centered_G = Matrix_With_Container_Original_Pixels_Converted_G.Subtract(Column_Mean_G, 0);
                var Matrix_Main_Converted_Centered_B = Matrix_With_Container_Original_Pixels_Converted_B.Subtract(Column_Mean_B, 0);
                

                //SVD R G B
                SingularValueDecomposition svd_R = new SingularValueDecomposition(Matrix_Main_Converted_Centered_R);
                var eigenvectors_R = svd_R.RightSingularVectors;
                //var Singularvalues_R = svd_R.Diagonal;
                Key_Eigenvectors_Colourful_R = eigenvectors_R;
                //Key2_Singularvalues = Singularvalues;
                SingularValueDecomposition svd_G = new SingularValueDecomposition(Matrix_Main_Converted_Centered_G);
                var eigenvectors_G = svd_G.RightSingularVectors;
                //var Singularvalues_G = svd_G.Diagonal;
                Key_Eigenvectors_Colourful_G = eigenvectors_G;

                SingularValueDecomposition svd_B = new SingularValueDecomposition(Matrix_Main_Converted_Centered_B);
                var eigenvectors_B = svd_B.RightSingularVectors;
                //var Singularvalues_B = svd_B.Diagonal;
                Key_Eigenvectors_Colourful_B = eigenvectors_B;
                
                // пошук середнього для пікселів ЦВЗ R G B (бабуїна)

                int tmp_sum_R = 0, tmp_sum_G = 0, tmp_sum_B = 0;
                int tmp_avg_R = 0, tmp_avg_G = 0, tmp_avg_B = 0;
                for (int i = 0; i < String_With_WaterMark_Pixels_Colourful_R.Length; i++)
                {
                    tmp_sum_R += Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_R[i]);
                    tmp_sum_G += Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_G[i]);
                    tmp_sum_B += Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_B[i]);
                }
                tmp_avg_R = tmp_sum_R / String_With_WaterMark_Pixels_Colourful_R.Length;
                tmp_avg_G = tmp_sum_G / String_With_WaterMark_Pixels_Colourful_G.Length;
                tmp_avg_B = tmp_sum_B / String_With_WaterMark_Pixels_Colourful_B.Length;

                textBox8.Text = Convert.ToString(tmp_avg_R);
                textBox9.Text = Convert.ToString(tmp_avg_G);
                textBox10.Text = Convert.ToString(tmp_avg_B);
                
                // заміна 16-ої компоненти на центровані значення(пікселів) ЦВЗ R G B (бабуїна)
                var Matrix_Main_Components_R = Matrix_Main_Converted_Centered_R.MultiplyByTranspose(eigenvectors_R.Transpose());
                var Matrix_Main_Components_G = Matrix_Main_Converted_Centered_G.MultiplyByTranspose(eigenvectors_G.Transpose());
                var Matrix_Main_Components_B = Matrix_Main_Converted_Centered_B.MultiplyByTranspose(eigenvectors_B.Transpose());
                
                for (int i = 0; i < Matrix_Main_Components_R.GetLength(0); i++)
                {
                    
                    Matrix_Main_Components_R[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1] = Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_R[i]) - tmp_avg_R;
                    Matrix_Main_Components_G[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1] = Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_G[i]) - tmp_avg_G;
                    Matrix_Main_Components_B[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1] = Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_B[i]) - tmp_avg_B;
                    /*
                    Matrix_Main_Components_R[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1] = Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_R[i]) - tmp_tmp_avg_R;
                    Matrix_Main_Components_G[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1] = Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_G[i]) - tmp_tmp_avg_G;
                    Matrix_Main_Components_B[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1] = Convert.ToInt32(String_With_WaterMark_Pixels_Colourful_B[i]) - tmp_tmp_avg_B;
                    */
                }
                
                // реконструкція конвертованої матриці
                var Back_To_Matrix_Main_Converted_Centered_R = Matrix_Main_Components_R.DotWithTransposed(eigenvectors_R);
                var Back_To_Matrix_Main_Converted_Centered_G = Matrix_Main_Components_G.DotWithTransposed(eigenvectors_G);
                var Back_To_Matrix_Main_Converted_Centered_B = Matrix_Main_Components_B.DotWithTransposed(eigenvectors_B);
                double[,] Matrix_Reconstructed_R = Back_To_Matrix_Main_Converted_Centered_R.Add(Column_Mean_R, 0);
                double[,] Matrix_Reconstructed_G = Back_To_Matrix_Main_Converted_Centered_G.Add(Column_Mean_G, 0);
                double[,] Matrix_Reconstructed_B = Back_To_Matrix_Main_Converted_Centered_B.Add(Column_Mean_B, 0);
                
                // повна реконструкція матриці
                var Matrix_Main_Reconstructed_Fully_R = new double[Container_Original_512.Height, Container_Original_512.Width];
                var Matrix_Main_Reconstructed_Fully_G = new double[Container_Original_512.Height, Container_Original_512.Width];
                var Matrix_Main_Reconstructed_Fully_B = new double[Container_Original_512.Height, Container_Original_512.Width];

                int k2 = 0, l2 = 0;
                for (int i = 0; i < Matrix_Reconstructed_R.GetLength(0); i++)
                {
                    for (int j = 0; j < Matrix_Reconstructed_R.GetLength(1); j++)
                    {
                        k2 = j / frame_dimension + (i / number_of_squares) * frame_dimension;
                        l2 = j % frame_dimension + (i % number_of_squares) * frame_dimension;
                        Matrix_Main_Reconstructed_Fully_R[k2, l2] = Convert.ToInt32(Matrix_Reconstructed_R[i, j]);
                        Matrix_Main_Reconstructed_Fully_G[k2, l2] = Convert.ToInt32(Matrix_Reconstructed_G[i, j]);
                        Matrix_Main_Reconstructed_Fully_B[k2, l2] = Convert.ToInt32(Matrix_Reconstructed_B[i, j]);
                    }
                }

                // вивід кольорового зображення після певних перетворень
                int tmp_counter = 0;
                for (int i = 0; i < Container_Original_512.Width; i ++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        String_With_Lena_Pixels_Colourful_R[tmp_counter] = (char)(Matrix_Main_Reconstructed_Fully_R[i, j]+0.5);
                        String_With_Lena_Pixels_Colourful_G[tmp_counter] = (char)(Matrix_Main_Reconstructed_Fully_G[i, j]+0.5);
                        String_With_Lena_Pixels_Colourful_B[tmp_counter] = (char)(Matrix_Main_Reconstructed_Fully_B[i, j]+0.5);
                        tmp_counter++;
                    }

                int p = 0;
                Container_Reconstructed_512 = new Bitmap(Container_Original_512.Height, Container_Original_512.Width);
                int processedPixelR;
                int processedPixelG;
                int processedPixelB;
                for (int i = 0; i < Container_Original_512.Width; i++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        processedPixelR = Convert.ToInt32(String_With_Lena_Pixels_Colourful_R[p]);
                        if (processedPixelR < 0) processedPixelR = 0;
                        if (processedPixelR > 255) processedPixelR = 255;
                        processedPixelG = Convert.ToInt32(String_With_Lena_Pixels_Colourful_G[p]);
                        if (processedPixelG < 0) processedPixelG = 0;
                        if (processedPixelG > 255) processedPixelG = 255;
                        processedPixelB = Convert.ToInt32(String_With_Lena_Pixels_Colourful_B[p]);
                        if (processedPixelB < 0) processedPixelB = 0;
                        if (processedPixelB > 255) processedPixelB = 255;
                        p++;
                        Container_Reconstructed_512.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixelR), Convert.ToByte(processedPixelG), Convert.ToByte(processedPixelB)));
                    }
                Container_Reconstructed_512.Save("..\\..\\..\\Lena_Reconstructed_Colourful.bmp");

                StringBuilder psnr = new StringBuilder();
                psnr.Append(Convert.ToString(ComparePSNR(Container_Original_512, Container_Reconstructed_512)));
                textBox1.Text = psnr.ToString();
                buttonWriteKey1.Show();
                label6.Show();
                textBox5.Show(); textBox8.Show();
                textBox6.Show(); textBox9.Show();
                textBox7.Show(); textBox10.Show();
                /*///////////////// СТАРИЙ ВАРІАНТ!!!!!!!!!!!!!!!!!!!!!!!!!11
                // зчитування пікселів контейнера (лени)
                StringBuilder String_With_Lena_Pixels_Colourful = new StringBuilder();
                for (int x = 0; x < Container_Original_512.Width; x++)
                    for (int y = 0; y < Container_Original_512.Height; y++)
                    {
                        String_With_Lena_Pixels_Colourful.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).R));
                        String_With_Lena_Pixels_Colourful.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).G));
                        String_With_Lena_Pixels_Colourful.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).B));
                    }
                // зчитування пікселів ЦВЗ (бабуїна)
                StringBuilder String_With_WaterMark_Pixels_Colourful = new StringBuilder();
                for (int i = 0; i < Watermark_Original_128.Height; i++)
                    for (int j = 0; j < Watermark_Original_128.Width; j++)
                    {
                        String_With_WaterMark_Pixels_Colourful.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).R));
                        String_With_WaterMark_Pixels_Colourful.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).G));
                        String_With_WaterMark_Pixels_Colourful.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).B));
                    }

                var Matrix_Main = new double[Container_Original_512.Height * 3, Container_Original_512.Width];
                
                //int tmpR = 0, tmpG = 1, tmpB = 2;
                //for (int i = 0; i < LenaOriginal512.Width * 3; i += 3)
                  //  for (int j = 0; j < LenaOriginal512.Height; j++)
                    //{
                      //  Matrix_Main[i, j] = String_With_Lena_Pixels_Colourful[tmpR];
                        //Matrix_Main[i + 1, j] = String_With_Lena_Pixels_Colourful[tmpG];
                        //Matrix_Main[i + 2, j] = String_With_Lena_Pixels_Colourful[tmpB];
                        //tmpR += 3;
                        //tmpG += 3;
                        //tmpB += 3;
                    //}

                int tmpR = 0, tmpG = 1, tmpB = 2;
                int tmpsumR = 0, tmpsumG = 0, tmpsumB = 0;
                int tmpavgR = 0, tmpavgG = 0, tmpavgB = 0;
                for (int i = 0; i < Container_Original_512.Height * 3; i += 3)
                    for (int j = 0; j < Container_Original_512.Width; j++)
                    {
                        Matrix_Main[i, j] = String_With_Lena_Pixels_Colourful[tmpR];
                        tmpsumR += String_With_Lena_Pixels_Colourful[tmpR];
                        Matrix_Main[i + 1, j] = String_With_Lena_Pixels_Colourful[tmpG];
                        tmpsumG += String_With_Lena_Pixels_Colourful[tmpG];
                        Matrix_Main[i + 2, j] = String_With_Lena_Pixels_Colourful[tmpB];
                        tmpsumB += String_With_Lena_Pixels_Colourful[tmpB];
                        tmpR += 3;
                        tmpG += 3;
                        tmpB += 3;
                    }
                tmpavgR = tmpsumR / Container_Original_512.Height;
                tmpavgG = tmpsumG / Container_Original_512.Height;
                tmpavgB = tmpsumB / Container_Original_512.Height;


                // конвертація основної матриці 1536х512 в 16-ти стовпцеву
                int frame_dimension = 4;
                int columns = frame_dimension * frame_dimension;
                int rows = ((Container_Original_512.Width / frame_dimension) * (Container_Original_512.Height / frame_dimension)) * 3; //49152
                var Matrix_Main_Converted = new double[rows, columns];

                int k1 = 0, l1 = 0, k_max = Container_Original_512.Height, l_max = Container_Original_512.Width;
                int number_of_squares = k_max / frame_dimension;

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < columns; j++)
                    {
                        //Matrix_Main[i, j] = (double)((byte)LenaOriginal512.GetPixel(((i * frame) / (int)(Math.Sqrt(rows * col))) * frame + j / frame, (i % ((int)(Math.Sqrt(rows * col)) / frame)) * frame + j % frame).ToArgb() & 0x000000ff);
                        k1 = j / frame_dimension + (i / number_of_squares) * frame_dimension;
                        l1 = j % frame_dimension + (i % number_of_squares) * frame_dimension;
                        Matrix_Main_Converted[i, j] = Matrix_Main[k1, l1];
                    }

                // центрування конвертованої матриці                       !!!!!!!!!тут помилка
                double[] Column_Mean = Matrix_Main_Converted.Mean(0);
                var Matrix_Main_Converted_Centered = Matrix_Main_Converted.Subtract(Column_Mean, 0);

                //SVD
                SingularValueDecomposition svd = new SingularValueDecomposition(Matrix_Main_Converted_Centered);
                var eigenvectors = svd.RightSingularVectors;
                var Singularvalues = svd.Diagonal;
                Key1_Eigenvectors = eigenvectors;
                Key2_Singularvalues = Singularvalues;

                double[] eigenvalues = Singularvalues.Pow(2);
                eigenvalues = eigenvalues.Divide(Matrix_Main_Converted.GetLength(0) - 1);

                // пошук середнього для пікселів бабуїна  ??? ТУТ помилка

                int tmp_sum = 0, tmp_avg = 0;
                for (int i = 0; i < String_With_WaterMark_Pixels_Colourful.Length; i++)
                    tmp_sum += (int)String_With_WaterMark_Pixels_Colourful[i];
                tmp_avg = tmp_sum / String_With_WaterMark_Pixels_Colourful.Length;

                //int tmp_sum_R = 0, tmp_sum_G = 0, tmp_sum_B = 0;
                //int tmp_avg_R = 0, tmp_avg_G = 0, tmp_avg_B = 0;
                //int tmpR1 = 0, tmpG1 = 1, tmpB1 = 2;
                //for (int i = 0; i < String_With_WaterMark_Pixels_Colourful.Length; i += 3)
                //{
                  //  tmp_sum_R += (int)String_With_WaterMark_Pixels_Colourful[tmpR1];
                    //tmp_sum_G += (int)String_With_WaterMark_Pixels_Colourful[tmpG1];
                    //tmp_sum_B += (int)String_With_WaterMark_Pixels_Colourful[tmpB1];
                    //tmpR1 += 3;
                    //tmpG1 += 3;
                    //tmpB1 += 3;
                //}
                //tmp_avg_R = tmp_sum_R / (String_With_WaterMark_Pixels_Colourful.Length / 3);
                //tmp_avg_G = tmp_sum_G / (String_With_WaterMark_Pixels_Colourful.Length / 3);
                //tmp_avg_B = tmp_sum_B / (String_With_WaterMark_Pixels_Colourful.Length / 3);

                // заміна 16-ої компоненти на центровані значення(пікселів) бабуїна
                var Matrix_Main_Components = Matrix_Main_Converted_Centered.MultiplyByTranspose(eigenvectors.Transpose());
                
                for (int i = 0; i < Matrix_Main_Components.GetLength(0); i++)
                {
                    //Matrix_Main_Components[i, 15] = String_With_WaterMark_Pixels_Colourful_Centered[i];
                    Matrix_Main_Components[i, 15] = Convert.ToInt32(String_With_WaterMark_Pixels_Colourful[i]) - tmp_avg;
                    //Matrix_Main_Components[i, 0] = 0;
                    //Matrix_Main_Components[i, 15] = String_With_WaterMark_Pixels_Colourful[i] - 10;
                }
                
               // int tmpRR = 0, tmpGG = 1, tmpBB = 2;
                //for (int i = 0; i < Container_Original_512.Width * 3; i += 3)
                  //  for (int j = 0; j < Container_Original_512.Height; j++)
                    //{
                      //  Matrix_Main_Components[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1] = String_With_Lena_Pixels_Colourful[tmpRR];
                       // Matrix_Main_Components[i + 1, (Component_Number = Int32.Parse(textBox4.Text)) - 1] = String_With_Lena_Pixels_Colourful[tmpGG];
                       // Matrix_Main_Components[i + 2, (Component_Number = Int32.Parse(textBox4.Text)) - 1] = String_With_Lena_Pixels_Colourful[tmpBB];
                        //tmpRR += 3;
                        //tmpGG += 3;
                        //tmpBB += 3;
                    //}
                
                // реконструкція конвертованої матриці
                var Back_To_Matrix_Main_Converted_Centered = Matrix_Main_Components.DotWithTransposed(eigenvectors);
                double[,] Matrix_Reconstructed = Back_To_Matrix_Main_Converted_Centered.Add(Column_Mean, 0);

                // повна реконструкція матриці
                var Matrix_Main_Reconstructed_Fully = new double[Container_Original_512.Height * 3, Container_Original_512.Width];
                int k2 = 0, l2 = 0;
                for (int i = 0; i < Matrix_Reconstructed.GetLength(0); i++)
                {
                    for (int j = 0; j < Matrix_Reconstructed.GetLength(1); j++)
                    {
                        k2 = j / frame_dimension + (i / number_of_squares) * frame_dimension;
                        l2 = j % frame_dimension + (i % number_of_squares) * frame_dimension;
                        Matrix_Main_Reconstructed_Fully[k2, l2] = Convert.ToInt32(Matrix_Reconstructed[i, j]);
                    }
                }

                // вивід кольорового зображення після певних перетворень
                int tmpR2 = 0, tmpG2 = 1, tmpB2 = 2;
                for (int i = 0; i < Container_Original_512.Width * 3; i += 3)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        String_With_Lena_Pixels_Colourful[tmpR2] = (char)Matrix_Main_Reconstructed_Fully[i, j];
                        String_With_Lena_Pixels_Colourful[tmpG2] = (char)Matrix_Main_Reconstructed_Fully[i + 1, j];
                        String_With_Lena_Pixels_Colourful[tmpB2] = (char)Matrix_Main_Reconstructed_Fully[i + 2, j];
                        tmpR2 += 3;
                        tmpG2 += 3;
                        tmpB2 += 3;
                    }

                int p = 0;
                Container_Reconstructed_512 = new Bitmap(Container_Original_512.Height, Container_Original_512.Width);
                int processedPixelR;
                int processedPixelG;
                int processedPixelB;
                for (int i = 0; i < Container_Original_512.Width; i++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        processedPixelR = Convert.ToInt32(String_With_Lena_Pixels_Colourful[p]);
                        if (processedPixelR < 0) processedPixelR = 0;
                        if (processedPixelR > 255) processedPixelR = 255;
                        p++;
                        processedPixelG = Convert.ToInt32(String_With_Lena_Pixels_Colourful[p]);
                        if (processedPixelG < 0) processedPixelG = 0;
                        if (processedPixelG > 255) processedPixelG = 255;
                        p++;
                        processedPixelB = Convert.ToInt32(String_With_Lena_Pixels_Colourful[p]);
                        if (processedPixelB < 0) processedPixelB = 0;
                        if (processedPixelB > 255) processedPixelB = 255;
                        p++;
                        Container_Reconstructed_512.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixelR), Convert.ToByte(processedPixelG), Convert.ToByte(processedPixelB)));
                    }
                Container_Reconstructed_512.Save("..\\..\\..\\Lena_Reconstructed_Colourful.bmp");

                StringBuilder psnr = new StringBuilder();
                psnr.Append(Convert.ToString(ComparePSNR(Container_Original_512, Container_Reconstructed_512)));
                textBox1.Text = psnr.ToString();
                buttonWriteKey1.Show();
                */
            }

        }

        // кнопка для запису першого ключа
        private void button10_Click(object sender, EventArgs e)
        {
            Colour_Mode = radioButtonGrayscale.Checked;
            //для чорнобілої версії
            if (Colour_Mode == true)
            {
                // запис ключів у файли
                StreamWriter outfile_Key_Eigenvectors_Grayscale = new StreamWriter(@"..\\..\\..\\eigenvectors_grayscale.txt");
                for (int i = 0; i < Key_Eigenvectors_Grayscale.GetLength(0); i++)
                {
                    for (int j = 0; j < Key_Eigenvectors_Grayscale.GetLength(1); j++)
                    {
                        outfile_Key_Eigenvectors_Grayscale.Write("{0} ", Key_Eigenvectors_Grayscale[i, j]);
                    }
                    outfile_Key_Eigenvectors_Grayscale.Write("{0}", Environment.NewLine);
                }
                outfile_Key_Eigenvectors_Grayscale.Close();
                buttonShowEncryptResult.Show();
                //buttonWriteKey2.Show();
            }
            else
            //для кольорової
            {
                // запис ключів у файли
                StreamWriter outfile_Key_Eigenvectors_Colourful_R = new StreamWriter(@"..\\..\\..\\eigenvectors_colourful_r.txt");
                StreamWriter outfile_Key_Eigenvectors_Colourful_G = new StreamWriter(@"..\\..\\..\\eigenvectors_colourful_g.txt");
                StreamWriter outfile_Key_Eigenvectors_Colourful_B = new StreamWriter(@"..\\..\\..\\eigenvectors_colourful_b.txt");
                for (int i = 0; i < Key_Eigenvectors_Colourful_R.GetLength(0); i++)
                {
                    for (int j = 0; j < Key_Eigenvectors_Colourful_R.GetLength(1); j++)
                    {
                        outfile_Key_Eigenvectors_Colourful_R.Write("{0} ", Key_Eigenvectors_Colourful_R[i, j]);
                        outfile_Key_Eigenvectors_Colourful_G.Write("{0} ", Key_Eigenvectors_Colourful_G[i, j]);
                        outfile_Key_Eigenvectors_Colourful_B.Write("{0} ", Key_Eigenvectors_Colourful_B[i, j]);
                    }
                    outfile_Key_Eigenvectors_Colourful_R.Write("{0}", Environment.NewLine);
                    outfile_Key_Eigenvectors_Colourful_G.Write("{0}", Environment.NewLine);
                    outfile_Key_Eigenvectors_Colourful_B.Write("{0}", Environment.NewLine);
                }
                outfile_Key_Eigenvectors_Colourful_R.Close();
                outfile_Key_Eigenvectors_Colourful_G.Close();
                outfile_Key_Eigenvectors_Colourful_B.Close();
                buttonShowEncryptResult.Show();
                //buttonWriteKey2.Show();
            }
        }

        // кнопка для запису другого ключа
        private void button11_Click(object sender, EventArgs e)
        {
            /*
            StreamWriter outfile_key2 = new StreamWriter(@"..\\..\\..\\Singularvalues.txt");
            for (int i = 0; i < Key2_Singularvalues.Length; i++)
                outfile_key2.Write("{0} ", Key2_Singularvalues[i]);
            outfile_key2.Close();*/
            buttonShowEncryptResult.Show();
        }

        // кнопка для виведення результатів шифрування
        private void button4_Click(object sender, EventArgs e)
        {
            pictureBoxImageResultWithWatermark.Image = Container_Reconstructed_512;
            label1.Show();
            textBox1.Show();
            pictureBoxImageResultWithWatermark.Show();
            button6.Show();
        }

        // кнопка для завантаження зображення зі знаком
        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBoxImageWithWatermark.Image = System.Drawing.Image.FromFile(openFileDialog1.FileName);
                Container_Reconstructed_512 = new Bitmap(openFileDialog1.FileName);
            }
        }

        // кнопка для зчитування першого ключа
        private void button6_Click(object sender, EventArgs e)
        {
            Colour_Mode = radioButtonGrayscale.Checked;
            //для чорнобілої версії
            if (Colour_Mode == true)
            {
                Key_Eigenvectors_Grayscale_Read = @"..\\..\\..\\eigenvectors_grayscale.txt";
                //buttonReadKey2.Show();
                buttonDecrypt.Show();
            }
            else
            //для кольорової
            {
                Key_Eigenvectors_Colourful_R_Read = @"..\\..\\..\\eigenvectors_colourful_r.txt";
                Key_Eigenvectors_Colourful_G_Read = @"..\\..\\..\\eigenvectors_colourful_g.txt";
                Key_Eigenvectors_Colourful_B_Read = @"..\\..\\..\\eigenvectors_colourful_b.txt";
                buttonDecrypt.Show();
            }
        }

        // кнопка для зчитування другого ключа
        private void button7_Click(object sender, EventArgs e)
        {
            /*
            Key2_Singularvalues_Read = @"..\\..\\..\\Singularvalues.txt";
            buttonDecrypt.Show();
            */
        }

        // кнопка для зчитування оригінального знаку (для дешифрування)
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Watermark_Original_128 = new Bitmap(openFileDialog1.FileName);
            }
            buttonReadKey1.Show();
        }

        // кнопка для дешифрування
        private void button8_Click(object sender, EventArgs e)
        {
            Colour_Mode = radioButtonGrayscale.Checked;
            //для чорнобілої версії
            if (Colour_Mode == true)
            {
                // зчитування пікселів контейнера з ЦВЗ (лени та бабуїна)
                StringBuilder String_With_Container_Reconstructed_Pixels = new StringBuilder();
                for (int x = 0; x < Container_Reconstructed_512.Width; x++)
                    for (int y = 0; y < Container_Reconstructed_512.Height; y++)
                        String_With_Container_Reconstructed_Pixels.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(x, y).ToArgb() & 0x000000ff));

                var Matrix_Main = new double[Container_Reconstructed_512.Height, Container_Reconstructed_512.Width];
                int counter = 0;
                for (int i = 0; i < Container_Reconstructed_512.Height; i++)
                    for (int j = 0; j < Container_Reconstructed_512.Width; j++)
                    {
                        Matrix_Main[i, j] = String_With_Container_Reconstructed_Pixels[counter];
                        counter++;
                    }

                // конвертація основної матриці 512х512 в 16-ти стовпцеву
                int frame_dimension = 4;
                int columns = frame_dimension * frame_dimension;
                int rows = (Container_Reconstructed_512.Width / frame_dimension) * (Container_Reconstructed_512.Height / frame_dimension);
                var Matrix_Main_Converted = new double[rows, columns];

                int k1 = 0, l1 = 0, k_max = Container_Reconstructed_512.Height, l_max = Container_Reconstructed_512.Width;

                int number_of_squares = k_max / frame_dimension;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < columns; j++)
                    {
                        //Matrix_Main[i, j] = (double)((byte)LenaOriginal512.GetPixel(((i * frame) / (int)(Math.Sqrt(rows * col))) * frame + j / frame, (i % ((int)(Math.Sqrt(rows * col)) / frame)) * frame + j % frame).ToArgb() & 0x000000ff);
                        k1 = j / frame_dimension + (i / number_of_squares) * frame_dimension;
                        l1 = j % frame_dimension + (i % number_of_squares) * frame_dimension;
                        Matrix_Main_Converted[i, j] = Matrix_Main[k1, l1];
                    }

                // центрування конвертованої матриці
                double[] Column_Mean = Matrix_Main_Converted.Mean(0);
                //Column_Mean[(Component_Number = Int32.Parse(textBox4.Text)) - 1] = TMP_MEAN;
                //Column_Mean[15] = 114.0;
                var Matrix_Main_Converted_Centered = Matrix_Main_Converted.Subtract(Column_Mean, 0);
                
                // дешифрування
                // зчитування ключів
                // перше eigenvectors_grayscale
                //string input = @"..\\..\\..\\eigenvectors_grayscale.txt";

                double[,] eigenvectors = new double[16, 16];
                String[,] array2D;
                int numberOfLines = 0, numberOfColumns = 0;
                string line;
                System.IO.StreamReader sr = new System.IO.StreamReader(Key_Eigenvectors_Grayscale_Read);

                while ((line = sr.ReadLine()) != null)
                {
                    numberOfColumns = line.Split(' ').Length;
                    numberOfLines++;
                }
                sr.Close();

                array2D = new String[numberOfLines, numberOfColumns];
                numberOfLines = 0;

                sr = new System.IO.StreamReader(Key_Eigenvectors_Grayscale_Read);
                while ((line = sr.ReadLine()) != null)
                {
                    String[] tempArray = line.Split(' ');
                    for (int i = 0; i < tempArray.Length; ++i)
                    {
                        array2D[numberOfLines, i] = tempArray[i];
                    }
                    numberOfLines++;
                }

                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 16; j++)
                        eigenvectors[i, j] = Convert.ToDouble(array2D[i, j]);

                /*
                // потім Singularvalues
                //string input1 = @"..\\..\\..\\Singularvalues.txt";
                
                double[,] Singularvalues1 = new double[1, 16];

                String[,] array2D1;
                int numberOfLines1 = 0, numberOfColumns1 = 0;
                string line1;
                System.IO.StreamReader sr1 = new System.IO.StreamReader(Key2_Singularvalues_Read);

                while ((line1 = sr1.ReadLine()) != null)
                {
                    numberOfColumns1 = line1.Split(' ').Length;
                    numberOfLines1++;
                }
                sr1.Close();

                array2D1 = new String[numberOfLines1, numberOfColumns1];
                numberOfLines1 = 0;

                sr1 = new System.IO.StreamReader(Key2_Singularvalues_Read);
                while ((line1 = sr1.ReadLine()) != null)
                {
                    String[] tempArray1 = line1.Split(' ');
                    for (int i = 0; i < tempArray1.Length; ++i)
                    {
                        array2D1[numberOfLines1, i] = tempArray1[i];
                    }
                    numberOfLines1++;
                }

                for (int i = 0; i < 1; i++)
                    for (int j = 0; j < 16; j++)
                        Singularvalues1[i, j] = Convert.ToDouble(array2D1[i, j]);

                double[] Singularvalues = new double[16];
                for (int i = 0; i < 16; i++)
                    Singularvalues[i] = Singularvalues1[0, i];
                */
                
                // зчитування центрованих значень(пікселів) ЦВЗ (бабуїна) із 16 компоненти 
                double[] Array_With_WaterMark_Pixels = new double[16384]; //128*128
                var Matrix_Main_Components = Matrix_Main_Converted_Centered.MultiplyByTranspose(eigenvectors.Transpose());
                for (int i = 0; i < Matrix_Main_Components.GetLength(0); i++)
                {
                    Array_With_WaterMark_Pixels[i] = Matrix_Main_Components[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1];
                    //Matrix_Main_Components[i, 15] = 0;
                }

                // пошук середнього для пікселів бабуїна      ТУТ МОЖЛИВА ПОМИЛКА tmp_avg
                StringBuilder String_With_WaterMark_Pixels = new StringBuilder();
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                        String_With_WaterMark_Pixels.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).ToArgb() & 0x000000ff));
                int tmp_sum = 0, tmp_avg = 0;
                for (int i = 0; i < String_With_WaterMark_Pixels.Length; i++)
                    tmp_sum += Convert.ToInt32(String_With_WaterMark_Pixels[i]);
                tmp_avg = tmp_sum / String_With_WaterMark_Pixels.Length;
                /*int tmp_sum = 0, tmp_avg = 0;
                for (int i = 0; i < Array_With_WaterMark_Pixels.Length; i++)
                    tmp_sum += (int)Array_With_WaterMark_Pixels[i];
                tmp_avg = tmp_sum / Array_With_WaterMark_Pixels.Length;

                int tmp_avg = 114;*/
                // додавання середнього значення пікселів ЦВЗ до центованих значень пікселів ЦВЗ (бабуїна)
                for (int i = 0; i < 16384; i++)
                    Array_With_WaterMark_Pixels[i] = Array_With_WaterMark_Pixels[i] + tmp_avg;

                // збереження чорнобілого зображення після певних перетворень
                StringBuilder String_With_Watermark_Reconstructed_Pixels = new StringBuilder();
                int tmp = 0;
                for (int i = 0; i < 16384; i++)
                {
                    String_With_Watermark_Reconstructed_Pixels.Append((char)(Array_With_WaterMark_Pixels[i]+0.5));
                    //tmp++;
                }

                int p = 0;
                Watermark_Reconstructed_128 = new Bitmap(Watermark_Original_128.Height, Watermark_Original_128.Width);
                int processedPixel;
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        processedPixel = Convert.ToInt32(String_With_Watermark_Reconstructed_Pixels[p]);
                        if (processedPixel < 0) processedPixel = 0;
                        if (processedPixel > 255) processedPixel = 255;
                        Watermark_Reconstructed_128.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixel), Convert.ToByte(processedPixel), Convert.ToByte(processedPixel)));
                        p++;
                    }


                Watermark_Reconstructed_128.Save("..\\..\\..\\BaboonOriginal128_Reconstructed.bmp");

                StringBuilder psnr = new StringBuilder();
                psnr.Append(Convert.ToString(ComparePSNR(Watermark_Original_128, Watermark_Reconstructed_128)));
                //psnr.Append(Convert.ToString(ComparePSNR(Watermark_Original_128, Watermark_Original_128)));
                textBox2.Text = psnr.ToString();
                buttonShowDecryptResult.Show();
            }
            else
            {
                // ДЕШИФРУВАННЯ КОЛЬОРОВЕ
                // НОВИЙ ВАРІАНТ
                // зчитування пікселів контейнера з ЦВЗ R G B (лени та бабуїна)

                StringBuilder String_With_Lena_Pixels_Colourful_R = new StringBuilder();
                StringBuilder String_With_Lena_Pixels_Colourful_G = new StringBuilder();
                StringBuilder String_With_Lena_Pixels_Colourful_B = new StringBuilder();
                for (int x = 0; x < Container_Reconstructed_512.Width; x++)
                    for (int y = 0; y < Container_Reconstructed_512.Height; y++)
                    {
                        String_With_Lena_Pixels_Colourful_R.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(x, y).R));
                        String_With_Lena_Pixels_Colourful_G.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(x, y).G));
                        String_With_Lena_Pixels_Colourful_B.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(x, y).B));
                    }

                var Matrix_Main_R = new double[Container_Reconstructed_512.Height, Container_Reconstructed_512.Width];
                var Matrix_Main_G = new double[Container_Reconstructed_512.Height, Container_Reconstructed_512.Width];
                var Matrix_Main_B = new double[Container_Reconstructed_512.Height, Container_Reconstructed_512.Width];

                int tmp_counter = 0;
                for (int i = 0; i < Container_Reconstructed_512.Width; i++)
                    for (int j = 0; j < Container_Reconstructed_512.Height; j++)
                    {
                        Matrix_Main_R[i, j] = String_With_Lena_Pixels_Colourful_R[tmp_counter];
                        Matrix_Main_G[i, j] = String_With_Lena_Pixels_Colourful_G[tmp_counter];
                        Matrix_Main_B[i, j] = String_With_Lena_Pixels_Colourful_B[tmp_counter];
                        tmp_counter++;
                    }

                // конвертація основних матриць R G B 512х512 в 16-ти стовпцеві
                int frame_dimension = 4;
                int columns = frame_dimension * frame_dimension;
                int rows = (Container_Reconstructed_512.Width / frame_dimension) * (Container_Reconstructed_512.Height / frame_dimension);
                var Matrix_Main_Converted_R = new double[rows, columns];
                var Matrix_Main_Converted_G = new double[rows, columns];
                var Matrix_Main_Converted_B = new double[rows, columns];

                int k1 = 0, l1 = 0, k_max = Container_Reconstructed_512.Height, l_max = Container_Reconstructed_512.Width;

                int number_of_squares = k_max / frame_dimension;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < columns; j++)
                    {
                        //Matrix_Main[i, j] = (double)((byte)LenaOriginal512.GetPixel(((i * frame) / (int)(Math.Sqrt(rows * col))) * frame + j / frame, (i % ((int)(Math.Sqrt(rows * col)) / frame)) * frame + j % frame).ToArgb() & 0x000000ff);
                        k1 = j / frame_dimension + (i / number_of_squares) * frame_dimension;
                        l1 = j % frame_dimension + (i % number_of_squares) * frame_dimension;
                        Matrix_Main_Converted_R[i, j] = Matrix_Main_R[k1, l1];
                        Matrix_Main_Converted_G[i, j] = Matrix_Main_G[k1, l1];
                        Matrix_Main_Converted_B[i, j] = Matrix_Main_B[k1, l1];
                    }

                // центрування конвертованих матриць
                double[] Column_Mean_R = Matrix_Main_Converted_R.Mean(0);
                double[] Column_Mean_G = Matrix_Main_Converted_G.Mean(0);
                double[] Column_Mean_B = Matrix_Main_Converted_B.Mean(0);
                var Matrix_Main_Converted_Centered_R = Matrix_Main_Converted_R.Subtract(Column_Mean_R, 0);
                var Matrix_Main_Converted_Centered_G = Matrix_Main_Converted_G.Subtract(Column_Mean_G, 0);
                var Matrix_Main_Converted_Centered_B = Matrix_Main_Converted_B.Subtract(Column_Mean_B, 0);

                // дешифрування
                // зчитування ключів eigenvectors R G B

                double[,] eigenvectors_R = new double[16, 16];
                double[,] eigenvectors_G = new double[16, 16];
                double[,] eigenvectors_B = new double[16, 16];
                String[,] array2D_R;
                String[,] array2D_G;
                String[,] array2D_B;
                int numberOfLines_R = 0, numberOfColumns_R = 0;
                int numberOfLines_G = 0, numberOfColumns_G = 0;
                int numberOfLines_B = 0, numberOfColumns_B = 0;
                string line_R;
                string line_G;
                string line_B;
                System.IO.StreamReader sr_R = new System.IO.StreamReader(Key_Eigenvectors_Colourful_R_Read);
                System.IO.StreamReader sr_G = new System.IO.StreamReader(Key_Eigenvectors_Colourful_G_Read);
                System.IO.StreamReader sr_B = new System.IO.StreamReader(Key_Eigenvectors_Colourful_B_Read);

                // для R
                while ((line_R = sr_R.ReadLine()) != null)
                {
                    numberOfColumns_R = line_R.Split(' ').Length;
                    numberOfLines_R++;
                }
                sr_R.Close();

                array2D_R = new String[numberOfLines_R, numberOfColumns_R];
                numberOfLines_R = 0;
           
                sr_R = new System.IO.StreamReader(Key_Eigenvectors_Colourful_R_Read);
                while ((line_R = sr_R.ReadLine()) != null)
                {
                    String[] tempArray = line_R.Split(' ');
                    for (int i = 0; i < tempArray.Length; ++i)
                    {
                        array2D_R[numberOfLines_R, i] = tempArray[i];
                    }
                    numberOfLines_R++;
                }
                sr_R.Close();

                // для G
                while ((line_G = sr_G.ReadLine()) != null)
                {
                    numberOfColumns_G = line_G.Split(' ').Length;
                    numberOfLines_G++;
                }
                sr_G.Close();

                array2D_G = new String[numberOfLines_G, numberOfColumns_G];
                numberOfLines_G = 0;

                sr_G = new System.IO.StreamReader(Key_Eigenvectors_Colourful_G_Read);
                while ((line_G = sr_G.ReadLine()) != null)
                {
                    String[] tempArray = line_G.Split(' ');
                    for (int i = 0; i < tempArray.Length; ++i)
                    {
                        array2D_G[numberOfLines_G, i] = tempArray[i];
                    }
                    numberOfLines_G++;
                }
                sr_G.Close();

                // для B
                while ((line_B = sr_B.ReadLine()) != null)
                {
                    numberOfColumns_B = line_B.Split(' ').Length;
                    numberOfLines_B++;
                }
                sr_B.Close();

                array2D_B = new String[numberOfLines_B, numberOfColumns_B];
                numberOfLines_B = 0;

                sr_B = new System.IO.StreamReader(Key_Eigenvectors_Colourful_B_Read);
                while ((line_B = sr_B.ReadLine()) != null)
                {
                    String[] tempArray = line_B.Split(' ');
                    for (int i = 0; i < tempArray.Length; ++i)
                    {
                        array2D_B[numberOfLines_B, i] = tempArray[i];
                    }
                    numberOfLines_B++;
                }
                sr_B.Close();

                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 16; j++)
                    {
                        eigenvectors_R[i, j] = Convert.ToDouble(array2D_R[i, j]);
                        eigenvectors_G[i, j] = Convert.ToDouble(array2D_G[i, j]);
                        eigenvectors_B[i, j] = Convert.ToDouble(array2D_B[i, j]);
                    }

                // зчитування центрованих значень(пікселів) ЦВЗ (бабуїна) із 16 компоненти 
                double[] Array_With_WaterMark_Pixels_R = new double[16384]; //128*128
                double[] Array_With_WaterMark_Pixels_G = new double[16384];
                double[] Array_With_WaterMark_Pixels_B = new double[16384];
                var Matrix_Main_Components_R = Matrix_Main_Converted_Centered_R.MultiplyByTranspose(eigenvectors_R.Transpose());
                var Matrix_Main_Components_G = Matrix_Main_Converted_Centered_G.MultiplyByTranspose(eigenvectors_G.Transpose());
                var Matrix_Main_Components_B = Matrix_Main_Converted_Centered_B.MultiplyByTranspose(eigenvectors_B.Transpose());
                for (int i = 0; i < Matrix_Main_Components_R.GetLength(0); i++)
                {
                    Array_With_WaterMark_Pixels_R[i] = Matrix_Main_Components_R[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1];
                    Array_With_WaterMark_Pixels_G[i] = Matrix_Main_Components_G[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1];
                    Array_With_WaterMark_Pixels_B[i] = Matrix_Main_Components_B[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1];
                }

                // пошук середнього для пікселів бабуїна      ТУТ МОЖЛИВА ПОМИЛКА tmp_avg
                StringBuilder String_With_WaterMark_Pixels_R = new StringBuilder();
                StringBuilder String_With_WaterMark_Pixels_G = new StringBuilder();
                StringBuilder String_With_WaterMark_Pixels_B = new StringBuilder();
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        String_With_WaterMark_Pixels_R.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).R));
                        String_With_WaterMark_Pixels_G.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).G));
                        String_With_WaterMark_Pixels_B.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).B));
                    }

                int tmp_sum_R = 0, tmp_avg_R = 0;
                int tmp_sum_G = 0, tmp_avg_G = 0;
                int tmp_sum_B = 0, tmp_avg_B = 0;
                for (int i = 0; i < String_With_WaterMark_Pixels_R.Length; i++)
                {
                    tmp_sum_R += Convert.ToInt32(String_With_WaterMark_Pixels_R[i]);
                    tmp_sum_G += Convert.ToInt32(String_With_WaterMark_Pixels_G[i]);
                    tmp_sum_B += Convert.ToInt32(String_With_WaterMark_Pixels_B[i]);
                }
                tmp_avg_R = tmp_sum_R / String_With_WaterMark_Pixels_R.Length;
                tmp_avg_G = tmp_sum_G / String_With_WaterMark_Pixels_G.Length;
                tmp_avg_B = tmp_sum_B / String_With_WaterMark_Pixels_B.Length;

                // додавання середнього значення пікселів ЦВЗ до центованих значень пікселів ЦВЗ (бабуїна)
                for (int i = 0; i < 16384; i++)
                {
                    Array_With_WaterMark_Pixels_R[i] = Array_With_WaterMark_Pixels_R[i] + tmp_avg_R;
                    Array_With_WaterMark_Pixels_G[i] = Array_With_WaterMark_Pixels_G[i] + tmp_avg_G;
                    Array_With_WaterMark_Pixels_B[i] = Array_With_WaterMark_Pixels_B[i] + tmp_avg_B;
                }
                
                // збереження чорнобілого зображення після певних перетворень
                StringBuilder String_With_Watermark_Reconstructed_Pixels_R = new StringBuilder();
                StringBuilder String_With_Watermark_Reconstructed_Pixels_G = new StringBuilder();
                StringBuilder String_With_Watermark_Reconstructed_Pixels_B = new StringBuilder();
                int tmp = 0;
                for (int i = 0; i < 16384; i++)
                {
                    String_With_Watermark_Reconstructed_Pixels_R.Append((char)(Array_With_WaterMark_Pixels_R[i] + 0.5));
                    String_With_Watermark_Reconstructed_Pixels_G.Append((char)(Array_With_WaterMark_Pixels_G[i] + 0.5));
                    String_With_Watermark_Reconstructed_Pixels_B.Append((char)(Array_With_WaterMark_Pixels_B[i] + 0.5));
                }

                int p = 0;
                Watermark_Reconstructed_128 = new Bitmap(Watermark_Original_128.Height, Watermark_Original_128.Width);
                int processedPixelR;
                int processedPixelG;
                int processedPixelB;
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        processedPixelR = Convert.ToInt32(String_With_Watermark_Reconstructed_Pixels_R[p]);
                        if (processedPixelR < 0) processedPixelR = 0;
                        if (processedPixelR > 255) processedPixelR = 255;
                        processedPixelG = Convert.ToInt32(String_With_Watermark_Reconstructed_Pixels_G[p]);
                        if (processedPixelG < 0) processedPixelG = 0;
                        if (processedPixelG > 255) processedPixelG = 255;
                        processedPixelB = Convert.ToInt32(String_With_Watermark_Reconstructed_Pixels_B[p]);
                        if (processedPixelB < 0) processedPixelB = 0;
                        if (processedPixelB > 255) processedPixelB = 255;
                        p++;
                        Watermark_Reconstructed_128.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixelR), Convert.ToByte(processedPixelG), Convert.ToByte(processedPixelB)));
                    }

                Watermark_Reconstructed_128.Save("..\\..\\..\\BaboonOriginal128_Reconstructed.bmp");

                StringBuilder psnr = new StringBuilder();
                psnr.Append(Convert.ToString(ComparePSNR(Watermark_Original_128, Watermark_Reconstructed_128)));
                textBox2.Text = psnr.ToString();
                buttonShowDecryptResult.Show();


                /* //СТАРИЙ ВАРІАНТ
                
                // зчитування пікселів лени та бабуїна 
                StringBuilder String_With_Lena_Pixels_Colourful = new StringBuilder();
                for (int x = 0; x < Container_Reconstructed_512.Width; x++)
                    for (int y = 0; y < Container_Reconstructed_512.Height; y++)
                    {
                        String_With_Lena_Pixels_Colourful.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(x, y).R));
                        String_With_Lena_Pixels_Colourful.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(x, y).G));
                        String_With_Lena_Pixels_Colourful.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(x, y).B));
                    }

                double[] Array_With_WaterMark_Pixels_Colourful = new double[49152];

                var Matrix_Main = new double[Container_Reconstructed_512.Height * 3, Container_Reconstructed_512.Width];

                int tmpR = 0, tmpG = 1, tmpB = 2;
                for (int i = 0; i < Container_Reconstructed_512.Width * 3; i += 3)
                    for (int j = 0; j < Container_Reconstructed_512.Height; j++)
                    {
                        Matrix_Main[i, j] = String_With_Lena_Pixels_Colourful[tmpR];
                        Matrix_Main[i + 1, j] = String_With_Lena_Pixels_Colourful[tmpG];
                        Matrix_Main[i + 2, j] = String_With_Lena_Pixels_Colourful[tmpB];
                        tmpR += 3;
                        tmpG += 3;
                        tmpB += 3;
                    }

                // конвертація основної матриці 1536х512 в 16-ти стовпцеву
                int frame_dimension = 4;
                int columns = frame_dimension * frame_dimension;
                int rows = ((Container_Reconstructed_512.Width / frame_dimension) * (Container_Reconstructed_512.Height / frame_dimension)) * 3;
                var Matrix_Main_Converted = new double[rows, columns];

                int k1 = 0, l1 = 0, k_max = Container_Reconstructed_512.Height, l_max = Container_Reconstructed_512.Width;
                int number_of_squares = k_max / frame_dimension;

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < columns; j++)
                    {
                        //Matrix_Main[i, j] = (double)((byte)LenaOriginal512.GetPixel(((i * frame) / (int)(Math.Sqrt(rows * col))) * frame + j / frame, (i % ((int)(Math.Sqrt(rows * col)) / frame)) * frame + j % frame).ToArgb() & 0x000000ff);
                        k1 = j / frame_dimension + (i / number_of_squares) * frame_dimension;
                        l1 = j % frame_dimension + (i % number_of_squares) * frame_dimension;
                        Matrix_Main_Converted[i, j] = Matrix_Main[k1, l1];
                    }

                // центрування конвертованої матриці
                double[] Column_Mean = Matrix_Main_Converted.Mean(0);
                var Matrix_Main_Converted_Centered = Matrix_Main_Converted.Subtract(Column_Mean, 0);

                // дешифрування
                // зчитування ключів
                // перше eigen vectors
                //string input = @"..\\..\\..\\eigenvectorsC.txt";

                double[,] eigenvectors = new double[16, 16];
                String[,] array2D;
                int numberOfLines = 0, numberOfColumns = 0;
                string line;
                System.IO.StreamReader sr = new System.IO.StreamReader(Key_Eigenvectors_Colourful_R_Read);

                while ((line = sr.ReadLine()) != null)
                {
                    numberOfColumns = line.Split(' ').Length;
                    numberOfLines++;
                }
                sr.Close();

                array2D = new String[numberOfLines, numberOfColumns];
                numberOfLines = 0;

                sr = new System.IO.StreamReader(Key_Eigenvectors_Colourful_R_Read);
                while ((line = sr.ReadLine()) != null)
                {
                    String[] tempArray = line.Split(' ');
                    for (int i = 0; i < tempArray.Length; ++i)
                    {
                        array2D[numberOfLines, i] = tempArray[i];
                    }
                    numberOfLines++;
                }

                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 16; j++)
                        eigenvectors[i, j] = Convert.ToDouble(array2D[i, j]);

                // потім Singularvalues
                //string input1 = @"..\\..\\..\\SingularvaluesC.txt";

                double[,] Singularvalues1 = new double[1, 16];

                String[,] array2D1;
                int numberOfLines1 = 0, numberOfColumns1 = 0;
                string line1;
                System.IO.StreamReader sr1 = new System.IO.StreamReader(Key2_Singularvalues_Read);

                while ((line1 = sr1.ReadLine()) != null)
                {
                    numberOfColumns1 = line1.Split(' ').Length;
                    numberOfLines1++;
                }
                sr1.Close();

                array2D1 = new String[numberOfLines1, numberOfColumns1];
                numberOfLines1 = 0;

                sr1 = new System.IO.StreamReader(Key2_Singularvalues_Read);
                while ((line1 = sr1.ReadLine()) != null)
                {
                    String[] tempArray1 = line1.Split(' ');
                    for (int i = 0; i < tempArray1.Length; ++i)
                    {
                        array2D1[numberOfLines1, i] = tempArray1[i];
                    }
                    numberOfLines1++;
                }

                for (int i = 0; i < 1; i++)
                    for (int j = 0; j < 16; j++)
                        Singularvalues1[i, j] = Convert.ToDouble(array2D1[i, j]);

                double[] Singularvalues = new double[16];
                for (int i = 0; i < 16; i++)
                    Singularvalues[i] = Singularvalues1[0, i];

                //double[] eigenvalues = Singularvalues.Pow(2);
                //eigenvalues = eigenvalues.Divide(Matrix_Main_Converted.GetLength(0) - 1);

                // заміна 16-ої компоненти на центровані значення(пікселів) бабуїна
                var Matrix_Main_Components = Matrix_Main_Converted_Centered.MultiplyByTranspose(eigenvectors.Transpose());
                for (int i = 0; i < Matrix_Main_Components.GetLength(0); i++)
                {
                    Array_With_WaterMark_Pixels_Colourful[i] = Matrix_Main_Components[i, (Component_Number = Int32.Parse(textBox4.Text)) - 1];
                    //Matrix_Main_Components[i, 15] = 0;
                }

                // пошук середнього для пікселів бабуїна
                //for (int i = 0; i < 49152; i++)
                 //   Array_With_WaterMark_Pixels_Colourful[i] = Array_With_WaterMark_Pixels_Colourful[i] + 126;

                int tmp_sum_R = 0, tmp_sum_G = 0, tmp_sum_B = 0;
                int tmp_avg_R = 0, tmp_avg_G = 0, tmp_avg_B = 0;
                int tmpR1 = 0, tmpG1 = 1, tmpB1 = 2;
                for (int i = 0; i < Array_With_WaterMark_Pixels_Colourful.Length; i += 3)
                {
                    tmp_sum_R += (int)Array_With_WaterMark_Pixels_Colourful[tmpR1];
                    tmp_sum_G += (int)Array_With_WaterMark_Pixels_Colourful[tmpG1];
                    tmp_sum_B += (int)Array_With_WaterMark_Pixels_Colourful[tmpB1];
                    tmpR1 += 3;
                    tmpG1 += 3;
                    tmpB1 += 3;
                }
                tmp_avg_R = tmp_sum_R / (Array_With_WaterMark_Pixels_Colourful.Length / 3);
                tmp_avg_G = tmp_sum_G / (Array_With_WaterMark_Pixels_Colourful.Length / 3);
                tmp_avg_B = tmp_sum_B / (Array_With_WaterMark_Pixels_Colourful.Length / 3);

                int tmpR2 = 0, tmpG2 = 1, tmpB2 = 2;
                for (int i = 0; i < 49152; i += 3)
                {
                    Array_With_WaterMark_Pixels_Colourful[tmpR2] = Array_With_WaterMark_Pixels_Colourful[tmpR2] + 126;
                    Array_With_WaterMark_Pixels_Colourful[tmpG2] = Array_With_WaterMark_Pixels_Colourful[tmpG2] + 126;
                    Array_With_WaterMark_Pixels_Colourful[tmpB2] = Array_With_WaterMark_Pixels_Colourful[tmpB2] + 126;
                    tmpR2 += 3;
                    tmpG2 += 3;
                    tmpB2 += 3;
                }


                // збереження кольорового зображення після певних перетворень
                int tmpRR = 0, tmpGR = 1, tmpBR = 2;
                for (int i = 0; i < 49152; i += 3)
                {
                    String_With_Lena_Pixels_Colourful[tmpRR] = (char)Array_With_WaterMark_Pixels_Colourful[i];
                    String_With_Lena_Pixels_Colourful[tmpGR] = (char)Array_With_WaterMark_Pixels_Colourful[i + 1];
                    String_With_Lena_Pixels_Colourful[tmpBR] = (char)Array_With_WaterMark_Pixels_Colourful[i + 2];
                    tmpRR += 3;
                    tmpGR += 3;
                    tmpBR += 3;
                }

                int p = 0;
                Watermark_Reconstructed_128 = new Bitmap(Watermark_Original_128.Height, Watermark_Original_128.Width);
                int processedPixelR;
                int processedPixelG;
                int processedPixelB;
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        processedPixelR = Convert.ToInt32(String_With_Lena_Pixels_Colourful[p]);
                        if (processedPixelR < 0) processedPixelR = 0;
                        if (processedPixelR > 255) processedPixelR = 255;
                        p++;
                        processedPixelG = Convert.ToInt32(String_With_Lena_Pixels_Colourful[p]);
                        if (processedPixelG < 0) processedPixelG = 0;
                        if (processedPixelG > 255) processedPixelG = 255;
                        p++;
                        processedPixelB = Convert.ToInt32(String_With_Lena_Pixels_Colourful[p]);
                        if (processedPixelB < 0) processedPixelB = 0;
                        if (processedPixelB > 255) processedPixelB = 255;
                        p++;
                        Watermark_Reconstructed_128.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixelR), Convert.ToByte(processedPixelG), Convert.ToByte(processedPixelB)));
                    }

                Watermark_Reconstructed_128.Save("..\\..\\..\\BaboonOriginal128_Reconstructed.bmp");

                StringBuilder psnr = new StringBuilder();
                psnr.Append(Convert.ToString(ComparePSNR(Watermark_Original_128, Watermark_Reconstructed_128)));
                textBox2.Text = psnr.ToString();
                buttonShowDecryptResult.Show();
                */
            }
        }

        // кнопка для виведення результатів дешифрування
        private void button9_Click(object sender, EventArgs e)
        {
            pictureBoxReconstructedBaboon.Image = Watermark_Reconstructed_128;
            label2.Show();
            textBox2.Show();
            pictureBoxReconstructedBaboon.Show();
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void labelEncryption_Click(object sender, EventArgs e)
        {

        }

        private void fillChartGrayScale(System.Windows.Forms.DataVisualization.Charting.Chart chart)
        {
            
            // ЧОРНОБІЛЕ
            int[,] array_with_RGB_2D = new int[512, 512];
       
            
                //chart1.Titles.Add("Grayscale CHART");
                // зчитування пікселів лени 
                for (int x = 0; x < Container_Original_512.Width; x++)
                    for (int y = 0; y < Container_Original_512.Height; y++)
                        array_with_RGB_2D[x, y] = Convert.ToInt32(Container_Original_512.GetPixel(x, y).ToArgb() & 0x000000ff);
                // перепис даних з 2 вимірного в 1 вимірний
                int counter1 = 0;
                int[] array_with_RGB_1D = new int[512 * 512];
                for (int i = 0; i < 512; i++)
                    for (int j = 0; j < 512; j++)
                    {
                        array_with_RGB_1D[counter1] = array_with_RGB_2D[i, j];
                        counter1++;
                    }

                // створення масиву інтенсивностей без дублікатів
                int[] array_value = array_with_RGB_1D.Distinct();

                // створення масиву кількостей дублікатів інтенсивностей
                int[] array_value_repeat = new int[array_value.Length];
                int counter2 = 0;
                for (int i = 0; i < array_value.Length; i++)
                {
                    for (int j = 0; j < array_with_RGB_1D.Length; j++)
                    {
                        if (array_value[i] == array_with_RGB_1D[j])
                        {
                            array_value_repeat[i] = counter2++;
                        }
                    }
                    counter2 = 0;
                }

                // виведення результатів на графік
                for (int i = 0; i < array_value.Length; i++)
                    chart.Series[0].Points.AddXY(Convert.ToDouble(array_value[i]), Convert.ToDouble(array_value_repeat[i]));
            }
            

            
            
        

        private void fillChartRGB(System.Windows.Forms.DataVisualization.Charting.Chart R, System.Windows.Forms.DataVisualization.Charting.Chart G, System.Windows.Forms.DataVisualization.Charting.Chart B)
        {
            int[,] array_with_R_2D = new int[512, 512];
            int[,] array_with_G_2D = new int[512, 512];
            int[,] array_with_B_2D = new int[512, 512];
            //КОЛЬОРОВЕ
            //chart.Titles.Add("R CHART");
            // зчитування пікселів лени 
            //int[,] array_with_RGB_2D = new int[512, 512];
            for (int x = 0; x < Container_Original_512.Width; x++)
                    for (int y = 0; y < Container_Original_512.Height; y++)
                    {
                        array_with_R_2D[x, y] = Convert.ToInt32(Container_Original_512.GetPixel(x, y).R);
                        array_with_G_2D[x, y] = Convert.ToInt32(Container_Original_512.GetPixel(x, y).G);
                        array_with_B_2D[x, y] = Convert.ToInt32(Container_Original_512.GetPixel(x, y).B);
                    }
            // перепис даних з 2 вимірного в 1 вимірний
            int counter1 = 0;
            int[] array_with_R_1D = new int[512 * 512];
            int[] array_with_G_1D = new int[512 * 512];
            int[] array_with_B_1D = new int[512 * 512];
            for (int i = 0; i < 512; i++)
                for (int j = 0; j < 512; j++)
                {
                    array_with_R_1D[counter1] = array_with_R_2D[i, j];
                    array_with_G_1D[counter1] = array_with_G_2D[i, j];
                    array_with_B_1D[counter1] = array_with_B_2D[i, j];
                    counter1++;
                }

            // створення масиву інтенсивностей без дублікатів
            int[] array_value_R = array_with_R_1D.Distinct();
            int[] array_value_G = array_with_G_1D.Distinct();
            int[] array_value_B = array_with_B_1D.Distinct();

            // створення масиву кількостей дублікатів інтенсивностей
            int[] array_value_repeat_R = new int[array_value_R.Length];
            int[] array_value_repeat_G = new int[array_value_G.Length];
            int[] array_value_repeat_B = new int[array_value_B.Length];

            int counterR = 0;
            for (int i = 0; i < array_value_R.Length; i++)
            {
                for (int j = 0; j < array_with_R_1D.Length; j++)
                    if (array_value_R[i] == array_with_R_1D[j])
                        array_value_repeat_R[i] = counterR++;
                counterR = 0;
            }

            int counterG = 0;
            for (int i = 0; i < array_value_G.Length; i++)
            {
                for (int j = 0; j < array_with_G_1D.Length; j++)
                    if (array_value_G[i] == array_with_G_1D[j])
                        array_value_repeat_G[i] = counterG++;
                counterG = 0;
            }

            int counterB = 0;
            for (int i = 0; i < array_value_B.Length; i++)
            {
                for (int j = 0; j < array_with_B_1D.Length; j++)
                    if (array_value_B[i] == array_with_B_1D[j])
                        array_value_repeat_B[i] = counterB++;
                counterB = 0;
            }

            // виведення результатів на графік
            for (int i = 0; i < array_value_R.Length; i++)
                R.Series[0].Points.AddXY(Convert.ToDouble(array_value_R[i]), Convert.ToDouble(array_value_repeat_R[i]));
            for (int i = 0; i < array_value_G.Length; i++)
                G.Series[0].Points.AddXY(Convert.ToDouble(array_value_G[i]), Convert.ToDouble(array_value_repeat_G[i]));
            for (int i = 0; i < array_value_B.Length; i++)
                B.Series[0].Points.AddXY(Convert.ToDouble(array_value_B[i]), Convert.ToDouble(array_value_repeat_B[i]));
        }
        private void Button2_Click_1(object sender, EventArgs e)
        {
            var newChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            chartArea1.Name = "ChartArea1";
            newChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            newChart.Legends.Add(legend1);
            newChart.Location = new Point(0, 0);
            newChart.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "GRAYSCALE";
            newChart.Series.Add(series1);
            newChart.Size = new Size(511, 404);
            newChart.TabIndex = 26;
            newChart.Text = "chart1";
            newChart.Series[0].Color = Color.Gray;

            var Chart_R = new System.Windows.Forms.DataVisualization.Charting.Chart();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartAreaR = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legendR = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series seriesR = new System.Windows.Forms.DataVisualization.Charting.Series();
            chartAreaR.Name = "ChartAreaR";
            Chart_R.ChartAreas.Add(chartAreaR);
            legendR.Name = "LegendR";
            Chart_R.Legends.Add(legendR);
            Chart_R.Location = new Point(0, 0);
            Chart_R.Name = "chartR";
            seriesR.ChartArea = "ChartAreaR";
            seriesR.Legend = "LegendR";
            seriesR.Name = "R";
            Chart_R.Series.Add(seriesR);
            Chart_R.Size = new Size(511, 404);
            Chart_R.TabIndex = 26;
            Chart_R.Text = "chartR";
            Chart_R.Series[0].Color = Color.Red;

            var Chart_G = new System.Windows.Forms.DataVisualization.Charting.Chart();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartAreaG = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legendG = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series seriesG = new System.Windows.Forms.DataVisualization.Charting.Series();
            chartAreaG.Name = "ChartAreaG";
            Chart_G.ChartAreas.Add(chartAreaG);
            legendG.Name = "LegendG";
            Chart_G.Legends.Add(legendG);
            Chart_G.Location = new Point(0, 0);
            Chart_G.Name = "chartG";
            seriesG.ChartArea = "ChartAreaG";
            seriesG.Legend = "LegendG";
            seriesG.Name = "G";
            Chart_G.Series.Add(seriesG);
            Chart_G.Size = new Size(511, 404);
            Chart_G.TabIndex = 26;
            Chart_G.Text = "chartG";
            Chart_G.Series[0].Color = Color.Green;

            var Chart_B = new System.Windows.Forms.DataVisualization.Charting.Chart();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartAreaB = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legendB = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series seriesB = new System.Windows.Forms.DataVisualization.Charting.Series();
            chartAreaB.Name = "ChartAreaB";
            Chart_B.ChartAreas.Add(chartAreaB);
            legendB.Name = "LegendB";
            Chart_B.Legends.Add(legendB);
            Chart_B.Location = new Point(0, 0);
            Chart_B.Name = "chartB";
            seriesB.ChartArea = "ChartAreaB";
            seriesB.Legend = "LegendB";
            seriesB.Name = "B";
            Chart_B.Series.Add(seriesB);
            Chart_B.Size = new Size(511, 404);
            Chart_B.TabIndex = 26;
            Chart_B.Text = "charB";
            Chart_B.Series[0].Color = Color.Aqua;

            Colour_Mode = radioButtonGrayscale.Checked;
            if (Colour_Mode == true)
            {
                Form f2 = new Form();
                f2.Size = new Size(550, 450);
                f2.Controls.Add(newChart);
                fillChartGrayScale(newChart);
                f2.Show();
            }
            else
            {
                Form f3 = new Form();
                Form f4 = new Form();
                Form f5 = new Form();
                f3.Size = new Size(550, 450);
                f4.Size = new Size(550, 450);
                f5.Size = new Size(550, 450);
                f3.Controls.Add(Chart_R);
                f4.Controls.Add(Chart_G);
                f5.Controls.Add(Chart_B);
                fillChartRGB(Chart_R, Chart_G, Chart_B);
                f3.Show();
                f4.Show();
                f5.Show();
            }
            
        }

        private void Chart1_Click(object sender, EventArgs e)
        {
        }

        private void Chart2_Click(object sender, EventArgs e)
        {    
        }

        private void Button3_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Watermark_Original_128 = new Bitmap(openFileDialog1.FileName);
            }   
        }

        private void Button4_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Watermark_Reconstructed_128 = new Bitmap(openFileDialog1.FileName);
                button5.Show();
            }
        }
        private void Button5_Click_1(object sender, EventArgs e)
        {
            Colour_Mode = radioButtonGrayscale.Checked;
            //для чорнобілої версії
            if (Colour_Mode == true)
            {
                //зчитування пікселів оригінального ЦВЗ               *************ЧОРНОБІЛЕ
                StringBuilder String_With_WaterMark_Pixels_Grayscale = new StringBuilder();
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                        String_With_WaterMark_Pixels_Grayscale.Append(Convert.ToChar(Watermark_Original_128.GetPixel(i, j).ToArgb() & 0x000000ff));

                var BaboonOriginal128_Matrix_Grayscale = new double[Watermark_Original_128.Height, Watermark_Original_128.Width];
                int counter = 0;
                for (int i = 0; i < Watermark_Original_128.Height; i++)
                    for (int j = 0; j < Watermark_Original_128.Width; j++)
                    {
                        BaboonOriginal128_Matrix_Grayscale[i, j] = String_With_WaterMark_Pixels_Grayscale[counter];
                        counter++;
                    }

                //зчитування пікселів отриманого ЦВЗ              
                StringBuilder String_With_WaterMark_Pixels_Grayscale_Reconstructed = new StringBuilder();
                for (int i = 0; i < Watermark_Reconstructed_128.Width; i++)
                    for (int j = 0; j < Watermark_Reconstructed_128.Height; j++)
                        String_With_WaterMark_Pixels_Grayscale_Reconstructed.Append(Convert.ToChar(Watermark_Reconstructed_128.GetPixel(i, j).ToArgb() & 0x000000ff));

                var BaboonOriginal128_Matrix_Grayscale_Reconstructed = new double[Watermark_Reconstructed_128.Height, Watermark_Reconstructed_128.Width];
                int counter1 = 0;
                for (int i = 0; i < Watermark_Reconstructed_128.Height; i++)
                    for (int j = 0; j < Watermark_Reconstructed_128.Width; j++)
                    {
                        BaboonOriginal128_Matrix_Grayscale_Reconstructed[i, j] = String_With_WaterMark_Pixels_Grayscale_Reconstructed[counter1];
                        counter1++;
                    }

                // обрахунок різниці між ЦВЗ
                var WatermarkDifference_Matrix_Grayscale = new double[Watermark_Original_128.Height, Watermark_Original_128.Width];
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                            WatermarkDifference_Matrix_Grayscale[i, j] = 255 - (Math.Abs(BaboonOriginal128_Matrix_Grayscale[i, j] - BaboonOriginal128_Matrix_Grayscale_Reconstructed[i, j]));
                
                // вивід чорнобілого зображення різниці
                int counter2 = 0;

                StringBuilder String_With_WaterMarkDifference_Pixels_Grayscale = String_With_WaterMark_Pixels_Grayscale;
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        String_With_WaterMarkDifference_Pixels_Grayscale[counter2] = (char)WatermarkDifference_Matrix_Grayscale[i, j];
                        counter2++;
                    }

                int p = 0;
                Watermark_Difference = new Bitmap(Watermark_Original_128.Height, Watermark_Original_128.Width);
                int processedPixel;
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        processedPixel = Convert.ToInt32(String_With_WaterMarkDifference_Pixels_Grayscale[p]);
                        if (processedPixel < 0) processedPixel = 0;
                        if (processedPixel > 255) processedPixel = 255;
                        Watermark_Difference.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixel), Convert.ToByte(processedPixel), Convert.ToByte(processedPixel)));
                        p++;
                    }

                Watermark_Difference.Save("..\\..\\..\\WatermarkDifference_Grayscale.bmp");
                pictureBoxWatermarkDifference.Image = Watermark_Difference;
                pictureBoxWatermarkDifference.Show();

                // запис матриці різниці у файл
                StreamWriter outfile_WatermarkDifference = new StreamWriter(@"..\\..\\..\\WatermarkDifference.txt");
                for (int i = 0; i < WatermarkDifference_Matrix_Grayscale.GetLength(0); i++)
                {
                    for (int j = 0; j < WatermarkDifference_Matrix_Grayscale.GetLength(1); j++)
                    {
                        outfile_WatermarkDifference.Write("{0} ", WatermarkDifference_Matrix_Grayscale[i, j]);
                    }
                    outfile_WatermarkDifference.Write("{0}", Environment.NewLine);
                }
                outfile_WatermarkDifference.Close();

                // пошук мінімального значення інтенсивності пікселя у матриці різниці
                double min = 255;
                for (int i = 0; i < WatermarkDifference_Matrix_Grayscale.GetLength(0); i++)
                    for (int j = 0; j < WatermarkDifference_Matrix_Grayscale.GetLength(1); j++)
                        if (min > WatermarkDifference_Matrix_Grayscale[i, j])
                            min = WatermarkDifference_Matrix_Grayscale[i, j];
                StreamWriter outfile_min = new StreamWriter(@"..\\..\\..\\min.txt");
                outfile_min.Write("{0} ", min);
                outfile_min.Close();

                //фільтрування матриці різниці
                var WatermarkDifference_Matrix_Grayscale_Filtered = WatermarkDifference_Matrix_Grayscale;

                /*textBox3.Text = Convert.ToString(min);
                double treshold = Double.Parse(textBox3.Text);*/
                double treshold;
                if (textBox3.Text == "255")
                {
                    textBox3.Text = Convert.ToString(min);
                    treshold = Double.Parse(textBox3.Text);
                }
                else
                {
                    treshold = Double.Parse(textBox3.Text);
                }

                
                for (int i = 0; i < WatermarkDifference_Matrix_Grayscale.GetLength(0); i++)
                    for (int j = 0; j < WatermarkDifference_Matrix_Grayscale.GetLength(1); j++)
                        if (WatermarkDifference_Matrix_Grayscale_Filtered[i, j] > treshold)
                            WatermarkDifference_Matrix_Grayscale_Filtered[i, j] = 255;

                int counter3 = 0;

                StringBuilder String_With_WaterMarkDifference_Filtered_Pixels_Grayscale = String_With_WaterMark_Pixels_Grayscale;
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        String_With_WaterMarkDifference_Filtered_Pixels_Grayscale[counter3] = (char)WatermarkDifference_Matrix_Grayscale_Filtered[i, j];
                        counter3++;
                    }

                int p1 = 0;
                Watermark_Difference_Filtered = new Bitmap(Watermark_Original_128.Height, Watermark_Original_128.Width);
                int processedPixel1;
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        processedPixel1 = Convert.ToInt32(String_With_WaterMarkDifference_Filtered_Pixels_Grayscale[p1]);
                        if (processedPixel1 < 0) processedPixel1 = 0;
                        if (processedPixel1 > 255) processedPixel1 = 255;
                        Watermark_Difference_Filtered.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixel1), Convert.ToByte(processedPixel1), Convert.ToByte(processedPixel1)));
                        p1++;
                    }

                Watermark_Difference_Filtered.Save("..\\..\\..\\WatermarkDifference_Grayscale_Filtered.bmp");
                pictureBoxWatermarkDifferenceFiltered.Image = Watermark_Difference_Filtered;
                pictureBoxWatermarkDifferenceFiltered.Show();

                // запис відфільтрованої матриці різниці у файл
                /*StreamWriter outfile_WatermarkDifference_Filtered = new StreamWriter(@"..\\..\\..\\WatermarkDifference_Filtered.txt");
                for (int i = 0; i < WatermarkDifference_Matrix_Grayscale_Filtered.GetLength(0); i++)
                {
                    for (int j = 0; j < WatermarkDifference_Matrix_Grayscale_Filtered.GetLength(1); j++)
                    {
                        outfile_WatermarkDifference_Filtered.Write("{0} ", WatermarkDifference_Matrix_Grayscale_Filtered[i, j]);
                    }
                    outfile_WatermarkDifference_Filtered.Write("{0}", Environment.NewLine);
                }
                outfile_WatermarkDifference_Filtered.Close();*/

                // запис відфільтрованої матриці різниці у файл та запис позицій артефактів у файл
                var WatermarkDifference_Grayscale_i = new double[Watermark_Original_128.Height, Watermark_Original_128.Width];
                var WatermarkDifference_Grayscale_j = new double[Watermark_Original_128.Height, Watermark_Original_128.Width];
                StreamWriter outfile_WatermarkDifference_Filtered = new StreamWriter(@"..\\..\\..\\WatermarkDifference_Filtered.txt");
                StreamWriter outfile_WatermarkDifference_Filtered_i = new StreamWriter(@"..\\..\\..\\WatermarkDifference_Filtered_i_Grayscale.txt");
                StreamWriter outfile_WatermarkDifference_Filtered_j = new StreamWriter(@"..\\..\\..\\WatermarkDifference_Filtered_j_Grayscale.txt");
                for (int i = 0; i < WatermarkDifference_Matrix_Grayscale_Filtered.GetLength(0); i++)
                {
                    for (int j = 0; j < WatermarkDifference_Matrix_Grayscale_Filtered.GetLength(1); j++)
                    {
                        outfile_WatermarkDifference_Filtered.Write("{0} ", WatermarkDifference_Matrix_Grayscale_Filtered[i, j]);
                        if (WatermarkDifference_Matrix_Grayscale_Filtered[i, j] == 255)
                        {
                            outfile_WatermarkDifference_Filtered_i.Write("{0} ", 257);
                            outfile_WatermarkDifference_Filtered_j.Write("{0} ", 257);
                        }
                        else
                        {
                            outfile_WatermarkDifference_Filtered_i.Write("{0} ", i);
                            outfile_WatermarkDifference_Filtered_j.Write("{0} ", j);
                        }
                    }
                    outfile_WatermarkDifference_Filtered.Write("{0}", Environment.NewLine);
                    outfile_WatermarkDifference_Filtered_i.Write("{0}", Environment.NewLine);
                    outfile_WatermarkDifference_Filtered_j.Write("{0}", Environment.NewLine);
                }
                outfile_WatermarkDifference_Filtered.Close();
                outfile_WatermarkDifference_Filtered_i.Close();
                outfile_WatermarkDifference_Filtered_j.Close();


            }
            else
            // для кольорової версії
            {
                //зчитування пікселів оригінального ЦВЗ               *************КОЛЬОРОВЕ
                StringBuilder String_With_Watermark_Pixels_Colourful_R = new StringBuilder();
                StringBuilder String_With_Watermark_Pixels_Colourful_G = new StringBuilder();
                StringBuilder String_With_Watermark_Pixels_Colourful_B = new StringBuilder();
                for (int x = 0; x < Watermark_Original_128.Width; x++)
                    for (int y = 0; y < Watermark_Original_128.Height; y++)
                    {
                        String_With_Watermark_Pixels_Colourful_R.Append(Convert.ToChar(Watermark_Original_128.GetPixel(x, y).R));
                        String_With_Watermark_Pixels_Colourful_G.Append(Convert.ToChar(Watermark_Original_128.GetPixel(x, y).G));
                        String_With_Watermark_Pixels_Colourful_B.Append(Convert.ToChar(Watermark_Original_128.GetPixel(x, y).B));
                    }

                var Matrix_Main_Watermark_R = new Int32[Watermark_Original_128.Height, Watermark_Original_128.Width];
                var Matrix_Main_Watermark_G = new Int32[Watermark_Original_128.Height, Watermark_Original_128.Width];
                var Matrix_Main_Watermark_B = new Int32[Watermark_Original_128.Height, Watermark_Original_128.Width];

                int counter_tmp = 0;
                for (int i = 0; i < Watermark_Original_128.Height; i++)
                    for (int j = 0; j < Watermark_Original_128.Width; j++)
                    {
                        Matrix_Main_Watermark_R[i, j] = String_With_Watermark_Pixels_Colourful_R[counter_tmp];
                        Matrix_Main_Watermark_G[i, j] = String_With_Watermark_Pixels_Colourful_G[counter_tmp];
                        Matrix_Main_Watermark_B[i, j] = String_With_Watermark_Pixels_Colourful_B[counter_tmp];
                        counter_tmp++;
                    }


                //зчитування пікселів отриманого ЦВЗ
                StringBuilder String_With_Watermark_Pixels_Colourful_R_Reconstructed = new StringBuilder();
                StringBuilder String_With_Watermark_Pixels_Colourful_G_Reconstructed = new StringBuilder();
                StringBuilder String_With_Watermark_Pixels_Colourful_B_Reconstructed = new StringBuilder();
                for (int x = 0; x < Watermark_Reconstructed_128.Width; x++)
                    for (int y = 0; y < Watermark_Reconstructed_128.Height; y++)
                    {
                        String_With_Watermark_Pixels_Colourful_R_Reconstructed.Append(Convert.ToChar(Watermark_Reconstructed_128.GetPixel(x, y).R));
                        String_With_Watermark_Pixels_Colourful_G_Reconstructed.Append(Convert.ToChar(Watermark_Reconstructed_128.GetPixel(x, y).G));
                        String_With_Watermark_Pixels_Colourful_B_Reconstructed.Append(Convert.ToChar(Watermark_Reconstructed_128.GetPixel(x, y).B));
                    }

                var Matrix_Main_R_Reconstructed = new Int32[Watermark_Reconstructed_128.Height, Watermark_Reconstructed_128.Width];
                var Matrix_Main_G_Reconstructed = new Int32[Watermark_Reconstructed_128.Height, Watermark_Reconstructed_128.Width];
                var Matrix_Main_B_Reconstructed = new Int32[Watermark_Reconstructed_128.Height, Watermark_Reconstructed_128.Width];

                int tmp_counter = 0;
                for (int i = 0; i < Watermark_Reconstructed_128.Width; i++)
                    for (int j = 0; j < Watermark_Reconstructed_128.Height; j++)
                    {
                        Matrix_Main_R_Reconstructed[i, j] = String_With_Watermark_Pixels_Colourful_R_Reconstructed[tmp_counter];
                        Matrix_Main_G_Reconstructed[i, j] = String_With_Watermark_Pixels_Colourful_G_Reconstructed[tmp_counter];
                        Matrix_Main_B_Reconstructed[i, j] = String_With_Watermark_Pixels_Colourful_B_Reconstructed[tmp_counter];
                        tmp_counter++;
                    }


                // обрахунок різниці між ЦВЗ
                var WatermarkDifference_Matrix_Colourful_R = new double[Watermark_Original_128.Height, Watermark_Original_128.Width];
                var WatermarkDifference_Matrix_Colourful_G = new double[Watermark_Original_128.Height, Watermark_Original_128.Width];
                var WatermarkDifference_Matrix_Colourful_B = new double[Watermark_Original_128.Height, Watermark_Original_128.Width];
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        WatermarkDifference_Matrix_Colourful_R[i, j] = 255 - (Math.Abs(Matrix_Main_Watermark_R[i, j] - Matrix_Main_R_Reconstructed[i, j]));
                        WatermarkDifference_Matrix_Colourful_G[i, j] = 255 - (Math.Abs(Matrix_Main_Watermark_G[i, j] - Matrix_Main_G_Reconstructed[i, j]));
                        WatermarkDifference_Matrix_Colourful_B[i, j] = 255 - (Math.Abs(Matrix_Main_Watermark_B[i, j] - Matrix_Main_B_Reconstructed[i, j]));
                    }


                // вивід кольорового зображення різниці
                StringBuilder String_With_Watermark_Difference_Pixels_Colourful_R = new StringBuilder();
                StringBuilder String_With_Watermark_Difference_Pixels_Colourful_G = new StringBuilder();
                StringBuilder String_With_Watermark_Difference_Pixels_Colourful_B = new StringBuilder();

                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        String_With_Watermark_Difference_Pixels_Colourful_R.Append((char)WatermarkDifference_Matrix_Colourful_R[i, j]);
                        String_With_Watermark_Difference_Pixels_Colourful_G.Append((char)WatermarkDifference_Matrix_Colourful_G[i, j]);
                        String_With_Watermark_Difference_Pixels_Colourful_B.Append((char)WatermarkDifference_Matrix_Colourful_B[i, j]);
                    }

                // Збереження зображення різниці
                int p = 0;
                Watermark_Difference = new Bitmap(Watermark_Original_128.Height, Watermark_Original_128.Width);
                int processedPixelR;
                int processedPixelG;
                int processedPixelB;
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        processedPixelR = Convert.ToInt32(String_With_Watermark_Difference_Pixels_Colourful_R[p]);
                        if (processedPixelR < 0) processedPixelR = 0;
                        if (processedPixelR > 255) processedPixelR = 255;
                        processedPixelG = Convert.ToInt32(String_With_Watermark_Difference_Pixels_Colourful_G[p]);
                        if (processedPixelG < 0) processedPixelG = 0;
                        if (processedPixelG > 255) processedPixelG = 255;
                        processedPixelB = Convert.ToInt32(String_With_Watermark_Difference_Pixels_Colourful_B[p]);
                        if (processedPixelB < 0) processedPixelB = 0;
                        if (processedPixelB > 255) processedPixelB = 255;
                        p++;
                        Watermark_Difference.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixelR), Convert.ToByte(processedPixelG), Convert.ToByte(processedPixelB)));
                    }

                Watermark_Difference.Save("..\\..\\..\\WatermarkDifference_Colourful.bmp");
                pictureBoxWatermarkDifference.Image = Watermark_Difference;
                pictureBoxWatermarkDifference.Show();
                
                // запис матриці різниці у файл
                        //для R
                StreamWriter outfile_WatermarkDifference_R = new StreamWriter(@"..\\..\\..\\WatermarkDifference_R.txt");
                for (int i = 0; i < WatermarkDifference_Matrix_Colourful_R.GetLength(0); i++)
                {
                    for (int j = 0; j < WatermarkDifference_Matrix_Colourful_R.GetLength(1); j++)
                    {
                        outfile_WatermarkDifference_R.Write("{0} ", WatermarkDifference_Matrix_Colourful_R[i, j]);
                    }
                    outfile_WatermarkDifference_R.Write("{0}", Environment.NewLine);
                }
                outfile_WatermarkDifference_R.Close();

                        //для G
                StreamWriter outfile_WatermarkDifference_G = new StreamWriter(@"..\\..\\..\\WatermarkDifference_G.txt");
                for (int i = 0; i < WatermarkDifference_Matrix_Colourful_G.GetLength(0); i++)
                {
                    for (int j = 0; j < WatermarkDifference_Matrix_Colourful_G.GetLength(1); j++)
                    {
                        outfile_WatermarkDifference_G.Write("{0} ", WatermarkDifference_Matrix_Colourful_G[i, j]);
                    }
                    outfile_WatermarkDifference_G.Write("{0}", Environment.NewLine);
                }
                outfile_WatermarkDifference_G.Close();

                        //для B
                StreamWriter outfile_WatermarkDifference_B = new StreamWriter(@"..\\..\\..\\WatermarkDifference_B.txt");
                for (int i = 0; i < WatermarkDifference_Matrix_Colourful_B.GetLength(0); i++)
                {
                    for (int j = 0; j < WatermarkDifference_Matrix_Colourful_B.GetLength(1); j++)
                    {
                        outfile_WatermarkDifference_B.Write("{0} ", WatermarkDifference_Matrix_Colourful_B[i, j]);
                    }
                    outfile_WatermarkDifference_B.Write("{0}", Environment.NewLine);
                }
                outfile_WatermarkDifference_B.Close();
                
                // пошук мінімального значення інтенсивності пікселя у матриці різниці
                        // для R
                double min_R = 255;
                for (int i = 0; i < WatermarkDifference_Matrix_Colourful_R.GetLength(0); i++)
                    for (int j = 0; j < WatermarkDifference_Matrix_Colourful_R.GetLength(1); j++)
                        if (min_R > WatermarkDifference_Matrix_Colourful_R[i, j])
                            min_R = WatermarkDifference_Matrix_Colourful_R[i, j];
                StreamWriter outfile_min_R = new StreamWriter(@"..\\..\\..\\min_R.txt");
                outfile_min_R.Write("{0} ", min_R);
                outfile_min_R.Close();

                        // для G
                double min_G = 255;
                for (int i = 0; i < WatermarkDifference_Matrix_Colourful_G.GetLength(0); i++)
                    for (int j = 0; j < WatermarkDifference_Matrix_Colourful_G.GetLength(1); j++)
                        if (min_G > WatermarkDifference_Matrix_Colourful_G[i, j])
                            min_G = WatermarkDifference_Matrix_Colourful_G[i, j];
                StreamWriter outfile_min_G = new StreamWriter(@"..\\..\\..\\min_G.txt");
                outfile_min_G.Write("{0} ", min_G);
                outfile_min_G.Close();

                        // для B
                double min_B = 255;
                for (int i = 0; i < WatermarkDifference_Matrix_Colourful_B.GetLength(0); i++)
                    for (int j = 0; j < WatermarkDifference_Matrix_Colourful_B.GetLength(1); j++)
                        if (min_B > WatermarkDifference_Matrix_Colourful_B[i, j])
                            min_B = WatermarkDifference_Matrix_Colourful_B[i, j];
                StreamWriter outfile_min_B = new StreamWriter(@"..\\..\\..\\min_B.txt");
                outfile_min_B.Write("{0} ", min_B);
                outfile_min_B.Close();
                
                //фільтрування матриць різниці
                var WatermarkDifference_Matrix_Filtered_R = WatermarkDifference_Matrix_Colourful_R;
                var WatermarkDifference_Matrix_Filtered_G = WatermarkDifference_Matrix_Colourful_G;
                var WatermarkDifference_Matrix_Filtered_B = WatermarkDifference_Matrix_Colourful_B;

                //textBox3.Text = Convert.ToString(min);
                //double treshold = Double.Parse(textBox3.Text);
                    //для R
                double treshold_R;
                if (textBox11.Text == "255")
                {
                    textBox11.Text = Convert.ToString(min_R);
                    treshold_R = Double.Parse(textBox11.Text);
                }
                else
                {
                    treshold_R = Double.Parse(textBox11.Text);
                }

                for (int i = 0; i < WatermarkDifference_Matrix_Colourful_R.GetLength(0); i++)
                    for (int j = 0; j < WatermarkDifference_Matrix_Colourful_R.GetLength(1); j++)
                        if (WatermarkDifference_Matrix_Colourful_R[i, j] > treshold_R)
                            WatermarkDifference_Matrix_Filtered_R[i, j] = 255;

                //для G
                double treshold_G;
                if (textBox12.Text == "255")
                {
                    textBox12.Text = Convert.ToString(min_G);
                    treshold_G = Double.Parse(textBox12.Text);
                }
                else
                {
                    treshold_G = Double.Parse(textBox12.Text);
                }

                for (int i = 0; i < WatermarkDifference_Matrix_Colourful_G.GetLength(0); i++)
                    for (int j = 0; j < WatermarkDifference_Matrix_Colourful_G.GetLength(1); j++)
                        if (WatermarkDifference_Matrix_Colourful_G[i, j] > treshold_G)
                            WatermarkDifference_Matrix_Filtered_G[i, j] = 255;

                //для B
                double treshold_B;
                if (textBox13.Text == "255")
                {
                    textBox13.Text = Convert.ToString(min_B);
                    treshold_B = Double.Parse(textBox13.Text);
                }
                else
                {
                    treshold_B = Double.Parse(textBox13.Text);
                }
                                               
                for (int i = 0; i < WatermarkDifference_Matrix_Colourful_B.GetLength(0); i++)
                    for (int j = 0; j < WatermarkDifference_Matrix_Colourful_B.GetLength(1); j++)
                        if (WatermarkDifference_Matrix_Colourful_B[i, j] > treshold_B)
                            WatermarkDifference_Matrix_Filtered_B[i, j] = 255;

                
                // вивід різниці
                StringBuilder String_With_WaterMarkDifference_Filtered_Pixels_Colourful_R = new StringBuilder();
                StringBuilder String_With_WaterMarkDifference_Filtered_Pixels_Colourful_G = new StringBuilder();
                StringBuilder String_With_WaterMarkDifference_Filtered_Pixels_Colourful_B = new StringBuilder();
                for (int i = 0; i < Watermark_Original_128.Width; i ++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        String_With_WaterMarkDifference_Filtered_Pixels_Colourful_R.Append((char)WatermarkDifference_Matrix_Filtered_R[i, j]);
                        String_With_WaterMarkDifference_Filtered_Pixels_Colourful_G.Append((char)WatermarkDifference_Matrix_Filtered_G[i, j]);
                        String_With_WaterMarkDifference_Filtered_Pixels_Colourful_B.Append((char)WatermarkDifference_Matrix_Filtered_B[i, j]);
                    }

                int p1 = 0;
                Watermark_Difference_Filtered = new Bitmap(Watermark_Original_128.Height, Watermark_Original_128.Width);
                int processedPixelR1;
                int processedPixelG1;
                int processedPixelB1;
                for (int i = 0; i < Watermark_Original_128.Width; i++)
                    for (int j = 0; j < Watermark_Original_128.Height; j++)
                    {
                        processedPixelR1 = Convert.ToInt32(String_With_WaterMarkDifference_Filtered_Pixels_Colourful_R[p1]);
                        if (processedPixelR1 < 0) processedPixelR1 = 0;
                        if (processedPixelR1 > 255) processedPixelR1 = 255;
                        processedPixelG1 = Convert.ToInt32(String_With_WaterMarkDifference_Filtered_Pixels_Colourful_G[p1]);
                        if (processedPixelG1 < 0) processedPixelG1 = 0;
                        if (processedPixelG1 > 255) processedPixelG1 = 255;
                        processedPixelB1 = Convert.ToInt32(String_With_WaterMarkDifference_Filtered_Pixels_Colourful_B[p1]);
                        if (processedPixelB1 < 0) processedPixelB1 = 0;
                        if (processedPixelB1 > 255) processedPixelB1 = 255;
                        p1++;
                        Watermark_Difference_Filtered.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixelR1), Convert.ToByte(processedPixelG1), Convert.ToByte(processedPixelB1)));
                    }
                Watermark_Difference_Filtered.Save("..\\..\\..\\WatermarkDifference_Colourful_Filtered.bmp");
                pictureBoxWatermarkDifferenceFiltered.Image = Watermark_Difference_Filtered;
                pictureBoxWatermarkDifferenceFiltered.Show();
                
                // запис відфільтрованої матриці різниці у файл
                        // для R
                StreamWriter outfile_WatermarkDifference_Filtered_R = new StreamWriter(@"..\\..\\..\\WatermarkDifference_Filtered_R.txt");
                for (int i = 0; i < WatermarkDifference_Matrix_Filtered_R.GetLength(0); i++)
                {
                    for (int j = 0; j < WatermarkDifference_Matrix_Filtered_R.GetLength(1); j++)
                    {
                        outfile_WatermarkDifference_Filtered_R.Write("{0} ", WatermarkDifference_Matrix_Filtered_R[i, j]);
                    }
                    outfile_WatermarkDifference_Filtered_R.Write("{0}", Environment.NewLine);
                }
                outfile_WatermarkDifference_Filtered_R.Close();
                        // для G
                StreamWriter outfile_WatermarkDifference_Filtered_G = new StreamWriter(@"..\\..\\..\\WatermarkDifference_Filtered_G.txt");
                for (int i = 0; i < WatermarkDifference_Matrix_Filtered_G.GetLength(0); i++)
                {
                    for (int j = 0; j < WatermarkDifference_Matrix_Filtered_G.GetLength(1); j++)
                    {
                        outfile_WatermarkDifference_Filtered_G.Write("{0} ", WatermarkDifference_Matrix_Filtered_G[i, j]);
                    }
                    outfile_WatermarkDifference_Filtered_G.Write("{0}", Environment.NewLine);
                }
                outfile_WatermarkDifference_Filtered_G.Close();
                        // для B
                StreamWriter outfile_WatermarkDifference_Filtered_B = new StreamWriter(@"..\\..\\..\\WatermarkDifference_Filtered_B.txt");
                for (int i = 0; i < WatermarkDifference_Matrix_Filtered_B.GetLength(0); i++)
                {
                    for (int j = 0; j < WatermarkDifference_Matrix_Filtered_B.GetLength(1); j++)
                    {
                        outfile_WatermarkDifference_Filtered_B.Write("{0} ", WatermarkDifference_Matrix_Filtered_B[i, j]);
                    }
                    outfile_WatermarkDifference_Filtered_B.Write("{0}", Environment.NewLine);
                }
                outfile_WatermarkDifference_Filtered_B.Close();

                /*
                 // запис відфільтрованої матриці різниці у файл та запис позицій артефактів у файл
                var WatermarkDifference_Grayscale_i = new double[Watermark_Original_128.Height, Watermark_Original_128.Width];
                var WatermarkDifference_Grayscale_j = new double[Watermark_Original_128.Height, Watermark_Original_128.Width];
                StreamWriter outfile_WatermarkDifference_Filtered = new StreamWriter(@"..\\..\\..\\WatermarkDifference_Filtered.txt");
                StreamWriter outfile_WatermarkDifference_Filtered_i = new StreamWriter(@"..\\..\\..\\WatermarkDifference_Filtered_i_Grayscale.txt");
                StreamWriter outfile_WatermarkDifference_Filtered_j = new StreamWriter(@"..\\..\\..\\WatermarkDifference_Filtered_j_Grayscale.txt");
                for (int i = 0; i < WatermarkDifference_Matrix_Grayscale_Filtered.GetLength(0); i++)
                {
                    for (int j = 0; j < WatermarkDifference_Matrix_Grayscale_Filtered.GetLength(1); j++)
                    {
                        outfile_WatermarkDifference_Filtered.Write("{0} ", WatermarkDifference_Matrix_Grayscale_Filtered[i, j]);
                        if (WatermarkDifference_Matrix_Grayscale_Filtered[i, j] == 255)
                        {
                            outfile_WatermarkDifference_Filtered_i.Write("{0} ", 257);
                            outfile_WatermarkDifference_Filtered_j.Write("{0} ", 257);
                        }
                        else
                        {
                            outfile_WatermarkDifference_Filtered_i.Write("{0} ", i);
                            outfile_WatermarkDifference_Filtered_j.Write("{0} ", j);
                        }
                    }
                    outfile_WatermarkDifference_Filtered.Write("{0}", Environment.NewLine);
                    outfile_WatermarkDifference_Filtered_i.Write("{0}", Environment.NewLine);
                    outfile_WatermarkDifference_Filtered_j.Write("{0}", Environment.NewLine);
                }
                outfile_WatermarkDifference_Filtered.Close();
                outfile_WatermarkDifference_Filtered_i.Close();
                outfile_WatermarkDifference_Filtered_j.Close();
                */
            }
        }

        private void PictureBoxOriginalImage_Click(object sender, EventArgs e)
        {

        }

        private void PictureBoxImageWithWatermark_Click(object sender, EventArgs e)
        {

        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void Label3_Click_1(object sender, EventArgs e)
        {

        }

        private void Label4_Click(object sender, EventArgs e)
        {

        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void PictureBoxWatermarkDifferenceFiltered_Click(object sender, EventArgs e)
        {

        }

        private void Button6_Click_1(object sender, EventArgs e)
        {
            // обрахунок різниці контейнера
            Colour_Mode = radioButtonGrayscale.Checked;
            //для чорнобілої версії
            if (Colour_Mode == true)
            {

                // зчитування пікселів лени       *************ЧОРНОБІЛЕ
                StringBuilder String_With_Lena_Pixels = new StringBuilder();
                for (int x = 0; x < Container_Original_512.Width; x++)
                    for (int y = 0; y < Container_Original_512.Height; y++)
                        String_With_Lena_Pixels.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).ToArgb() & 0x000000ff));

                var Matrix_Main = new Int32[Container_Original_512.Height, Container_Original_512.Width];
                int counter = 0;
                for (int i = 0; i < Container_Original_512.Height; i++)
                    for (int j = 0; j < Container_Original_512.Width; j++)
                    {
                        Matrix_Main[i, j] = String_With_Lena_Pixels[counter];
                        counter++;
                    }

                //зчитування пікселів отриманої лени            
                StringBuilder String_With_Lena_Pixels_Reconstructed = new StringBuilder();
                for (int i = 0; i < Container_Reconstructed_512.Width; i++)
                    for (int j = 0; j < Container_Reconstructed_512.Height; j++)
                        String_With_Lena_Pixels_Reconstructed.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(i, j).ToArgb() & 0x000000ff));

                var LenaOriginal512_Matrix_Reconstructed = new double[Container_Reconstructed_512.Height, Container_Reconstructed_512.Width];
                int counter1 = 0;
                for (int i = 0; i < Container_Reconstructed_512.Height; i++)
                    for (int j = 0; j < Container_Reconstructed_512.Width; j++)
                    {
                        LenaOriginal512_Matrix_Reconstructed[i, j] = String_With_Lena_Pixels_Reconstructed[counter1];
                        counter1++;
                    }

                // обрахунок різниці між ЦВЗ
                var ContainerDifference_Matrix_Grayscale = new double[Container_Original_512.Height, Container_Original_512.Width];
                for (int i = 0; i < Container_Original_512.Width; i++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                        ContainerDifference_Matrix_Grayscale[i, j] = 255 - (Math.Abs(Matrix_Main[i, j] - LenaOriginal512_Matrix_Reconstructed[i, j]));

                // вивід чорнобілого зображення різниці
                int counter2 = 0;

                //StringBuilder String_With_ContainerDifference_Pixels_Grayscale = String_With_Lena_Pixels;
                StringBuilder String_With_ContainerDifference_Pixels_Grayscale = new StringBuilder();
                for (int i = 0; i < Container_Original_512.Width; i++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        //String_With_ContainerDifference_Pixels_Grayscale[counter2] = (char)ContainerDifference_Matrix_Grayscale[i, j];
                        //counter2++;
                        String_With_ContainerDifference_Pixels_Grayscale.Append((char)ContainerDifference_Matrix_Grayscale[i, j]);
                    }

                int p = 0;
                Container_Difference = new Bitmap(Container_Original_512.Height, Container_Original_512.Width);
                int processedPixel;
                for (int i = 0; i < Container_Original_512.Width; i++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        processedPixel = Convert.ToInt32(String_With_ContainerDifference_Pixels_Grayscale[p]);
                        if (processedPixel < 0) processedPixel = 0;
                        if (processedPixel > 255) processedPixel = 255;
                        Container_Difference.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixel), Convert.ToByte(processedPixel), Convert.ToByte(processedPixel)));
                        p++;
                    }

                Container_Difference.Save("..\\..\\..\\ContainerDifference_Grayscale.bmp");
                pictureBox1.Image = Container_Difference;
                pictureBox1.Show();

                // запис матриці різниці у файл
                StreamWriter outfile_ContainerDifference = new StreamWriter(@"..\\..\\..\\ContainerDifference.txt");
                for (int i = 0; i < ContainerDifference_Matrix_Grayscale.GetLength(0); i++)
                {
                    for (int j = 0; j < ContainerDifference_Matrix_Grayscale.GetLength(1); j++)
                    {
                        outfile_ContainerDifference.Write("{0} ", ContainerDifference_Matrix_Grayscale[i, j]);
                    }
                    outfile_ContainerDifference.Write("{0}", Environment.NewLine);
                }
                outfile_ContainerDifference.Close();
            }
            else
            // ДЛЯ КОЛЬОРОВОЇ
            {
                // зчитування пікселів лени       *************Кольорове
                StringBuilder String_With_Lena_Pixels_Colourful_R = new StringBuilder();
                StringBuilder String_With_Lena_Pixels_Colourful_G = new StringBuilder();
                StringBuilder String_With_Lena_Pixels_Colourful_B = new StringBuilder();
                for (int x = 0; x < Container_Original_512.Width; x++)
                    for (int y = 0; y < Container_Original_512.Height; y++)
                    {
                        String_With_Lena_Pixels_Colourful_R.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).R));
                        String_With_Lena_Pixels_Colourful_G.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).G));
                        String_With_Lena_Pixels_Colourful_B.Append(Convert.ToChar(Container_Original_512.GetPixel(x, y).B));
                    }

                var Matrix_Main_Container_R = new Int32[Container_Original_512.Height, Container_Original_512.Width];
                var Matrix_Main_Container_G = new Int32[Container_Original_512.Height, Container_Original_512.Width];
                var Matrix_Main_Container_B = new Int32[Container_Original_512.Height, Container_Original_512.Width];

                int counter_tmp = 0;
                for (int i = 0; i < Container_Original_512.Height; i++)
                    for (int j = 0; j < Container_Original_512.Width; j++)
                    {
                        Matrix_Main_Container_R[i, j] = String_With_Lena_Pixels_Colourful_R[counter_tmp];
                        Matrix_Main_Container_G[i, j] = String_With_Lena_Pixels_Colourful_G[counter_tmp];
                        Matrix_Main_Container_B[i, j] = String_With_Lena_Pixels_Colourful_B[counter_tmp];
                        counter_tmp++;
                    }
                
                // зчитування пікселів контейнера з ЦВЗ R G B (лени та бабуїна)

                StringBuilder String_With_Lena_Pixels_Colourful_R_Reconstructed = new StringBuilder();
                StringBuilder String_With_Lena_Pixels_Colourful_G_Reconstructed = new StringBuilder();
                StringBuilder String_With_Lena_Pixels_Colourful_B_Reconstructed = new StringBuilder();
                for (int x = 0; x < Container_Reconstructed_512.Width; x++)
                    for (int y = 0; y < Container_Reconstructed_512.Height; y++)
                    {
                        String_With_Lena_Pixels_Colourful_R_Reconstructed.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(x, y).R));
                        String_With_Lena_Pixels_Colourful_G_Reconstructed.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(x, y).G));
                        String_With_Lena_Pixels_Colourful_B_Reconstructed.Append(Convert.ToChar(Container_Reconstructed_512.GetPixel(x, y).B));
                    }

                var Matrix_Main_R_Reconstructed = new Int32[Container_Reconstructed_512.Height, Container_Reconstructed_512.Width];
                var Matrix_Main_G_Reconstructed = new Int32[Container_Reconstructed_512.Height, Container_Reconstructed_512.Width];
                var Matrix_Main_B_Reconstructed = new Int32[Container_Reconstructed_512.Height, Container_Reconstructed_512.Width];

                int tmp_counter = 0;
                for (int i = 0; i < Container_Reconstructed_512.Width; i++)
                    for (int j = 0; j < Container_Reconstructed_512.Height; j++)
                    {
                        Matrix_Main_R_Reconstructed[i, j] = String_With_Lena_Pixels_Colourful_R_Reconstructed[tmp_counter];
                        Matrix_Main_G_Reconstructed[i, j] = String_With_Lena_Pixels_Colourful_G_Reconstructed[tmp_counter];
                        Matrix_Main_B_Reconstructed[i, j] = String_With_Lena_Pixels_Colourful_B_Reconstructed[tmp_counter];
                        tmp_counter++;
                    }

                // обрахунок різниці між ЦВЗ
                var ContainerDifference_Matrix_Colourful_R = new double[Container_Original_512.Height, Container_Original_512.Width];
                var ContainerDifference_Matrix_Colourful_G = new double[Container_Original_512.Height, Container_Original_512.Width];
                var ContainerDifference_Matrix_Colourful_B = new double[Container_Original_512.Height, Container_Original_512.Width];
                for (int i = 0; i < Container_Original_512.Width; i++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        ContainerDifference_Matrix_Colourful_R[i, j] = 255 - (Math.Abs(Matrix_Main_Container_R[i, j] - Matrix_Main_R_Reconstructed[i, j]));
                        ContainerDifference_Matrix_Colourful_G[i, j] = 255 - (Math.Abs(Matrix_Main_Container_G[i, j] - Matrix_Main_G_Reconstructed[i, j]));
                        ContainerDifference_Matrix_Colourful_B[i, j] = 255 - (Math.Abs(Matrix_Main_Container_B[i, j] - Matrix_Main_B_Reconstructed[i, j]));
                    }
                
                // вивід кольорового зображення після певних перетворень
                StringBuilder String_With_Container_Difference_Pixels_Colourful_R = new StringBuilder();
                StringBuilder String_With_Container_Difference_Pixels_Colourful_G = new StringBuilder();
                StringBuilder String_With_Container_Difference_Pixels_Colourful_B = new StringBuilder();

                for (int i = 0; i < Container_Original_512.Width; i++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        String_With_Container_Difference_Pixels_Colourful_R.Append((char)ContainerDifference_Matrix_Colourful_R[i, j]);
                        String_With_Container_Difference_Pixels_Colourful_G.Append((char)ContainerDifference_Matrix_Colourful_G[i, j]);
                        String_With_Container_Difference_Pixels_Colourful_B.Append((char)ContainerDifference_Matrix_Colourful_B[i, j]);
                    }

                // Збереження зображення різниці
                int p = 0;
                Container_Reconstructed_512 = new Bitmap(Container_Original_512.Height, Container_Original_512.Width);
                int processedPixelR;
                int processedPixelG;
                int processedPixelB;
                for (int i = 0; i < Container_Original_512.Width; i++)
                    for (int j = 0; j < Container_Original_512.Height; j++)
                    {
                        processedPixelR = Convert.ToInt32(String_With_Container_Difference_Pixels_Colourful_R[p]);
                        if (processedPixelR < 0) processedPixelR = 0;
                        if (processedPixelR > 255) processedPixelR = 255;
                        processedPixelG = Convert.ToInt32(String_With_Container_Difference_Pixels_Colourful_G[p]);
                        if (processedPixelG < 0) processedPixelG = 0;
                        if (processedPixelG > 255) processedPixelG = 255;
                        processedPixelB = Convert.ToInt32(String_With_Container_Difference_Pixels_Colourful_B[p]);
                        if (processedPixelB < 0) processedPixelB = 0;
                        if (processedPixelB > 255) processedPixelB = 255;
                        p++;
                        Container_Reconstructed_512.SetPixel(i, j, Color.FromArgb(Convert.ToByte(processedPixelR), Convert.ToByte(processedPixelG), Convert.ToByte(processedPixelB)));
                    }
                Container_Reconstructed_512.Save("..\\..\\..\\ContainerDifference_Colourful.bmp");
                pictureBox1.Image = Container_Reconstructed_512;
                pictureBox1.Show();

                // запис матриць різниці R G B у файл 
                // для R
                StreamWriter outfile_ContainerDifference_R = new StreamWriter(@"..\\..\\..\\ContainerDifference_R.txt");
                for (int i = 0; i < ContainerDifference_Matrix_Colourful_R.GetLength(0); i++)
                {
                    for (int j = 0; j < ContainerDifference_Matrix_Colourful_R.GetLength(1); j++)
                    {
                        outfile_ContainerDifference_R.Write("{0} ", ContainerDifference_Matrix_Colourful_R[i, j]);
                    }
                    outfile_ContainerDifference_R.Write("{0}", Environment.NewLine);
                }
                outfile_ContainerDifference_R.Close();
                // для G
                StreamWriter outfile_ContainerDifference_G = new StreamWriter(@"..\\..\\..\\ContainerDifference_G.txt");
                for (int i = 0; i < ContainerDifference_Matrix_Colourful_G.GetLength(0); i++)
                {
                    for (int j = 0; j < ContainerDifference_Matrix_Colourful_G.GetLength(1); j++)
                    {
                        outfile_ContainerDifference_G.Write("{0} ", ContainerDifference_Matrix_Colourful_G[i, j]);
                    }
                    outfile_ContainerDifference_G.Write("{0}", Environment.NewLine);
                }
                outfile_ContainerDifference_G.Close();
                // для B
                StreamWriter outfile_ContainerDifference_B = new StreamWriter(@"..\\..\\..\\ContainerDifference_B.txt");
                for (int i = 0; i < ContainerDifference_Matrix_Colourful_B.GetLength(0); i++)
                {
                    for (int j = 0; j < ContainerDifference_Matrix_Colourful_B.GetLength(1); j++)
                    {
                        outfile_ContainerDifference_B.Write("{0} ", ContainerDifference_Matrix_Colourful_B[i, j]);
                    }
                    outfile_ContainerDifference_B.Write("{0}", Environment.NewLine);
                }
                outfile_ContainerDifference_B.Close();
            }
        }

        private void PictureBoxWatermarkDifference_Click(object sender, EventArgs e)
        {

        }

        private void Button7_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Container_Original_512 = new Bitmap(openFileDialog1.FileName);
            }
        }

        private void Button8_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Container_Reconstructed_512 = new Bitmap(openFileDialog1.FileName);
                button6.Show();
            }
        }
    }
}
