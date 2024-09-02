using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeismicShadowZonesApp
{
    public class Vector3D
    {
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3D(Point3D pFrom, Point3D pTo)
        {
            X = pTo.X - pFrom.X;
            Y = pTo.Y - pFrom.Y;
            Z = pTo.Z - pFrom.Z;
        }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Vector3D GetProjectetVectorToXyPlane()
        {
            return new Vector3D(this.X, this.Y, 0);
        }

        public Vector3D GetUnitVector()
        {
            double length = GetLength();
            if (length == 0) throw new Exception($"Lenght of vector is 0. ({X}, {Y}, {Z})");
            return new Vector3D(this.X / length, this.Y / length, this.Z / length);
        }

        public bool IsEmpty()
        {
            double tol = 1.0e-6;
            return Math.Abs(X) < tol && Math.Abs(Y) < tol && Math.Abs(Z) < tol;
        }

        public double GetLength()
        {
            double length = Math.Sqrt(X * X + Y * Y + Z * Z);
            return length;
        }

        public Vector3D RotateZaxis(bool clockwise)
        {
            if (clockwise)
            {
                return new Vector3D(Y, -X, Z);
            }
            else
            {
                return new Vector3D(-Y, X, Z);
            }
        }

        public Vector3D Reverse()
        {
            return new Vector3D(-this.X, -this.Y, -this.Y);
        }

        //public bool IsEmpty()
        //{
        //    return X == 0 && Y == 0 && Z == 0;
        //}

        static public Vector3D Add(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        static public double Dot(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        static public Vector3D Cross(Vector3D a, Vector3D b)
        {
            double vx = a.Y * b.Z - a.Z * b.Y; // a2b3−a3b2
            double vy = a.Z * b.X - a.X * b.Z; // a3b1−a1b3
            double vz = a.X * b.Y - a.Y * b.X; // a1b2−a2b1

            return new Vector3D(vx, vy, vz);
        }

        public override string ToString()
        {
            return $"X {X:0.###}  Y: {Y:0.###}  Z: {Z:0.###}";
        }

        public static double AngleBetweenVectors_not_used(Vector3D a, Vector3D b)
        {
            double dot = Dot(a, b);
            double lena = a.GetLength();
            double lenb = b.GetLength();
            double cosangle = dot / (lena * lenb);

            return Math.Acos(cosangle);
        }

        public static bool IsAngleBetweenVectorsAcute(Vector3D a, Vector3D b, double tol = -1.0e-12)
        {
            return Vector3D.Dot(a, b) > tol;
        }

        public static bool AreEqual(Vector3D a, Vector3D b, double tol = 1.0e-4)
        {
            bool areEq = Math.Abs(a.X - b.X) < tol &&
                Math.Abs(a.Y - b.Y) < tol &&
                Math.Abs(a.Z - b.Z) < tol;

            //bool areEqHack = Math.Abs(a.X + b.X) < tol &&   //T O D O:
            //    Math.Abs(a.Y + b.Y) < tol &&
            //    Math.Abs(a.Z + b.Z) < tol;

            return areEq; // || areEqHack;
        }
    }

    public class Point3D
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D Clone()
        {
            return new Point3D(this.X, this.Y, this.Z);
        }


        public override string ToString()
        {
            return $"{X:0.###} {Y:0.###} {Z:0.###}";
        }

        public bool AreEqual(Point3D other, double tol = 1.0e-10)
        {
            bool areEqual =
                Math.Abs(other.X - this.X) < tol &&
                Math.Abs(other.Y - this.Y) < tol &&
                Math.Abs(other.Z - this.Z) < tol;
            return areEqual;
        }
    }


}
