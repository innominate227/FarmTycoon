using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public static class ScriptUtils
    {

        public static string[] Split2(this string str, char delimiter, char escape)
        {
            //do a normal split
            string[] normalTokens = str.Split(delimiter);

            //list to return
            List<string> fixedTokens = new List<string>();

            //create a string 
            string currentToken = "";
            foreach (string normalToken in normalTokens)
            {
                currentToken += normalToken;

                //determine the number of trailing backslashes. starting with the last character increase numberOfTailingEscapes until we reach a non escape or we count the whole string
                int numberOfTailingEscapes = 0;
                while (currentToken.Length - numberOfTailingEscapes - 1 > 0 && currentToken[currentToken.Length - numberOfTailingEscapes - 1] == '\\')
                {
                    numberOfTailingEscapes++;
                }

                //if the number of trailing escapes is even we had a non escaped delimeter
                //Exmaple  "AAAA\)" has an escaped delimiter. "AAAA\\)" does not the backslahes is delimiting the other backslash. "AAAA\\\)" Does because the third bachslash. "AAAA\\\\)" does not two delimited backslashes.
                if (numberOfTailingEscapes % 2 == 0)
                {
                    fixedTokens.Add(currentToken);
                    currentToken = "";
                }
                else
                {
                    //is was escaped.  It got removed in the split so add it back in there
                    currentToken += delimiter;
                }
            }

            //return the fixed split list
            return fixedTokens.ToArray();
        }




        public static string ReadUntilCloseBrace(ref int lineNum, string[] lines)
        {
            string ret = "";
            int braceCount = 0;
            while (true)
            {
                //if the line contains a brace keep trac of how many brace in we are
                if (lines[lineNum].Trim().StartsWith("{"))
                {
                    braceCount++;
                }
                else if (lines[lineNum].Trim().StartsWith("}"))
                {
                    braceCount--;
                }

                //add line to our return section
                ret += "\r\n" + lines[lineNum].Trim();
                
                //if brace count is 0 were done
                if (braceCount == 0) { break; }

                //read next line next time
                lineNum++;
            }
            return ret;
        }

    }
}
