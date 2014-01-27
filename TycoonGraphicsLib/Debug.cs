using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{
    public class GraphicsDebug
    {
        public static event Action ValueChanged;


        private static string _debug1;
        private static string _debug2;
        private static string _debug3;
        private static string _debug4;
        private static string _debug5;
        private static string _debug6;
        private static string _debug7;
        private static string _debug8;
        private static string _debug9;
        private static string _debug10;
        private static string _debug11;
        private static string _debug12;



        public static string Debug1
        {
            get { return _debug1; }
            set { _debug1 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug2
        {
            get { return _debug2; }
            set { _debug2 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug3
        {
            get { return _debug3; }
            set { _debug3 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug4
        {
            get { return _debug4; }
            set { _debug4 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug5
        {
            get { return _debug5; }
            set { _debug5 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug6
        {
            get { return _debug6; }
            set { _debug6 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug7
        {
            get { return _debug7; }
            set { _debug7 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug8
        {
            get { return _debug8; }
            set { _debug8 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug9
        {
            get { return _debug9; }
            set { _debug9 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug10
        {
            get { return _debug10; }
            set { _debug10 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug11
        {
            get { return _debug11; }
            set { _debug11 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
        public static string Debug12
        {
            get { return _debug12; }
            set { _debug12 = value; if (ValueChanged != null) { ValueChanged(); } }
        }
    }
}
