using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Diagnostics;

namespace magic_cube {
    public static class Helpers {
        /// <summary>
        /// Create a triangle which can be used for more complex models
        /// </summary>
        /// <param name="p0">The first position of the mesh</param>
        /// <param name="p1">The second position of the mesh</param>
        /// <param name="p2">The third position of the mesh</param>
        /// <param name="m">A <see cref="Material"/> to be applied to the triangle</param>
        /// <returns><see cref="GeometryModel3D"/></returns>
        public static GeometryModel3D createTriangleModel(Point3D p0, Point3D p1, Point3D p2, Material m) {
            MeshGeometry3D triangleMesh = new MeshGeometry3D();
            triangleMesh.Positions.Add(p0);
            triangleMesh.Positions.Add(p1);
            triangleMesh.Positions.Add(p2);

            triangleMesh.TriangleIndices.Add(0);
            triangleMesh.TriangleIndices.Add(1);
            triangleMesh.TriangleIndices.Add(2);

            Vector3D normal = calculateNormal(p0, p1, p2);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);

            return new GeometryModel3D(triangleMesh, m);
        }
        
        public static Model3DGroup createRectangleModel(Point3D[] p, Material m) {
            if (p.Length != 4) {
                Debug.Print("BYE!");
                return null;
            }

            Model3DGroup rect = new Model3DGroup();

            rect.Children.Add(createTriangleModel(p[0], p[1], p[2], m));
            rect.Children.Add(createTriangleModel(p[0], p[2], p[3], m));

            return rect;
        }

        /// <summary>
        /// Calculate the normal of a plane
        /// </summary>
        /// <param name="p0">The first point of the plane</param>
        /// <param name="p1">The second point of the plane</param>
        /// <param name="p2">The third point of the plane</param>
        /// <returns><see cref="Vector3D"/> representing the plane's normal</returns>
        private static Vector3D calculateNormal(Point3D p0, Point3D p1, Point3D p2) {
            Vector3D v1 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v2 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);

            return Vector3D.CrossProduct(v1, v2);
        }
    }
}
