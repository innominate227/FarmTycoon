using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// Settings for the game
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The full path to the settings file
        /// </summary>
        private string _settingsFile;

        /// <summary>
        /// True if saves should be compressed, both compressed and uncompressed saves will be readable
        /// </summary>
        private bool _compressSaves = false;
        
        /// <summary>
        /// True if frames per second should try to limit itself to 60
        /// </summary>
        private bool _limitTo60fps = false;
        
        /// <summary>
        /// True if the game should use multiple threads
        /// </summary>
        private bool _multiThread = true;

        /// <summary>
        /// Force OpenGl to use 1024x1024 as the texture size
        /// </summary>
        private bool _forceMinTextureSize = false;

        /// <summary>
        /// The editor to use for editing text files in scenario edit mode
        /// </summary>
        private string _textEditor = @"C:\Program Files (x86)\Notepad++\Notepad++.exe";
        
        /// <summary>
        /// Folder where game saves are located
        /// </summary>
        private string _savesFolder;

        /// <summary>
        /// Folder where scenarios are located
        /// </summary>
        private string _scenariosFolder;

        /// <summary>
        /// Folder where user scenarios are located
        /// </summary>
        private string _userScenariosFolder;

        /// <summary>
        /// Folder where game data is located
        /// </summary>
        private string _dataFolder;



        /// <summary>
        /// Create a new settings file, that reads, and writes settings to the file passed
        /// </summary>
        public Settings(string settingsFile)
        {
            _settingsFile = settingsFile;

            //if settings file exsist read it, otherwise write the settings file
            if (File.Exists(_settingsFile))
            {
                ReadSettings();
            }
            else
            {
                SetDefaults();
                WriteSettings();
            }            
        }

        private void SetDefaults()
        {
            //get path of the exe
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;

            _savesFolder = exeDir + "Saves";
            _scenariosFolder = exeDir + "Scenarios";
            _userScenariosFolder = exeDir + "UserScenarios";
            _dataFolder = exeDir + "Data";
        }



        /// <summary>
        /// True if saves should be compressed, both compressed and uncompressed saves will be readable
        /// </summary>
        public bool CompressSaves
        {
            get { return _compressSaves; }
            set 
            {
                _compressSaves = value;
                WriteSettings();
            }
        }

        /// <summary>
        /// True if frames per second should try to limit itself to 60
        /// </summary>
        public bool LimitTo60fps
        {
            get { return _limitTo60fps; }
            set 
            {
                _limitTo60fps = value; 
                WriteSettings();
            }
        }

        /// <summary>
        /// True if the game should use multiple threads
        /// </summary>
        public bool MultiThread
        {
            get { return _multiThread; }
            set 
            {
                _multiThread = value;
                WriteSettings();
            }
        }

        /// <summary>
        /// Force OpenGl to use 1024x1024 as the texture size
        /// </summary>
        public bool ForceMinTextureSize
        {
            get { return _forceMinTextureSize; }
            set
            {
                _forceMinTextureSize = value;
                WriteSettings();
            }
        }


        /// <summary>
        /// The editor to use for editing text files in scenario edit mode
        /// </summary>
        public string TextEditor
        {
            get { return _textEditor; }
            set
            {
                _textEditor = value;
                WriteSettings();
            }
        }


        /// <summary>
        /// Folder where game saves are located
        /// </summary>
        public string SavesFolder
        {
            get { return _savesFolder; }
            set
            {
                _savesFolder = value;
                WriteSettings();
            }
        }

        /// <summary>
        /// Folder where scenarios are located
        /// </summary>
        public string ScenariosFolder
        {
            get { return _scenariosFolder; }
            set
            {
                _scenariosFolder = value;
                WriteSettings();
            }
        }

        /// <summary>
        /// Folder where user scenarios are located
        /// </summary>
        public string UserScenariosFolder
        {
            get { return _userScenariosFolder; }
            set
            {
                _userScenariosFolder = value;
                WriteSettings();
            }
        }

        /// <summary>
        /// Folder where game data is located
        /// </summary>
        public string DataFolder
        {
            get { return _dataFolder; }
            set
            {
                _dataFolder = value;
                WriteSettings();
            }
        }






        /// <summary>
        /// Wrtie the current settings to the file
        /// </summary>
        private void WriteSettings()
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            
            XmlWriter xmlWriter = XmlWriter.Create(_settingsFile, xmlWriterSettings);
            xmlWriter.WriteStartElement("Settings");

            xmlWriter.WriteStartElement("Saves");
            xmlWriter.WriteValue(_savesFolder);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Scenarios");
            xmlWriter.WriteValue(_scenariosFolder);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("UserScenarios");
            xmlWriter.WriteValue(_userScenariosFolder);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Data");
            xmlWriter.WriteValue(_dataFolder);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CompressSaves");            
            xmlWriter.WriteValue(_compressSaves);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("LimitTo60fps");
            xmlWriter.WriteValue(_limitTo60fps);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("MultiThread");
            xmlWriter.WriteValue(_multiThread);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ForceMinTextureSize");
            xmlWriter.WriteValue(_forceMinTextureSize);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("TextEditor");
            xmlWriter.WriteValue(_textEditor);
            xmlWriter.WriteEndElement();
            
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }

        /// <summary>
        /// Read the current settings from the file
        /// </summary>
        private void ReadSettings()
        {
            //try and read settings from the file
            XmlReader xmlReader = null;
            try
            {
                xmlReader = XmlReader.Create(_settingsFile);

                xmlReader.ReadToFollowing("Saves");
                _savesFolder = xmlReader.ReadElementContentAsString();
                xmlReader.ReadToFollowing("Scenarios");
                _scenariosFolder = xmlReader.ReadElementContentAsString();
                xmlReader.ReadToFollowing("UserScenarios");
                _userScenariosFolder = xmlReader.ReadElementContentAsString();
                xmlReader.ReadToFollowing("Data");
                _dataFolder = xmlReader.ReadElementContentAsString();
                xmlReader.ReadToFollowing("CompressSaves");
                _compressSaves = xmlReader.ReadElementContentAsBoolean();
                xmlReader.ReadToFollowing("LimitTo60fps");
                _limitTo60fps = xmlReader.ReadElementContentAsBoolean();
                xmlReader.ReadToFollowing("MultiThread");
                _multiThread = xmlReader.ReadElementContentAsBoolean();
                xmlReader.ReadToFollowing("ForceMinTextureSize");
                _forceMinTextureSize = xmlReader.ReadElementContentAsBoolean();
                xmlReader.ReadToFollowing("TextEditor");
                _textEditor = xmlReader.ReadElementContentAsString();
            }
            catch 
            {
                //on error parsing just use defaults
            }
            finally
            {
                //any errors reading just close the read, and use the default values for thing that were not read
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }
        }

    }
}
