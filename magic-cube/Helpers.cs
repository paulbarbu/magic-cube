using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace magic_cube {
    public static class Helpers {
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

        private static Vector3D calculateNormal(Point3D p0, Point3D p1, Point3D p2) {
            Vector3D v1 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v2 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);

            return Vector3D.CrossProduct(v1, v2);
        }
    }
}
