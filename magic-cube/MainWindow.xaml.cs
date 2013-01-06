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
            Cube c1 = new Cube(new Point3D(5, 5, 5), 1, null, new DiffuseMaterial(new SolidColorBrush(Colors.Black)));
            
            Cube c = new Cube(new Point3D(0, 0, 0), 5, new Dictionary<CubeFace, Material>() {
                {CubeFace.F, new DiffuseMaterial(new SolidColorBrush(Colors.White))},
                {CubeFace.R, new DiffuseMaterial(new SolidColorBrush(Colors.Blue))},
                {CubeFace.U, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow))},
            });

            ModelVisual3D model = new ModelVisual3D();
            model.Content = c.group;
            this.mainViewport.Children.Add(model);


            ModelVisual3D model1 = new ModelVisual3D();
            model1.Content = c1.group;
            this.mainViewport.Children.Add(model1);
        }
    }
}
