using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace magic_cube {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        Point startMoveCamera;
        bool allowMoveCamera = false, allowMoveLayer = false;
        Transform3DGroup rotations;

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            int size = 3;
            double edge_len = 1;
            double space = 0.05;
            double len = edge_len * size + space * (size - 1);
            double distanceFactor = 2.3;

            RubikCube c = new RubikCube(size, new Point3D(-len/2, -len/2, -len/2), edge_len, space);

            Point3D cameraPos = new Point3D(len * distanceFactor, len * distanceFactor, len * distanceFactor);
            PerspectiveCamera camera = new PerspectiveCamera(
                cameraPos,
                new Vector3D(-cameraPos.X, -cameraPos.Y, -cameraPos.Z),
                new Vector3D(0, 1, 0),
                45
            );

            rotations = new Transform3DGroup();
            c.Transform = rotations;
            
            this.mainViewport.Camera = camera;
            this.mainViewport.Children.Add(c);
            this.mainViewport.Children.Add(Helpers.createTouchFaces(len, size, rotations, new DiffuseMaterial(new SolidColorBrush(Colors.Transparent))));
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            startMoveCamera = e.GetPosition(this);
            allowMoveCamera = true;
            this.Cursor = Cursors.SizeAll;
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            allowMoveCamera = false;
            this.Cursor = Cursors.Arrow;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e) {
            if (allowMoveCamera) {
                moveCamera(e.GetPosition(this));                
            }

            if(allowMoveLayer){
                moveLayer(e.GetPosition((UIElement)sender));
            }
        }

        private void moveCamera(Point p) {
            double distX = p.X - startMoveCamera.X;
            double distY = p.Y - startMoveCamera.Y;

            startMoveCamera = p;

            RotateTransform3D rotationX = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), distY), new Point3D(0, 0, 0));
            RotateTransform3D rotationY = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), distX), new Point3D(0, 0, 0));

            rotations.Children.Add(rotationX);
            rotations.Children.Add(rotationY);
        }

        private void moveLayer(Point p) {
            VisualTreeHelper.HitTest(this.mainViewport, null, new HitTestResultCallback(resultCb), new PointHitTestParameters(p));
        }
        
        private HitTestResultBehavior resultCb(HitTestResult r) {
            MyModelVisual3D model = r.VisualHit as MyModelVisual3D;

            if (model != null) {
                Debug.Print(model.Tag);
            }
            
            return HitTestResultBehavior.Continue;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            allowMoveLayer = true;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            allowMoveLayer = false;
        }
    }
}
