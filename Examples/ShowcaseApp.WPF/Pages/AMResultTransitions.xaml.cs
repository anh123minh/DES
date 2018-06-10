using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimulationV1.WPF.Pages
{
    /// <summary>
    /// Interaction logic for AMResultTransitions.xaml
    /// </summary>
    public partial class AMResultTransitions : Window
    {
        public DataVertex[] ArrayTransitions;
        private DataVertex vertexTransition;

        public AMResultTransitions(DataVertex[] arrayTransitions)
        {
            InitializeComponent();
            ArrayTransitions = arrayTransitions;
            CbbTransitions.ItemsSource = ArrayTransitions;
        }
        private void btnPlacesIn_Click(object sender, RoutedEventArgs e)
        {
            var tntb1 = vertexTransition.ListTimeNow;
            var pttb1 = vertexTransition.ListTimePlaceIn;
            var asm = new AMMultiChart(tntb1, pttb1);
            asm.Show();
        }

        private void btnPlacesOut_Click(object sender, RoutedEventArgs e)
        {
            var tntb1 = vertexTransition.ListTimeNow;
            var pttb1 = vertexTransition.ListTimePlaceOut;
            var asm = new AMMultiChart(tntb1, pttb1);
            asm.Show();
        }

        private void btnPlacesCommon_Click(object sender, RoutedEventArgs e)
        {
            var tntb1 = vertexTransition.ListTimeNow;
            var pttb1 = vertexTransition.ListTimePlaceIn;
            var pttb2 = vertexTransition.ListTimePlaceOut;
            var asm = new AMMultiChart(tntb1, pttb1,pttb2);
            asm.Show();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vertexTransition = (DataVertex) CbbTransitions.SelectedItem;
        }
    }
}
