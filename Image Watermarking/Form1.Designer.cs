namespace Diploma
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonOriginalImageDownload = new System.Windows.Forms.Button();
            this.buttonOriginalWatermarkDownload = new System.Windows.Forms.Button();
            this.pictureBoxOriginalImage = new System.Windows.Forms.PictureBox();
            this.pictureBoxOriginalWatermark = new System.Windows.Forms.PictureBox();
            this.buttonEncrypt = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.radioButtonGrayscale = new System.Windows.Forms.RadioButton();
            this.radioButtonColourful = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxImageResultWithWatermark = new System.Windows.Forms.PictureBox();
            this.buttonShowEncryptResult = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonImageWithWatermarkDownload = new System.Windows.Forms.Button();
            this.buttonReadKey1 = new System.Windows.Forms.Button();
            this.buttonReadKey2 = new System.Windows.Forms.Button();
            this.pictureBoxImageWithWatermark = new System.Windows.Forms.PictureBox();
            this.buttonDecrypt = new System.Windows.Forms.Button();
            this.buttonShowDecryptResult = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.pictureBoxReconstructedBaboon = new System.Windows.Forms.PictureBox();
            this.buttonWriteKey1 = new System.Windows.Forms.Button();
            this.buttonWriteKey2 = new System.Windows.Forms.Button();
            this.labelEncryption = new System.Windows.Forms.Label();
            this.labelDecryption = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.pictureBoxWatermarkDifference = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBoxWatermarkDifferenceFiltered = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginalImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginalWatermark)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImageResultWithWatermark)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImageWithWatermark)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReconstructedBaboon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWatermarkDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWatermarkDifferenceFiltered)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOriginalImageDownload
            // 
            this.buttonOriginalImageDownload.Location = new System.Drawing.Point(97, 40);
            this.buttonOriginalImageDownload.Name = "buttonOriginalImageDownload";
            this.buttonOriginalImageDownload.Size = new System.Drawing.Size(106, 43);
            this.buttonOriginalImageDownload.TabIndex = 0;
            this.buttonOriginalImageDownload.Text = "Download Image";
            this.buttonOriginalImageDownload.UseVisualStyleBackColor = true;
            this.buttonOriginalImageDownload.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonOriginalWatermarkDownload
            // 
            this.buttonOriginalWatermarkDownload.Location = new System.Drawing.Point(209, 40);
            this.buttonOriginalWatermarkDownload.Name = "buttonOriginalWatermarkDownload";
            this.buttonOriginalWatermarkDownload.Size = new System.Drawing.Size(100, 43);
            this.buttonOriginalWatermarkDownload.TabIndex = 1;
            this.buttonOriginalWatermarkDownload.Text = "Download Watermark";
            this.buttonOriginalWatermarkDownload.UseVisualStyleBackColor = true;
            this.buttonOriginalWatermarkDownload.Click += new System.EventHandler(this.button2_Click);
            // 
            // pictureBoxOriginalImage
            // 
            this.pictureBoxOriginalImage.Location = new System.Drawing.Point(12, 92);
            this.pictureBoxOriginalImage.Name = "pictureBoxOriginalImage";
            this.pictureBoxOriginalImage.Size = new System.Drawing.Size(256, 256);
            this.pictureBoxOriginalImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxOriginalImage.TabIndex = 2;
            this.pictureBoxOriginalImage.TabStop = false;
            this.pictureBoxOriginalImage.Click += new System.EventHandler(this.PictureBoxOriginalImage_Click);
            // 
            // pictureBoxOriginalWatermark
            // 
            this.pictureBoxOriginalWatermark.Location = new System.Drawing.Point(274, 92);
            this.pictureBoxOriginalWatermark.Name = "pictureBoxOriginalWatermark";
            this.pictureBoxOriginalWatermark.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxOriginalWatermark.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxOriginalWatermark.TabIndex = 3;
            this.pictureBoxOriginalWatermark.TabStop = false;
            // 
            // buttonEncrypt
            // 
            this.buttonEncrypt.Location = new System.Drawing.Point(277, 269);
            this.buttonEncrypt.Name = "buttonEncrypt";
            this.buttonEncrypt.Size = new System.Drawing.Size(99, 32);
            this.buttonEncrypt.TabIndex = 4;
            this.buttonEncrypt.Text = "Encrypt";
            this.buttonEncrypt.UseVisualStyleBackColor = true;
            this.buttonEncrypt.Visible = false;
            this.buttonEncrypt.Click += new System.EventHandler(this.button3_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // radioButtonGrayscale
            // 
            this.radioButtonGrayscale.AutoSize = true;
            this.radioButtonGrayscale.Location = new System.Drawing.Point(12, 18);
            this.radioButtonGrayscale.Name = "radioButtonGrayscale";
            this.radioButtonGrayscale.Size = new System.Drawing.Size(72, 17);
            this.radioButtonGrayscale.TabIndex = 5;
            this.radioButtonGrayscale.TabStop = true;
            this.radioButtonGrayscale.Text = "Grayscale";
            this.radioButtonGrayscale.UseVisualStyleBackColor = true;
            // 
            // radioButtonColourful
            // 
            this.radioButtonColourful.AutoSize = true;
            this.radioButtonColourful.Location = new System.Drawing.Point(12, 42);
            this.radioButtonColourful.Name = "radioButtonColourful";
            this.radioButtonColourful.Size = new System.Drawing.Size(66, 17);
            this.radioButtonColourful.TabIndex = 6;
            this.radioButtonColourful.TabStop = true;
            this.radioButtonColourful.Text = "Colourful";
            this.radioButtonColourful.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(160, 364);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "PSNR -";
            this.label1.Visible = false;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // pictureBoxImageResultWithWatermark
            // 
            this.pictureBoxImageResultWithWatermark.Location = new System.Drawing.Point(12, 393);
            this.pictureBoxImageResultWithWatermark.Name = "pictureBoxImageResultWithWatermark";
            this.pictureBoxImageResultWithWatermark.Size = new System.Drawing.Size(256, 256);
            this.pictureBoxImageResultWithWatermark.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxImageResultWithWatermark.TabIndex = 8;
            this.pictureBoxImageResultWithWatermark.TabStop = false;
            this.pictureBoxImageResultWithWatermark.Visible = false;
            this.pictureBoxImageResultWithWatermark.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // buttonShowEncryptResult
            // 
            this.buttonShowEncryptResult.Location = new System.Drawing.Point(12, 354);
            this.buttonShowEncryptResult.Name = "buttonShowEncryptResult";
            this.buttonShowEncryptResult.Size = new System.Drawing.Size(139, 33);
            this.buttonShowEncryptResult.TabIndex = 9;
            this.buttonShowEncryptResult.Text = "Encryption result";
            this.buttonShowEncryptResult.UseVisualStyleBackColor = true;
            this.buttonShowEncryptResult.Visible = false;
            this.buttonShowEncryptResult.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(209, 361);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(87, 20);
            this.textBox1.TabIndex = 10;
            this.textBox1.Visible = false;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // buttonImageWithWatermarkDownload
            // 
            this.buttonImageWithWatermarkDownload.Location = new System.Drawing.Point(444, 43);
            this.buttonImageWithWatermarkDownload.Name = "buttonImageWithWatermarkDownload";
            this.buttonImageWithWatermarkDownload.Size = new System.Drawing.Size(132, 41);
            this.buttonImageWithWatermarkDownload.TabIndex = 11;
            this.buttonImageWithWatermarkDownload.Text = "Download Image With Watermark";
            this.buttonImageWithWatermarkDownload.UseVisualStyleBackColor = true;
            this.buttonImageWithWatermarkDownload.Click += new System.EventHandler(this.button5_Click);
            // 
            // buttonReadKey1
            // 
            this.buttonReadKey1.Location = new System.Drawing.Point(715, 92);
            this.buttonReadKey1.Name = "buttonReadKey1";
            this.buttonReadKey1.Size = new System.Drawing.Size(86, 41);
            this.buttonReadKey1.TabIndex = 12;
            this.buttonReadKey1.Text = "Read first key";
            this.buttonReadKey1.UseVisualStyleBackColor = true;
            this.buttonReadKey1.Visible = false;
            this.buttonReadKey1.Click += new System.EventHandler(this.button6_Click);
            // 
            // buttonReadKey2
            // 
            this.buttonReadKey2.Location = new System.Drawing.Point(715, 189);
            this.buttonReadKey2.Name = "buttonReadKey2";
            this.buttonReadKey2.Size = new System.Drawing.Size(86, 41);
            this.buttonReadKey2.TabIndex = 13;
            this.buttonReadKey2.Text = "Read second key";
            this.buttonReadKey2.UseVisualStyleBackColor = true;
            this.buttonReadKey2.Visible = false;
            this.buttonReadKey2.Click += new System.EventHandler(this.button7_Click);
            // 
            // pictureBoxImageWithWatermark
            // 
            this.pictureBoxImageWithWatermark.Location = new System.Drawing.Point(444, 92);
            this.pictureBoxImageWithWatermark.Name = "pictureBoxImageWithWatermark";
            this.pictureBoxImageWithWatermark.Size = new System.Drawing.Size(256, 256);
            this.pictureBoxImageWithWatermark.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxImageWithWatermark.TabIndex = 14;
            this.pictureBoxImageWithWatermark.TabStop = false;
            this.pictureBoxImageWithWatermark.Click += new System.EventHandler(this.PictureBoxImageWithWatermark_Click);
            // 
            // buttonDecrypt
            // 
            this.buttonDecrypt.Location = new System.Drawing.Point(715, 143);
            this.buttonDecrypt.Name = "buttonDecrypt";
            this.buttonDecrypt.Size = new System.Drawing.Size(86, 40);
            this.buttonDecrypt.TabIndex = 15;
            this.buttonDecrypt.Text = "Decrypt";
            this.buttonDecrypt.UseVisualStyleBackColor = true;
            this.buttonDecrypt.Visible = false;
            this.buttonDecrypt.Click += new System.EventHandler(this.button8_Click);
            // 
            // buttonShowDecryptResult
            // 
            this.buttonShowDecryptResult.Location = new System.Drawing.Point(444, 354);
            this.buttonShowDecryptResult.Name = "buttonShowDecryptResult";
            this.buttonShowDecryptResult.Size = new System.Drawing.Size(152, 33);
            this.buttonShowDecryptResult.TabIndex = 16;
            this.buttonShowDecryptResult.Text = "Decryption result";
            this.buttonShowDecryptResult.UseVisualStyleBackColor = true;
            this.buttonShowDecryptResult.Visible = false;
            this.buttonShowDecryptResult.Click += new System.EventHandler(this.button9_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(613, 364);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "PSNR -";
            this.label2.Visible = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(662, 361);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(87, 20);
            this.textBox2.TabIndex = 18;
            this.textBox2.Visible = false;
            // 
            // pictureBoxReconstructedBaboon
            // 
            this.pictureBoxReconstructedBaboon.Location = new System.Drawing.Point(444, 393);
            this.pictureBoxReconstructedBaboon.Name = "pictureBoxReconstructedBaboon";
            this.pictureBoxReconstructedBaboon.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxReconstructedBaboon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxReconstructedBaboon.TabIndex = 19;
            this.pictureBoxReconstructedBaboon.TabStop = false;
            this.pictureBoxReconstructedBaboon.Visible = false;
            // 
            // buttonWriteKey1
            // 
            this.buttonWriteKey1.Location = new System.Drawing.Point(274, 307);
            this.buttonWriteKey1.Name = "buttonWriteKey1";
            this.buttonWriteKey1.Size = new System.Drawing.Size(73, 41);
            this.buttonWriteKey1.TabIndex = 20;
            this.buttonWriteKey1.Text = "Write first key";
            this.buttonWriteKey1.UseVisualStyleBackColor = true;
            this.buttonWriteKey1.Visible = false;
            this.buttonWriteKey1.Click += new System.EventHandler(this.button10_Click);
            // 
            // buttonWriteKey2
            // 
            this.buttonWriteKey2.Location = new System.Drawing.Point(353, 307);
            this.buttonWriteKey2.Name = "buttonWriteKey2";
            this.buttonWriteKey2.Size = new System.Drawing.Size(71, 41);
            this.buttonWriteKey2.TabIndex = 21;
            this.buttonWriteKey2.Text = "Write second key";
            this.buttonWriteKey2.UseVisualStyleBackColor = true;
            this.buttonWriteKey2.Visible = false;
            this.buttonWriteKey2.Click += new System.EventHandler(this.button11_Click);
            // 
            // labelEncryption
            // 
            this.labelEncryption.AutoSize = true;
            this.labelEncryption.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.labelEncryption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelEncryption.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelEncryption.Location = new System.Drawing.Point(97, 9);
            this.labelEncryption.Name = "labelEncryption";
            this.labelEncryption.Size = new System.Drawing.Size(112, 26);
            this.labelEncryption.TabIndex = 22;
            this.labelEncryption.Text = "Encryption";
            this.labelEncryption.Click += new System.EventHandler(this.labelEncryption_Click);
            // 
            // labelDecryption
            // 
            this.labelDecryption.AutoSize = true;
            this.labelDecryption.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.labelDecryption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDecryption.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelDecryption.Location = new System.Drawing.Point(444, 9);
            this.labelDecryption.Name = "labelDecryption";
            this.labelDecryption.Size = new System.Drawing.Size(112, 26);
            this.labelDecryption.TabIndex = 23;
            this.labelDecryption.Text = "Decryption";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(590, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 43);
            this.button1.TabIndex = 24;
            this.button1.Text = "Read Original Watermark";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(837, 159);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(162, 44);
            this.button2.TabIndex = 25;
            this.button2.Text = "Show  Histogram(s)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.Button2_Click_1);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1013, 46);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(119, 43);
            this.button3.TabIndex = 26;
            this.button3.Text = "Read Original Watermark";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3_Click_1);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1013, 95);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(119, 43);
            this.button4.TabIndex = 27;
            this.button4.Text = "Read Reconstructed Watermark";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Button4_Click_1);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1013, 150);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(119, 41);
            this.button5.TabIndex = 28;
            this.button5.Text = "Count Watermark Difference";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.Button5_Click_1);
            // 
            // pictureBoxWatermarkDifference
            // 
            this.pictureBoxWatermarkDifference.Location = new System.Drawing.Point(839, 226);
            this.pictureBoxWatermarkDifference.Name = "pictureBoxWatermarkDifference";
            this.pictureBoxWatermarkDifference.Size = new System.Drawing.Size(174, 175);
            this.pictureBoxWatermarkDifference.TabIndex = 29;
            this.pictureBoxWatermarkDifference.TabStop = false;
            this.pictureBoxWatermarkDifference.Visible = false;
            this.pictureBoxWatermarkDifference.Click += new System.EventHandler(this.PictureBoxWatermarkDifference_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(839, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 26);
            this.label3.TabIndex = 30;
            this.label3.Text = "Analytics";
            this.label3.Click += new System.EventHandler(this.Label3_Click_1);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(1138, 103);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(39, 20);
            this.textBox3.TabIndex = 31;
            this.textBox3.Text = "255";
            this.textBox3.TextChanged += new System.EventHandler(this.TextBox3_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1138, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Enter a threshold:";
            this.label4.Click += new System.EventHandler(this.Label4_Click);
            // 
            // pictureBoxWatermarkDifferenceFiltered
            // 
            this.pictureBoxWatermarkDifferenceFiltered.Location = new System.Drawing.Point(839, 407);
            this.pictureBoxWatermarkDifferenceFiltered.Name = "pictureBoxWatermarkDifferenceFiltered";
            this.pictureBoxWatermarkDifferenceFiltered.Size = new System.Drawing.Size(174, 175);
            this.pictureBoxWatermarkDifferenceFiltered.TabIndex = 33;
            this.pictureBoxWatermarkDifferenceFiltered.TabStop = false;
            this.pictureBoxWatermarkDifferenceFiltered.Visible = false;
            this.pictureBoxWatermarkDifferenceFiltered.Click += new System.EventHandler(this.PictureBoxWatermarkDifferenceFiltered_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(274, 226);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Enter component number:";
            this.label5.Visible = false;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(277, 243);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(48, 20);
            this.textBox4.TabIndex = 35;
            this.textBox4.Text = "16";
            this.textBox4.Visible = false;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(1231, 150);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(114, 41);
            this.button6.TabIndex = 36;
            this.button6.Text = "Count Container Difference";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Visible = false;
            this.button6.Click += new System.EventHandler(this.Button6_Click_1);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(837, 81);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(71, 20);
            this.textBox5.TabIndex = 37;
            this.textBox5.Visible = false;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(837, 107);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(71, 20);
            this.textBox6.TabIndex = 38;
            this.textBox6.Visible = false;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(837, 133);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(71, 20);
            this.textBox7.TabIndex = 39;
            this.textBox7.Visible = false;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(931, 81);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(71, 20);
            this.textBox8.TabIndex = 40;
            this.textBox8.Visible = false;
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(931, 107);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(71, 20);
            this.textBox9.TabIndex = 41;
            this.textBox9.Visible = false;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(931, 133);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(71, 20);
            this.textBox10.TabIndex = 42;
            this.textBox10.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(837, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "Mean values:";
            this.label6.Visible = false;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(1231, 46);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(114, 43);
            this.button7.TabIndex = 44;
            this.button7.Text = "Read Original Container";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.Button7_Click_1);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(1231, 95);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(114, 43);
            this.button8.TabIndex = 45;
            this.button8.Text = "Read Container with Watermark";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.Button8_Click_1);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(1089, 226);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(256, 256);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 46;
            this.pictureBox1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(590, 535);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(200, 100);
            this.tabControl1.TabIndex = 47;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(192, 74);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 74);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(1185, 81);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(40, 20);
            this.textBox11.TabIndex = 48;
            this.textBox11.Text = "255";
            // 
            // textBox12
            // 
            this.textBox12.Location = new System.Drawing.Point(1185, 103);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(40, 20);
            this.textBox12.TabIndex = 49;
            this.textBox12.Text = "255";
            // 
            // textBox13
            // 
            this.textBox13.Location = new System.Drawing.Point(1185, 129);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(40, 20);
            this.textBox13.TabIndex = 50;
            this.textBox13.Text = "255";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1357, 682);
            this.Controls.Add(this.textBox13);
            this.Controls.Add(this.textBox12);
            this.Controls.Add(this.textBox11);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox10);
            this.Controls.Add(this.textBox9);
            this.Controls.Add(this.textBox8);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pictureBoxWatermarkDifferenceFiltered);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBoxWatermarkDifference);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelDecryption);
            this.Controls.Add(this.labelEncryption);
            this.Controls.Add(this.buttonWriteKey2);
            this.Controls.Add(this.buttonWriteKey1);
            this.Controls.Add(this.pictureBoxReconstructedBaboon);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonShowDecryptResult);
            this.Controls.Add(this.buttonDecrypt);
            this.Controls.Add(this.pictureBoxImageWithWatermark);
            this.Controls.Add(this.buttonReadKey2);
            this.Controls.Add(this.buttonReadKey1);
            this.Controls.Add(this.buttonImageWithWatermarkDownload);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonShowEncryptResult);
            this.Controls.Add(this.pictureBoxImageResultWithWatermark);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButtonColourful);
            this.Controls.Add(this.radioButtonGrayscale);
            this.Controls.Add(this.buttonEncrypt);
            this.Controls.Add(this.pictureBoxOriginalWatermark);
            this.Controls.Add(this.pictureBoxOriginalImage);
            this.Controls.Add(this.buttonOriginalWatermarkDownload);
            this.Controls.Add(this.buttonOriginalImageDownload);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Koliada Image Watermarking";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginalImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginalWatermark)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImageResultWithWatermark)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImageWithWatermark)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReconstructedBaboon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWatermarkDifference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWatermarkDifferenceFiltered)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOriginalImageDownload;
        private System.Windows.Forms.Button buttonOriginalWatermarkDownload;
        private System.Windows.Forms.PictureBox pictureBoxOriginalImage;
        private System.Windows.Forms.PictureBox pictureBoxOriginalWatermark;
        private System.Windows.Forms.Button buttonEncrypt;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RadioButton radioButtonGrayscale;
        private System.Windows.Forms.RadioButton radioButtonColourful;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxImageResultWithWatermark;
        private System.Windows.Forms.Button buttonShowEncryptResult;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonImageWithWatermarkDownload;
        private System.Windows.Forms.Button buttonReadKey1;
        private System.Windows.Forms.Button buttonReadKey2;
        private System.Windows.Forms.PictureBox pictureBoxImageWithWatermark;
        private System.Windows.Forms.Button buttonDecrypt;
        private System.Windows.Forms.Button buttonShowDecryptResult;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.PictureBox pictureBoxReconstructedBaboon;
        private System.Windows.Forms.Button buttonWriteKey1;
        private System.Windows.Forms.Button buttonWriteKey2;
        private System.Windows.Forms.Label labelEncryption;
        private System.Windows.Forms.Label labelDecryption;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.PictureBox pictureBoxWatermarkDifference;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBoxWatermarkDifferenceFiltered;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.TextBox textBox12;
        private System.Windows.Forms.TextBox textBox13;
    }
}

