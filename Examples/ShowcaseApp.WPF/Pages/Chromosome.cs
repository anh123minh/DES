using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphX.PCL.Common.Enums;
using GraphX.Controls;
using GraphX.Controls.Models;
using TrafficManagement.WPF.Models;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using Microsoft.Win32;
using QuickGraph;
using TrafficManagement.WPF.FileSerialization;
using Rect = GraphX.Measure.Rect;
using QuickGraph.Algorithms.RankedShortestPath;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using AForge.Genetic;

namespace TrafficManagement.WPF.Pages
{
    public class Chromosome:ShortArrayChromosome,IChromosome
    {
      //  public ushort[] path = null;
        public IEnumerable<DataEdge> EdgeStore=null;
        public IEnumerable<DataVertex> VertexStore=null;
        
        public Chromosome(int length,int maxvalue, IEnumerable<DataEdge> EdgeStore, IEnumerable<DataVertex> VertexStore) :base(length,maxvalue)
        {
           // this.path = path;
            base.maxValue = maxvalue;
            base.length = length;
            this.EdgeStore = EdgeStore;
            this.VertexStore = VertexStore;
        }
        protected Chromosome(Chromosome sourse):base(sourse)
        {
          //  path = sourse.path;
            maxValue = sourse.maxValue;
            length = sourse.length;
            EdgeStore = sourse.EdgeStore;
            VertexStore = sourse.VertexStore;
        }
        public override IChromosome CreateNew()
        {
            return new Chromosome(length,maxValue,EdgeStore,VertexStore);
        }
        public override IChromosome Clone()
        {
            return new Chromosome(this);
        }
    }
}
