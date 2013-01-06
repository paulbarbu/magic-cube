using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace magic_cube {
    public class RubikCube : Cube {
        /// <summary>
        /// The cube will be size x size x size
        /// </summary>
        private int size;

        private Point3D origin;

        /// <summary>
        /// Length of the cube edge
        /// </summary>
        private double edge_len;

        /// <summary>
        /// Space between the cubes forming the bigger cube
        /// </summary>
        private double space;

        public RubikCube(int size, Point3D o, double len = 1, double space = 0.1) {
            this.size = size;
            this.origin = o;
            this.edge_len = len;
            this.space = space;
            group = new Model3DGroup();

            createCube();
        }

        protected override void createCube(){
            Cube c;

            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    for (int x = 0; x < size; x++) {
                        c = new Cube(new Point3D(origin.X + (edge_len + space) * x, origin.Y + (edge_len + space) * y, origin.Z + (edge_len + space) * z), edge_len, new Dictionary<CubeFace, Material>() {
                            {CubeFace.F, new DiffuseMaterial(new SolidColorBrush(Colors.White))},
                            {CubeFace.R, new DiffuseMaterial(new SolidColorBrush(Colors.Blue))},
                            {CubeFace.U, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow))},
                        });

                        group.Children.Add(c.group);
                    }
                }
            }
        }
    }
}
