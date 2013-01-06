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

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            int edge_len = 1;
            int size = 5; // the cube will be size x size x size
            double space = 0.1; //space between the cubes forming the bigger cube

            Cube c;
            ModelVisual3D m;

            Point3D o = new Point3D();
            o.X = o.Y = o.Z = 0;

            for(int y = 0; y<size; y++){
                for (int z = 0; z < size; z++) {
                    for (int x = 0; x < size; x++) {
                        c = new Cube(new Point3D(o.X + (edge_len + space) * x, o.Y + (edge_len + space) * y, o.Z + (edge_len + space) * z), edge_len, new Dictionary<CubeFace, Material>() {
                            {CubeFace.F, new DiffuseMaterial(new SolidColorBrush(Colors.White))},
                            {CubeFace.R, new DiffuseMaterial(new SolidColorBrush(Colors.Blue))},
                            {CubeFace.U, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow))},
                        });

                        m = new ModelVisual3D();
                        m.Content = c.group;
                        this.mainViewport.Children.Add(m);
                    }
                }
            }
        }
    }
}
