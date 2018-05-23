using System.Collections.ObjectModel;
using System.Windows.Controls.DataVisualization.Charting;
using System;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace StepLineChart
{

    public class StepLineSeries : LineSeries
    {
        //#region public PointCollection Points  
        /// <summary>  
        /// Gets the collection of points that make up the line.  
        /// </summary>  
        public PointCollection Points
        {
            get { return GetValue(PointsProperty) as PointCollection; }
            set { SetValue(PointsProperty, value); }
        }

        protected override void UpdateShapeFromPoints(IEnumerable<Point> points)
        {
            if (points.Any())
            {
                PointCollection pointCollection = new PointCollection();
                foreach (Point point in points)
                {
                    pointCollection.Add(point);
                }
                Points = CreateStepLineSeries(pointCollection);
            }
            else
            {
                Points = null;
            }
        }

        PointCollection CreateStepLineSeries(PointCollection source)
        {
            PointCollection returnValue = new PointCollection();
            var zeroy = source[0].Y;
            var oney = source[0].X;// doi voi nguon
            foreach (var z in source)
            {
                if (z.Y < zeroy)
                {
                    oney = z.Y;
                    break;
                }
            }
            var flag = true;//xac dinh co phai day tang deu
            double dev = 0;
            var f = 0;
            do
            {
                dev = source[f + 1].Y - source[f].Y;
                flag = flag && Math.Abs(dev - oney) < 1;
                f++;
            } while (f < source.Count - 1);

            for (int i = 0; i < source.Count; i++)
            {
                Point currentValue = source[i];
                returnValue.Add(currentValue);
                if (i < source.Count - 1)
                {
                    Point nextValue = source[i + 1];
                    if (!flag)//neu la day tang deu thi nhay sang else
                    {
                        if (Math.Abs(nextValue.Y - oney) > 1)
                        {
                            returnValue.Add(new Point(nextValue.X, currentValue.Y));
                        }
                        else
                        {
                            returnValue.Add(new Point(currentValue.X, zeroy));
                            returnValue.Add(new Point(nextValue.X, zeroy));
                        }
                    }
                    else
                    {
                        returnValue.Add(new Point(currentValue.X, nextValue.Y));
                    }
                }
            }
            return returnValue;
        }
        //PointCollection CreateStepLineSeries(PointCollection source)
        //{
        //    PointCollection returnValue = new PointCollection();
        //    for (int i = 0; i < source.Count; i++)
        //    {
        //        Point currentValue = source[i];
        //        returnValue.Add(currentValue);
        //        if (i < source.Count - 1)
        //        {
        //            Point nextValue = source[i + 1];
        //            returnValue.Add(new Point(nextValue.X, currentValue.Y));
        //        }
        //    }
        //    return returnValue;
        //}
    }
}
