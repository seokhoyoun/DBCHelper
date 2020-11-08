using System;

namespace DBCHelper
{
    class Program
    {
        static void Main()
        {

            DBCParser parser = new DBCParser();

            parser.LoadFile();

            var messages = parser.MessageDictionary;

            var nodes = parser.NetworkNodeDictionary;

            var attribute = new AttributeCAN();

            
        }
    }
}
