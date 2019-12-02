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
        public MainWindow()
        {
            InitializeComponent();
            //for (int i = 0; i < 4; i++)
            //{
            //    var line1 = new Line();
            //    line1.StrokeThickness = 1;
            //    line1.Stroke = new SolidColorBrush(Colors.Black);
            //    canvas.Children.Add(line1);

            //    line1.X1 = startX + i * width;
            //    line1.Y1 = startY;

            //    line1.X2 = startX + i * width;
            //    line1.Y2 = startY + 3 * height;
            //}

            //for (int i = 0; i < 4; i++)
            //{
            //    var line2 = new Line();
            //    line2.StrokeThickness = 1;
            //    line2.Stroke = new SolidColorBrush(Colors.Black);
            //    canvas.Children.Add(line2);

            //    line2.X1 = startX;
            //    line2.Y1 = startY + i * height;

            //    line2.X2 = startX + 3 * width;
            //    line2.Y2 = startY + i * height;
            //}
        }

        private void previewImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        Tuple<int, int> _blank =new Tuple<int, int>(2,2);
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
                            cropImage.Tag = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }

        }
        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _lastPosition;
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
