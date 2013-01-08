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

        //TODO: use Rect3D?
        public RubikCube(int size, Point3D o, double len = 1, double space = 0.1) {
            this.size = size;
            this.origin = o;
            this.edge_len = len;
            this.space = space;

            createCube();
        }

        protected override void createCube() {
            Cube c;
            Dictionary<CubeFace, Material> colors;

            double x_offset, y_offset, z_offset;

            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    for (int x = 0; x < size; x++) {
                        x_offset = (edge_len + space) * x;
                        y_offset = (edge_len + space) * y;
                        z_offset = (edge_len + space) * z;

                        Point3D p = new Point3D(origin.X + x_offset, origin.Y + y_offset, origin.Z + z_offset);

                        colors = setFaceColors(x, y, z);

                        c = new Cube(p, edge_len, colors);
                        this.Children.Add(c);
                    }
                }
            }
        }

        private Dictionary<CubeFace, Material> setFaceColors(int x, int y, int z){
            Dictionary<CubeFace, Material> colors = new Dictionary<CubeFace,Material>();

            if (x == 0) {
                colors.Add(CubeFace.L, new DiffuseMaterial(new SolidColorBrush(Colors.Red)));
            }

            if (y == 0) {
                colors.Add(CubeFace.D, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow)));
            }

            if (z == 0) {
                colors.Add(CubeFace.B, new DiffuseMaterial(new SolidColorBrush(Colors.Green)));
            }

            if (x == size-1) {
                colors.Add(CubeFace.R, new DiffuseMaterial(new SolidColorBrush(Colors.Orange)));
            }

            if (y == size - 1) {
                colors.Add(CubeFace.U, new DiffuseMaterial(new SolidColorBrush(Colors.White)));
            }

            if (z == size - 1) {
                colors.Add(CubeFace.F, new DiffuseMaterial(new SolidColorBrush(Colors.Blue)));
            }

            return colors;
        }
    }
}
