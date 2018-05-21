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

        PointCollection CreateStepLineSeries(PointCollection source)
        {
            PointCollection returnValue = new PointCollection();
            for (int i = 0; i < source.Count; i++)
            {
                Point currentValue = source[i];
                returnValue.Add(currentValue);
                if (i < source.Count - 1)
                {
                    Point nextValue = source[i + 1];
                    if (nextValue.Y <= currentValue.Y)
                    {
                        returnValue.Add(new Point(nextValue.X, currentValue.Y));
                    }
                    else
                    {
                        returnValue.Add(new Point(currentValue.X, nextValue.Y));
                    }
                }
            }
            return returnValue;
        }
    }
}
