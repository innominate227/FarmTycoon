using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Manage texture for a gameobject, change texture based on conditions and events defined in the config file
    /// </summary>
    public class TextureManager : ISavable
    {
        #region Member Vars

        /// <summary>
        /// Info about the textures we can show
        /// </summary>
        private TexturesInfoSet _texturesInfoSet;

        /// <summary>
        /// GameTile to update        
        /// </summary>
        private GameTile _tile;
        
        /// <summary>
        /// The object that owns this texture manager
        /// </summary>
        private IGameObject _gameObject;
        
        /// <summary>
        /// Notification for checking for conditional texture changes
        /// </summary>
        private Notification _conditionalCheckNotification;

        /// <summary>
        /// Notification for updating the textures frame
        /// </summary>
        private Notification _frameNotification;

        /// <summary>
        /// If we are currently showing a temp texture (for an action) then this is that texture,
        /// if not this an empty string
        /// </summary>
        private string _actionTextureShowing = "";

        /// <summary>
        /// Set to true if we are currently register for the inventory changed event on the gameobject
        /// </summary>
        private bool _registeredForInventoryEvent = false;

        /// <summary>
        /// The frame number we are on
        /// </summary>
        private int _frameOn = 0;

        /// <summary>
        /// The max number of frames
        /// </summary>
        private int _frameCount = 0;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new TextureManager call Setup or ReadState before using
        /// </summary>
        public TextureManager()
        {
        }

        /// <summary>
        /// setup a new TextureManager
        /// </summary>
        public void Setup(ITexturesInfo texturesInfo, IGameObject gameObject)
        {
            _gameObject = gameObject;
            _texturesInfoSet = texturesInfo.Textures;
        }
        
        /// <summary>
        /// Sets the GameTile that will be updated by the TextureManager.
        /// Pass null to stop updating the tiles texture
        /// NOTE: GameObjects using texture manager will need to call this after a load so the TextureManager can get a pointer to the texture to update.
        /// </summary>
        public void SetTileToUpdate(GameTile tile)
        {
            _tile = tile;
            CreateConditionCheckTimerIfNeeded();
            HandelInventoryChangeIfNeeded();
        }

        /// <summary>
        /// Create the timer to check the texture conditions at an interval (if nessisary)
        /// Or if its no lonfer nessisary then get rid of it.
        /// </summary>
        private void CreateConditionCheckTimerIfNeeded()
        {
            //we should actually have a tile to update, and it should have multiple textures based on conditions,
            //or else to condition timer is unnessisary
            bool needConditionCheckTimer = false;
            if (_tile != null && _gameObject is IHasTraits)
            {
                //if there are any conditions that are based of traits or random numbers, we need to setup a condition check timer
                foreach (TextureInfo textureInfo in _texturesInfoSet.Textures)
                {
                    foreach (ConditionInfo condition in textureInfo.Conditions)
                    {
                        if (condition is TraitConditionInfo || condition is RandomConditionInfo)
                        {
                            needConditionCheckTimer = true;
                        }
                    }
                }
            }

            if (needConditionCheckTimer && _conditionalCheckNotification == null)
            {
                //register for check conditions notification
                _conditionalCheckNotification = Program.GameThread.Clock.RegisterNotification(Refresh, 1.0, true);
            }
            else if (needConditionCheckTimer == false && _conditionalCheckNotification != null)
            {
                //we no longer need the check conditions notification
                Program.GameThread.Clock.RemoveNotification(_conditionalCheckNotification);
            }
        }
        
        /// <summary>
        /// Create or delete the frame notification timer if needed, and sets the frame on and frame count
        /// </summary>
        private void SetupFrame(int frameCount, double frameRate, bool resetFrameOn)
        {
            _frameCount = frameCount;

            if (resetFrameOn || _frameOn >= _frameCount)
            {
                _frameOn = 0;
            }

            //need a timer if we have at least 2 frames
            bool needFrameTimer = (frameCount > 1);

            //unsubscribe the old notification
            if (_frameNotification != null)
            {
                Program.GameThread.Clock.RemoveNotification(_frameNotification);
            }

            //subscribe again at new rate if needed
            if (needFrameTimer)
            {
                _frameNotification = Program.GameThread.Clock.RegisterNotification(NextFrame, frameRate, true);
                _tile.Frame = _frameOn.ToString();  
            }
            else
            {
                _tile.Frame = "";
            }
        }

        /// <summary>
        /// Register the handler to check the invenotry of the game object (if nessisary)
        /// Or if its no lonfer nessisary then remove the handler.
        /// </summary>
        private void HandelInventoryChangeIfNeeded()
        {
            //we should actually have a texture to update, and it should have multiple textures based on conditions,
            //or else to condition timer is unnessisary
            bool needInventoryChangedEventHandler = false;
            if (_tile != null && _gameObject is IHasInventory)
            {
                //if there are any conditions that are based off inventory, we will need to event handler
                foreach (TextureInfo textureInfo in _texturesInfoSet.Textures)
                {
                    foreach (ConditionInfo condition in textureInfo.Conditions)
                    {
                        if (condition is InventoryConditionInfo)
                        {
                            needInventoryChangedEventHandler = true;
                        }
                    }
                }
            }

            if (needInventoryChangedEventHandler && _registeredForInventoryEvent == false)
            {
                //register for the inventory changed event
                (_gameObject as IHasInventory).Inventory.UnderlyingList.ListChanged += new Action(Refresh);
                _registeredForInventoryEvent = true;
            }
            else if (needInventoryChangedEventHandler == false && _registeredForInventoryEvent)
            {
                //unregister for the inventory changed event
                (_gameObject as IHasInventory).Inventory.UnderlyingList.ListChanged -= new Action(Refresh);
                _registeredForInventoryEvent = false;
            }

        }
                
        /// <summary>
        /// Delete the texture manager        
        /// </summary>
        public void Delete()
        {
            if (_conditionalCheckNotification != null)
            {
                Program.GameThread.Clock.RemoveNotification(_conditionalCheckNotification);
            }
            if (_frameNotification != null)
            {
                Program.GameThread.Clock.RemoveNotification(_frameNotification);
            }
        }
        
        #endregion

        #region Logic

        /// <summary>
        /// Update the tile to show the next frame
        /// </summary>
        private void NextFrame()
        {
            _frameOn++;
            if (_frameOn >= _frameCount)
            {
                _frameOn = 0;
            }

            _tile.Frame = _frameOn.ToString();
            _tile.Update();
        }

        /// <summary>
        /// Show the correct texture based on conditions defined in the farmData xml.
        /// Or if a temp texture is showing show that
        /// </summary>
        public void Refresh()
        {
            if (_actionTextureShowing != "")
            {
                UpdateTexture(_actionTextureShowing);
            }
            else
            {
                //find a texture whos conditions we meet and update to show that texture
                foreach (TextureInfo textureInfo in _texturesInfoSet.Textures)
                {
                    bool textureConditionsMet = CheckTextureInfoConditions(textureInfo);
                    if (textureConditionsMet)
                    {

                        //create the animation timer if needed
                        SetupFrame(textureInfo.Frames, textureInfo.FrameRate, false);

                        UpdateTexture(textureInfo.Texture);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Check if the conditions are met for a texture info
        /// </summary>
        private bool CheckTextureInfoConditions(TextureInfo textureInfo)
        {
            foreach (ConditionInfo condition in textureInfo.Conditions)
            {
                //if the condition is not met return false
                if (condition.ConditionMet(_gameObject) == false)
                {
                    return false;
                }
            }

            //all conditions met
            return true;
        }
        
        /// <summary>
        /// Update the texture to be the string passed.
        /// Properly set the texture on the tile.
        /// </summary>
        private void UpdateTexture(string textureName)
        {
            if (_tile != null && _tile.Texture != textureName)
            {
                //set the teture for the tile
                _tile.Texture = textureName;
                _tile.Update();
            }
        }
        
        /// <summary>
        /// Use the proper temp texture for when the action passed is done
        /// Does nothing is no temp texture set for the action passed
        /// </summary>
        public void SetTextureForActionOrEvent(ActionOrEventType actionOrEventType)
        {
            //check that we have a temp texture for that action, if not do nothing
            if (_texturesInfoSet.TempTextures.ContainsKey(actionOrEventType) == false)
            {
                return;
            }

            //update to use that texture, remeber the temp texture we are showing
            TempTextureInfo tempTexture = _texturesInfoSet.TempTextures[actionOrEventType];
            _actionTextureShowing = tempTexture.Texture;
            
            //create the animation timer if needed
            SetupFrame(tempTexture.Frames, tempTexture.FrameRate, true);

            //update the texture
            UpdateTexture(_actionTextureShowing);
        }
        
        /// <summary>
        /// Stop showing an Action Or Event texture
        /// </summary>
        public void ClearTextureForActionOrEvent()
        {
            //no longer showing the temp texture            
            _actionTextureShowing = "";

            //refresh to go back to the correct normal texture
            Refresh();
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteInfo(_texturesInfoSet.InfoSetOwner);
            writer.WriteObject(_gameObject);
            writer.WriteNotification(_conditionalCheckNotification);
            writer.WriteNotification(_frameNotification);
            writer.WriteString(_actionTextureShowing);
            writer.WriteBool(_registeredForInventoryEvent);
            writer.WriteInt(_frameCount);
            writer.WriteInt(_frameOn);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _texturesInfoSet = reader.ReadInfo<ITexturesInfo>().Textures;
            _gameObject = reader.ReadObject<IGameObject>();
            _conditionalCheckNotification = reader.ReadNotification(Refresh);
            _frameNotification = reader.ReadNotification(Refresh);
            _actionTextureShowing = reader.ReadString();
            _registeredForInventoryEvent = reader.ReadBool();
            _frameCount = reader.ReadInt();
            _frameOn = reader.ReadInt();
        }

        public void AfterReadStateV1()
        {
            //if we were registered for the inventory changed event when we saved, register again
            if (_registeredForInventoryEvent)
            {
                (_gameObject as IHasInventory).Inventory.UnderlyingList.ListChanged += new Action(Refresh);
            }
        }
        #endregion

    }
}
