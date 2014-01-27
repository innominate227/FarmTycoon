using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Security;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Plays through a scenario script, by listening for date changes and playing the correct section of the script.
    /// </summary>
    public class ScriptPlayer
    {

        #region Member Vars

        /// <summary>
        /// Script to play
        /// </summary>
        private ScriptSandbox _script;

        /// <summary>
        /// Interface into the game that the script uses to effect the game
        /// </summary>
        private ScriptGameInterface _gameInterface;

        /// <summary>
        /// Full text of the script
        /// </summary>
        private string _scriptText;

        #endregion

        #region Setup

        /// <summary>
        /// Create a script player to play the script passed.
        /// The script player needs a reference to the calandar that will tell it when a day has passed
        /// </summary>
        public ScriptPlayer(string scriptText)
        {
            _scriptText = scriptText;

            //create the assembly to use to run the script
            CreateScriptAssembly();

            //create interface for the script to talk back to the game through.
            _gameInterface = new ScriptGameInterface();

        }


        /// <summary>
        /// Handle the calandar date changed event that tell the script when to evaluate
        /// </summary>
        public void HandleDateChanged()
        {
            //the script needs to be consulted once each game day
            GameState.Current.Calandar.DateChanged += new Action(Calandar_DateChanged);
        }


        #endregion

        #region Properties
        
        /// <summary>
        /// Full text of the script.
        /// Not setting the text just effect what is saved, the script will not reload until the game file is reopened
        /// </summary>
        public string ScriptText
        {
            get { return _scriptText; }
            set { _scriptText = value; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Raised when the date changes
        /// </summary>
        private void Calandar_DateChanged()
        {
            int date = GameState.Current.Calandar.Date;

            //only run script if we have one, and are not editing the scenario
            if (_script != null && Program.Game.ScenarioEditMode == false)
            {
                try
                {
                    _script.DoScript(date, _gameInterface);                    
                }
                catch (Exception e)
                {
                    //an error occurred in the script, let the player know the scenario they are running has bugs
                    new MessageWindow("Farm Script Error", "The scenario you are playing has a bug in its farm script: \n\n\n" + e.Message, false, 200, 200);
                }

                AfterScript();
            }
        }

        /// <summary>
        /// Calls functions that need to happend after the script runs each day
        /// </summary>
        private void AfterScript()
        {
            GameState.Current.Prices.CheckForChangesAfterScript();
        }
                
        /// <summary>
        /// unload the loaded script if one is loaded
        /// </summary>
        public void UnloadScript()
        {            
            ScriptSandbox.Unload();            
        }

        private void CreateScriptAssembly()
        {
            //unload old script
            UnloadScript();
            
            //compile the script
            CSharpCodeProvider codeProvider = new CSharpCodeProvider(new Dictionary<String, String> { { "CompilerVersion", "v4.0" } });
            CompilerParameters compilerParams = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = false, };            
            compilerParams.ReferencedAssemblies.Add(typeof(ScriptPlayer).Assembly.Location);
            CompilerResults compilerResults = codeProvider.CompileAssemblyFromSource(compilerParams, _scriptText);

            string compuilerOutput = "";
            foreach (string str in compilerResults.Output)
            {
                compuilerOutput += str + "\r\n";
            }

            string scriptFile = Path.GetTempFileName() + "_FarmScript.dll";
            File.Copy(compilerResults.PathToAssembly, scriptFile, true);
            
            //get the ITycoonScript object inside a Sandbox that limits access to file system, internet, etc
            _script = ScriptSandbox.Create();
            _script.LoadScriptObject(Path.GetFullPath(scriptFile));
                        
        }

        /// <summary>
        /// Get the state of the script in a string
        /// </summary>
        public string GetScriptState()
        {
            return _script.SaveState();
        }

        /// <summary>
        /// Set the state of the script from a string
        /// </summary>
        public void SetScriptState(string state)
        {
            _script.LoadState(state);
        }


        #endregion


    }
}
