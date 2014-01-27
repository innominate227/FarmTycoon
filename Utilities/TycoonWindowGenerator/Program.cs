using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TycoonWindowGenerator
{    
    public class Program
    {
        static int Main(string[] args)
        {
            try
            {

                string assembly = Path.GetFullPath(args[0]);
                string genFileName = args[1];

                Console.WriteLine(assembly);

                Assembly controlAssembly = System.Reflection.Assembly.LoadFile(assembly);

                
                StreamWriter writeFile = new StreamWriter(genFileName);

                //write header stuff
                writeFile.WriteLine("//This is an auto generated file to not modify this file!!!");
                writeFile.WriteLine();
                writeFile.WriteLine("using System;");
                writeFile.WriteLine("using TycoonGraphicsLib;");
                writeFile.WriteLine();
                writeFile.WriteLine("namespace FarmTycoon");
                writeFile.WriteLine("{");

                
                foreach (Type type in controlAssembly.GetTypes())
                {
                    if (type.BaseType == typeof(TycoonWindowGenerationLib.TycoonWindow_Form_Gen) || type.BaseType == typeof(TycoonWindowGenerationLib.TycoonPanel_UserControl_Gen))
                    {
                        string windowClass = CreateWindowClass(type);
                        writeFile.WriteLine(windowClass);
                        writeFile.WriteLine();
                        writeFile.WriteLine();
                        writeFile.WriteLine();
                    }
                }

                writeFile.WriteLine("}"); //end name space
                writeFile.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }
            return 0;
        }



        private static string CreateWindowClass(Type type)
        {
            //instansate an object of that type, and get all its fields
            object instance = type.GetConstructor(new Type[0]).Invoke(null);
            FieldInfo[] fields = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            //get the name of the window / panel
            string windowName = instance.GetType().Name;

            //mapping between objects and their names
            Dictionary<object, string> memberVariableNames = new Dictionary<object, string>();


            memberVariableNames.Add(instance, "this");

            
            //create a block with the properties for this window
            StringBuilder thisPropertiesBlock = new StringBuilder();
            foreach (PropertyInfo property in instance.GetType().GetProperties())
            {
                if (property.Name.StartsWith("Tycoon_"))
                {
                    string propertyName = property.Name.Replace("Tycoon_", "");
                    string propertyValue = GetValueAsString(property.GetValue(instance, null));
                    thisPropertiesBlock.AppendLine("\t\t\t" + "this." + propertyName + " = " + propertyValue + ";");
                }
            }


            //create a block with a memeber variable for each gui component
            StringBuilder memberVarsBlock = new StringBuilder();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.Name.Contains("Tycoon") || (field.FieldType.BaseType != null && field.FieldType.BaseType.Name.Contains("UserControl")))
                {
                    object windowControl = field.GetValue(instance);
                    if (windowControl != null)
                    {
                        string controlType = field.FieldType.Name.Replace("_Gen", "");
                        string controlName = (string)windowControl.GetType().GetProperty("Name").GetValue(windowControl, null);
                        memberVarsBlock.AppendLine("\t\t" + "private " + controlType + " " + controlName + ";");
                        memberVariableNames.Add(windowControl, controlName);
                    }
                }
            }


            
            //create a block to new each control
            StringBuilder memberDefsBlock = new StringBuilder();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.Name.Contains("Tycoon") || (field.FieldType.BaseType != null && field.FieldType.BaseType.Name.Contains("UserControl")))
                {
                    object windowControl = field.GetValue(instance);
                    if (windowControl != null)
                    {
                        string controlType = field.FieldType.Name.Replace("_Gen", "");
                        string controlName = (string)windowControl.GetType().GetProperty("Name").GetValue(windowControl, null);                        
                        memberDefsBlock.AppendLine("\t\t\t" + controlName + " = new " + controlType + "();");                        
                    }
                }
            }


            //create a block with a variable propeties for each gui component
            StringBuilder memberSetPropsBlock = new StringBuilder();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.Name.Contains("Tycoon") || (field.FieldType.BaseType != null && field.FieldType.BaseType.Name.Contains("UserControl")))
                {
                    object windowControl = field.GetValue(instance);
                    if (windowControl != null)
                    {
                        string controlType = field.FieldType.Name.Replace("_Gen", "");                    
                        string controlName = (string)windowControl.GetType().GetProperty("Name").GetValue(windowControl, null);
                        memberSetPropsBlock.AppendLine("\t\t\t");                        
                        foreach (PropertyInfo property in windowControl.GetType().GetProperties())
                        {
                            if (property.Name.StartsWith("Tycoon_"))
                            {
                                string propertyName = property.Name.Replace("Tycoon_", "");
                                string propertyValue = GetValueAsString(property.GetValue(windowControl, null));
                                memberSetPropsBlock.AppendLine("\t\t\t" + controlName + "." + propertyName + " = " + propertyValue + ";");
                            }
                        }
                                                   
                    }
                }
            }


            //create the children add block
            StringBuilder childrenAddBlock = new StringBuilder();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.Name.Contains("Tycoon") || (field.FieldType.BaseType != null && field.FieldType.BaseType.Name.Contains("UserControl")))
                {
                    object windowControl = field.GetValue(instance);
                    if (windowControl != null)
                    {
                        string controlType = field.FieldType.Name.Replace("_Gen", "");
                        string controlName = (string)windowControl.GetType().GetProperty("Name").GetValue(windowControl, null);
                        
                        object parent = windowControl.GetType().GetProperty("Parent").GetValue(windowControl, null);
                        string parentName = memberVariableNames[parent];
                        childrenAddBlock.AppendLine("\t\t\t" + parentName + ".AddChild(" + controlName + ");");
                    }
                }
            }


            StringBuilder classBuilder = new StringBuilder();
            
                        
            classBuilder.AppendLine("\tpublic partial class " + windowName);
            classBuilder.AppendLine("\t{");
            classBuilder.AppendLine("\t\t");
            classBuilder.AppendLine(memberVarsBlock.ToString());
            classBuilder.AppendLine("\t\t");
            classBuilder.AppendLine("\t\t" + "[System.CodeDom.Compiler.GeneratedCode(\"TycoonWindowGenerator\", \"1.0\")]");
            classBuilder.AppendLine("\t\t" + "public void InitializeComponent()");
            classBuilder.AppendLine("\t\t" + "{");
            classBuilder.AppendLine(memberDefsBlock.ToString());
            classBuilder.AppendLine("\t\t\t");
            classBuilder.AppendLine(thisPropertiesBlock.ToString());
            classBuilder.AppendLine("\t\t\t");
            classBuilder.AppendLine(memberSetPropsBlock.ToString());
            classBuilder.AppendLine("\t\t\t");
            classBuilder.AppendLine(childrenAddBlock.ToString());
            classBuilder.AppendLine("\t\t" + "}");
            classBuilder.AppendLine("\t\t");
            classBuilder.AppendLine("\t\t");
            classBuilder.AppendLine("\t}"); //end class
            

            return classBuilder.ToString();
        }


        /// <summary>
        /// Get a value as a string that should go in code
        /// </summary>
        private static string GetValueAsString(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else if (value.GetType() == typeof(List<string>))
            {
                StringBuilder ret = new StringBuilder();
                ret.Append("new System.Collections.Generic.List<string>(new string[]{");
                bool first = true;
                foreach (string str in (List<string>)value)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        ret.Append(", ");
                    }
                    ret.Append("\"" + str + "\"");                    
                }
                ret.Append("})");
                return ret.ToString();
            }
            else if (value.GetType() == typeof(bool))
            {
                return value.ToString().ToLower();
            }
            else if (value.GetType() == typeof(string))
            {
                return ("\"" + (string)value + "\"").Replace("\r\n","\\n");
            }
            else if (value.GetType() == typeof(int) || value.GetType() == typeof(double))
            {
                return value.ToString();
            }
            else if (value.GetType() == typeof(System.Drawing.Color))
            {
                System.Drawing.Color valueAsColor = (System.Drawing.Color)value;
                return "System.Drawing.Color.FromArgb(" + valueAsColor.A.ToString() + ", " + valueAsColor.R.ToString() + ", " + valueAsColor.G.ToString() + ", " + valueAsColor.B.ToString() + ")";
            }
            else if (value.GetType() == typeof(System.Drawing.Font))
            {
                System.Drawing.Font valueAsFont = (System.Drawing.Font)value;
                return "new System.Drawing.Font(\"" + valueAsFont.Name + "\", " + valueAsFont.Size.ToString() + "f, System.Drawing.FontStyle." + valueAsFont.Style.ToString() + ")";
            }
            else if (value.GetType() == typeof(System.Drawing.StringAlignment))
            {
                return "System.Drawing.StringAlignment." + value.ToString();
            }
            else
            {
                throw new Exception("Unrecognized value type");
            }
        }

    }
}
