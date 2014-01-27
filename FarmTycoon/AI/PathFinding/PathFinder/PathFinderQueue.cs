using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGenerics.DataStructures.Queues;

namespace FarmTycoon
{
    
    /// <summary>
    /// Queue used for path finding.  T is the node we are finding paths between
    /// </summary>
    public class PathFinderQueue<T>
    {

        /// <summary>
        /// An item in the path finder Queue
        /// </summary>
        private class PathFinderQueueNode<U>
        {
            /// <summary>
            /// True if the queue item is not valid because a better item was added with a lower total cost
            /// </summary>
            public bool Invalidated = false;

            /// <summary>
            /// The node in the queue
            /// </summary>
            public U Node;

            /// <summary>
            /// The cost it took to move to the node
            /// </summary>
            public int MovmentCost;

            /// <summary>
            /// A* Heuristic for this node
            /// </summary>
            public int Heuristic;
            
            /// <summary>
            /// The prioirty of the item in the queue            
            /// </summary>
            public int Priority
            {
                get { return MovmentCost + Heuristic; }
            }
        }


        /// <summary>
        /// nodes currently in the path finder queue
        /// </summary>
        private Dictionary<T, PathFinderQueueNode<T>> _nodesInQueue = new Dictionary<T, PathFinderQueueNode<T>>();

        /// <summary>
        /// The path finder queue
        /// </summary>
        private PriorityQueue<PathFinderQueueNode<T>> _queue = new PriorityQueue<PathFinderQueueNode<T>>(PriorityQueueType.Minimum);

        /// <summary>
        /// Add a node to the path finder queue, if the cost to get to it is lower than the currently know cost to get to it.
        /// </summary>
        public bool Enqueue(T node, int cost, int heuristic)
        {
            if (_nodesInQueue.ContainsKey(node) == false)
            {
                //if not already in the queue add it
                PathFinderQueueNode<T> item = new PathFinderQueueNode<T>();
                item.Node = node;
                item.MovmentCost = cost;
                item.Heuristic = heuristic;
                item.Invalidated = false;
                _queue.Add(item, item.Priority);
                _nodesInQueue.Add(node, item);
                return true;
            }
            else
            {
                //check if the new way is shorter or not
                PathFinderQueueNode<T> oldItem = _nodesInQueue[node];
                //if the new way to get there is shorter then update the queu with the new way
                if (oldItem.MovmentCost > cost)
                {
                    //old item is not invalid
                    oldItem.Invalidated = true;

                    //create a new item and put it in the queue
                    PathFinderQueueNode<T> newItem = new PathFinderQueueNode<T>();
                    newItem.Node = node;
                    newItem.MovmentCost = cost;
                    newItem.Heuristic = heuristic;
                    newItem.Invalidated = false;
                    _queue.Add(newItem, cost);
                    _nodesInQueue[node] = newItem;
                    return true;
                }
                else
                {
                    //old item was better so dont add
                    return false;
                }
            }

            
        }

        /// <summary>
        /// dequeue the node with the top priority (lowest cost + heuristic), and return the movement cost to get to that node
        /// </summary>        
        public T Dequeue(out int movementCost)
        {
            //if nothing in the queue the return null
            if (_queue.Count == 0)
            {
                movementCost = int.MaxValue;
                return default(T);
            }

            //get an item from the queue
            PathFinderQueueNode<T> nextItem = _queue.Dequeue();

            //if the item is invalid 
            while (nextItem.Invalidated)
            {
                //see if theres another item to get form the queue, if not ret null
                if (_queue.Count == 0)
                {
                    movementCost = int.MaxValue;
                    return default(T);
                }

                //get an item from the queue
                nextItem = _queue.Dequeue();
            }
                        
            //return the item
            movementCost = nextItem.MovmentCost;
            return nextItem.Node;            
        }


    }
}

