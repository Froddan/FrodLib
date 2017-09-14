using System;
using System.ComponentModel;
using FrodLib.Extensions;

namespace FrodLib
{

    public struct Rect : IEquatable<Rect>
    {
        private double _height;
        private double _width;
        private double _x;
        private double _y;

        public static readonly Rect Empty = new Rect(double.PositiveInfinity, double.PositiveInfinity, double.NegativeInfinity, double.NegativeInfinity);

        public Rect(Point pointA, Point pointB)
        {
            _x = Math.Min(pointA.X, pointB.X);
            _y = Math.Min(pointA.Y, pointB.Y);
            _width = Math.Abs(pointA.X - pointB.X);
            _height = Math.Abs(pointA.Y - pointB.Y);
        }

        public Rect(Point coord, double width, double height)
            : this(coord.X, coord.Y, width, height)
        {
        }

        public Rect(double x, double y, double width, double height)
            : this()
        {
            if (!double.IsNegativeInfinity(width) && !double.IsNegativeInfinity(height) && (width < 0 || height < 0))
            {
                throw new ArgumentException("Width and/or height can't be negative");
            }

            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public double Bottom
        {
            get { return _y + _height; }
        }

        public Point BottomLeft
        {
            get
            {
                return new Point(X, Y + Height);
            }
        }

        public Point BottomRight
        {
            get
            {
                return new Point(X + Width, Y + Height);
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("Can't set a value on an empty rectangle");
                }
                if (value < 0)
                {
                    throw new ArgumentException("Height can't be negative");
                }
                _height = value;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return double.IsPositiveInfinity(X) && double.IsPositiveInfinity(Y) && double.IsNegativeInfinity(Width) && double.IsNegativeInfinity(Height);
            }
        }

        public double Left
        {
            get { return _x; }
        }

        public Point Location
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("Can't set a value on an empty rectangle");
                }
                X = value.X;
                Y = value.Y;
            }
        }

        public double Right
        {
            get { return _x + _width; }
        }

        public double Top
        {
            get { return _y; }
        }

        public Point TopLeft
        {
            get
            {
                return new Point(X, Y);
            }
        }

        public Point TopRight
        {
            get
            {
                return new Point(X + Width, Y);
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("Can't set a value on an empty rectangle");
                }
                if (value < 0)
                {
                    throw new ArgumentException("Width can't be negative");
                }
                _width = value;
            }
        }

        public double X
        {
            get { return _x; }
            set
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("Can't set a value on an empty rectangle");
                }
                _x = value;
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("Can't set a value on an empty rectangle");
                }
                _y = value;
            }
        }

        public bool Contains(Rect rect)
        {
            return this.Left <= rect.Left && this.Top <= rect.Top && this.Right >= rect.Right && this.Bottom >= rect.Bottom;
        }

        public bool Contains(Point point)
        {
            return Contains(new Rect(point, point));
        }

        public bool Contains(double x, double y)
        {
            return Contains(new Rect(x, y, 0, 0));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Rect))
            {
                return false;
            }
            return Equals((Rect)obj);
        }

        public bool Equals(Rect other)
        {
            return this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 7;
                hash = (hash * 269) ^ this._x.GetHashCode();
                hash = (hash * 269) ^ this._y.GetHashCode();
                hash = (hash * 269) ^ this._width.GetHashCode();
                hash = (hash * 269) ^ this._height.GetHashCode();
                return hash;
            }
        }

        public void Inflate(double width, double height)
        {
            X -= width;
            Y -= height;
            Width += width;
            Height += height;
        }

        public bool IntersectsWith(Rect rect)
        {
            if (rect.Left > this.Right) return false;
            else if (rect.Right < this.Left) return false;
            else if (rect.Top > this.Bottom) return false;
            else if (rect.Bottom < this.Top) return false;
            else return true;
        }

        public void Intersect(Rect rect)
        {
            if (!IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {

                double left = Math.Max(this.Left, rect.Left);
                double top = Math.Max(this.Top, rect.Top);
                double right = Math.Min(this.Right, rect.Right);
                double bottom = Math.Min(this.Bottom, rect.Bottom);

                this = new Rect(new Point(left, top), new Point(right, bottom));
            }
        }

        public void Offset(double offsetX, double offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Scale(double scaleX, double scaleY)
        {
            Width *= scaleX;
            Height *= scaleY;
        }

        public void Union(Point point)
        {
            double left = Math.Min(this.Left, point.X);
            double top = Math.Min(this.Top, point.Y);
            double right = Math.Max(this.Right, point.X);
            double bottom = Math.Max(this.Bottom, point.Y);

            this = new Rect(new Point(left, top), new Point(right, bottom));
        }

        public void Union(Rect rect)
        {
            double left = Math.Min(this.Left, rect.Left);
            double top = Math.Min(this.Top, rect.Top);
            double right = Math.Max(this.Right, rect.Right);
            double bottom = Math.Max(this.Bottom, rect.Bottom);

            this = new Rect(new Point(left, top), new Point(right, bottom));
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3}", _x, _y, _width, _height);
        }

        public static bool operator ==(Rect rectA, Rect rectB)
        {
            return rectA.Equals(rectB);
        }

        public static bool operator !=(Rect rectA, Rect rectB)
        {
            return !rectA.Equals(rectB);
        }
    }
}