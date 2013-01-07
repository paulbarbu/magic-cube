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
            double total_length = (edge_len + space) * size * 2;
            double middle = ((edge_len + space) * size)/2;

            RubikCube c = new RubikCube(size, new Point3D(-middle, -middle, -middle), edge_len, space);
            
            PerspectiveCamera camera = new PerspectiveCamera(
                new Point3D(total_length, total_length, total_length),
                new Vector3D(-total_length, -total_length, -total_length),
                new Vector3D(0, 1, 0),
                45
            );

            rotations = new Transform3DGroup();
            c.group.Transform = rotations;

            ModelVisual3D m = new ModelVisual3D();
            m.Content = c.group;

            this.mainViewport.Camera = camera;
            this.mainViewport.Children.Add(m);
        }
        

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            startMoveCamera = e.GetPosition(this);
            allowMoveCamera = true;
            this.Cursor = Cursors.SizeAll;

            Debug.Print("DOWN");
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            allowMoveCamera = false;
            this.Cursor = Cursors.Arrow;
            Debug.Print("UP");
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
            RayMeshGeometry3DHitTestResult r = (RayMeshGeometry3DHitTestResult)VisualTreeHelper.HitTest(this.mainViewport, p);

            if (r == null) {
                return;
            }

            //TODO: ignore black sides 
            MeshGeometry3D m = r.MeshHit.Clone();
            GeometryModel3D g = new GeometryModel3D(m, new DiffuseMaterial(new SolidColorBrush(Colors.HotPink)));

            ModelVisual3D v = new ModelVisual3D();
            v.Content = g;

            this.mainViewport.Children.Add(v);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            allowMoveLayer = true;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            allowMoveLayer = false;
        }

    }
}
