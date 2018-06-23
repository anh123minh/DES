﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;

namespace SimulationV1.WPF.Pages
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

        //PointCollection CreateStepLineSeries(PointCollection source)
        //{
        //    PointCollection returnValue = new PointCollection();
        //    var zeroy = source[0].Y;
        //    var oney = source[0].X;// doi voi nguon
        //    //for (int i = 1; i < source.Count - 1; i++)
        //    //{
        //    //    if (source[i].Y < zeroy)
        //    //    {
        //    //        oney = source[i].Y;
        //    //        break;
        //    //    }
        //    //}
        //    foreach (var z in source)
        //    {
        //        if (z.Y < zeroy)
        //        {
        //            oney = z.Y;
        //            break;
        //        }
        //    }

        //    for (int i = 0; i < source.Count; i++)
        //    {
        //        Point currentValue = source[i];
        //        returnValue.Add(currentValue);
        //        if (i < source.Count - 1)
        //        {
        //            Point nextValue = source[i + 1];
        //            if (nextValue.Y <= currentValue.Y)//y sau cao hon y truoc?
        //            {
        //                returnValue.Add(new Point(nextValue.X, currentValue.Y));
                        
        //            }
        //            else
        //            {
        //                //if (Math.Abs(nextValue.Y - oney) < 1)
        //                //{
        //                //    returnValue.Add(new Point(currentValue.X, zeroy));
        //                //    returnValue.Add(new Point(nextValue.X, zeroy));

        //                //}
        //                //else
        //                //{
        //                //    returnValue.Add(new Point(currentValue.X, nextValue.Y));
        //                //}
        //                returnValue.Add(new Point(currentValue.X, nextValue.Y));
        //            }
        //        }
        //    }
        //    return returnValue;
        //}
        PointCollection CreateStepLineSeries(PointCollection source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (i < source.Count - 1)
                {
                    if (Math.Abs(source[i].X - source[i + 1].X) < 0.1 && source[i].Y > source[i + 1].Y)
                    {
                        var temp = source[i];
                        source[i] = source[i + 1];
                        source[i + 1] = temp;
                    }
                }

            }
            PointCollection returnValue = new PointCollection();
            for (int i = 0; i < source.Count; i++)
            {
                Point currentValue = source[i];
                returnValue.Add(currentValue);
                if (i < source.Count - 1)
                {
                    Point nextValue = source[i + 1];
                    returnValue.Add(new Point(nextValue.X, currentValue.Y));
                }
            }
            return returnValue;
        }
        ////PointCollection CreateStepLineSeries(PointCollection source)
        ////{
        ////    PointCollection returnValue = new PointCollection();
        ////    var zeroy = source[0].Y;
        ////    var oney = source[0].X;// doi voi nguon
        ////    foreach (var z in source)
        ////    {
        ////        if (z.Y < zeroy)
        ////        {
        ////            oney = z.Y;
        ////            break;
        ////        }
        ////    }
        ////    var flag = true;//xac dinh co phai day tang deu
        ////    double dev = 0;
        ////    var f = 0;
        ////    do
        ////    {
        ////        dev = source[f + 1].Y - source[f].Y;
        ////        flag = flag && Math.Abs(dev - oney) < 1;
        ////        f++;
        ////    } while (f < source.Count - 1);

        ////    for (int i = 0; i < source.Count; i++)
        ////    {
        ////        Point currentValue = source[i];
        ////        returnValue.Add(currentValue);
        ////        if (i < source.Count - 1)
        ////        {
        ////            Point nextValue = source[i + 1];
        ////            if (!flag)//neu la day tang deu thi nhay sang else
        ////            {
        ////                if (Math.Abs(nextValue.Y - oney) > 1)
        ////                {
        ////                    returnValue.Add(new Point(nextValue.X, currentValue.Y));
        ////                }
        ////                else
        ////                {
        ////                    returnValue.Add(new Point(currentValue.X, zeroy));
        ////                    returnValue.Add(new Point(nextValue.X, zeroy));
        ////                }
        ////            }
        ////            else
        ////            {
        ////                returnValue.Add(new Point(currentValue.X, nextValue.Y));
        ////            }
        ////        }
        ////    }
        ////    return returnValue;
        ////}
    }
}
