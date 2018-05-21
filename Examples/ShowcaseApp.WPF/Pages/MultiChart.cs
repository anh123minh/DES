using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace SimulationV1.WPF.Pages
{
    public class MultiChart : System.Windows.Controls.DataVisualization.Charting.Chart
    {
        #region SeriesSource (DependencyProperty)

        public IEnumerable SeriesSource
        {
            get
            {
                return (IEnumerable)GetValue(SeriesSourceProperty);
            }
            set
            {
                SetValue(SeriesSourceProperty, value);
            }
        }

        public static readonly DependencyProperty SeriesSourceProperty = DependencyProperty.Register(
            name: "SeriesSource",
            propertyType: typeof(IEnumerable),
            ownerType: typeof(MultiChart),
            typeMetadata: new PropertyMetadata(
                defaultValue: default(IEnumerable),
                propertyChangedCallback: new PropertyChangedCallback(OnSeriesSourceChanged)
            )
        );

        private static void OnSeriesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IEnumerable oldValue = (IEnumerable)e.OldValue;
            IEnumerable newValue = (IEnumerable)e.NewValue;
            MultiChart source = (MultiChart)d;
            source.OnSeriesSourceChanged(oldValue, newValue);
        }
        private static void OnSeriesTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataTemplate oldValue = (DataTemplate)e.OldValue;
            DataTemplate newValue = (DataTemplate)e.NewValue;
            MultiChart source = (MultiChart)d;
            source.OnSeriesTemplateChanged(oldValue, newValue);
        }
        protected virtual void OnSeriesTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            this.SeriesTemplate = newValue;
            OnSeriesSourceChanged(SeriesSource, SeriesSource);
        }

        protected virtual void OnSeriesSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            this.Series.Clear();

            if (newValue != null)
            {
                foreach (object item in newValue)
                {
                    DataTemplate dataTemplate = null;

                    if (this.SeriesTemplate != null)
                    {
                        dataTemplate = this.SeriesTemplate;
                    }

                    // load data template content
                    if (dataTemplate != null)
                    {
                        Series series = dataTemplate.LoadContent() as Series;

                        if (series != null)
                        {
                            // set data context
                            series.DataContext = item;

                            this.Series.Add(series);
                        }
                    }
                }
            }
        }

        #endregion

        #region SeriesTemplate (DependencyProperty)

        public DataTemplate SeriesTemplate
        {
            get
            {
                return (DataTemplate)GetValue(SeriesTemplateProperty);
            }
            set
            {
                SetValue(SeriesTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty SeriesTemplateProperty = DependencyProperty.Register(
            name: "SeriesTemplate",
            propertyType: typeof(DataTemplate),
            ownerType: typeof(MultiChart),
            typeMetadata: new PropertyMetadata(
                defaultValue: default(DataTemplate),
                propertyChangedCallback: new PropertyChangedCallback(OnSeriesTemplateChanged)
            )
        );
        #endregion
    }
}
