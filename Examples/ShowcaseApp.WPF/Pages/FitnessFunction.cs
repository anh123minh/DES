using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphX.PCL.Common.Enums;
using GraphX.Controls;
using GraphX.Controls.Models;
using Simulation.WPF.Models;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using Microsoft.Win32;
using QuickGraph;
using Simulation.WPF.FileSerialization;
using Rect = GraphX.Measure.Rect;
using QuickGraph.Algorithms.RankedShortestPath;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using AForge.Genetic;

namespace Simulation.WPF.Pages
{
   public class FitnessFunction:IFitnessFunction
    
    {       
       private int edgeCout=0;
       public IEnumerable<DataEdge> EdgeStore;
       public List<DataVertex> VertexStore;        
       public FitnessFunction(IEnumerable<DataEdge> EdgeStore, List<DataVertex> VertexStore)
        {
            int i = 0;
            this.EdgeStore = EdgeStore;
           this.VertexStore = VertexStore;            
            foreach (DataEdge ed in EdgeStore)
                i++;
            this.edgeCout = i;
        }

        public double Evaluate(IChromosome chromosome)
        {
            return 1/Alpha(chromosome); 
        }
        public object Translate(IChromosome chromosome)
        {
            return chromosome.ToString();
        }
        // Вычисление усреднего коэффициента использования каналов
        public double Alpha(IChromosome chromosome)
        {
            bool overload = false;
            double _alpha = 0;
            // массив номера маршрутов
            ushort[] ipath = ((ShortArrayChromosome)chromosome).Value;
            // Копия IEnumerable<DataEdge>
            IEnumerable<DataEdge> CopyDataEdge= EdgeStore;
            List<DataVertex> CopyDataVertex = VertexStore;
            // Обход всех ИП и вычисление нагрузок каналов
            for (int i=0; i<ipath.GetLength(0);i++)                
            {
                string nameIP = "ИП " + (i + 1).ToString();
                foreach(DataVertex vertex in CopyDataVertex)
                    if (vertex.Text==nameIP)   // если узел является "ИП i"
                    {                      
                        foreach(DataEdge ed in vertex.ListPath[ipath[i]])
                            foreach(DataEdge channel in CopyDataEdge)
                                // сравнить два канала
                                if ((ed.Source==channel.Source && ed.Target==channel.Target) || (ed.Source == channel.Target && ed.Target == channel.Source))
                        {
                            channel.Load = channel.Load + vertex.Traffic;
                                       
                        }                       
                      //  break;
                    }              
            }
           
            foreach (DataEdge ed in CopyDataEdge)
            {
                if (ed.Load > ed.Capacity) overload = true;                  
                _alpha = _alpha + (ed.Load/ed.Capacity);
                ed.Load = 0;
                ed.Alpha = 0;
            }

            if (overload == true) return 1;
            else
            return (_alpha/edgeCout);
        }
    }
}
