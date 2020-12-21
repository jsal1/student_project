using System.Collections.Generic;
using System.Drawing;

namespace teh_zrenie2
{
    public class Vectors
    {

        /// <summary>
        /// Список точек контура изображения
        /// </summary>
        public List<Point> ListPointsImage { get; set; }
        
        /// <summary>
        /// Количество точек на изображении
        /// </summary>
        public int CountPointsImage { get; set; }
        /// <summary>
        /// Количество точек на контуре изображения
        /// </summary>
        public int CounterPointsContour { get; set; }
        /// <summary>
        /// Число связанных точек контура в 8 направлениях от текущей
        /// </summary>
        public int[] CounterWay { get; set; }
        /// <summary>
        /// Метрическая длина контура
        /// </summary>
        public int CounterLength { get; set; }
        /// <summary>
        /// Кривизна точек (0;90;135)
        /// </summary>
        public int[] CounterAngle { get; set; }
        /// <summary>
        /// Класс изображения
        /// </summary>
        public int ClassImage { get; set; }
        /// <summary>
        /// Содержит количество распознанных изображений
        /// </summary>
        public int recog { get; set; }
    }
}
