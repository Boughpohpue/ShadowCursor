using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ShadowCursor
{
    /// <summary>
    /// Interaction logic for TransparentWindow.xaml
    /// </summary>
    public partial class TransparentWindow : Window
    {
        private readonly BitmapImage _imageSource = new BitmapImage(
            new Uri(@"pack://application:,,,/ShadowCursor;component/Images/cursor.png"));
        private readonly DoubleAnimation _opacityAnimation = new DoubleAnimation
        {
            To = 0.0,
            Duration = new Duration(TimeSpan.FromMilliseconds(693))
        };

        public TransparentWindow()
        {
            InitializeComponent();
            Top = 0;
            Left = 0;
            Height = 1080;
            Width = 1920;
        }

        public void AddNewCursor(Point p)
        {
            var img = new Image
            {
                Width = 20,
                Height = 20,
                Opacity = 1.0,
                Source = _imageSource
            };
            MyCanvas.Children.Add(img);
            Canvas.SetTop(img, p.Y);
            Canvas.SetLeft(img, p.X);
            var storyBoard = new Storyboard();
            storyBoard.Children.Add(_opacityAnimation);
            Storyboard.SetTarget(_opacityAnimation, img);
            Storyboard.SetTargetProperty(_opacityAnimation, 
                new PropertyPath(OpacityProperty));
            storyBoard.Begin();
        }
    }
}
