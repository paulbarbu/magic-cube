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
using System.IO;

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

        //TODO: create a RubikCube instance out of a 2D matrix so I can display the scrambled cube when loading a saved game
        //TODO: upon save also save the current difficulty
        Point startMoveCamera;
        bool allowMoveCamera = false, allowMoveLayer = false, gameOver = false;
        int size = 3;
        double edge_len = 1;
        double space = 0.05;
        double len;

        Transform3DGroup rotations = new Transform3DGroup();
        RubikCube c;
        MyModelVisual3D touchFaces;
        Movement movement = new Movement();
        HashSet<string> touchedFaces = new HashSet<string>();

        string defaultTitle = "Magic Cube - ";
        Difficulty currentDifficulty = Difficulty.Normal;

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            double distanceFactor = 2.3;
            len = edge_len * size + space * (size - 1);
            
            Point3D cameraPos = new Point3D(len * distanceFactor, len * distanceFactor, len * distanceFactor);
            PerspectiveCamera camera = new PerspectiveCamera(
                cameraPos,
                new Vector3D(-cameraPos.X, -cameraPos.Y, -cameraPos.Z),
                new Vector3D(0, 1, 0),
                45
            );

            this.mainViewport.Camera = camera;
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
                gameOver = true;
                saveMenu.IsEnabled = false;
                Debug.Print("!!!!! GAME OVER !!!!!");
            }

            Debug.Print("\n");
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            init(currentDifficulty, defaultTitle + "Normal");
        }

        private void NewGame_Click(object sender, RoutedEventArgs e) {
            string d = ((MenuItem)sender).Tag.ToString();

            switch(d){
                case "Easy":
                    currentDifficulty = Difficulty.Easy;
                    break;
                case "Normal":
                    currentDifficulty = Difficulty.Normal;
                    break;
                case "Hard":
                    currentDifficulty = Difficulty.Hard;
                    break;
                case "Very Hard":
                    currentDifficulty = Difficulty.VeryHard;
                    break;
            }

            init(currentDifficulty, defaultTitle + d);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.F5) {
                init(currentDifficulty, this.Title);
            }
        }

        private void init(Difficulty d, string title, string file=null) {
            this.mainViewport.Children.Remove(c);
            this.mainViewport.Children.Remove(touchFaces);
            rotations.Children.Clear();


            if (file != null) {
                c = new RubikCube(file, size, new Point3D(-len / 2, -len / 2, -len / 2), TimeSpan.FromMilliseconds(370), edge_len, space);
            }
            else{
                c = new RubikCube(size, new Point3D(-len / 2, -len / 2, -len / 2), TimeSpan.FromMilliseconds(370), edge_len, space);
            }

            c.Transform = rotations;

            touchFaces = Helpers.createTouchFaces(len, size, rotations,
                    new DiffuseMaterial(new SolidColorBrush(Colors.Transparent)));

            this.mainViewport.Children.Add(c);
            this.mainViewport.Children.Add(touchFaces);


            if (!enableAnimations.IsChecked) {
                c.animationDuration = TimeSpan.FromMilliseconds(0);
            }

            if (file == null) {
                scramble(d);
            }

            gameOver = false;

            this.Title = title;
            saveMenu.IsEnabled = true;
        }

        private void enableAnimations_Checked(object sender, RoutedEventArgs e) {
            if(c != null){
                c.animationDuration = TimeSpan.FromMilliseconds(370);
            }
        }

        private void enableAnimations_Unchecked(object sender, RoutedEventArgs e) {
            if (c != null) {
                c.animationDuration = TimeSpan.FromMilliseconds(0);
            }
        }

        //TODO: show a load/save dialog
        private void saveMenu_Click(object sender, RoutedEventArgs e) {
            c.save();
        }

        private void loadMenu_Click(object sender, RoutedEventArgs e) {
            init(Difficulty.Easy, defaultTitle.TrimEnd(new char[]{' ', '-'}), "abc");

            //TODO: check for solved cube
        }
    }
}
