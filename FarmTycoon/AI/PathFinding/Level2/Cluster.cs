using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoorLocations = System.Collections.Generic.List<FarmTycoon.Location>;
using DoorLocationsList = System.Collections.Generic.List<System.Collections.Generic.List<FarmTycoon.Location>>;



namespace FarmTycoon
{
    /// <summary>
    /// A square set of locations used for heircal path finding.
    /// </summary>
    public class Cluster
    {
        /// <summary>
        /// Size of each cluster
        /// </summary>
        public const int CLUSTER_SIZE = 10;

        /// <summary>
        /// Location at the corner of the cluster.
        /// </summary>
        private Location[] _corner = new Location[4];
        
        /// <summary>
        /// The four adjacent location clusters in each cardinal direction (null if there is no cluster in that direction)
        /// </summary>
        private Cluster[] _adjacent = new Cluster[4];

        /// <summary>
        /// The level 2 nodes inside this cluster, for each side of the cluster
        /// </summary>
        private List<Level2Node>[] _nodes = new List<Level2Node>[4];

        /// <summary>
        /// Paths that go within the cluster, and the number of times they go within the cluster
        /// </summary>
        private Dictionary<AiPath, int> _paths = new Dictionary<AiPath, int>();
        
        /// <summary>
        /// Setup the cluster so that it knows its neighbors
        /// </summary>
        public void Setup(int x, int y, Cluster northEast, Cluster southEast, Cluster southWest, Cluster northWest)
        {
            //get the locations for the corners of the cluster
            int nearX = x * CLUSTER_SIZE;
            int farX = nearX + CLUSTER_SIZE - 1;
            if (farX >= GameState.Current.Locations.Size){ farX = GameState.Current.Locations.Size-1;}
            int nearY = y * CLUSTER_SIZE;
            int farY = nearY + CLUSTER_SIZE - 1;
            if (farY >= GameState.Current.Locations.Size){ farY = GameState.Current.Locations.Size-1;}

            //get the corners of the cluster
            _corner[(int)CardinalDirection.North] = GameState.Current.Locations.GetLocation(nearX, nearY);
            _corner[(int)CardinalDirection.East] = GameState.Current.Locations.GetLocation(farX, nearY);
            _corner[(int)CardinalDirection.South] = GameState.Current.Locations.GetLocation(farX, farY);
            _corner[(int)CardinalDirection.West] = GameState.Current.Locations.GetLocation(nearX, farY);

            //intitlize adjacent clusters
            _adjacent[(int)OrdinalDirection.NorthEast] = northEast;
            _adjacent[(int)OrdinalDirection.SouthEast] = southEast;
            _adjacent[(int)OrdinalDirection.SouthWest] = southWest;
            _adjacent[(int)OrdinalDirection.NorthWest] = northWest;

            //initilize node lists for each edge
            foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            {
                _nodes[(int)dir] = new List<Level2Node>();
            }            
        }

        /// <summary>
        /// Find the doors between this cluster and the clusters in each direction, and create (or recreate) the nodes for them
        /// (note this will also find the doors for each of the adjacent clusters on their side shared with this cluster)
        /// </summary>
        public void ReCreateCluster()
        {
            //delete all current nodes first so we dont find paths to ones that will be deleted
            foreach (OrdinalDirection side in DirectionUtils.AllOrdinalDirections)
            {
                DeleteNodesOnSide(side);
            }

            //any paths that are inside the cluster should be deleted because they may no longer be valid
            foreach (AiPath path in _paths.Keys.ToArray())
            {
                path.Delete();
            }
            _paths.Clear();

            //create doors for each side
            foreach (OrdinalDirection side in DirectionUtils.AllOrdinalDirections)
            {
                CreateDoorNodes(side);
            }
        }


