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
using Microsoft.Win32;

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
            Normal = 2,
            Hard = 30,
            VeryHard = 40
        }

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

        List<KeyValuePair<Move, RotationDirection>> doneMoves = new List<KeyValuePair<Move, RotationDirection>>();

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
            List<Move> moveList = new List<Move> {Move.B, Move.D, Move.E, Move.F, Move.L, Move.M, Move.R, Move.S, Move.U};
            List<KeyValuePair<Move, RotationDirection>> moves = new List<KeyValuePair<Move, RotationDirection>>();

            for (int i = 0; i < (int)d; i++ ) {
                int index = r.Next(0, moveList.Count);
                                
                if (r.Next(0, 101) == 0) {
                    direction = RotationDirection.ClockWise;
                }
                else {
                    direction = RotationDirection.CounterClockWise;
                }
                
                Debug.Print("Move: {0} {1}", moveList[index].ToString(), direction.ToString());

                moves.Add(new KeyValuePair<Move, RotationDirection>(moveList[index], direction));
                doneMoves.Add(new KeyValuePair<Move, RotationDirection>(moveList[index], direction));
            }

            c.rotate(moves);
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
                c.rotate(m);
                doneMoves.Add(m);
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
            solveMenu.IsEnabled = true;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e) {
            solveMenu.IsEnabled = false;
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
            solveMenu.IsEnabled = true;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.F5) {
                solveMenu.IsEnabled = false;
                init(currentDifficulty, this.Title);
                solveMenu.IsEnabled = true;
            }
        }

        private void init(Difficulty d, string title, string file=null) {
            doneMoves.Clear();
            this.mainViewport.Children.Remove(c);
            this.mainViewport.Children.Remove(touchFaces);
            rotations.Children.Clear();
            
            if (file != null) {
                c = new RubikCube(readCube(file, out doneMoves), size, new Point3D(-len / 2, -len / 2, -len / 2), TimeSpan.FromMilliseconds(370), edge_len, space);
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

        private void saveMenu_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = DateTime.Now.ToString("dd-MM-yy Hmm");
            dlg.DefaultExt = ".rubik";
            dlg.Filter = "Magic Cube Save Files (.rubik)|*.rubik";

            if (true == dlg.ShowDialog()) {
                save(dlg.FileName, c.projection.projection, doneMoves);
            }
        }

        private void loadMenu_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".rubik";
            dlg.Filter = "Magic Cube Save Files (.rubik)|*.rubik";

            if (true == dlg.ShowDialog()) {
                doneMoves.Clear();
                solveMenu.IsEnabled = false;

                try {
                    init(currentDifficulty, defaultTitle.TrimEnd(new char[] { ' ', '-' }), dlg.FileName);
                }
                catch (InvalidDataException) {
                    MessageBox.Show("The file contains an invalid cube!\nNew game will start!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                    init(currentDifficulty, defaultTitle + "Normal");                    
                }

                if(c.isUnscrambled()){
                    MessageBox.Show("The file contains a solved cube!\nNew game will start!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                    init(currentDifficulty, defaultTitle + "Normal");
                }

                solveMenu.IsEnabled = true;
            }
        }

        private void solveMenu_Click(object sender, RoutedEventArgs e) {
            gameOver = true;
            solveMenu.IsEnabled = false;

            List<KeyValuePair<Move, RotationDirection>> m = new List<KeyValuePair<Move, RotationDirection>>();

            for (int i = doneMoves.Count - 1; i >= 0; i--) {
                m.Add(new KeyValuePair<Move, RotationDirection>(doneMoves[i].Key, (RotationDirection)(-1 * (int)doneMoves[i].Value)));
            }

            c.rotate(m);
        }
        
        private void save(string fileName, CubeFace[,] projection, List<KeyValuePair<Move, RotationDirection>> moves) {
            using (StreamWriter f = new StreamWriter(fileName)) {
                for (int i = 0; i < size * 4; i++) {
                    for (int j = 0; j < size * 3; j++) {
                        f.Write(projection[i, j].ToString() + " ");
                    }
                    f.WriteLine();
                }

                foreach (var m in doneMoves) {
                    f.WriteLine("{0} {1}", m.Key, m.Value);
                }
            }
        }

        private CubeFace[,] readCube(string fileName, out List<KeyValuePair<Move, RotationDirection>> moves) {
            CubeFace[,] projection = new CubeFace[size * 4, size * 3];

            using (StreamReader r = new StreamReader(fileName)) {
                for (int i = 0; i < size * 4; i++) {
                    string[] line = null;
                    try {
                        line = r.ReadLine().Split(' ');
                    }
                    catch (NullReferenceException) {
                        throw new InvalidDataException();
                    }
                    for (int j = 0; j < size * 3; j++) {
                        try {
                            projection[i, j] = (CubeFace)Enum.Parse(typeof(CubeFace), line[j]);
                        }
                        catch (ArgumentException) {
                            throw new InvalidDataException();
                        }
                        catch (IndexOutOfRangeException) {
                            throw new InvalidDataException();
                        }
                    }
                }

                moves = new List<KeyValuePair<Move,RotationDirection>>();

                while(!r.EndOfStream){
                    string[] line = null;
                    line = r.ReadLine().Split(' ');

                    try {
                        moves.Add(new KeyValuePair<Move, RotationDirection>((Move)Enum.Parse(typeof(Move), line[0]),
                            (RotationDirection)Enum.Parse(typeof(RotationDirection), line[1])));
                    }
                    catch (ArgumentException) {
                        throw new InvalidDataException();
                    }
                }
            }

            return projection;
        }
    }
}
