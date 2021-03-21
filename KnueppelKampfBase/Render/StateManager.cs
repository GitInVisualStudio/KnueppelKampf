using KnueppelKampfBase.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace KnueppelKampfBase.Render
{
    /// <summary>
    /// Dient zur einfachen verwaltung des Graphics
    /// </summary>
    public class StateManager
    {
        private static Graphics g;
        public static float partialTicks;
        private static State state = new State(true);
        private static Stopwatch watch = new Stopwatch();

        public static float delta;
        public static State State => state;
        public static Graphics Graphics => g;
        public static float ScaleX => state.ScaleX;
        public static float ScaleY => state.ScaleY;
        public static float TranslateX => state.TranslateX;
        public static float TranslateY => state.TranslateY;
        public static float Rotation => state.Rotation;
        public static Color Color => state.Color;
        public static Font Font => state.Font;

        /// <summary>
        /// Updated die Graphics-Instanz zum Zeichnen
        /// </summary>
        /// <param name="g"></param>
        public static void Update(Graphics g)
        {
            delta = (float)watch.Elapsed.TotalSeconds;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            watch.Reset();
            watch.Start();
            SetGraphics(g);
        }

        /// <summary>
        /// Zeichnet einen String
        /// </summary>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void DrawString(string text, float x, float y)
        {
            g.DrawString(text, Font, new SolidBrush(Color), x, y);
        }

        public static void DrawString(string text, Vector position) => DrawString(text, position.X, position.Y);

        public static void DrawCenteredString(string text, float x, float y)
        {
            Vector size = GetStringSize(text);
            DrawString(text, x - size.X / 2, y - size.Y / 2);
        }

        public static void DrawCenteredString(string text, Vector pos) => DrawCenteredString(text, pos.X, pos.Y);

        /// <summary>
        /// Zeichnet ein Bild
        /// </summary>
        /// <param name="img"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void DrawImage(Bitmap img, float x, float y)
        {
            g.DrawImage(img, x, y);
        }

        public static void DrawImage(Bitmap img, float x, float y, float width, float height, float opacity = 1.0f)
        {
            ColorMatrix matrix = new ColorMatrix
            {
                Matrix33 = opacity
            };
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            g.DrawImage(img, new Rectangle(0, 0, (int)width, (int)height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);
        }

        public static void DrawImage(Bitmap img, Vector pos) => DrawImage(img, pos.X, pos.Y);

        /// <summary>
        /// Zeichnet ein Rechteck
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void DrawRect(float x, float y, float width, float height, float lineWidth = 1)
        {
            g.DrawRectangle(new Pen(new SolidBrush(Color), lineWidth), x, y, width, height);
        }

        public static void DrawRect(Vector position, float width, float height, float lineWidth = 1) => DrawRect(position.X, position.Y, width, height, lineWidth);

        public static void DrawRect(Vector position, Vector size, float lineWidth = 1) => DrawRect(position.X, position.Y, size.X, size.Y, lineWidth);

        /// <summary>
        /// Zeichnet ein gefülltest Rechteck;
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void FillRect(float x, float y, float width, float height)
        {
            g.FillRectangle(new SolidBrush(Color), x, y, width, height);
        }

        public static void FillRect(Vector pos, float width, float height) => FillRect(pos.X, pos.Y, width, height);

        public static void FillRect(Vector pos, Vector size) => FillRect(pos.X, pos.Y, size.X, size.Y);

        /// <summary>
        /// Zeichnet einen Kreis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r">Radius</param>
        /// <param name="lineWidth">Linien stärke</param>
        public static void DrawCircle(float x, float y, float r, float lineWidth = 1)
        {
            g.DrawEllipse(new Pen(new SolidBrush(Color), lineWidth), x, y, r, r);
        }

        public static void DrawCircle(Vector position, float r, float lineWidth = 1) => DrawCircle(position.X, position.Y, r, lineWidth);


        /// <summary>
        /// Zeichnet einen Kreis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r">Radius</param>
        /// <param name="lineWidth">Linien stärke</param>
        public static void FillCircle(float x, float y, float r)
        {
            g.FillEllipse(new SolidBrush(Color), x - r / 2, y - r / 2, r, r);
        }

        /// <summary>
        /// zeichnet einen kreis mit fabverlauf
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="angle"></param>
        public static void FillGradientCircle(float x, float y, float r, Color c1, Color c2, float angle = 90)
        {
            LinearGradientBrush brush = GetGradientBrush(new Vector(x - r / 2f, y - r / 2f), new Vector(r + 2, r + 2), c1, c2, angle);
            g.FillEllipse(brush, x - r / 2, y - r / 2, r, r);
        }

        public static void DrawCircle(Vector position, float r) => DrawCircle(position.X, position.Y, r);

        public static void FillCircle(float x, float y, float r, float a)
        {
            int angle = (int)a;
            PointF[] points = new PointF[(int)angle];
            for (int i = 0; i < angle; i++)
                points[i] = new PointF(x + (float)System.Math.Sin((float)i) * r / 2f, y + (float)System.Math.Cos((float)i) * r / 2f);
            g.FillPolygon(new SolidBrush(Color), points);
        }

        /// <summary>
        /// Zeichnet eine Linie
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        public static void DrawLine(float x, float y, float x1, float y1, float width = 1)
        {
            g.DrawLine(new Pen(new SolidBrush(Color), width), x, y, x1, y1);
        }

        public static void DrawLine(Vector v1, Vector v2, float width = 1) => DrawLine(v1.X, v1.Y, v2.X, v2.Y, width);


        /// <summary>
        /// Dreht die Transformation
        /// </summary>
        /// <param name="angle">In Grad</param>
        public static void Rotate(float angle)
        {
            g.RotateTransform(angle);
            state.Rotation = angle;
        }

        /// <summary>
        /// Transformiert die Matrix zu einem bestimmten Punkt
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Translate(float x, float y)
        {
            g.TranslateTransform(x, y);
            State.TranslateX = x;
            State.TranslateY = y;
        }

        public static void Translate(Vector vector)
        {
            g.TranslateTransform(vector.X, vector.Y);
            State.TranslateX = vector.X;
            State.TranslateY = vector.Y;
        }

        /// <summary>
        /// Skaliert die Transformation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Scale(float x, float y)
        {
            if (x == 0)
                x = (float)1E-3;
            if (y == 0)
                y = (float)1E-3;
            g.ScaleTransform(x, y);
            state.ScaleX = x;
            state.ScaleY = y;
        }

        public static void Scale(float x) => Scale(x, x);


        /// <summary>
        /// Erstellt ein neues State, damit die Transformation unverändert wiederhergestellt werden kan
        /// </summary>
        public static void Push()
        {
            State state = new State();
            StateManager.state = state;
        }

        /// <summary>
        /// Stellt die letzt Transformation wieder her
        /// </summary>
        public static void Pop()
        {
            g.ResetTransform();
            state = state.PrevState;
            Scale(state.ScaleX, state.ScaleY);
            Translate(state.TranslateX, state.TranslateY);
            Rotate(state.Rotation);
            SetGraphics(state.Graphics);
        }

        /// <summary>
        /// Gibt die String-Breite zurück
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float GetStringWidth(string s) => GetStringSize(s).X;


        /// <summary>
        /// Gibt die String-Höhe zurück
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float GetStringHeight(string s) => GetStringSize(s).Y;


        /// <summary>
        /// Gibt die Größe des String wieder
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector GetStringSize(string s)
        {
            SizeF size = g.MeasureString(s, Font);
            return new Vector(size.Width, size.Height);
        }


        /// <summary>
        /// Setzt die Farbe mit der Gezeichnet werden soll
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void SetColor(int r, int g, int b) => SetColor(r, g, b, 255);

        public static void SetColor(int r, int g, int b, int a) => state.Color = Color.FromArgb(a, r, g, b);

        public static void SetColor(Color color) => state.Color = color;

        public static void SetColor(int color)
        {
            //8-Bit
            int a = color >> 24 & 255;
            int r = color >> 16 & 255;
            int g = color >> 8 & 255;
            int b = color & 255;
            SetColor(r, g, b, a);
        }

        public static void SetFont(Font f) => state.Font = f;

        public static void SetGraphics(Graphics g)
        {
            StateManager.g = g;
            state.Graphics = g;
            if (g != null)
                g.SmoothingMode = SmoothingMode.AntiAlias;
        }

        /// <summary>
        /// zeichnet ein polygon mit den punkten
        /// </summary>
        /// <param name="points"></param>
        public static void DrawPolygon(PointF[] points) => g.DrawPolygon(new Pen(new SolidBrush(Color)), points);
        public static void FillPolygon(PointF[] points) => g.FillPolygon(new SolidBrush(Color), points);

        /// <summary>
        /// zeichnet ein rundes "rechteck"
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="r"></param>
        /// <param name="res"></param>
        public static void DrawRoundRect(Vector location, Vector size, float r = 10, int res = 100)
            => g.DrawPolygon(new Pen(new SolidBrush(Color)), GetRoundRectPoints(location.X, location.Y, size.X, size.Y, r, res));

        public static void FillRoundRect(float x, float y, float width, float height, float r = 10, int res = 100)
            => g.FillPolygon(new SolidBrush(Color), GetRoundRectPoints(x, y, width, height, r, res));

        public static void FillRoundRect(Vector location, Vector size, float r = 10, int res = 100)
            => FillRoundRect(location.X, location.Y, size.X, size.Y, r, res);

        /// <summary>
        /// gibt die punkte für ein rundes "rechteck" zurück
        /// r gibt an um wie viele pixel das rechteck abgerundet werden soll
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="r"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static PointF[] GetRoundRectPoints(float x, float y, float width, float height, float r, int res)
        {
            if (height < 1 || width < 1)
            {
                PointF[] p = new PointF[1];
                p[0] = new PointF(x, y);
                return p;
            }

            PointF[] points = new PointF[res];

            for (float i = 0, k = 0; i < System.Math.PI * 2; i += (float)System.Math.PI * 2 / (float)res, k++)
            {
                float offsetX = r, offsetY = r;
                if (i <= System.Math.PI)
                    offsetX = width - r;
                if (i <= System.Math.PI / 2 || i >= System.Math.PI * 3 / 2)
                    offsetY = height - r;
                points[(int)k] = new PointF(x + (float)System.Math.Sin(i) * r + offsetX, y + (float)System.Math.Cos(i) * r + offsetY);
            }

            return points;
        }

        /// <summary>
        /// zeichnet ein rundes "rechteck" mit fabverlauf
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="angle"></param>
        /// <param name="r"></param>
        /// <param name="res"></param>
        public static void FillGradientRoundRect(Vector location, Vector size, Color c1, Color c2, float angle = 0.0f, float r = 10, int res = 100)
        {
            if (size.X < 1 || size.Y < 1)
                return;
            g.FillPolygon(GetGradientBrush(location, size, c1, c2, angle), GetRoundRectPoints(location.X, location.Y, size.X, size.Y, r, res));
        }

        private static LinearGradientBrush GetGradientBrush(Vector location, Vector size, Color c1, Color c2, float angle)
        {
            return new LinearGradientBrush(new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), c1, c2, angle);
        }

        /// <summary>
        /// zeichnet ein rechteck mit fabverlauf
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="angle"></param>
        public static void FillGradientRect(Vector location, Vector size, Color c1, Color c2, float angle = 0.0f)
        {
            g.FillRectangle(GetGradientBrush(location, size, c1, c2, angle), location.X, location.Y, size.X, size.Y);
        }

        public static void DrawGradientRect(Vector location, Vector size, Color c1, Color c2, float angle = 0.0f)
        {
            g.DrawRectangle(new Pen(GetGradientBrush(location, size, c1, c2, angle)), location.X, location.Y, size.X, size.Y);
        }
    }
}