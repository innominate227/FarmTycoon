using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{    
    public partial class Worker
    {
        #region Member Vars
                        
        /// <summary>
        /// Vehicle the worker is using, or null for none
        /// </summary>
        private Equipment _vehicle = null;
                
        /// <summary>
        /// Tow the worker is using, or null for none
        /// </summary>
        private Equipment _tow = null;

        #endregion
        
        #region Properties
        
        /// <summary>
        /// The vehicle the worker is using, if worker is not on vehicle will return null
        /// </summary>
        public Equipment Vehicle
        {
            get { return _vehicle; }
        }

        /// <summary>
        /// Type of tow the worker is using, if worker is not using tow will return null
        /// </summary>
        public Equipment Tow
        {
            get { return _tow; }
        }

        #endregion

        #region Logic
        
        /// <summary>
        /// Apply the action or event type to the worker, and to the vehicle and tow if the worker is on a vehicle/tow
        /// </summary>
        public void ApplyActionOrEventToTraits(ActionOrEventType actionOrEventType)
        {
            bool onEquipmnet = (_vehicle != null);
            _traits.ApplyActionOrEventToTraits(actionOrEventType, onEquipmnet);
            if (_vehicle != null)
            {
                _vehicle.Traits.ApplyActionOrEventToTraits(actionOrEventType);
            }
            if (_tow != null)
            {
                _vehicle.Traits.ApplyActionOrEventToTraits(actionOrEventType);
            }
        }

        /// <summary>
        /// Set vehicle or tow that the worker should use
        /// </summary>
        public void GetOnEquipment(Equipment equipment)
        {
            if (equipment.EquipmentInfo.IsVehicle)
            {
                SetVehicle(equipment);
            }
            else
            {
                SetTow(equipment);
            }
        }

        /// <summary>
        /// Have the worker get of their vehicle, or their tow
        /// </summary>
        public void GetOffEquipment(Equipment equipment)
        {
            if (equipment.EquipmentInfo.IsVehicle)
            {
                SetVehicle(null);
            }
            else
            {
                SetTow(null);
            }
        }

        /// <summary>
        /// Set the type of vehicle the worker is using, or null for no vehicle
        /// </summary>
        private void SetVehicle(Equipment newVehicle)
        {
            //make sure we ether were on a vehicle, and are getting off (moving to null vehicle)
            //or that we were not on a vehicle and we are getting on one
            Debug.Assert((_vehicle == null && newVehicle != null) || (_vehicle != null && newVehicle == null));
                        
            if (_vehicle != null)
            {
                //the old vehicle should no longer update our textrue
                _vehicle.TextureManager.SetTileToUpdate(null);

                //old vehicle should no longer effect our delays
                _delays.RemoveEffectingDelaySet(_vehicle.Delays);
            }
                                    
            //set new tractor type
            _vehicle = newVehicle;

            //update inventory for the new tractor
            AdjustWorkerInventorySize();
                        
            if (newVehicle != null)
            {
                //stop using the normal texture manager
                _textureManager.SetTileToUpdate(null);

                //use the texture manager from the equipment to manage the workers texture
                newVehicle.TextureManager.SetTileToUpdate(_workerTile);
                newVehicle.TextureManager.Refresh();

                //new vehicle should now effect our delays
                _delays.AddEffectingDelaySet(newVehicle.Delays);
            }
            else
            {
                //use the workers normal texture manager to manage the texture
                _textureManager.SetTileToUpdate(_workerTile);
            }
            _workerPosition.UpdatePosition();
        }
        
        /// <summary>
        /// Set the type of tow the worker is using or null for no tow
        /// </summary>
        private void SetTow(Equipment newTow)
        {
            //make sure we ether we either had a tow, and are getting off (moving to null tow)
            //or that we did not have atoe and we are getting one
            Debug.Assert((_tow == null && newTow != null) || (_tow != null && newTow == null));

            
            if (_tow != null)
            {
                //the old tow should no longer update our textrue
                _tow.TextureManager.SetTileToUpdate(null);

                //old tow should no longer effect our delays
                _delays.RemoveEffectingDelaySet(_tow.Delays);
            }

            //set new tow type
            _tow = newTow;

            //update inventory for the new tow
            AdjustWorkerInventorySize();

            if (newTow != null)
            {
                //use the texture manager from the equipment to manage the workers texture
                _towTile.Hidden = false;
                newTow.TextureManager.SetTileToUpdate(_towTile);
                newTow.TextureManager.Refresh();

                //new tow should now effect our delays
                _delays.AddEffectingDelaySet(newTow.Delays);
            }
            else
            {
                //if no tow hide tow tile
                _towTile.Hidden = true;
                _towTile.Update();
            }
            _towPosition.UpdatePosition();

        }
        
        /// <summary>
        /// Update the workers inventory size based on equipmnet it has
        /// </summary>
        private void AdjustWorkerInventorySize()
        {
            //determine capacity multiplier, and capacity extra
            //the capacity extra will be the size of the equipment so that the equipment effectivly takes up no extra room in the inventory
            double capacityMultiplier = 1.0;
            int capacityExtra = 0;
            if (_vehicle != null)
            {
                capacityMultiplier *= _vehicle.EquipmentInfo.InventorySizeMultiplier;
                capacityExtra += _vehicle.EquipmentInfo.ItemTypeInfo.Size;
            }
            if (_tow != null)
            {
                capacityMultiplier *= _tow.EquipmentInfo.InventorySizeMultiplier;
                capacityExtra += _tow.EquipmentInfo.ItemTypeInfo.Size;
            }

            //update capcaity
            Inventory.AdjustCapacityMultiplier(capacityMultiplier, capacityExtra);
        }

        #endregion

        #region Save Load
        private void WriteStateV1Equipment(StateWriterV1 writer)
        {
            writer.WriteObject(_vehicle);
            writer.WriteObject(_tow);
        }

        private void ReadStateV1Equipment(StateReaderV1 reader)
		{
			_vehicle = reader.ReadObject<Equipment>();
			_tow = reader.ReadObject<Equipment>();
		}

        #endregion

    }
}
