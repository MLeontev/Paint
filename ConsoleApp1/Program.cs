using System;
using System.Device.Location;

namespace GetLocationProperty
{
    class Program
    {
        static void Main(string[] args)
        {
            GetLocationProperty();

            Console.ReadKey();
        }

        static void GetLocationProperty()
        {
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();

            watcher.TryStart(false, TimeSpan.FromMilliseconds(1000));

            GeoCoordinate coord = watcher.Position.Location;
            while (coord.IsUnknown)
            {

            }
            Console.WriteLine(coord);

            //if (coord.IsUnknown != true)
            //{
            //    Console.WriteLine("Lat: {0}, Long: {1}",
            //        coord.Latitude,
            //        coord.Longitude);
            //}
            //else
            //{
            //    Console.WriteLine("Unknown latitude and longitude.");
            //}
        }
    }
}