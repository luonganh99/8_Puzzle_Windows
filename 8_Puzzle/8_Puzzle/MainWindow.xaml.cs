using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Threading;

namespace _8_Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int startX = 30;
        const int startY = 30;
        const int width = 75;
        const int height = 100;
        const int MAX = 9;
        int[] chess = new int[] {0,1,2,3,4,5,6,7,8};
        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _lastPosition;
        Point _lastUsePosition;
        String filename = "";
        DispatcherTimer dt = new DispatcherTimer();
        int decrement = 100;
        int m, s;
        Image[] cropImages = new Image[8];

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 8; i++)
            {
                cropImages[i] = new Image();
            }
        }
        private void previewImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        private void BtnPickPicture_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            if (screen.ShowDialog()==true)
            {
                //Tạo random chess
                do
                {
                    chess = randomChess(chess);
                } while (!isSolvable(chess));
                filename = screen.FileName;
                createGame(filename);
            }
        }
        private void createGame(string filename)
        {
            var source = new BitmapImage(
            new Uri(filename, UriKind.Absolute));

            previewImage.Width = 3 * width;
            previewImage.Height = 3 * height;
            previewImage.Source = source;

            Canvas.SetLeft(previewImage, 400);
            Canvas.SetTop(previewImage, 30);
            int count = 0;
            // Bat dau cat thanh 9 manh
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!((i == 2) && (j == 2)))
                    {
                        var h = (int)source.Height;
                        var w = (int)source.Width;

                        var rect = new Int32Rect(j * (w / 3), i * (h / 3), w / 3, h / 3); //x,y,width,height
                        var cropBitmap = new CroppedBitmap(source, rect);

                        cropImages[count] = new Image();

                        cropImages[count].Stretch = Stretch.Fill;
                        cropImages[count].Width = width;
                        cropImages[count].Height = height;
                        cropImages[count].Source = cropBitmap;
                        canvas.Children.Add(cropImages[count]);

                        cropImages[count].Tag = new Tuple<int, int>(i, j);
                        //Random
                        //Đổi i,j -> vị trí nào(k) trong mảng một chiều chess
                        int k = 3 * i + j;

                        //Kiểm tra k ở đâu trong chess đã random
                        int index;
                        for (index = 0; index < MAX; index++)
                        {
                            if (k == chess[index])
                            {
                                break;
                            }
                        }
                        //Lấy vị trí có k trong chess (là index) đổi qua 2 chiều m,n . Đây là vị trí ảo để set lên giao diện
                        int m = index / 3;
                        int n = index % 3;

                        Canvas.SetLeft(cropImages[count], startX + n * (width + 2));
                        Canvas.SetTop(cropImages[count], startY + m * (height + 2));

                        cropImages[count].MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                        cropImages[count].PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                        count++;
                    }
                }
            }

            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += dtTicker;
            dt.Start();
        }
        private int[] randomChess(int[] chess)
        {
            Random rnd = new Random();
            for(int i = 0; i< MAX; i++)
            {
                int j;
                do
                {
                    j = rnd.Next(MAX);
                } while (i == j);
                int temp = chess[i];
                chess[i] = chess[j];
                chess[j] = temp;
            }
            return chess;
        }
        private bool isSolvable(int[] chess)
        {
            int number_of_inv = 0;
            for (int i = 0; i < MAX; i++)
            {
                for (int j = i + 1; j < MAX; j++)
                {
                    if (chess[i] > chess[j]) 
                        number_of_inv++;
                }
            }
            return (number_of_inv % 2 == 0);
        }
        private void CropImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var position = e.GetPosition(this);

            var image = sender as Image;

            //Lấy vị trí hiện tại giao diện của image
            int i = ((int)position.Y - startY) / height;
            int j = ((int)position.X - startX) / width;

            //Lấy vị trí cũ 
            int i_last = ((int)_lastUsePosition.Y - startY) / height;
            int j_last = ((int)_lastUsePosition.X -startX) / width;

            //Chuyển vị trí i,j sang vị trí k trong chess 
            int k = 3 * i + j;
            int k_last = 3 * i_last + j_last;

            //Cập nhật chess 
            if( k>=0 && k <= 8)
            {
                if (chess[k] == 8 && (k == k_last + 1 || k == k_last - 1 || k == k_last + 3 || k == k_last - 3))
                {
                    chess[k] = chess[k_last];
                    chess[k_last] = 8;

                    int x = (int)(position.X - startX) / (width + 2) * (width + 2) + startX;
                    int y = (int)(position.Y - startY) / (height + 2) * (height + 2) + startY;
                    Canvas.SetLeft(_selectedBitmap, x);
                    Canvas.SetTop(_selectedBitmap, y);

                    //Kiểm tra 
                    bool check = true;
                    for (int m = 0; m < MAX; m++)
                    {
                        if (chess[m] != m)
                        {
                            check = false;
                        }
                    }

                    if (check)
                    {
                        MessageBox.Show($"Win");
                        dt.Stop();
                    }
                }
                else
                {
                    int x = (int)(_lastUsePosition.X - startX) / (width + 2) * (width + 2) + startX;
                    int y = (int)(_lastUsePosition.Y - startY) / (height + 2) * (height + 2) + startY;
                    Canvas.SetLeft(_selectedBitmap, x);
                    Canvas.SetTop(_selectedBitmap, y);
                }
            }
            else
            {
                int x = (int)(_lastUsePosition.X - startX) / (width + 2) * (width + 2) + startX;
                int y = (int)(_lastUsePosition.Y - startY) / (height + 2) * (height + 2) + startY;
                Canvas.SetLeft(_selectedBitmap, x);
                Canvas.SetTop(_selectedBitmap, y);
            }
        }
        private void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _selectedBitmap = sender as Image;
            _lastPosition = e.GetPosition(this);
            _lastUsePosition = e.GetPosition(this);
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);

            int i = ((int)position.Y - startY) / height;
            int j = ((int)position.X - startX) / width;
            this.Title = $"{position.X} - {position.Y}, a[{i}][{j}]";
            if (_isDragging == true && i >= 0 && i < 3 && j >=0 && j <3)
            {
                var dx = position.X - _lastPosition.X;
                var dy = position.Y - _lastPosition.Y;

                var lastLeft = Canvas.GetLeft(_selectedBitmap);
                var lastTop = Canvas.GetTop(_selectedBitmap);
                Canvas.SetLeft(_selectedBitmap, lastLeft + dx);
                Canvas.SetTop(_selectedBitmap, lastTop + dy);

                _lastPosition = position;
            }
        }
        private void btnSaveGame_Click(object sender, RoutedEventArgs e)
        {
            const string fname = "save.txt";
            var writer = new StreamWriter(fname);
            //Lưu địa chỉ hình ảnh
            writer.WriteLine(filename);

            //Lưu thời gian đang chơi
            writer.WriteLine(decrement.ToString());

            //Lưu thông tin chess
            for(int i= 0; i < MAX; i++)
            {
                writer.Write($"{chess[i]} ");
            }
            writer.Close();
            MessageBox.Show("Saved Game!");
        }
        private void btnLoadGame_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            screen.Filter = "Text|*.txt|All|*.*";
            if (screen.ShowDialog() == true)
            {
                resetGame();
                var fname = screen.FileName;
                var reader = new StreamReader(fname);
                var firstline = reader.ReadLine();
                var secondline = reader.ReadLine();
                filename = firstline;
                decrement = int.Parse(secondline);
                var tokens = reader.ReadLine().Split(new string[] { " " }, StringSplitOptions.None);
                for (int i = 0; i < MAX; i++)
                {
                    chess[i] = int.Parse(tokens[i]);
                }
                createGame(filename);
                reader.Close();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void dtTicker(object sender, EventArgs e)
        {
            decrement--;
            if (decrement < 60)
            {
                m = 0;
                s = decrement;
            }
            if(decrement >= 60)
            {
                m = decrement / 60;
                s = decrement - m * 60;
            }
            lbTimer.Content = $"{m} : {s}";
            if (decrement == 0)
            {
                MessageBox.Show("You lose !");
                dt.Stop();
            }
        }

        private void resetGame()
        {
            filename = "";
            previewImage.Source = null;
            for(int i = 0; i < 8; i++)
            {
                cropImages[i].Source = null;
            }
            dt.Stop();
        }
    }
}
