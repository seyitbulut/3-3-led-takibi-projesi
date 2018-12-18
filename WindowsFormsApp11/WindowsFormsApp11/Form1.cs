using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using Point = System.Drawing.Point;
using System.Drawing.Imaging;
using System.IO.Ports;

namespace WindowsFormsApp11
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection VideoCapTureDevices;
        private VideoCaptureDevice Finalvideo;

        public Form1()
        {
            InitializeComponent();
        }

        int R; 
        int G;
        int B;

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.DataSource = SerialPort.GetPortNames();
            int numbers = comboBox2.Items.Count;
            if (numbers==0)
            {
                comboBox2.Enabled = false;
                button4.Enabled = false;
            }
            else
            {
                comboBox2.Enabled = true;
                button4.Enabled = true;
            }

            VideoCapTureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCapTureDevices)
            {

                comboBox1.Items.Add(VideoCaptureDevice.Name);

            }

            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Finalvideo = new VideoCaptureDevice(VideoCapTureDevices[comboBox1.SelectedIndex].MonikerString);
            Finalvideo.NewFrame += new NewFrameEventHandler(Finalvideo_NewFrame);
            Finalvideo.Start();
        }


        void Finalvideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;
            if (radioButton1.Checked)
            {

                
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                
                filter.CenterColor = new RGB(Color.FromArgb(R, G, B));
                filter.Radius = 100;
                filter.ApplyInPlace(image1);
                nesnebul(image1);

            }
           

        }
        public void nesnebul(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;


            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));
            image.UnlockBits(objectsData);


            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            pictureBox2.Image = image;

            foreach (Rectangle recs in rects)
            {
                if (rects.Length > 0)
                {
                    Rectangle objectRect = rects[0];
                    Graphics g = pictureBox1.CreateGraphics();
                    using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                    {
                        g.DrawRectangle(pen, objectRect);
                    }
                    int objectX = objectRect.X + (objectRect.Width / 2);
                    int objectY = objectRect.Y + (objectRect.Height / 2);
                    g.Dispose();


                    this.Invoke((MethodInvoker)delegate
                    {
                        richTextBox1.Text = objectRect.Location.ToString() + "\n" + richTextBox1.Text + "\n";
                    });
                    if (serialPort1.IsOpen)
                    { 
                        if ((objectX > 0 && objectX < 200) && objectY < 150)
                        {
                            serialPort1.Write("1");
                        }
                        else if ((objectX > 200 && objectX < 400) && objectY < 150)
                        {
                            serialPort1.Write("2");
                        }
                        else if ((objectX > 400 && objectX < 600) && objectY < 150)
                        {
                            serialPort1.Write("3");
                        }
                        else if ((objectX > 0 && objectX < 200) && (objectY > 150 && objectY < 300))
                        {
                            serialPort1.Write("4");
                        }
                        else if ((objectX > 200 && objectX < 400) && (objectY > 150 && objectY < 300))
                        {
                            serialPort1.Write("5");
                        }
                        else if ((objectX > 400 && objectX < 600) && (objectY > 150 && objectY < 300))
                        {
                            serialPort1.Write("6");
                        }
                        else if ((objectX > 0 && objectX < 200) && objectY > 300)
                        {
                            serialPort1.Write("7");
                        }
                        else if ((objectX > 200 && objectX < 400) && objectY > 300)
                        {
                            serialPort1.Write("8");
                        }
                        else if ((objectX > 400 && objectX < 600) && objectY > 300)
                        {
                            serialPort1.Write("9");
                        }
                    }
                    
                }
            }
        }

            private Point[] ToPointsArray(List<IntPoint> points)
        {
            Point[] array = new Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new Point(points[i].X, points[i].Y);
            }

            return array;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            R = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            G = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            B = trackBar3.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }

            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox2.SelectedItem.ToString();
            serialPort1.BaudRate = 9600;
            serialPort1.Open();
        }
    }

}
