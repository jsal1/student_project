using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace teh_zrenie2
{
    public class AttributeSearch
    {
        /// <summary>
        /// Вернет контур изображения в виде списка точек
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <returns>Список точек</returns>
        public List<Point> ListPointsImage(Bitmap image)
        {
            List<Point> СontourPointsImage = new List<Point>();

            //--поиск контура
            Point FirstPix = Point.Empty;
            for (int height = 0; height < image.Height - 1; height++)
            {
                for (int width = 0; width < image.Width - 1; width++)
                {
                    if (image.GetPixel(width, height).GetBrightness() < 1)//black
                    {
                        FirstPix = new Point(width, height);
                        break;
                    }
                }
                if (FirstPix != Point.Empty)
                {
                    СontourPointsImage.Add(FirstPix);
                    break;
                }
            }
            ScaningImage(image, FirstPix, СontourPointsImage);

            return СontourPointsImage;//----------------------------
        }
        /// <summary>
        /// Вычисляет количество точек на изображении
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <returns></returns>
        public int CounterPoints(Bitmap image)
        {
            int CountPixels = 0;
            for (int height = 0; height < image.Height; height++)
                for (int width = 0; width < image.Width; width++)
                    if (image.GetPixel(width, height).GetBrightness() < 1)
                        CountPixels++;
            return CountPixels;
        }
        /// <summary>
        /// Вычисляет количество точек на контуре изображения
        /// </summary>
        /// <param name="points">Список точек</param>
        /// <returns></returns>
        public int CounterPointsContour(List<Point> points)
        {
            return points.Count();
        }
        /// <summary>
        /// Вычисляет число связанных точек контура в 8 направлениях от текущей
        /// </summary>
        /// <param name="points">Список точек</param>
        /// <returns>Массив направлений</returns>
        public int[] CounterWay(List<Point> points)
        {
            int N1 = 0, N2 = 0, N3 = 0, N4 = 0,
                N5 = 0, N6 = 0, N7 = 0, N8 = 0;
            for (int way = 0; way < points.Count - 1; way++)
            {
                if (points[way].X > points[way + 1].X)
                {
                    if (points[way].Y > points[way + 1].Y)
                        N4++;
                    else if (points[way].Y < points[way + 1].Y)
                        N6++;
                    else //==
                        N5++;
                }
                else if (points[way].X < points[way + 1].X)
                {
                    if (points[way].Y > points[way + 1].Y)
                        N2++;
                    else if (points[way].Y < points[way + 1].Y)
                        N8++;
                    else //==
                        N1++;
                }
                else //==
                {
                    if (points[way].Y > points[way + 1].Y)
                        N3++;
                    else if (points[way].Y < points[way + 1].Y)
                        N7++;
                }
            }
            int[] N = new int[8];
            N[0] = N1; N[1] = N2; N[2] = N3; N[3] = N4; 
            N[4] = N5;  N[5] = N6; N[6] = N7; N[7] = N8;
            return N;
        }
        /// <summary>
        /// Вычисляет метрическую длину контура 
        /// </summary>
        /// <param name="N">Массив направлений</param>
        /// <returns></returns>
        public int CounterLength(int[] N)
        {
            return (int)(1 * (N[0] + N[2] + N[4] + N[6]) + Math.Sqrt(2) * (N[1] + N[3] + N[5] + N[7]));
        }
        /// <summary>
        /// Вычисляет кривизну точек 
        /// </summary>
        /// <param name="points"></param>
        /// <returns>Массив углов 1-0гр/2-90гр/3-135гр</returns>
        public int[] CounterAngle(List<Point> points)
        {
            int N90 = 0, N135 = 0, N0 = 0;
            for (int p = 1; p < points.Count - 1; p++)
            {
                Point p0 = points[p - 1],
                      p1 = points[p],//центр
                      p2 = points[p + 1];

                //создаем угол в виде массива 3на3
                int[,] ishodnik = new int[3, 3];
                int x1 = p1.X - p0.X,
                    y1 = p1.Y - p0.Y,
                    x2 = p1.X - p2.X,
                    y2 = p1.Y - p2.Y;
                mass(x1, y1, ishodnik);
                mass(x2, y2, ishodnik);
                ishodnik[1, 1] = 1;
            
                int[,] maska1 = {   { 0, 1, 0 },
                                    { 0, 1, 0 },
                                    { 0, 1, 0 } };
                if (MyEquals(ishodnik, maska1))
                {
                    N0++;
                    continue;
                }
                int[,] maska2 = {   { 0, 0, 0 },
                                    { 1, 1, 1 },
                                    { 0, 0, 0 } };
                if (MyEquals(ishodnik, maska2))
                {
                    N0++;
                    continue;
                }
                int[,] maska3 = {   { 0, 0, 1 },
                                    { 0, 1, 0 },
                                    { 1, 0, 0 } };
                if (MyEquals(ishodnik, maska3))
                {
                    N0++;
                    continue;
                }
                int[,] maska4 = {   { 1, 0, 0 },
                                    { 0, 1, 0 },
                                    { 0, 0, 1 } };
                if (MyEquals(ishodnik, maska4))
                {
                    N90++;
                    continue;
                }
                int[,] maska5 = {   { 1, 0, 1 },
                                    { 0, 1, 0 },
                                    { 0, 0, 0 } };
                if (MyEquals(ishodnik, maska5))
                {
                    N90++;
                    continue;
                }
                int[,] maska6 = {   { 0, 0, 0 },
                                    { 0, 1, 0 },
                                    { 1, 0, 1 } };
                if (MyEquals(ishodnik, maska6))
                {
                    N90++;
                    continue;
                }
                int[,] maska7 = {   { 0, 0, 1 },
                                    { 0, 1, 0 },
                                    { 0, 0, 1 } };
                if (MyEquals(ishodnik, maska7))
                {
                    N90++;
                    continue;
                }
                int[,] maska8 = {   { 1, 0, 0 },
                                    { 0, 1, 0 },
                                    { 1, 0, 0 } };
                if (MyEquals(ishodnik, maska8))
                {
                    N90++;
                    continue;
                }
                int[,] maska9 = {   { 0, 1, 0 },
                                    { 0, 1, 0 },
                                    { 0, 0, 1 } };
                if (MyEquals(ishodnik, maska9))
                {
                    N135++;
                    continue;
                }
                int[,] maska10 = {   { 0, 0, 1 },
                                    { 0, 1, 0 },
                                    { 0, 1, 0 } };
                if (MyEquals(ishodnik, maska10))
                {
                    N135++;
                    continue;
                }
                int[,] maska11 = {   { 1, 0, 0 },
                                    { 0, 1, 0 },
                                    { 0, 1, 0 } };
                if (MyEquals(ishodnik, maska11))
                {
                    N135++;
                    continue;
                }
                int[,] maska12 = {   { 0, 1, 0 },
                                    { 0, 1, 0 },
                                    { 1, 0, 0 } };
                if (MyEquals(ishodnik, maska12))
                {
                    N135++;
                    continue;
                }
                int[,] maska13 = {   { 0, 0, 0 },
                                    { 0, 1, 1 },
                                    { 1, 0, 0 } };
                if (MyEquals(ishodnik, maska13))
                {
                    N135++;
                    continue;
                }
                int[,] maska14 = {   { 0, 0, 0 },
                                    { 1, 1, 0 },
                                    { 0, 0, 1 } };
                if (MyEquals(ishodnik, maska14))
                {
                    N135++;
                    continue;
                }
                int[,] maska15 = {   { 0, 0, 1 },
                                    { 1, 1, 0 },
                                    { 0, 0, 0 } };
                if (MyEquals(ishodnik, maska15))
                {
                    N135++;
                    continue;
                }
                int[,] maska16 = {   { 1, 0, 0 },
                                    { 0, 1, 1 },
                                    { 0, 0, 0 } };
                if (MyEquals(ishodnik, maska16))
                {
                    N135++;
                    continue;
                }
            }
            return new int[] { N0, N90, N135 };
        }




            /// <summary>
            /// Поиск точек контура на изображении
            /// </summary>
            /// <param name="image">Входящее изображение</param>
            /// <param name="firstPix">Первая точка пересечения контура</param>
            /// <param name="сontourPointsImage">Список точек контура</param>
            /// <returns>Список точек контура</returns>
        private void ScaningImage(Bitmap image, Point firstPix, List<Point> сontourPointsImage)
        {
            int x = firstPix.X,
                    y = firstPix.Y - 1;
            WAY DirectWay = WAY.North;
            int iteration = 0;
            bool work = true;
            while (work)
            {
                switch (DirectWay)
                {
                    case WAY.North:
                        {
                            if (image.GetPixel(x, y).GetBrightness() < 1)//black ///left
                            {
                                if (!сontourPointsImage.Contains(new Point(x, y)))
                                    сontourPointsImage.Add(new Point(x, y));
                                x--;
                                DirectWay = WAY.West;
                            }
                            else //white ///right
                            {
                                x++;
                                DirectWay = WAY.East;
                            }
                        }
                        break;
                    case WAY.East:
                        {
                            if (image.GetPixel(x, y).GetBrightness() < 1)//black ///left
                            {
                                if (!сontourPointsImage.Contains(new Point(x, y)))
                                    сontourPointsImage.Add(new Point(x, y));
                                y--;
                                DirectWay = WAY.North;
                            }
                            else //white ///right
                            {
                                y++;
                                DirectWay = WAY.South;
                            }
                        }
                        break;
                    case WAY.South:
                        {
                            if (image.GetPixel(x, y).GetBrightness() < 1)//black ///left
                            {
                                if (!сontourPointsImage.Contains(new Point(x, y)))
                                    сontourPointsImage.Add(new Point(x, y));
                                x++;
                                DirectWay = WAY.East;
                            }
                            else //white ///right
                            {
                                x--;
                                DirectWay = WAY.West;
                            }
                        }
                        break;
                    case WAY.West:
                        {
                            if (image.GetPixel(x, y).GetBrightness() < 1)//black ///left
                            {
                                if (!сontourPointsImage.Contains(new Point(x, y)))
                                    сontourPointsImage.Add(new Point(x, y));
                                y++;
                                DirectWay = WAY.South;
                            }
                            else //white ///right
                            {
                                y--;
                                DirectWay = WAY.North;
                            }
                        }
                        break;
                }
                iteration++;
                if (iteration > 9)
                    if (firstPix == new Point(x, y))
                        work = false;
            }
        }
        private enum WAY
        {
            North,
            East,
            South,
            West
        }
        /// <summary>
        /// Cравнивает массивы на идентичность
        /// </summary>
        /// <param name="arr1">Массив 1</param>
        /// <param name="arr2">Массив 2</param>
        /// <returns></returns>
        private bool MyEquals(int[,] arr1, int[,] arr2)
        {
            if (arr1 == null || arr2 == null)
                return false;
            else
            {
                for (int i = 0; i < arr1.GetLength(0); i++)
                    for (int j = 0; j < arr1.GetLength(1); j++)
                    {
                        if (arr1[i, j] != arr2[i, j])
                            return false;
                    }
                return true;
            }
        }
        /// <summary>
        /// Создает точку в массиве 3на3
        /// </summary>
        /// <param name="x">х координата</param>
        /// <param name="y">у координата</param>
        /// <param name="ishodnik">Массив в котором поставить точку</param>
        private void mass(int x, int y, int[,] ishodnik)
        {
            if (x > 0)
            {
                if (y > 0)
                {
                    ishodnik[2, 2] = 1;
                }
                else if (y < 0)
                {
                    ishodnik[0, 2] = 1;
                }
                else ishodnik[1, 2] = 1;
            }
            else if (x < 0)
            {
                if (y > 0)
                {
                    ishodnik[2, 0] = 1;
                }
                else if (y < 0)
                {
                    ishodnik[0, 0] = 1;
                }
                else ishodnik[1, 0] = 1;
            }
            else
            {
                if (y > 0)
                {
                    ishodnik[2, 1] = 1;
                }
                else if (y < 0)
                {
                    ishodnik[0, 1] = 1;
                }
            }
        }
    }
}