        /// <summary>
        /// Make the path passed dependent on this cluster.
        /// If this cluster changes the path will be deleted.
        /// Note that if you make a path dependent on the cluster more than once you will have to
        /// BreakPathDependence the same number of times to remove the dependence.
        /// </summary>        
        public void MakePathDependent(AiPath path)
        {
            if (_paths.ContainsKey(path) == false)
            {
                _paths.Add(path, 1);
            }
            else
            {
                _paths[path] += 1;
            }
        }
        
        /// <summary>
        /// Break dependence of the path pass to this this cluster.        
        /// Note that if you make a path dependent on the cluster more than once you will have to
        /// BreakPathDependence the same number of times to remove the dependence.
        /// </summary>        
        public void BreakPathDependent(AiPath path)
        {
            if (_paths.ContainsKey(path))
            {            
                _paths[path] -= 1;
                if (_paths[path] == 0)
                {
                    _paths.Remove(path);
                }
            }
        }
        
        /// <summary>
        /// Find the doors between this cluster and the cluster in the ordinal direction passed, and create (or recreate) the nodes for them
        /// (note this will also find the doors for that cluster in the direction opposite of the direction passed)
        /// </summary>
        public void CreateDoorNodes(OrdinalDirection side)
        {
            OrdinalDirection oppositeDir = DirectionUtils.OppositeDirection(side);

            //get the cluster in that direction
            Cluster adjacent = _adjacent[(int)side];

            //if no adjacent then there are no doors on that side
            if (adjacent == null){ return; }
            
            //clear the current door nodes for this cluster in this direction, and the adjacent cluster in the opposite direction
            DeleteNodesOnSide(side);
            adjacent.DeleteNodesOnSide(DirectionUtils.OppositeDirection(side));
            
            //create list of locations where doors should be placed
            DoorLocationsList doorLocations = GetDoorLocations(side);
            
            //foreach set of door locations create the door node(s)
            CreateDoorNodesOnSide(side, doorLocations);
        }
        
        /// <summary>
        /// Delete the level 2 door nodes on the side passed
        /// </summary>
        private void DeleteNodesOnSide(OrdinalDirection side)
        {            
            foreach (Level2Node node in _nodes[(int)side])
            {
                node.Delete();
            }
            _nodes[(int)side].Clear();
        }
        
        /// <summary>
        /// Get a list of locations where doors should be placed on the side passed.
        /// Reaturns a list of lists of locations,  each list of locations is a range of uninterupted area where a door should be placed
        /// </summary>
        private DoorLocationsList GetDoorLocations(OrdinalDirection side)
        {
            //the list of door locations we will return
            DoorLocationsList toRet = new DoorLocationsList();
            DoorLocations currentDoor = new DoorLocations();

            //walk down the line between the two clusters, and find doors
            OrdinalDirection walkDirection = DirectionUtils.ClockwiseOne(side);
            Location locationOn = _corner[(int)DirectionUtils.CounterClockwiseOneCardinal(side)];
            Location end = _corner[(int)DirectionUtils.ClockwiseOneCardinal(side)];
            while (true)
            {
                //check if we can walk across the cluster boundary at this location                
                bool canCross = CanWalkUtil.CanWalkBetween(locationOn, side);

                //or if we can walk acorss the cluster boundary the other way (this can be caused by one way walking rules)
                if (canCross == false)
                {
                    canCross = CanWalkUtil.CanWalkBetween(locationOn.GetAdjacent(side), DirectionUtils.OppositeDirection(side));
                }

                if (canCross)
                {
                    //add the loction to the current door
                    currentDoor.Add(locationOn);
                }
                else if (currentDoor.Count > 0)
                {
                    //we have gotten to spot we can not cross, and we have a door
                    //then add to the door to the list, and start on a new one
                    toRet.Add(currentDoor);
                    currentDoor = new DoorLocations();
                }

                //we have reached the end so we are done
                if (locationOn == end)
                {
                    break;
                }

                //walk to the next location
                locationOn = locationOn.GetAdjacent(walkDirection);
            }

            //add final door to the list
            if (currentDoor.Count > 0)
            {
                toRet.Add(currentDoor);
            }

            //return the list
            return toRet;
        }
        
