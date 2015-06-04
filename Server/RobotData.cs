using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class RobotData
    {
        private Stopwatch updateTime;
        // Time of loop in SECONDS
        public double loopTime;

        public ConcurrentDictionary<String, double> ReadPosition;
        public ConcurrentDictionary<String, double> LastPosition;
        public ConcurrentDictionary<String, double> Velocity;
        public ConcurrentDictionary<String, double> LastVelocity;
        public ConcurrentDictionary<String, double> acceleration;
        public ConcurrentDictionary<String, double> DesiredPosition;
        public ConcurrentDictionary<String, double> CommandedPosition;

        public Trajectory goTo;


        public RobotData()
        {
            ReadPosition = new ConcurrentDictionary<string, double>();
            LastPosition = new ConcurrentDictionary<string, double>();
            Velocity = new ConcurrentDictionary<string, double>();
            LastVelocity = new ConcurrentDictionary<string, double>();
            acceleration = new ConcurrentDictionary<string, double>();
            CommandedPosition = new ConcurrentDictionary<string, double>();

            setupDictionaries(ReadPosition);
            setupDictionaries(LastPosition);
            setupDictionaries(Velocity);
            setupDictionaries(LastVelocity);
            setupDictionaries(acceleration);
            setupDictionaries(CommandedPosition);

            goTo = new Trajectory(new double[] { 540.5, -18.1, 833.3 }, this);

            updateTime = new Stopwatch();
            loopTime = 0;

            // Start timer
            updateTime.Start();

        }


        void setupDictionaries(ConcurrentDictionary<string, double> dic)
        {
            dic.TryAdd("X", 0);
            dic.TryAdd("Y", 0);
            dic.TryAdd("Z", 0);
            dic.TryAdd("A", 0);
            dic.TryAdd("B", 0);
            dic.TryAdd("C", 0);
        }

        public void updateRobotInfo(double x, double y, double z, double a, double b, double c)
        {

            LastPosition["X"] = ReadPosition["X"];
            LastPosition["Y"] = ReadPosition["Y"];
            LastPosition["Z"] = ReadPosition["Z"];
            LastPosition["A"] = ReadPosition["A"];
            LastPosition["B"] = ReadPosition["B"];
            LastPosition["C"] = ReadPosition["C"];

            ReadPosition["X"] = x;
            ReadPosition["Y"] = y;
            ReadPosition["Z"] = z;
            ReadPosition["A"] = a;
            ReadPosition["B"] = b;
            ReadPosition["C"] = c;

            LastVelocity["X"] = Velocity["X"];
            LastVelocity["Y"] = Velocity["Y"];
            LastVelocity["Z"] = Velocity["Z"];
            LastVelocity["A"] = Velocity["A"];
            LastVelocity["B"] = Velocity["B"];
            LastVelocity["C"] = Velocity["C"];

            updateTime.Stop();
            loopTime = 1.0 * updateTime.ElapsedTicks / TimeSpan.TicksPerSecond;

            Velocity["X"] = 1.0 * (ReadPosition["X"] - LastPosition["X"]) / loopTime;
            Velocity["Y"] = 1.0 * (ReadPosition["Y"] - LastPosition["Y"]) / loopTime;
            Velocity["Z"] = 1.0 * (ReadPosition["Z"] - LastPosition["Z"]) / loopTime;
            Velocity["A"] = 1.0 * (ReadPosition["A"] - LastPosition["A"]) / loopTime;
            Velocity["B"] = 1.0 * (ReadPosition["B"] - LastPosition["B"]) / loopTime;
            Velocity["C"] = 1.0 * (ReadPosition["C"] - LastPosition["C"]) / loopTime;

            acceleration["X"] = 1.0 * (Velocity["X"] - LastVelocity["X"]) / loopTime;
            acceleration["Y"] = 1.0 * (Velocity["Y"] - LastVelocity["Y"]) / loopTime;
            acceleration["Z"] = 1.0 * (Velocity["Z"] - LastVelocity["Z"]) / loopTime;
            acceleration["A"] = 1.0 * (Velocity["A"] - LastVelocity["A"]) / loopTime;
            acceleration["B"] = 1.0 * (Velocity["B"] - LastVelocity["B"]) / loopTime;
            acceleration["C"] = 1.0 * (Velocity["C"] - LastVelocity["C"]) / loopTime;

            updateTime.Restart();
        }

        public void newPosition(double[] finalPosition)
        {

            goTo = new Trajectory(finalPosition, this);
        }

        public void updateComandPosition()
        {

            goTo.updateSpeed();
            //double[] commandArray = getKukaDisplacement();
            for (int i = 0; i < 3; i++)
            {
                CommandedPosition[goTo.axisKey[i]] = goTo.normalVector[i];
            }

        }

        public double[] getKukaDisplacement()
        {
            double[] displacement = new double[3];
            if (goTo.IsActive == false)
            {
                Console.WriteLine("NOT ACTIVE!");
                // Robot hasnt started moving
                //goTo.IsActive = true;
                return new double[] { 0, 0, 0 };
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    displacement[i] = goTo.RefPos(i) - ReadPosition[goTo.axisKey[i]];
                    if (displacement[i] > 0.5)
                    {
                        Console.WriteLine("Error, {1} Axis sent a huge distance, at {0}mm", displacement[i], goTo.axisKey[i]);
                        displacement[i] = 0.5;
                    }
                    else if (displacement[i] < -0.5)
                    {

                        Console.WriteLine("Error, {1} Axis sent a huge distance, at {0}mm", displacement[i], goTo.axisKey[i]);
                        displacement[i] = -0.5;
                    }


                }

            }
            return displacement;
        }

    }
}

