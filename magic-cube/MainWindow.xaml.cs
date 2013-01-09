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
            c.Transform = rotations;
            
            this.mainViewport.Camera = camera;
            this.mainViewport.Children.Add(c);

            double small_num = Math.Pow(10, -5);
            double offset = len/size;

            MyModelVisual3D face = new MyModelVisual3D();
            MyModelVisual3D touchFace;

            for (int y = 0; y < size; y++ ) {
                for (int z = 0; z < size; z++) {
                    for (int x = 0; x < size; x++) {
                        if (y == size - 1 && false) { //up
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle + x*offset, middle+small_num, -middle),
                                new Point3D(-middle + x*offset, middle+small_num, middle),
                                new Point3D(-middle + (x+1) * offset,  middle+small_num, middle),       
                                new Point3D(-middle + (x+1) * offset,  middle+small_num, -middle),
                                }, new DiffuseMaterial(new SolidColorBrush(Colors.HotPink)));
                                 
                            touchFace.Tag = "UV" + x;

                            face.Children.Add(touchFace);


                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                            new Point3D(-middle, middle+small_num, -middle + z*offset),
                            new Point3D(-middle, middle+small_num, -middle + (z+1)*offset),
                            new Point3D(middle,  middle+small_num, -middle + (z+1)*offset),       
                            new Point3D(middle,  middle+small_num, -middle + z*offset),
                            }, new DiffuseMaterial(new SolidColorBrush(Colors.AliceBlue)));

                            touchFace.Tag = "UH" + z;

                            face.Children.Add(touchFace);
                        }

                        if (y == 0 && false) { //down
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle + x*offset, -middle-small_num, -middle),
                                new Point3D(-middle + x*offset, -middle-small_num, middle),
                                new Point3D(-middle + (x+1) * offset,  -middle-small_num, middle),       
                                new Point3D(-middle + (x+1) * offset,  -middle-small_num, -middle),
                                }, new DiffuseMaterial(new SolidColorBrush(Colors.HotPink)), false);

                            touchFace.Tag = "DV" + x;

                            face.Children.Add(touchFace);


                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle, -middle-small_num, -middle + z*offset),
                                new Point3D(-middle, -middle-small_num, -middle + (z+1)*offset),
                                new Point3D(middle,  -middle-small_num, -middle + (z+1)*offset),       
                                new Point3D(middle,  -middle-small_num, -middle + z*offset),
                                }, new DiffuseMaterial(new SolidColorBrush(Colors.AliceBlue)), false);

                            touchFace.Tag = "DH" + z;

                            face.Children.Add(touchFace);
                        }

                        if (z == size - 1 && false) { //front                            
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle + x*offset, middle, middle+small_num),
                                new Point3D(-middle + x*offset, -middle, middle+small_num),
                                new Point3D(-middle + (x+1)*offset, -middle, middle+small_num),       
                                new Point3D(-middle + (x+1)*offset, middle, middle+small_num),
                                }, new DiffuseMaterial(new SolidColorBrush(Colors.HotPink)));

                            touchFace.Tag = "FV" + x;
                            
                            face.Children.Add(touchFace);
                            
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                  
                                new Point3D(-middle, middle - y*offset, middle+small_num),
                                new Point3D(-middle, middle - (y+1)*offset, middle+small_num),
                                new Point3D(middle, middle - (y+1)*offset, middle+small_num),       
                                new Point3D(middle, middle - y*offset, middle+small_num),
                                }, new DiffuseMaterial(new SolidColorBrush(Colors.AliceBlue)));

                            touchFace.Tag = "FH" + y;

                            face.Children.Add(touchFace);
                        }

                        if (z == 0 &&false) { //back
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(-middle + x*offset, middle, -middle-small_num),
                                new Point3D(-middle + x*offset, -middle, -middle-small_num),
                                new Point3D(-middle + (x+1) * offset, -middle, -middle-small_num),       
                                new Point3D(-middle + (x+1) * offset, middle, -middle-small_num),
                                }, new DiffuseMaterial(new SolidColorBrush(Colors.HotPink)), false);

                            touchFace.Tag = "BV" + x;

                            face.Children.Add(touchFace);

                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                  
                                new Point3D(-middle, middle - y*offset, -middle-small_num),
                                new Point3D(-middle, middle - (y+1)*offset, -middle-small_num),
                                new Point3D(middle, middle - (y+1)*offset, -middle-small_num),       
                                new Point3D(middle, middle - y*offset, -middle-small_num),
                                }, new DiffuseMaterial(new SolidColorBrush(Colors.AliceBlue)), false);

                            touchFace.Tag = "BH" + y;

                            face.Children.Add(touchFace);
                        }

                        if (x == size - 1) { //right          
                            
                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                                
                                new Point3D(middle+small_num, middle, -middle + (z+1)*offset),
                                new Point3D(middle+small_num, -middle, -middle + (z+1)*offset),
                                new Point3D(middle+small_num, -middle, -middle + z*offset),       
                                new Point3D(middle+small_num, middle, -middle + z*offset),
                                }, new DiffuseMaterial(new SolidColorBrush(Colors.HotPink)));

                            touchFace.Tag = "RV" + z;

                            face.Children.Add(touchFace);

                            touchFace = new MyModelVisual3D();
                            touchFace.Content = Helpers.createRectangleModel(new Point3D[]{                           
                                new Point3D(middle+small_num, -middle + (y+1)*offset, middle),
                                new Point3D(middle+small_num, -middle + y*offset, middle),
                                new Point3D(middle+small_num, -middle + y*offset, -middle),       
                                new Point3D(middle+small_num, -middle + (y+1)*offset, -middle),
                                }, new DiffuseMaterial(new SolidColorBrush(Colors.AliceBlue)));

                            touchFace.Tag = "RH" + y;

                            face.Children.Add(touchFace);
                        }
                    }
                }
            }


            face.Transform = rotations;
            this.mainViewport.Children.Add(face);

            /*
            MyModelVisual3D upper_far = new MyModelVisual3D();
            upper_far.Content = Helpers.createRectangleModel(new Point3D[]{
                new Point3D(-middle, middle+small_num, -middle),
                new Point3D(-middle, middle+small_num, 0),
                new Point3D(middle,  middle+small_num, 0),                
                new Point3D(middle,  middle+small_num, -middle),
            }, new DiffuseMaterial(new SolidColorBrush(Colors.HotPink)));
            upper_far.Tag = "roz";

            MyModelVisual3D upper_right = new MyModelVisual3D();
            upper_right.Content = Helpers.createRectangleModel(new Point3D[]{
                new Point3D(0, middle+small_num, -middle),
                new Point3D(0, middle+small_num, middle),
                new Point3D(middle,  middle+small_num, middle),                
                new Point3D(middle,  middle+small_num, -middle),
            }, new DiffuseMaterial(new SolidColorBrush(Colors.Red)));

            upper_right.Tag = "rosu";

            this.mainViewport.Children.Add(upper_far);
            this.mainViewport.Children.Add(upper_right);*/
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
