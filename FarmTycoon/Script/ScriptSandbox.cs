using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using System.Security.Policy;

namespace FarmTycoon
{
    [SecuritySafeCritical]
    public class ScriptSandbox : MarshalByRefObject
    {
        /// <summary>
        /// Script being run in the sandbox.
        /// </summary>
        private IScenarioScript _script = null;

        private static AppDomain _domain = null;

        public ScriptSandbox()
        {
        }

        [SecuritySafeCritical]
        public static ScriptSandbox Create()
        {
            AppDomainSetup setup = new AppDomainSetup()
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                ApplicationName = "Sandbox",
                DisallowBindingRedirects = true,
                DisallowCodeDownload = true,
                DisallowPublisherPolicy = true
            };

            PermissionSet permissions = new PermissionSet(PermissionState.None);
            permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            _domain = AppDomain.CreateDomain("Sandbox", null, setup, permissions,
                typeof(ScriptSandbox).Assembly.Evidence.GetHostEvidence<StrongName>());

            return (ScriptSandbox)Activator.CreateInstanceFrom(_domain, typeof(ScriptSandbox).Assembly.ManifestModule.FullyQualifiedName, typeof(ScriptSandbox).FullName).Unwrap();
        }

        [SecuritySafeCritical]
        public void LoadScriptObject(string assemblyPath)
        {
            new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, assemblyPath).Assert();
            var assembly = Assembly.LoadFile(assemblyPath);
            CodeAccessPermission.RevertAssert();

            Type type = assembly.GetType("FarmScript.Script");
            object instance = Activator.CreateInstance(type);

            _script = (IScenarioScript)instance;
        }

        [SecuritySafeCritical]
        public static void Unload()
        {
            if (_domain != null)
            {
                AppDomain.Unload(_domain);
                _domain = null;
            }
        }

        public void DoScript(int day, ScriptGameInterface game)
        {
            _script.DoScript(day, game);
        }

        public string SaveState()
        {
            return _script.SaveState();
        }

        public void LoadState(string state)
        {
            _script.LoadState(state);
        }
    }
}
