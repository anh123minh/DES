using System.Collections.Generic;
using System.Management.Instrumentation;
using QuickGraph.Algorithms.Observers;
using QuickGraph;
using QuickGraph.Algorithms.ShortestPath;
using System;

namespace SimulationV1.WPF.Pages
{
    public class YenAlgorithm
    {
        private DataVertex sourceVertex;
        private DataVertex targetVertex;
        // limit for amount of paths
        private int k;
        private BidirectionalGraph<DataVertex,DataEdge> graph;

        /*
          double type of tag comes from Dijkstra’s algorithm,
          which is used to get one shortest path.
        */

        public YenAlgorithm(BidirectionalGraph<DataVertex, DataEdge> graph, DataVertex s,
          DataVertex t, int k)
        {
            sourceVertex = s;
            targetVertex = t;
            this.k = k;
            this.graph = graph;
        }

        public IEnumerable<IEnumerable<DataEdge>> Execute()
        {
            var listShortestWays = new List<IEnumerable<DataEdge>>();
            // find the first shortest way
            var shortestWay = GetShortestPathInGraph(graph);
            double minDistanceShorterPath = GetPathDistance(shortestWay);
            listShortestWays.Add(shortestWay);

            /*
             * in case of Dijkstra’s algorithm couldn't find any ways
             */
            try
            {
                if (shortestWay == null)
                {
                    throw new InstanceNotFoundException();
                }
            }
            catch (InstanceNotFoundException)
            {
                return null;
            }
            for (var i = 0; i < k - 1; i++)
            {
                var minDistance = double.MaxValue;
                IEnumerable<DataEdge> pathSlot = null;
                // slote for graph state without some edge
                BidirectionalGraph<DataVertex, DataEdge> graphSlot = null;
                foreach (var edge in shortestWay)
                {
                   /* var newGraph=graph;
                    foreach (var path in listShortestWays)
                        foreach( var ed in path)
                        {
                            if (ed.Source==edge.Source)
                                newGraph = RemoveEdge(newGraph, ed);
                        }
*/
                    // get new state without the edge
                    var newGraph = RemoveEdge(graph, edge);

                    //find shortest way in the new graph
                    var newPath = GetShortestPathInGraph(newGraph);                    
                    if (newPath == null)
                    {
                        continue;
                    }
                   
                    var pathWeight = GetPathDistance(newPath);                  
                    if (pathWeight > minDistance)
                    {
                        continue;
                    }                   
                    minDistance = pathWeight;
                    pathSlot = newPath;
                    graphSlot = newGraph;
                }
                if (pathSlot == null)
                {
                    break;
                }
                listShortestWays.Add(pathSlot);
                shortestWay = pathSlot;               
                graph = graphSlot;
            }
            return listShortestWays;
        }

        private double GetPathDistance(IEnumerable<DataEdge> edges)
        {
            var pathSum = 0.0;
            foreach (var edge in edges)
            {
                pathSum += edge.Weight;
            }
            return pathSum;
        }

        private IEnumerable<DataEdge> GetShortestPathInGraph(
          BidirectionalGraph<DataVertex,DataEdge> graph)
        {
            Func<DataEdge, double> edgeWeights = E => E.Weight;
            // calc distances beetween the start vertex and other
            var dijkstra = new DijkstraShortestPathAlgorithm<DataVertex, DataEdge>(graph, edgeWeights);
            var vis = new VertexPredecessorRecorderObserver<DataVertex, DataEdge>();
            using (vis.Attach(dijkstra))
                dijkstra.Compute(sourceVertex);

            // get shortest path from start (source) vertex to target
            IEnumerable<DataEdge> path;

            return vis.TryGetPath(targetVertex, out path) ? path : null;

        }

        private BidirectionalGraph<DataVertex,DataEdge> RemoveEdge( BidirectionalGraph<DataVertex, DataEdge> old,DataEdge edgeRemoving)
        {
            // get copy of the grapth using Serialization and Deserialization
            var copyGraph = old.Clone();

            // remove the edge
            foreach (var edge in copyGraph.Edges)
            {
                if (edge == edgeRemoving)
                {
                    copyGraph.RemoveEdge(edge);
                    break;
                }
            }

            // get all edges but the removing one
            var oldEdges = new List<DataEdge>();
            foreach (var edge in old.Edges)
            {
                if (edge != edgeRemoving)
                {
                    oldEdges.Add(edge);
                }
            }

            return copyGraph;
        }
    }
}