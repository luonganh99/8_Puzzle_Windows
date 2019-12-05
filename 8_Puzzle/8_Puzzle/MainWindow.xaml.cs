using Gma.System.MouseKeyHook;
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
        const int MAX = 9;
        const int startX = 50;
        const int startY = 160;
        const int width = 75;
        const int height = 100;
 
        // Mảng lưu trữ trạng thái của game, mỗi hình ảnh khi mới khởi tạo được đánh số từ 0 -> 8
        // Vd: 
        // 0 1 2
        // 3 4 5
        // 6 7 8
        int[] _chess = new int[] {0,1,2,3,4,5,6,7,8};

        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _lastPosition;

        Tuple<int, int> _blank = new Tuple<int, int>(0, 0); //Vị trí ô trống 
        Image[,] _cropImage = new Image[3, 3]; //Mảng 2 chiều các mảng hình ảnh

        DispatcherTimer _timer = new DispatcherTimer();

        int _decrement = 300;

        int _min;
        int _sec;

        String _fileName = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        //Chọn hình ảnh và tạo Game
        private void btnSelectPicture_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            if (screen.ShowDialog()==true)
            {
                //Tạo random _chess
                do
                {
                    _chess = randomChess(_chess);
                } while (!isSolvable(_chess)); //random _chess có thể giải được
                _fileName = screen.FileName;
                createGame(_fileName);
            }
        }
        
        //Di chuyển bằng phím mũi tên
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var (iblank, jblank) = _blank as Tuple<int, int>;
            int k = 3 * iblank + jblank;
            int kold;
            Image image;
            int iImage, jImage;
            switch (e.Key)
            {
                case Key.Left:
                    if (jblank == 2 ) break;

                    image = _cropImage[iblank , jblank + 1];                   
                    (iImage, jImage) = image.Tag as Tuple<int, int>;
                    kold = 3 * iImage + jImage;
                    if (jImage - 1 == jblank)
                    {
                        //Cập nhật giao diện
                        Canvas.SetTop(image, (int)(iblank * (height + 2) + startY));
                        Canvas.SetLeft(image, (int)(jblank * (width + 2) + startX));

                        //Cập nhật model
                        _blank = new Tuple<int, int>(iImage, jImage);
                        _cropImage[iblank, jblank] = image;
                        _cropImage[iblank, jblank].Tag = new Tuple<int, int>(iblank, jblank);
                        _cropImage[iImage, jImage] = null;
                        _chess[k] = _chess[kold];
                        _chess[kold] = 8;


                    }
                    break;
                case Key.Right:
                    if (jblank == 0) break;
                    image = _cropImage[iblank, jblank -1];
                    (iImage, jImage) = image.Tag as Tuple<int, int>;
                    kold = 3 * iImage + jImage;

                    if (jImage + 1 == jblank)
                    {
                        Canvas.SetTop(image, (int)(iblank * (height + 2) + startY));
                        Canvas.SetLeft(image, (int)(jblank * (width + 2) + startX));
                        _blank = new Tuple<int, int>(iImage, jImage);
                        _cropImage[iblank, jblank] = image;
                        _cropImage[iblank, jblank].Tag = new Tuple<int, int>(iblank, jblank);
                        _cropImage[iImage, jImage] = null;
                        _chess[k] = _chess[kold];
                        _chess[kold] = 8;
                    }
                    break;
                
                case Key.Up:
                    if (iblank ==2) break;
                    image = _cropImage[iblank+1, jblank];
                    (iImage, jImage) = image.Tag as Tuple<int, int>;
                    kold = 3 * iImage + jImage;

                    if (iImage - 1 == iblank)
                    {
                        Canvas.SetTop(image, (int)(iblank * (height + 2) + startY));
                        Canvas.SetLeft(image, (int)(jblank * (width + 2) + startX));
                        _blank = new Tuple<int, int>(iImage, jImage);
                        _cropImage[iblank, jblank] = image;
                        _cropImage[iblank, jblank].Tag = new Tuple<int, int>(iblank,jblank);
                        _cropImage[iImage, jImage] = null;
                        _chess[k] = _chess[kold];
                        _chess[kold] = 8;
                    }
                    break;
                case Key.Down:
                    if (iblank == 0 ) break;
                    image = _cropImage[iblank - 1, jblank];
                    (iImage, jImage) = image.Tag as Tuple<int, int>;
                    kold = 3 * iImage + jImage;

                    if (iImage + 1 == iblank)
                    {
                        Canvas.SetTop(image, (int)(iblank * (height + 2) + startY));
                        Canvas.SetLeft(image, (int)(jblank * (width + 2) + startX));
                        _blank = new Tuple<int, int>(iImage, jImage);
                        _cropImage[iblank, jblank] = image;
                        _cropImage[iblank, jblank].Tag = new Tuple<int, int>(iblank, jblank);
                        _cropImage[iImage, jImage] = null;
                        _chess[k] = _chess[kold];
                        _chess[kold] = 8;
                    }
                    break;
                default:
                    break;
            }

            if (checkWin() && _timer.IsEnabled)
            {
                MessageBox.Show("You won!");
                _timer.Stop();
                this.Close();
            }
        }

        //Di chuyển bằng kéo thả chuột
        private void CropImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var position = e.GetPosition(this);

            int x = (int)(position.X - startX) / (width + 2) * (width + 2) + startX;
            int y = (int)(position.Y - startY) / (height + 2) * (height + 2) + startY;

            var (iblank, jblank) = _blank;
            var image = sender as Image;
            int j = (int)(position.X - startX) / (width+2);
            int i = (int)(position.Y - startY) / (height+2);
            var (iold, jold) = _selectedBitmap.Tag as Tuple<int, int>;
            int k = 3 * i + j;
            int kold = 3 * iold + jold;
            if (i == iblank && j == jblank && i<3 && j<3 &&((iold==i+1 && jold==j) || (iold==i-1 && jold==j) || (iold==i && jold==j+1) || (iold==i && jold==j-1)))
            {
                _chess[k] = _chess[kold];
                _chess[kold] = 8;
                Canvas.SetLeft(_selectedBitmap, x);
                Canvas.SetTop(_selectedBitmap, y);
                _blank = _selectedBitmap.Tag as Tuple<int, int>;
                _selectedBitmap.Tag = new Tuple<int, int>(i, j);
                _cropImage[iblank, jblank] = _selectedBitmap;
                _cropImage[iold, jold] = null;
                if (checkWin() && _timer.IsEnabled)
                {
                    MessageBox.Show("You won!");
                    _timer.Stop();
                    this.Close();
                }

            }
            else
            {
                Canvas.SetTop(_selectedBitmap, (int)(iold * (height + 2) + startY));
                Canvas.SetLeft(_selectedBitmap, (int)(jold * (width + 2) + startX));
            }
        }
        private void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _selectedBitmap = sender as Image;
            _lastPosition = e.GetPosition(this);
            
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);

            int i = ((int)position.Y - startY) / height;
            int j = ((int)position.X - startX) / width;
            //this.Title = $"{position.X} - {position.Y}, a[{i}][{j}]";
            if (_isDragging == true)
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

        //Lưu và tải Game
        private void btnSaveGame_Click(object sender, RoutedEventArgs e)
        {
            const string fname = "save.txt";
            var writer = new StreamWriter(fname);
            //Lưu địa chỉ hình ảnh
            writer.WriteLine(_fileName);

            //Lưu thời gian đang chơi
            writer.WriteLine(_decrement.ToString());

            //Lưu thông tin chess
            for(int i= 0; i < MAX; i++)
            {
                writer.Write($"{_chess[i]} ");
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
                _fileName = firstline;
                _decrement = int.Parse(secondline);
                var tokens = reader.ReadLine().Split(new string[] { " " }, StringSplitOptions.None);
                for (int i = 0; i < MAX; i++)
                {
                    _chess[i] = int.Parse(tokens[i]);
                }
                createGame(_fileName);
                reader.Close();
            }
        }

        //Các hàm hỗ trợ
        private void createGame(string filename)
        {
            var source = new BitmapImage(new Uri(filename, UriKind.Absolute));

            previewImage.Width = 4 * width;
            previewImage.Height = 4 * height;
            previewImage.Source = source;

            Canvas.SetLeft(previewImage, 430);
            Canvas.SetTop(previewImage, 90);

            // Bắt đầu cắt thành 9 mảnh
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!((i == 2) && (j == 2)))
                    {
                        var h = (int)source.Height;
                        var w = (int)source.Width;

                        var rect = new Int32Rect(j * (w / 3), i * (h / 3), w / 3, h / 3); 
                        var cropBitmap = new CroppedBitmap(source, rect);

                        var cropImage = new Image();
                        cropImage.Stretch = Stretch.Fill;
                        cropImage.Width = width;
                        cropImage.Height = height;
                        cropImage.Source = cropBitmap;
                        canvas.Children.Add(cropImage);

                        //Kiểm tra vị trí i,j mảnh hình ảnh trong _chess (đã random) và set giao diện tương ứng 
                        //Đổi i,j -> vị trí k trong mảng một chiều _chess
                        int k = 3 * i + j;

                        //Kiểm tra k ở đâu trong _chess đã random
                        int m = 0, n = 0;
                        for (int index = 0; index < MAX; index++)
                        {
                            if (k == _chess[index])
                            {
                                //Lấy vị trí có k trong _chess (là index) đổi qua vị trí mảng 2 chiều m,n 
                                m = index / 3;
                                n = index % 3;
                            }
                        }
                        //m,n là vị trí mảnh hình ảnh sẽ được set lên giao diện
                        cropImage.Tag = new Tuple<int, int>(m, n);
                        Canvas.SetLeft(cropImage, startX + n * (width + 2));
                        Canvas.SetTop(cropImage, startY + m * (height + 2));

                        cropImage.MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                        cropImage.PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;

                        _cropImage[m, n] = cropImage;
                    }
                }
            }

            // Cập nhật _blank từ _chess
            for (int index = 0; index < MAX; index++)
            {
                if (8 == _chess[index])
                {
                    int m = index / 3;
                    int n = index % 3;
                    _blank = new Tuple<int, int>(m, n);
                }
            }

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timerTicker;
            _timer.Start();
        }
        private void resetGame()
        {
            _fileName = "";
            canvas.Children.Remove(previewImage);
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    canvas.Children.Remove(_cropImage[i, j]);
                }
            }
            _timer.Stop();
        }
        private int[] randomChess(int[] _chess)
        {
            Random rnd = new Random();
            for (int i = 0; i < MAX; i++)
            {
                int j;
                do
                {
                    j = rnd.Next(MAX);
                } while (i == j);
                int temp = _chess[i];
                _chess[i] = _chess[j];
                _chess[j] = temp;
            }
            return _chess;
        }
        private bool isSolvable(int[] _chess)
        {
            int number_of_inv = 0;
            for (int i = 0; i < MAX; i++)
            {
                for (int j = i + 1; j < MAX; j++)
                {
                    if (_chess[i] > _chess[j] && _chess[i] != 8 && _chess[j] != 8)
                        number_of_inv++;
                }
            }
            return (number_of_inv % 2 == 0);
        }
        private void timerTicker(object sender, EventArgs e)
        {
            _decrement--;
            if (_decrement < 60)
            {
                _min = 0;
                _sec = _decrement;
            }
            if(_decrement >= 60)
            {
                _min = _decrement / 60;
                _sec = _decrement - _min * 60;
            }
            lbTimer.Content = $"{_min} : {_sec}";
            if (_decrement == 0)
            {
                MessageBox.Show("You lose!");
                _timer.Stop();
            }
        }
        private bool checkWin()
        {
            //Kiểm tra 
            bool check = true;
            for (int i = 0; i < MAX; i++)
            {
                if (_chess[i] != i)
                {
                    check = false;
                }
            }
            return check;
        }

    }
}
