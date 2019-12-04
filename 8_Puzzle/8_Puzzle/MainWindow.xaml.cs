using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Tuple<int, int> _blank = new Tuple<int, int>(2, 2);
        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _lastPosition;
        Image[,] _cropImage = new Image[3,3];
        public MainWindow()
        {
            InitializeComponent();
            //Dang ky su kien hook
            
          
        }
       
        private void previewImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
      
        private void BtnPickPicture_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            if (screen.ShowDialog()==true)
            {
                var source = new BitmapImage(
                   new Uri(screen.FileName, UriKind.Absolute));
                Debug.WriteLine($"{source.Width} - {source.Height}");
                previewImage.Width =3*width;
                previewImage.Height = 3*height;
                previewImage.Source = source;

                Canvas.SetLeft(previewImage, 400);
                Canvas.SetTop(previewImage, 30);
                // Bat dau cat thanh 9 manh
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (!((i == 2) && (j == 2)))
                        {
                            var h = (int)source.Height;
                            var w = (int)source.Width;
                           
                            var rect = new Int32Rect(i * (w / 3), j * (h / 3), w / 3, h / 3);
                            var cropBitmap = new CroppedBitmap(source,
                                rect);

                            var cropImage = new Image();
                            cropImage.Stretch = Stretch.Fill;
                            cropImage.Width = width;
                            cropImage.Height = height;
                            cropImage.Source = cropBitmap;
                            canvas.Children.Add(cropImage);
                            Canvas.SetLeft(cropImage, startX + j * (width + 2));
                            Canvas.SetTop(cropImage, startY + i * (height + 2));


                            cropImage.MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                            cropImage.PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                            _cropImage[i,j]=cropImage;
                            //cropImage.KeyUp += CropImage_KeyUp;
                            cropImage.Tag = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }

        }
        //Bắt phim bấm arrow key
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var (iblank, jblank) = _blank as Tuple<int, int>;

            Image image;
            int iImage, jImage;
            switch (e.Key)
            {
                case Key.Left:
                    if (jblank == 2 ) break;
                    image = _cropImage[iblank , jblank + 1];                   
                    (iImage, jImage) = image.Tag as Tuple<int, int>;
                
                    if (jImage - 1 == jblank)
                    {
                        Canvas.SetTop(image, (int)(iblank * (height + 2) + startY));
                        Canvas.SetLeft(image, (int)(jblank * (width + 2) + startX));
                        _blank = new Tuple<int, int>(iImage, jImage);
                        _cropImage[iblank, jblank] = image;
                        _cropImage[iblank, jblank].Tag = new Tuple<int, int>(iblank, jblank);
                        _cropImage[iImage, jImage] = null;
                    }
                    break;
                case Key.Right:
                    if (jblank == 0) break;
                    image = _cropImage[iblank, jblank -1];
                    (iImage, jImage) = image.Tag as Tuple<int, int>;

                    if (jImage + 1 == jblank)
                    {
                        Canvas.SetTop(image, (int)(iblank * (height + 2) + startY));
                        Canvas.SetLeft(image, (int)(jblank * (width + 2) + startX));
                        _blank = new Tuple<int, int>(iImage, jImage);
                        _cropImage[iblank, jblank] = image;
                        _cropImage[iblank, jblank].Tag = new Tuple<int, int>(iblank, jblank);
                        _cropImage[iImage, jImage] = null;
                    }
                    break;
                
                case Key.Up:
                    if (iblank ==2) break;
                    image = _cropImage[iblank+1, jblank];
                    (iImage, jImage) = image.Tag as Tuple<int, int>;

                    if (iImage - 1 == iblank)
                    {
                        Canvas.SetTop(image, (int)(iblank * (height + 2) + startY));
                        Canvas.SetLeft(image, (int)(jblank * (width + 2) + startX));
                        _blank = new Tuple<int, int>(iImage, jImage);
                        _cropImage[iblank, jblank] = image;
                        _cropImage[iblank, jblank].Tag = new Tuple<int, int>(iblank,jblank);
                        _cropImage[iImage, jImage] = null;

                    }
                    break;
                case Key.Down:
                    if (iblank == 0 ) break;
                    image = _cropImage[iblank - 1, jblank];
                    (iImage, jImage) = image.Tag as Tuple<int, int>;

                    if (iImage + 1 == iblank)
                    {
                        Canvas.SetTop(image, (int)(iblank * (height + 2) + startY));
                        Canvas.SetLeft(image, (int)(jblank * (width + 2) + startX));
                        _blank = new Tuple<int, int>(iImage, jImage);
                        _cropImage[iblank, jblank] = image;
                        _cropImage[iblank, jblank].Tag = new Tuple<int, int>(iblank, jblank);
                        _cropImage[iImage, jImage] = null;
                    }
                    break;
                default:
                    break;

            }
        }
        //bắt kéo thả

      
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
            if (i == iblank && j == jblank && i<3 && j<3 &&((iold==i+1 && jold==j) || (iold==i-1 && jold==j) || (iold==i && jold==j+1) || (iold==i && jold==j-1)))
            {

                Canvas.SetLeft(_selectedBitmap, x);
                Canvas.SetTop(_selectedBitmap, y);
                _blank = _selectedBitmap.Tag as Tuple<int, int>;
                _selectedBitmap.Tag = new Tuple<int, int>(i, j);
                _cropImage[iblank, jblank] = _selectedBitmap;
                _cropImage[iold, jold] = null;
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

            this.Title = $"{position.X} - {position.Y}, a[{i}][{j}]";

            if (_isDragging)
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

      
    }
}
