// See https://aka.ms/new-console-template for more information
using SeismicShadowZonesApp;

Console.WriteLine("Hello, World!");

//WorldMap.jpg
double lonDK = 12.58;
double latDK = 55.67;

var shadowZones = new ShadowZones(lonDK, latDK);

shadowZones.CreateShadowZoneBitmap(@"C:\temp\ShadowZones.jpg");

