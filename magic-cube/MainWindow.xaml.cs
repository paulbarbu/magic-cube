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
            int size = 2;
            double edge_len = 1;
            double space = 0.05;
            double len = edge_len * size + space * (size - 1);
            double distanceFactor = 2.3;
            double middle = (edge_len*size + space*(size-1))/2;

            RubikCube c = new RubikCube(size, new Point3D(-middle, -middle, -middle), edge_len, space);

            Point3D cameraPos = new Point3D(len * distanceFactor, len * distanceFactor, len * distanceFactor);
            PerspectiveCamera camera = new PerspectiveCamera(
                cameraPos,
                new Vector3D(-cameraPos.X, -cameraPos.Y, -cameraPos.Z),
                new Vector3D(0, 1, 0),
                45
            );

            rotations = new Transform3DGroup();
            c.group.Transform = rotations;

            ModelVisual3D m = new ModelVisual3D();
            m.Content = c.group;

            this.mainViewport.Camera = camera;
            this.mainViewport.Children.Add(m);

            double small_num = Math.Pow(10, -5)+0.3;

            double[] o = new double[] { -middle, edge_len + space / 2, middle };
            Model3DGroup touchFaces = new Model3DGroup();

            //MyModelVisual3D asd = new MyModelVisual3D();

            /*
            for (int y = 0; y < o.Length; y++) {
                for (int z = 0; z < o.Length; z++) {
                    for (int x = 0; x < o.Length; x++) {
                        if (o[z] == middle) { //front
                            double distanceX = edge_len + space;
                            if (x == 0) {
                                distanceX = edge_len + space / 2;
                            }


                            touchFaces.Children.Add(Helpers.createRectangleModel(new Point3D[]{
                                new Point3D(o[x], y*middle+small_num, z),
                                new Point3D(o[x], y*middle+small_num, z),
                                new Point3D(o[x],  y*middle+small_num, z),                
                                new Point3D(o[x],  y*middle+small_num, z),
                            }, new DiffuseMaterial(new SolidColorBrush(Colors.HotPink))));
                        }
                    }
                }
            }*/

            MyModelVisual3D upper_far = new MyModelVisual3D();
            upper_far.Content = Helpers.createRectangleModel(new Point3D[]{
                new Point3D(-middle, middle+small_num, -middle),
                new Point3D(-middle, middle+small_num, 0),
                new Point3D(middle,  middle+small_num, 0),                
                new Point3D(middle,  middle+small_num, -middle),
            }, new DiffuseMaterial(new SolidColorBrush(Colors.HotPink)));
            upper_far.Tag = "UF";

            ModelVisual3D upper_right = new ModelVisual3D();
            upper_right.Content = Helpers.createRectangleModel(new Point3D[]{
                new Point3D(0, middle+small_num, -middle),
                new Point3D(0, middle+small_num, middle),
                new Point3D(middle,  middle+small_num, middle),                
                new Point3D(middle,  middle+small_num, -middle),
            }, new DiffuseMaterial(new SolidColorBrush(Colors.Red)));

            this.mainViewport.Children.Add(upper_far);
            this.mainViewport.Children.Add(upper_right);
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
            RayMeshGeometry3DHitTestResult r = (RayMeshGeometry3DHitTestResult)VisualTreeHelper.HitTest(this.mainViewport, p);

            if (r == null) {
                return;
            }

            try {
                MyModelVisual3D model = (MyModelVisual3D)r.VisualHit;
                Debug.Print(model.Tag);
            }
            catch (InvalidCastException ex) {
                ModelVisual3D model = (ModelVisual3D)r.VisualHit;
            }

            MeshGeometry3D m = r.MeshHit.Clone();
            GeometryModel3D g = new GeometryModel3D(m, new DiffuseMaterial(new SolidColorBrush(Colors.Black)));

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
