using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SaveLoadGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string[] lines = textBox1.Text.Split('\n');

            List<Tuple<string, string>> members = new List<Tuple<string, string>>();

            
            foreach(string line in lines)
            {
                string[] tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);                
                if (tokens.Length < 3)
                {
                    continue;
                }

                string varType = tokens[1].Trim(';');
                string varName = tokens[2].Trim(';', '\r');
                if (varName.StartsWith("_"))
                {
                    members.Add(new Tuple<string, string>(varType, varName));
                }

            }

            textBox2.Text = "";

            textBox2.Text += "#region Save Load" + "\r\n";

            textBox2.Text += "\t\t" + "public override void WriteStateV1(StateWriterV1 writer)" + "\r\n";
            textBox2.Text += "\t\t" + "{" + "\r\n";
            textBox2.Text += "\t\t\t" + "base.WriteStateV1(writer);" + "\r\n";

            foreach (Tuple<string, string> member in members)
            {
                textBox2.Text += "\t\t\t" + "writer.Write";
                if (member.Item1 == "string" || member.Item1 == "int" || member.Item1 == "float" || member.Item1 == "double" || member.Item1 == "bool")
                {
                    textBox2.Text += member.Item1.Substring(0,1).ToUpper();
                    textBox2.Text += member.Item1.Substring(1);
                }
                else if (member.Item1.Contains("List"))
                {
                    textBox2.Text += "ObjectList<" + member.Item1.Replace("List<", "").Replace(">", "") + ">";
                }
                else if (member.Item1.Contains("[]"))
                {
                    textBox2.Text += "ObjectArray<" + member.Item1.Replace("[]", "") + ">";
                }
                else
                {
                    textBox2.Text += "Object";
                }

                textBox2.Text += "(" + member.Item2 + ");" + "\r\n";
            }

            textBox2.Text += "\t\t" + "}" + "\r\n";
            textBox2.Text += "\t\t" + "\r\n";



            textBox2.Text += "\t\t" + "public override void ReadStateV1(StateReaderV1 reader)" + "\r\n";
            textBox2.Text += "\t\t" + "{" + "\r\n";
            textBox2.Text += "\t\t\t" + "base.ReadStateV1(reader);" + "\r\n";

            foreach (Tuple<string, string> member in members)
            {

                textBox2.Text += "\t\t\t" + member.Item2 + " = reader.Read";

                if (member.Item1 == "string" || member.Item1 == "int" || member.Item1 == "float" || member.Item1 == "double" || member.Item1 == "bool")
                {
                    textBox2.Text += member.Item1.Substring(0, 1).ToUpper();
                    textBox2.Text += member.Item1.Substring(1);
                }
                else if (member.Item1.Contains("List"))
                {
                    textBox2.Text += "ObjectList<" + member.Item1.Replace("List<","").Replace(">", "") + ">";
                }
                else if (member.Item1.Contains("[]"))
                {
                    textBox2.Text += "ObjectArray<" + member.Item1.Replace("[]", "") + ">";
                }
                else
                {
                    textBox2.Text += "Object<" + member.Item1 + ">";
                }

                textBox2.Text += "();" + "\r\n";
            }

            textBox2.Text += "\t\t" + "}" + "\r\n";
            textBox2.Text += "\t\t" + "\r\n";



            textBox2.Text += "\t\t" + "public override void AfterReadStateV1()" + "\r\n";
            textBox2.Text += "\t\t" + "{" + "\r\n";
            textBox2.Text += "\t\t\t" + "base.AfterReadStateV1();" + "\r\n";
            textBox2.Text += "\t\t" + "}" + "\r\n";
            


            textBox2.Text += "\t\t" + "#endregion";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.SelectAll();
            Clipboard.SetText(textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}
