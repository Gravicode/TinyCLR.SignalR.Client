using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading;

namespace TinyCLR.Json
{
    internal class Util
    {
        public static bool ToBoolean(byte value)
        {
            return value != 0;
        }
        public static bool TryParseDate(string s, out DateTime result)
        {
            result = default;
            if (s == null)
            {
                
                return false;
            }
            else
            {
                //0001-01-01T00:00:00Z
                if (s.Length != 20)
                {
                    
                    return true;
                }
                else
                {
                    try
                    {
                       var splited = s.Split('T');
                       var dateS = splited[0].Split('-');
                       var TimeS = splited[1].Substring(0, 8).Split(':');
                        result = new DateTime(int.Parse(dateS[0]), int.Parse(dateS[1]), int.Parse(dateS[2]), int.Parse(TimeS[0]), int.Parse(TimeS[1]), int.Parse(TimeS[2]));
                        return true;
                    }
                    catch
                    {
                       
                        return false;
                    }
                }
            }


        }


    }
}