        /// <summary>
        /// Create door nodes for the side passed given the door locations passed
        /// </summary>
        private void CreateDoorNodesOnSide(OrdinalDirection side, DoorLocationsList doorsLocations)
        {
            foreach (DoorLocations doorLocations in doorsLocations)
            {
                //see if there are locations at the doors that are prefered
                bool nodeCreated = false;
                foreach (Location doorLocation in doorLocations)
                {
                    if (IsGoodDoorLocation(doorLocation, side))
                    {
                        CreateDoorNode(side, doorLocation);
                        nodeCreated = true;
                    }
                }

                //if node(s) were created at path points dont create anymore
                if (nodeCreated){ continue; }
                
                //create a node in the middle of the door (unless it has a high cost then create one near the middle)                
                int mid = doorLocations.Count / 2;
                Location doorLocationMid = doorLocations[mid];
                if (IsBadDoorLocation(doorLocationMid, side) && mid + 1 < doorLocations.Count)
                {
                    //we can just move to the next one over, because we never have two high cost paths right beside each other.
                    doorLocationMid = doorLocations[mid + 1];
                }
                
                //create a doot at the mid point
                CreateDoorNode(side, doorLocationMid);
            }
        }

        /// <summary>
        /// Return if this is a good location for a door (there is a path here or on the other side of the door)
        /// </summary>
        private bool IsGoodDoorLocation(Location doorLocation, OrdinalDirection side)
        {
            //if it bad it is not prefered
            if (IsBadDoorLocation(doorLocation, side)) { return false; }

            //look for DoWalk objects
            foreach (GameObject obj in doorLocation.AllObjects)
            {
                if (obj.PathEffect.HasFlag(ObjectsEffectOnPath.DoWalk) || obj.PathEffect.HasFlag(ObjectsEffectOnPath.DoWalkPlus)) { return true; }
            }
            foreach (GameObject obj in doorLocation.GetAdjacent(side).AllObjects)
            {
                if (obj.PathEffect.HasFlag(ObjectsEffectOnPath.DoWalk) || obj.PathEffect.HasFlag(ObjectsEffectOnPath.DoWalkPlus)) { return true; }
            }
            return false;
        }
        
        /// <summary>
        /// Return if this is a bad location for a door (there is a do not walk here or on the other side of the door)
        /// </summary>
        private bool IsBadDoorLocation(Location doorLocation, OrdinalDirection side)
        {
            //look for dont walk objects
            foreach (GameObject obj in doorLocation.AllObjects)
            {
                if (obj.PathEffect.HasFlag(ObjectsEffectOnPath.DontWalk)) { return true; }
            }
            foreach (GameObject obj in doorLocation.GetAdjacent(side).AllObjects)
            {
                if (obj.PathEffect.HasFlag(ObjectsEffectOnPath.DontWalk)) { return true; }
            }
            return false;
        }
                
        /// <summary>
        /// Create a node to be a door between this cluster, and the cluster on the sie passed.
        /// </summary>
        private void CreateDoorNode(OrdinalDirection side, Location doorLocation)
        {            
            OrdinalDirection oppositeSide = DirectionUtils.OppositeDirection(side);
            
            //get the cluster adjacent to me on that side
            Cluster adjacentCluster = _adjacent[(int)side];

            //get the location adjacent on that side            
            Location adjacentLocation = doorLocation.GetAdjacent(side);

            //get the cost for going through the door
            int cost;
            CanWalkUtil.CanWalkTo(doorLocation, side, out cost);

            //get the cost for going through the door the other way
            int adjacentCost;
            CanWalkUtil.CanWalkTo(adjacentLocation, oppositeSide, out adjacentCost);
            
            //the level 1 path to go through the door is just one node long (the adjacent location)
            LocationPathNode level1PathToAdjacent = new LocationPathNode();
            level1PathToAdjacent.Location = adjacentLocation;
            level1PathToAdjacent.Next = null;

            //the level 1 path to go the other way is similar
            LocationPathNode level1PathToMe = new LocationPathNode();
            level1PathToMe.Location = doorLocation;
            level1PathToMe.Next = null;
            
            //create my node
            Level2Node myNode = new Level2Node(this, doorLocation);

            //create the node in the adjacent cluster
            Level2Node adjacentNode = new Level2Node(adjacentCluster, adjacentLocation);

            //create my edge to the adjacent node
            myNode.AddEdge(new Level2Edge(adjacentNode, cost, level1PathToAdjacent));
            
            //create adjacent node edge to me
            adjacentNode.AddEdge(new Level2Edge(myNode, adjacentCost, level1PathToMe));

            //add the node to me
            this.AddNode(side, myNode);

            //add the adjecent node to my neighbor
            adjacentCluster.AddNode(oppositeSide, adjacentNode);
        }
        

