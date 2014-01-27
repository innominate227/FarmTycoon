using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// Handler for when we have arrived at a destination
    /// </summary>    
    public delegate void ArrivedAtDestinationHandler(Location destination);

    /// <summary>
    /// Handler for when we have finished waiting
    /// </summary>
    public delegate void FinishedWaitingHandler(Location doneWaitingAt);

    /// <summary>
    /// Mover moves a object around the map.
    /// </summary>
    public class Mover
    {
        //base speed for movers when off / on the path
        public const double OFF_PATH_BASE_SPEED = 1.0 / 12.0; //One Tile every 2 game hours
        public const double ON_PATH_BASE_SPEED = 1.0 / 24.0; //One Tile every 1 game hours 
        public const double OFF_PATH_BASE_INTERVAL = OFF_PATH_BASE_SPEED / 16.0; //One Movement every 2/16 hours)
        public const double ON_PATH_BASE_INTERVAL = ON_PATH_BASE_SPEED / 16.0; //One Movement every 1/16 hours)

        /// <summary>
        /// Event raised when we arrive at a destination
        /// </summary>
        private ArrivedAtDestinationHandler m_arrivedAtDestination;

        /// <summary>
        /// Event raised when we have finished waiting
        /// </summary>
        private FinishedWaitingHandler m_finishedWaiting;
        
        
        /// <summary>
        /// Location the object is leaving
        /// </summary>
        private Location m_leaving;

        /// <summary>
        /// Location the man is going toward
        /// </summary>
        private Location m_going;
        
        /// <summary>
        /// The direction to the object is moving
        /// </summary>
        private OrdinalDirection m_direction;

        /// <summary>
        /// The detination the object is moving to
        /// </summary>
        private Location m_destination;
        
        /// <summary>
        /// Is the object currently waiting (not moving)
        /// </summary>
        private bool m_waiting = false;
        
        /// <summary>
        /// How long in game days the object waits between moves at its current speed
        /// </summary>
        private double m_movementNotificationInterval = OFF_PATH_BASE_INTERVAL;

        /// <summary>
        /// number between 1 and 16 that tells how close the object is to the tile its going to
        /// </summary>
        private int m_distToDest = 16;
        
        /// <summary>
        /// true if the object is unable to read the next desitnation
        /// </summary>
        private bool m_unableToReachDestination = false;

        /// <summary>
        /// The tile that should be moved
        /// TODO: replace this with MovedTileManager
        /// </summary>
        private Tile m_tileToMove;
        
        /// <summary>
        /// The notification the object moves on each notification
        /// </summary>
        private Notification m_notification;


        /// <summary>
        /// Create an object mover
        /// </summary>        
        public Mover()
        {
        }

        
        /// <summary>
        /// Setup the object mover with the location the object starts on
        /// </summary>        
        private void Setup(Location startLocation, Tile tileToMove)
        {
            m_tileToMove = tileToMove;

            //set the initial leaving and going
            m_leaving = startLocation.SouthEast;
            m_going = startLocation;
            m_distToDest = 16;
        }


        public void SetHandlers(ArrivedAtDestinationHandler arrivedAtDestination, FinishedWaitingHandler finishedWaiting)
        {
            m_arrivedAtDestination = arrivedAtDestination;
            m_finishedWaiting = finishedWaiting;
        }
        

        /// <summary>
        /// Start the object moving
        /// </summary>
        private void StartMoving()
        {
            m_notification = Program.GameThread.Clock.RegisterNotification(TimeNotification, m_movementNotificationInterval, false);

            //move once, so everything get set correctly
            Move();
        }

        /// <summary>
        /// Set the destination the object should move toward
        /// </summary>
        public void MoveToward(Location destination)
        {
            m_destination = destination;
        }

        /// <summary>
        /// Have the object wait for a certain period of time
        /// </summary>
        public void Wait(double waitTime)
        {
            Debug.Assert(waitTime > 0);
            m_waiting = true;
            m_notification = Program.GameThread.Clock.UpdateNotification(m_notification, waitTime, false);
        }

        /// <summary>
        /// If the object was waiting, have it abort that wait, and start moving again
        /// </summary>
        public void AbortWaiting()
        {
            if (m_waiting)
            {
                //stop waiting
                m_waiting = false;

                //set the notification interval back to the objects speed
                m_notification = Program.GameThread.Clock.UpdateNotification(m_notification, m_movementNotificationInterval, false);
            }
        }



        /// <summary>
        /// number between 1 and 16 that tells how close the man is to the tile its going to
        /// </summary>
        public int DistToDest
        {
            get { return m_distToDest; }
        }

        /// <summary>
        /// Location the object is going toward 
        /// (not nesisarly the final desitnation the object alwats moves from one location to an adajcent location.
        /// this is one of the two locations the worker is moving between the one its moving toward)
        /// </summary>
        public Location Going
        {
            get { return m_going; }
        }

        /// <summary>
        /// Location the object is leaving
        /// </summary>
        public Location Leaving
        {
            get { return m_leaving; }
        }
        
        /// <summary>
        /// The detination the object is moving to, this will be set to null once the object has arrived at the destination.
        /// </summary>        
        public Location Destination
        {
            get { return m_destination; }
        }



        /// <summary>
        /// Called by the clock when an amount of time has pased, based on speed that it needs to move.
        /// Or when waiting that the waiting time has passed
        /// </summary>
        private void TimeNotification()
        {
            //if we were waiting we, are not longer waiting, update the interval to the new speed
            if (m_waiting)
            {
                m_waiting = false;

                //call the done waiting handler                
                m_finishedWaiting(m_going);
                

                //set the notification interval back to the workers speed
                m_notification = Program.GameThread.Clock.UpdateNotification(m_notification, m_movementNotificationInterval, false);
            }
            else
            {
                Move();
            }
        }


        /// <summary>
        /// move the worker 1/16 of the way between two tiles
        /// </summary>
        private void Move()
        {
            //one more step toward the destination land
            m_distToDest += 1;
            if (m_distToDest > 16)
            {
                //if we reached the destination land then reset the count
                m_distToDest = 1;

                //determine the location we should go to next (we may also end up determining that we need to wait at the current land for some amount of time)
                DetermineNextLocation();

                //TODO: Event to the object to Update the location of the gameobject                

                //if we instead started waiting then dont move, also setting m_distToDest to 16 to force recalculating where to go
                //prevent a double move when we start moving again
                if (m_waiting)
                {
                    m_distToDest = 16;
                    return;
                }
            }
            
            //update the position of objects tile
            UpdateTilePosition();
        }



        /// <summary>
        /// Determine the next location to go to, and update leaving and going based on the next location.
        /// Or if the worker needs to wait at the current location, then dont update next location, and instead set waiting, or delayed flags
        /// </summary>
        private void DetermineNextLocation()
        {

            //if we got to the destination we need to raise the ArriveAtDestination event
            //the event may send us back to the same destination in which case we should raise the event again
            while (m_destination == m_going)
            {
                //no destination now
                m_destination = null;

                //call the arrived at destination handler
                m_arrivedAtDestination(m_going);
                                
                //when we arrive at a destination we might have started waiting, if we did then dont try and determine the next location
                if (m_waiting)
                {
                    return;
                }
            }

            //if a new destination has been set
            if (m_destination != null)
            {
                //if there is a destination, then the object should move toward that destination
                //find path to destination,  TODO: we only use the next step in the path can we make this more efficent due to that
                List<Location> pathToDestination = Program.Game.Tools.FastestPathFinder.FindPath(m_going, m_destination);

                //we should either not find a path, or find a path of at least two tile.
                //if we found a path of one tile lenght that means we were already at the desitination which we should have taken care of that case above.
                Debug.Assert(pathToDestination == null || pathToDestination.Count > 1);

                if (pathToDestination == null)
                {
                    //object cant reach the destination
                    m_unableToReachDestination = true;
                    Program.Game.IssueManager.ReportIssue(this, "Cant Reach", "Object cannot reach destination.");
                }
                else
                {
                    m_unableToReachDestination = false;
                    Program.Game.IssueManager.ClearIssue(this, "Cant Reach");

                    //we are leaving where we were going before, and update where we were going                                        
                    m_leaving = m_going;
                    m_going = pathToDestination[1];

                    //determine what direction the object is traveling
                    foreach (OrdinalDirection possibleDirection in DirectionUtils.AllOrdinalDirections)
                    {
                        if (m_leaving.GetAdjacent(possibleDirection) == m_going)
                        {
                            m_direction = possibleDirection;
                        }
                    }
                }
            }

            //if there is not a destination based on the task (or the worker can not reach it), then just have the object wait for a while, so they dont eat up the processor
            if (m_destination == null || m_unableToReachDestination)
            {
                Wait(0.1);
                return;
            }

            //what the speed used to be
            double oldMovementNotificationInterval = m_movementNotificationInterval;

            //set notification interval to correct value, based on if there is a road on both locations or not
            if ((m_going.Contains<Road>() == false || m_leaving.Contains<Road>() == false))
            {
                m_movementNotificationInterval = OFF_PATH_BASE_INTERVAL;// *VehicleType.MoveSpeedMultiplier * TowType.MoveSpeedMultiplier;
            }
            else
            {
                m_movementNotificationInterval = ON_PATH_BASE_INTERVAL;// *VehicleType.MoveSpeedMultiplier * TowType.MoveSpeedMultiplier;
            }

            //if speed changed update notification with new speed
            if (oldMovementNotificationInterval != m_movementNotificationInterval)
            {
                m_notification = Program.GameThread.Clock.UpdateNotification(m_notification, m_movementNotificationInterval, false);
            }
        }
                
        /// <summary>
        /// Update the tile for the worker / vehcile
        /// </summary>
        private void UpdateTilePosition()
        {
            //determine the locaiont the worker tiles should show up at
            float workerX = m_leaving.X + ((m_going.X - m_leaving.X) / 16.0f * m_distToDest);
            float workerY = m_leaving.Y + ((m_going.Y - m_leaving.Y) / 16.0f * m_distToDest);
            float workerZ = m_leaving.Z + ((m_going.Z - m_leaving.Z) / 16.0f * m_distToDest);

            //Program.Debug.SetDebug3("Worker Z: " + workerZ.ToString("R"));

            //determine if the worker is facing left or right
            string leftOrRight = "left";
            if (((m_direction == OrdinalDirection.NorthEast || m_direction == OrdinalDirection.SouthEast) && Program.UserInterface.Graphics.ViewRotation == TycoonGraphicsLib.ViewDirection.North) ||
                ((m_direction == OrdinalDirection.NorthWest || m_direction == OrdinalDirection.SouthWest) && Program.UserInterface.Graphics.ViewRotation == TycoonGraphicsLib.ViewDirection.South) ||
                ((m_direction == OrdinalDirection.SouthWest || m_direction == OrdinalDirection.SouthEast) && Program.UserInterface.Graphics.ViewRotation == TycoonGraphicsLib.ViewDirection.East) ||
                ((m_direction == OrdinalDirection.NorthEast || m_direction == OrdinalDirection.NorthEast) && Program.UserInterface.Graphics.ViewRotation == TycoonGraphicsLib.ViewDirection.West))
            {
                leftOrRight = "right";
            }

            //set rendering position for the tile
            m_tileToMove.RenderingInfo.X = workerX;
            m_tileToMove.RenderingInfo.Y = workerY;
            m_tileToMove.RenderingInfo.Z = workerZ;
            //m_tile.RenderingInfo.TextureQuartet = m_vehicleType.Texture + "_" + leftOrRight;

            //set position info
            m_tileToMove.TilePosition.Y = workerY;
            m_tileToMove.TilePosition.X = workerX;
            if (m_leaving.Z > m_going.Z)
            {
                m_tileToMove.TilePosition.ZMin = m_leaving.Z + 0.05f;
                m_tileToMove.TilePosition.ZMax = m_leaving.Z + 3.00f;
            }
            else
            {
                m_tileToMove.TilePosition.ZMin = m_going.Z + 0.05f;
                m_tileToMove.TilePosition.ZMax = m_going.Z + 3.00f;
            }

            m_tileToMove.TilePosition.YMax = workerY + 0.55f;
            m_tileToMove.TilePosition.YMin = workerY - 0.55f;

            m_tileToMove.TilePosition.XMax = workerX + 0.55f;
            m_tileToMove.TilePosition.XMin = workerX - 0.55f;


            m_tileToMove.Update();
        }




        /// <summary>
        /// Called right before the worker is deleted
        /// </summary>
        private void DeleteNotification()
        {
            //we only regiter for notification once the worker is actually placed so this may be null
            if (m_notification != null)
            {
                Program.GameThread.Clock.RemoveNotification(m_notification);
            }
        }



        private void WriteMovementState(ObjectState state)
        {
            state.SetValue("TowLocationLeaving", m_towLeavingLocation);
            state.SetValue("LocationLeaving", m_vehicleLeavingTowGoingLocation);
            state.SetValue("LocationGoing", m_vehicleGoingLocation);
            state.SetValue("DirectionToDest", m_directionToDest);
            state.SetValue("TowDirectionToDest", m_towDirectionToDest);
            state.SetValue("Waiting", m_waiting);
            state.SetValue("Delayed", m_dealyed);
            state.SetValue("MovementNotificationInterval", m_movementNotificationInterval);
            state.SetValue("DistToDest", m_distToDest);
            state.SetValue("UnableToReachDestination", m_unableToReachDestination);

            Program.GameThread.Clock.WriteNotificationState(m_notification, state);
        }

        private void ReadMovementState(ObjectState state)
        {
            m_towLeavingLocation = state.GetValue<Location>("TowLocationLeaving");
            m_vehicleLeavingTowGoingLocation = state.GetValue<Location>("LocationLeaving");
            m_vehicleGoingLocation = state.GetValue<Location>("LocationGoing");
            m_directionToDest = state.GetValue<OrdinalDirection>("DirectionToDest");
            m_towDirectionToDest = state.GetValue<OrdinalDirection>("TowDirectionToDest");
            m_waiting = state.GetValue<bool>("Waiting");
            m_dealyed = state.GetValue<bool>("Delayed");
            m_movementNotificationInterval = state.GetValue<double>("MovementNotificationInterval");
            m_distToDest = state.GetValue<int>("DistToDest");
            m_unableToReachDestination = state.GetValue<bool>("UnableToReachDestination");

            //read in the notification 
            m_notification = Program.GameThread.Clock.ReadNotificationState(TimeNotification, state);


        }

    }
}

