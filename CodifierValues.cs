using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codifier.Values
{
    public static partial class CodifierValues
    {
        public static List<string> DecimalValues = new List<string>() {
          "0","1","2","3","4",
          "5","6","7","8","9" 
        };

        public static List<string> HexadecimalValues = new List<string>() {
          "0","1","2","3","4",
          "5","6","7","8","9",
          "a","b","c","d","e",
          "f","A","B","C","D",
          "E","F"          
        };

        public static List<string> IntegerTypeSuffixValues = new List<string>() {
          "u","l","U","L"  
        };

        public static List<string> RealTypeSuffixValues = new List<string>() {
          "f","d","m",
          "F","D","M"
        };

        public static List<string> EscapeCharacterValues = new List<string>() {
          "\'","\"","\\","0","a",
          "b","f","n","r","t",
          "u","U","x","v"
        };


    }
}
