using System;
using System.Windows;

namespace ShadowCursor
{
    /// <summary>
    /// Whole mouse hook logic can be moved to TransparentWindow logic.
    /// This window is here just to exit the app easily when not in Debug mode.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TransparentWindow _transparentWindow;
        private const double _stepSize = 6.9;
        private Point _prevPoint;

        public MainWindow(TransparentWindow transparentWindow)
        {
            InitializeComponent();
            _transparentWindow = transparentWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _transparentWindow.Show();
            _prevPoint = MouseInterop.GetCursorPosition();
            MouseInterop.MouseHookEvent += OnMouseHookEvent;
            MouseInterop.StartMouseHook();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            _transparentWindow.Close();
            MouseInterop.MouseHookEvent -= OnMouseHookEvent;
            MouseInterop.StopMouseHook();
            Application.Current.Shutdown();
        }        
        private void OnMouseHookEvent(object? sender, MouseInteropEventArgs e)
        {
            if (e.MouseMessage != MouseMessages.WM_MOUSEMOVE)
                return;
            var segment = new LineSegment(_prevPoint, e.Point);
            if (segment.Length < _stepSize)
                return;
            segment.GetSegmentPointsAtDistance(_stepSize)
                .ForEach(p => _transparentWindow.AddNewCursor(p));
            _prevPoint = e.Point;
        }
    }
}
