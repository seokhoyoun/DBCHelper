using System;

namespace DBCHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            DBCParser parser = new DBCParser();

            parser.LoadFile();

            var messages = parser.MessageDictionary;

            var nodes = parser.NetworkNodeDictionary;

            var attribute = new CANAttribute();

            
        }
    }
}
