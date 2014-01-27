using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Finds fast paths from one location to another in the game world.
    /// Or from one level 2 node to another level 2 node
    /// </summary>
    public class PathFinder
    {


        
        /// <summary>
        /// Finds the fastest level 2 path, and returns the cost for the path
        /// </summary>
        public static Level2PathNode FindLevel2Path(Level2Node start, Level2Node end, out int totalCost)
        {
            //the location of the end node, used to calculate heirstics
            Location endLocation = end.Location;

            //level 2 nodes that need to be visited
            PathFinderQueue<Level2Node> openList = new PathFinderQueue<Level2Node>();

            //level 2 node we came from to get here, each level 2 node is mapped to the level 2 node we came from to get there
            Dictionary<Level2Node, Level2Node> previous = new Dictionary<Level2Node, Level2Node>();

            //level 2 edge we came on to get here, each level 2 node is mapped to the level 2 edge we came on to get there
            Dictionary<Level2Node, Level2Edge> previousEdge = new Dictionary<Level2Node, Level2Edge>();

            //land that no longer needs to be visited
            HashSet<Level2Node> closedList = new HashSet<Level2Node>();
            
            //heuristic is the minimal possible cost between the two tiles
            int heuristic = LocationUtils.DistanceApart(start.Location, end.Location) * CanWalkUtil.HIGHWAY_COST;

            //add the start location to the open List
            openList.Enqueue(start, 0, heuristic);

            //search unil we found the end
            while (true)
            {
                //get the next node in the open list, and the distance to get to that node
                int costToNodeOn;
                Level2Node nodeOn = openList.Dequeue(out costToNodeOn);

                //if theres no more nodes then there is no path
                if (nodeOn == null)
                {
                    totalCost = int.MaxValue;
                    return null;
                }

                //if we found the end then stop searching
                if (nodeOn == end)
                {
                    totalCost = costToNodeOn;
                    break;
                }

                //add the node we are on to the closed list
                closedList.Add(nodeOn);

                //add all the adjacent nodes to the open list (if there are not alrady in the closed list)
                foreach (Level2Edge edge in nodeOn.Edges)
                {
                    //get the node the edge leads to
                    Level2Node adjacentNode = edge.Destination;
                    int edgeCost = edge.Cost;

                    //dont add nodes that are in the closed list
                    if (closedList.Contains(adjacentNode)) { continue; }

                    //heuristic is the minimal possible cost between the two tiles
                    heuristic = LocationUtils.DistanceApart(adjacentNode.Location, endLocation) * CanWalkUtil.HIGHWAY_COST;

                    //item will be updated if its already in the queue (as long as it has a smaller cost)
                    bool hadSmallerCost = openList.Enqueue(adjacentNode, costToNodeOn + edgeCost, heuristic);

                    //if the item was added or update then set the previous
                    if (hadSmallerCost)
                    {
                        if (previous.ContainsKey(adjacentNode) == false)
                        {
                            previous.Add(adjacentNode, nodeOn);
                            previousEdge.Add(adjacentNode, edge);
                        }
                        else
                        {
                            previous[adjacentNode] = nodeOn;
                            previousEdge[adjacentNode] = edge;
                        }
                    }
                }
            }


            //create the path from the end node to the start node
            Level2PathNode pathNode = null;
            Level2Node traceBack = end;            
            while (true)
            {
                //we are going tracing backward so the current path node will be the next node on the path we create
                Level2PathNode nextPathNode = pathNode;

                //the edge we took to get to the node
                Level2Edge traceBackEdge = previousEdge[traceBack];

                //create a level 2 node
                pathNode = new Level2PathNode();

                //the path to get between this level 2 node and the next is the level 1 path
                pathNode.Level1Path = traceBackEdge.Level1Path;

                //set the next level 2 node
                pathNode.Next = nextPathNode;

                //the level 2 node will go through the cluster we end at
                pathNode.Clusters.Add(traceBackEdge.Destination.Cluster);

                //and the cluster we start at (only add if it is a different cluster)
                if (traceBack.Cluster != traceBackEdge.Destination.Cluster)
                {
                    pathNode.Clusters.Add(traceBack.Cluster);
                }

                //keep walking back                
                traceBack = previous[traceBack];

                //if back to start we are done tracing back
                if (traceBack == start) { break; }

            }

            return pathNode;
        }



        /// <summary>
        /// Finds the fastest path between two locations, without optimazation, and the cost of that path.
        /// Optionally pass a cluster to restrict the path to be within a cluster
        /// </summary>
        public static LocationPathNode FindDirectPath(Location start, Location end, Cluster withInCluster, out int totalCost)
        {
            //if start is end there is no cost, and there is no "next node"
            if (start == end)
            {
                totalCost = 0;
                return null;
            }

            //locations that need to be visited
            PathFinderQueue<Location> openList = new PathFinderQueue<Location>();

            //locations we came from to get here, each location is mapped to the location we came from to get there
            Dictionary<Location, Location> previous = new Dictionary<Location, Location>();

            //land that no longer needs to be visited
            HashSet<Location> closedList = new HashSet<Location>();
            
            //heuristic is the minimal possible cost between the two tiles
            int heuristic = LocationUtils.DistanceApart(start, end) * CanWalkUtil.HIGHWAY_COST;

            //add the start location to the open List
            openList.Enqueue(start, 0, heuristic);

            //search unil we found the end
            while (true)
            {
                //get the next location in the open list, and the distance to get to that location
                int costToLocationOn;
                Location locationOn = openList.Dequeue(out costToLocationOn);

                //if theres no more locations then there is no path
                if (locationOn == null)
                {
                    totalCost = int.MaxValue;
                    return null;
                }
                
                //if we found the end then stop searching
                if (locationOn == end)
                {
                    totalCost = costToLocationOn;
                    break;
                }

                //add the square were on to the closed list
                closedList.Add(locationOn);

                //add all the adjacent locations to the open list (if there are not alrady in the closed list)
                foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
                {
                    //get location in that direction
                    Location adjacentLocation = locationOn.GetAdjacent(dir);
                    
                    //dont add locations that are in the closed list
                    if (closedList.Contains(adjacentLocation)) { continue; }

                    //if we are staying within the same cluster and this would be going outside the cluster then dont go there                    
                    if (withInCluster != null && withInCluster.IsLocationInCluster(adjacentLocation) == false) { continue; }

                    //determine if we can walk there
                    int cost;
                    bool canGoThere = CanWalkUtil.CanWalkTo(locationOn, dir, out cost);

                    //dont add locations we cannot walk to
                    if (canGoThere == false) { continue; }
                    

                    //heuristic is the minimal possible cost between the two tiles
                    heuristic = LocationUtils.DistanceApart(adjacentLocation, end) * CanWalkUtil.HIGHWAY_COST;

                    //item will be updated if its already in the queue (as long as it has a smaller cost)
                    bool hadSmallerCost = openList.Enqueue(adjacentLocation, costToLocationOn + cost, heuristic);

                    //if the item was added or update then set the previous
                    if (hadSmallerCost)
                    {
                        if (previous.ContainsKey(adjacentLocation) == false)
                        {
                            previous.Add(adjacentLocation, locationOn);
                        }
                        else
                        {
                            previous[adjacentLocation] = locationOn;
                        }
                    }
                }
            }


            //create the path from the end Location to the start Location (but not incliding the start location)
            LocationPathNode pathNode = null;
            Location traceBack = end;
            while (true)
            {
                //we are going tracing backward so the current path node will be the next node on the path we create
                LocationPathNode nextPathNode = pathNode;
                
                //create node that point to the next node on the path
                pathNode = new LocationPathNode();
                pathNode.Location = traceBack;
                pathNode.Next = nextPathNode;
                
                //keep walking back
                traceBack = previous[traceBack];

                //if back to start we are done tracing back (note we dont add the start node to the path)
                if (traceBack == start) { break; }
            }

            return pathNode;
        }
        

    }
}
