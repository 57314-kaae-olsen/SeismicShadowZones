using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using DHI.Projections;
using System.Numerics;

namespace SeismicShadowZonesApp
{
    internal class ShadowZones
    {
        private readonly double _yourLon;
        private readonly double _yourLat;
        public ShadowZones(double yourLon, double yourLat)
        {
            _yourLon = yourLon;
            _yourLat = yourLat;
        }

        public void CreateShadowZoneBitmap(string shadowZonesFilename)
        {
            // https://crs-explorer.proj.org/?ignoreWorld=false&allowDeprecated=false&authorities=ESRI&activeTypes=PROJECTED_CRS&map=osm
            // https://crs-explorer.proj.org/wkt1/ESRI/54004.txt
            DHI.Projections.MapProjection mapProj = new MapProjection(@"PROJCS[""World_Mercator"", GEOGCS[""WGS 84"",DATUM[""WGS_1984"",SPHEROID[""WGS 84"",6378137,298.257223563,AUTHORITY[""EPSG"",""7030""]],AUTHORITY[""EPSG"",""6326""]],PRIMEM[""Greenwich"",0,AUTHORITY[""EPSG"",""8901""]],UNIT[""degree"",0.0174532925199433,AUTHORITY[""EPSG"",""9122""]],AUTHORITY[""EPSG"",""4326""]],PROJECTION[""Mercator_2SP""],PARAMETER[""standard_parallel_1"",0],PARAMETER[""central_meridian"",0],PARAMETER[""false_easting"",0],PARAMETER[""false_northing"",0],UNIT[""metre"",1,AUTHORITY[""EPSG"",""9001""]],AXIS[""Easting"",EAST],AXIS[""Northing"",NORTH],AUTHORITY[""ESRI"",""54004""]]");//@"GEOGCS[""GCS_WGS_84_longitude-latitude-height"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433],LINUNIT[""Meter"",1.0]]");
            mapProj.Geo2Proj(-180,  60, out double nw_easting, out double nw_northing);
            mapProj.Geo2Proj( 180,  60, out double ne_easting, out double ne_northing);
            mapProj.Geo2Proj(-180, -60, out double sw_easting, out double sw_northing);
            mapProj.Geo2Proj( 180, -60, out double se_easting, out double se_northing);

            double w_pixel = 120;  // 180 Degr east
            double e_pixel = 3970; // 180 degr west

            double n_pixel = 1032; // 60 degr north
            double s_pixel = 2628; // 60 degr south

            int w_pixel_boundary = 38;
            int e_pixel_boundary = 4056;
            int n_pixel_boundary = 70;
            int s_pixel_boundary = 2872;

            try
            {
                //Image woldImage = Bitmap.FromFile(@"C:\Users\niels\OneDrive\Documents\WorldMap.jpg");
                Image woldImage = Bitmap.FromFile("WorldMap.jpg");

                using (Bitmap worldbitmap = new Bitmap(woldImage))
                {
                    // for each pixel within map boundary 
                    for (int ix = w_pixel_boundary; ix <= e_pixel_boundary; ix++)
                    //int ix = 2048;
                    {
                        for (int iy = n_pixel_boundary; iy <= s_pixel_boundary; iy++)
                        //int iy = 1832;
                        {
                            // (ix, iy)  => (easting, northing)
                            double easting = (ix - w_pixel) / (e_pixel - w_pixel) * (ne_easting - nw_easting) + nw_easting;
                            double northing = (iy - s_pixel) / (n_pixel - s_pixel) * (ne_northing - se_northing) + se_northing;

                            mapProj.Proj2Geo(easting, northing, out double lon, out double lat);

                            mapProj.Geo2Xyz(lon, lat, 0, out double x, out double y, out double z);
                            Vector3D pixelVector = new Vector3D(x, y, z).GetUnitVector();

                            mapProj.Geo2Xyz(_yourLon, _yourLat, 0, out double yourX, out double yourY, out double yourZ);
                            Vector3D yourVector = new Vector3D(yourX, yourY, yourZ).GetUnitVector();

                            double cosAlpha = Vector3D.Dot(pixelVector, yourVector);
                            double alphaRad = Math.Acos(cosAlpha);

                            float correctionFactor = 1.0f;

                            if (!_IsPShadowZone(alphaRad) && !_IsSShadowZone(alphaRad))
                            {
                                correctionFactor = 1.0f;
                            }
                            else if (_IsPShadowZone(alphaRad) && _IsSShadowZone(alphaRad))
                            {
                                correctionFactor = 0.3f;
                            }
                            else if (!_IsPShadowZone(alphaRad) && _IsSShadowZone(alphaRad))
                            {
                                correctionFactor = 0.4f;
                            }
                            else if (!_IsPShadowZone(alphaRad) && _IsSShadowZone(alphaRad))
                            {
                                throw new Exception("Unexpected type of shadow zone");
                            }


                            if (correctionFactor != 1)
                            {
                                Color color = worldbitmap.GetPixel(ix, iy);

                                float red = (float)color.R;
                                float green = (float)color.G;
                                float blue = (float)color.B;

                                red *= correctionFactor;
                                green *= correctionFactor;
                                blue *= correctionFactor;

                                Color colDark = Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
                                worldbitmap.SetPixel(ix, iy, colDark);
                            }
                        }

                    }
                    worldbitmap.Save(shadowZonesFilename, ImageFormat.Jpeg);
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool _IsPShadowZone(double angleRad)
        {
            double posAngleDegr = Math.Abs(angleRad) / Math.PI * 180.0;

            return (posAngleDegr > 103 && posAngleDegr < 142);
        }

        public bool _IsSShadowZone(double angleRad)
        {
            double posAngleDegr = Math.Abs(angleRad) / Math.PI * 180.0;

            return (posAngleDegr > 103);
        }
    }
}