        /// <summary>
        /// Add the node passed to the cluster on the side passed, and create all local (intra-cluster) edges 
        /// (inter-cluster edges should have already been created)
        /// </summary>
        private void AddNode(OrdinalDirection newNodeSide, Level2Node newNode)
        {
            //add edges between it and exsisting nodes
            AddNewNodeEdges(newNode, true);

            //add the new node to the list of nodes in the cluster
            _nodes[(int)newNodeSide].Add(newNode);
        }
        
        /// <summary>
        /// Add the node passed to the cluster as a temporary node.  
        /// The node will point to the other nodes in the cluster, but not be added to the list of nodes in the cluster.        
        /// </summary>
        public void AddTempNode(Level2Node tempNode)
        {
            //add edges between it and exsisting nodes
            AddNewNodeEdges(tempNode, false);
        }

        /// <summary>
        /// Add new edges between the node passed and other nodes in the cluster
        /// optionally add the new edges as temporary edges
        /// </summary>
        private void AddNewNodeEdges(Level2Node node, bool stayInCluster)
        {
            //if we need to stay with in this clist we will pas ourself to the find path, otherwise we pass null
            Cluster stayWithin = this;
            if (stayInCluster == false)
            {
                stayWithin = null;
            }

            //find path between this node and all other non-temp nodes currently in the cluster
            foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            {
                foreach (Level2Node currentNode in _nodes[(int)dir])
                {
                    //find if there is a path between the two nodes within the cluster
                    int cost;
                    LocationPathNode pathStart = PathFinder.FindDirectPath(node.Location, currentNode.Location, stayWithin, out cost);
                    if (cost != int.MaxValue)
                    {
                        //get the reverse path.  
                        int reverseCost;
                        LocationPathNode pathReverse = PathFinder.FindDirectPath(currentNode.Location, node.Location, stayWithin, out reverseCost);

                        //there is a path, create an edge from the node to the current and the opposite
                        Level2Edge nodeToCurrent = new Level2Edge(currentNode, cost, pathStart);
                        Level2Edge currentToNode = new Level2Edge(node, reverseCost, pathReverse);
                        
                        //add edges to the nodes
                        node.AddEdge(nodeToCurrent);
                        currentNode.AddEdge(currentToNode);
                    }
                }
            }
        }
        

        /// <summary>
        /// Return true if the location passed is insdie the cluster
        /// </summary>
        public bool IsLocationInCluster(Location location)
        {
            return (location.X >= _corner[(int)CardinalDirection.North].X &&
                    location.X <= _corner[(int)CardinalDirection.South].X &&
                    location.Y >= _corner[(int)CardinalDirection.North].Y &&
                    location.Y <= _corner[(int)CardinalDirection.South].Y);
        }



        public void DEBUG_MarkDoors()
        {
            foreach (OrdinalDirection side in DirectionUtils.AllOrdinalDirections)
            {
                foreach (Level2Node doorNode in _nodes[(int)side])
                {
                    doorNode.Location.Find<Land>().CornerToHighlight = LandCorner.Center;
                }
            }
        }


    }
}
