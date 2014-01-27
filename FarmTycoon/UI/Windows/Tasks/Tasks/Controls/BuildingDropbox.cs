using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    public partial class BuildingDropbox : TycoonPanel
    {
        public event Action LocationChanged;

        /// <summary>
        /// Mapping form a building name to the building
        /// </summary>
        private Dictionary<string, IStorageBuilding> _nameToBuilding = new Dictionary<string, IStorageBuilding>();

        /// <summary>
        /// Mapping form a building to the building name
        /// </summary>
        private Dictionary<IStorageBuilding, string> _buildingToName = new Dictionary<IStorageBuilding, string>();

        public BuildingDropbox()
        {
            //intilize
            InitializeComponent();
             
        }

        public void Setup(GameObjectPredicate<IStorageBuilding> optionsPredicate)
        {
            takeToDropbox.Items.Clear();
            takeToDropbox.Items.Add("Nearest");
            takeToDropbox.Text = "Nearest";
            _nameToBuilding.Add("Nearest", null);
            foreach (IStorageBuilding storageBuilding in GameState.Current.MasterObjectList.FindAll<IStorageBuilding>())
            {
                //dont allow taking to places that done meet the options predicate
                if (optionsPredicate != null && optionsPredicate(storageBuilding) == false) { continue; }

                //dont show the delivery area in the drop box
                if (storageBuilding is DeliveryArea) { continue; }

                //get the name for the building
                string buildingName = storageBuilding.Name;

                //make sure the name is unique
                if (_nameToBuilding.ContainsKey(buildingName))
                {
                    //if not add numbers to the end until it is
                    int buildingNameExtra = 2;
                    while (_nameToBuilding.ContainsKey(buildingName + "(" + buildingNameExtra.ToString() + ")"))
                    {
                        buildingNameExtra++;
                    }
                    buildingName = buildingName + "(" + buildingNameExtra.ToString() + ")";
                }

                //add to the dropbox
                takeToDropbox.Items.Add(buildingName);

                //add  to the mapping
                _nameToBuilding.Add(buildingName, storageBuilding);
                _buildingToName.Add(storageBuilding, buildingName);
            }

            //raise event if selected location chanes
            takeToDropbox.TextChanged += new Action<TycoonControl>(delegate
            {
                IStorageBuilding newLoc = _nameToBuilding[takeToDropbox.Text];
                if (LocationChanged != null)
                {
                    LocationChanged();
                }
            });
            
        }

        public IStorageBuilding SelectedLocation
        {
            get 
            {
                return _nameToBuilding[takeToDropbox.Text]; 
            }
            set 
            {
                if (value == null)
                {
                    takeToDropbox.Text = "Nearest";
                }
                else
                {
                    takeToDropbox.Text = _buildingToName[value];
                }
            }
        }


    }
}
