using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Elephant.Components
{
    public class SundirectionComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SundirectionComponent()
            : base("Sundirection", "Sun",
                "Get the direction of the sun for a given place and time",
                "Extra", "Elephant")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.Register_IntegerParam("GMT", "GMT", "Positive or negative offset for GMT timezone",1);
            pManager.Register_IntegerParam("Hour", "H", "Hour of the day", 12);
            pManager.Register_IntegerParam("Day", "D", "Day of the month", 1);
            pManager.Register_IntegerParam("Month", "M", "Month of the year",6);
            pManager.Register_IntegerParam("Year", "Y", "Month of the year", 2012);
            pManager.Register_DoubleParam("Latitude", "Lat", "Latitudal coordinates for the position of inquery", 52.2066);
            pManager.Register_DoubleParam("Longitude", "Long", "Longitudal coordinates for the position of inquery", 5.6422);
            pManager.Register_VectorParam("North", "N", "The vector indicating the north direction", new Vector3d(1,0,0));
            pManager.Register_VectorParam("Up", "U", "The vector pointing to the sky", new Vector3d(0,0,1));
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("azimuth", "azi", "Azimuth of the sun");
            pManager.Register_DoubleParam("altitude", "alt", "Altitude of the sun");
            pManager.Register_VectorParam("direction", "V", "Vector indicating the direction of the sun");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int GMT, Hour, Day, Month, Year;
            GMT = Hour = Day = Month = Year = 0;
            double Latitude, Longitude;
            Latitude = Longitude = 0.0;
            Vector3d North, Up;
            North = Up = new Vector3d();

            DA.GetData("GMT", ref GMT);
            DA.GetData("Hour", ref Hour);
            DA.GetData("Day", ref Day);
            DA.GetData("Month", ref Month);
            DA.GetData("Year", ref Year);
            DA.GetData("Latitude", ref Latitude);
            DA.GetData("Longitude", ref Longitude);
            DA.GetData("North", ref North);
            DA.GetData("Up", ref Up);


            North.Unitize();
            Up.Unitize();

            DateTime time = new DateTime(Year, Month, Day, Hour, 0, 0, DateTimeKind.Utc);
            time = time.AddHours(-GMT);
            this.CalculateSunPosition(time, Latitude, Longitude);
            DA.SetData("azimuth", azimuth);
            DA.SetData("altitude", altitude);

            // plane: x = north; y = up
            Point3d p0 = new Point3d(0, 0, 0);
            Point3d p1 = new Point3d(1, 0, 0);
            p1.Transform(Transform.Translation(North));
            p1.Transform(Transform.Rotation(this.azimuth, Up, p0));

            Plane azPlane = new Plane(p0, new Vector3d(p1), Up);
            p1.Transform(Transform.Rotation(this.altitude, azPlane.ZAxis, p0));
            Vector3d dir = new Vector3d(p1);
            dir.Unitize();
            DA.SetData("direction", dir);
        }

        private const double Deg2Rad = Math.PI / 180.0;
        private const double Rad2Deg = 180.0 / Math.PI;
        private double altitude;
        private double azimuth;

        /*!
         * \brief Calculates the sun light.
         *
         * CalcSunPosition calculates the suns "position" based on a
         * given date and time in local time, latitude and longitude
         * expressed in decimal degrees. It is based on the method
         * found here:
         * http://www.astro.uio.no/~bgranslo/aares/calculate.html
         * The calculation is only satisfiably correct for dates in
         * the range March 1 1900 to February 28 2100.
         * \param dateTime Time and date in local time.
         * \param latitude Latitude expressed in decimal degrees.
         * \param longitude Longitude expressed in decimal degrees.
         */
        public void CalculateSunPosition(
          DateTime dateTime, double latitude, double longitude)
        {
            // Convert to UTC
            dateTime = dateTime.ToUniversalTime();

            // Number of days from J2000.0.
            double julianDate = 367 * dateTime.Year -
              (int)((7.0 / 4.0) * (dateTime.Year +
              (int)((dateTime.Month + 9.0) / 12.0))) +
              (int)((275.0 * dateTime.Month) / 9.0) +
              dateTime.Day - 730531.5;

            double julianCenturies = julianDate / 36525.0;

            // Sidereal Time
            double siderealTimeHours = 6.6974 + 2400.0513 * julianCenturies;

            double siderealTimeUT = siderealTimeHours +
              (366.2422 / 365.2422) * (double)dateTime.TimeOfDay.TotalHours;

            double siderealTime = siderealTimeUT * 15 + longitude;

            // Refine to number of days (fractional) to specific time.
            julianDate += (double)dateTime.TimeOfDay.TotalHours / 24.0;
            julianCenturies = julianDate / 36525.0;

            // Solar Coordinates
            double meanLongitude = CorrectAngle(Deg2Rad *
              (280.466 + 36000.77 * julianCenturies));

            double meanAnomaly = CorrectAngle(Deg2Rad *
              (357.529 + 35999.05 * julianCenturies));

            double equationOfCenter = Deg2Rad * ((1.915 - 0.005 * julianCenturies) *
              Math.Sin(meanAnomaly) + 0.02 * Math.Sin(2 * meanAnomaly));

            double elipticalLongitude =
              CorrectAngle(meanLongitude + equationOfCenter);

            double obliquity = (23.439 - 0.013 * julianCenturies) * Deg2Rad;

            // Right Ascension
            double rightAscension = Math.Atan2(
              Math.Cos(obliquity) * Math.Sin(elipticalLongitude),
              Math.Cos(elipticalLongitude));

            double declination = Math.Asin(
              Math.Sin(rightAscension) * Math.Sin(obliquity));

            // Horizontal Coordinates
            double hourAngle = CorrectAngle(siderealTime * Deg2Rad) - rightAscension;

            if (hourAngle > Math.PI)
            {
                hourAngle -= 2 * Math.PI;
            }

            this.altitude = Math.Asin(Math.Sin(latitude * Deg2Rad) *
              Math.Sin(declination) + Math.Cos(latitude * Deg2Rad) *
              Math.Cos(declination) * Math.Cos(hourAngle));

            // Nominator and denominator for calculating Azimuth
            // angle. Needed to test which quadrant the angle is in.
            double aziNom = -Math.Sin(hourAngle);
            double aziDenom =
              Math.Tan(declination) * Math.Cos(latitude * Deg2Rad) -
              Math.Sin(latitude * Deg2Rad) * Math.Cos(hourAngle);

            this.azimuth = Math.Atan(aziNom / aziDenom);

            if (aziDenom < 0) // In 2nd or 3rd quadrant
            {
                this.azimuth += Math.PI;
            }
            else if (aziNom < 0) // In 4th quadrant
            {
                this.azimuth += 2 * Math.PI;
            }

            // Altitude
            //Console.WriteLine("Altitude: " + altitude * Rad2Deg);

            // Azimut
            //Console.WriteLine("Azimuth: " + azimuth * Rad2Deg);
        }

        /*!
        * \brief Corrects an angle.
        *
        * \param angleInRadians An angle expressed in radians.
        * \return An angle in the range 0 to 2*PI.
        */
        private static double CorrectAngle(double angleInRadians)
        {
            if (angleInRadians < 0)
            {
                return 2 * Math.PI - (Math.Abs(angleInRadians) % (2 * Math.PI));
            }
            else if (angleInRadians > 2 * Math.PI)
            {
                return angleInRadians % (2 * Math.PI);
            }
            else
            {
                return angleInRadians;
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Icons.sun;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1871f1cd-daab-4ee1-be8f-5f370a7b5feb}"); }
        }
    }
}