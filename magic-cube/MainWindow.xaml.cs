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

        private enum Difficulty {
            Easy = 10,
            Normal = 20,
            Hard = 30,
            VeryHard = 40
        }

        //TODO: scramble the cube before the game starts (speed up the animation) + difficulty levels
        //TODO: create a RubikCube instance out of a 2D matrix so I can display the scrambled cube when loading a saved game
        //TODO: animations: enable, disable

        Point startMoveCamera;
        bool allowMoveCamera = false, allowMoveLayer = false;
        Transform3DGroup rotations = new Transform3DGroup();
        RubikCube c;
        bool gameOver = false;

        Movement movement = new Movement();
        HashSet<string> touchedFaces = new HashSet<string>();

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            int size = 3;
            double edge_len = 1;
            double space = 0.05;
            double len = edge_len * size + space * (size - 1);
            double distanceFactor = 2.3;
            
            c = new RubikCube(new Cube2D(size), size, new Point3D(-len / 2, -len / 2, -len / 2), TimeSpan.FromMilliseconds(250), edge_len, space);
            c.Transform = rotations;

            Point3D cameraPos = new Point3D(len * distanceFactor, len * distanceFactor, len * distanceFactor);
            PerspectiveCamera camera = new PerspectiveCamera(
                cameraPos,
                new Vector3D(-cameraPos.X, -cameraPos.Y, -cameraPos.Z),
                new Vector3D(0, 1, 0),
                45
            );

            this.mainViewport.Camera = camera;
            this.mainViewport.Children.Add(c);
            this.mainViewport.Children.Add(
                Helpers.createTouchFaces(len, size, rotations, 
                    new DiffuseMaterial(new SolidColorBrush(Colors.Transparent))));
        }

        private void scramble(Difficulty d) {
            Random r = new Random();
            RotationDirection direction;
            List<KeyValuePair<Move, CubeFace>> moves = new List<KeyValuePair<Move, CubeFace>>{
                {new KeyValuePair<Move, CubeFace>(Move.B, CubeFace.R)},
                {new KeyValuePair<Move, CubeFace>(Move.D, CubeFace.R)},
                {new KeyValuePair<Move, CubeFace>(Move.E, CubeFace.R)},
                {new KeyValuePair<Move, CubeFace>(Move.F, CubeFace.R)},
                {new KeyValuePair<Move, CubeFace>(Move.L, CubeFace.F)},
                {new KeyValuePair<Move, CubeFace>(Move.M, CubeFace.F)},
                {new KeyValuePair<Move, CubeFace>(Move.R, CubeFace.F)},
                {new KeyValuePair<Move, CubeFace>(Move.S, CubeFace.R)},
                {new KeyValuePair<Move, CubeFace>(Move.U, CubeFace.F)},
            };    

            for (int i = 0; i < (int)d; i++ ) {
                int index = r.Next(0, moves.Count);
                                
                if (r.Next(0, 101) == 0) {
                    direction = RotationDirection.ClockWise;
                }
                else {
                    direction = RotationDirection.CounterClockWise;
                }

                Debug.Print("Move: {0} {1}", moves[index].Key.ToString(), direction.ToString());
                c.rotate(new KeyValuePair<Move, RotationDirection>(moves[index].Key, direction), moves[index].Value);
            }
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
                touchedFaces.Add(model.Tag);
            }
            
            return HitTestResultBehavior.Continue;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            touchedFaces.Clear();
            allowMoveLayer = true;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            allowMoveLayer = false;
            movement.TouchedFaces = touchedFaces;

            if (gameOver) {
                return;
            }

            KeyValuePair<Move, RotationDirection> m = movement.getMove();

            if (m.Key != Move.None) {
                c.rotate(m, movement.getDominantFace());
            }
            else {
                Debug.Print("Invalid move!");
            }

            if (c.isUnscrambled()) {
                Debug.Print("!!!!! GAME OVER !!!!!");
                gameOver = true;
            }

            Debug.Print("\n");
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            scramble(Difficulty.Easy);

            c.animationDuration = TimeSpan.FromMilliseconds(370);
        }
    }
}
