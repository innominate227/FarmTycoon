using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{


    public partial class Animal
    {
        #region Member Vars

        /// <summary>
        /// Notification called everytime a day has passed
        /// </summary>
        private Notification _dayPassedNotification;

        /// <summary>
        /// Traits for the animal
        /// </summary>
        private TraitSet _traits;

        /// <summary>
        /// Quality for the animal
        /// </summary>
        private Quality _quality;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Setup the quality of an animal
        /// </summary>
        private void SetupQuality()
        {
            _traits = new TraitSet();
            _traits.Setup(_animalInfo);

            _quality = new Quality();
            _quality.Setup(_traits, _animalItemType);
                        
            //create day passed handler for the animal
            _dayPassedNotification = Program.GameThread.Clock.RegisterNotification(DayPassed, 1.0, true);            
        }

        /// <summary>
        /// Called when the animal is deleted
        /// </summary>
        private void DeleteQuality()
        {
            _traits.Delete();
            _quality.Delete();
            Program.GameThread.Clock.RemoveNotification(_dayPassedNotification);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The quality for the animal
        /// </summary>
        public IQuality Quality
        {
            get { return _quality; }
        }

        /// <summary>
        /// The traits of the animal
        /// </summary>
        public TraitSet Traits
        {
            get { return _traits; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Called when a day has passed
        /// </summary>
        private void DayPassed()
        {
            //apply the animal to the land in its pasture
            if (_pastrue != null)
            {
                foreach (Land land in _pastrue.OrderedLand)
                {
                    land.Traits.ApplyItemToTraits(_animalItemType);
                }
            }
        }

        #endregion

        #region Save Load
        private void WriteStateV1Quality(StateWriterV1 writer)
        {
            writer.WriteNotification(_dayPassedNotification);
            writer.WriteObject(_traits);
            writer.WriteObject(_quality);
        }

        private void ReadStateV1Quality(StateReaderV1 reader)
        {
            _dayPassedNotification = reader.ReadNotification(DayPassed);
            _traits = reader.ReadObject<TraitSet>();
            _quality = reader.ReadObject<Quality>();
        }
        #endregion
   
    }
}
