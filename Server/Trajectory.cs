using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Trajectory
    {

        private List<double[,]> quinticParameters;
        //private readonly Quaternion finalOrientation;
        public double AVE_SPEED = 30; // Average velocity used to calculate trajectories (mm/s)
        private TimeSpan trajectoryTime;
        private bool isActive;
        private Stopwatch elapsedTime;
        public string[] axisKey = new string[] { "X", "Y", "Z" };
        double[] _finalPosition;
        int quinticLength = -1;
        public double[] normalVector, displacemnet;

        RobotData _robot;

        /// <summary>
        /// Overloaded constructor for various poses
        /// 
        /// Set up variables, quaternion for slerp and time from cardinal distance
        /// Populate quintinParameters with x y z quintic curves where each entry in the list is the next point
        /// </summary>
        public Trajectory(double[] FinalPose, RobotData robot)
        {
            _finalPosition = FinalPose;




            elapsedTime = new Stopwatch();
            quinticLength = -1;
            /* if (pose.Length == 6)
             {
                 finalOrientation = StaticFunctions.MakeQuaternion(pose);
             }*/
            _robot = robot;
            double distance = Math.Sqrt(Math.Pow(FinalPose[0] - robot.ReadPosition["X"], 2) + Math.Pow(FinalPose[1] - robot.ReadPosition["Y"], 2) + Math.Pow(FinalPose[2] - robot.ReadPosition["Z"], 2));
            trajectoryTime = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(1000 * 1.2 * (distance / AVE_SPEED)));
            quinticParameters = new List<double[,]>();
            FindQuintic(_finalPosition);




        }

        public void updateSpeed()
        {
            displacemnet = new double[3];
            for (int i = 0; i < 3; i++)
            {
                displacemnet[i] = _finalPosition[i] - _robot.ReadPosition[axisKey[i]];
            }
            normalVector = new double[3];
            double magnatude = Math.Sqrt(Math.Pow(displacemnet[0], 2) + Math.Pow(displacemnet[1], 2) + Math.Pow(displacemnet[2], 2));
            double sf = 0;
            if (magnatude > 1) sf = (magnatude > 20) ? 1 : magnatude / 20;

            for (int i = 0; i < 3; i++)
            {
                normalVector[i] = 0.3 * sf * displacemnet[i] / magnatude;

            }
        }

        public void startMovement()
        {
            elapsedTime.Restart();
            isActive = true;
        }

        /// <summary>
        /// Controls when the trajectory begins moving
        /// </summary>
        public Boolean IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                if (!isActive & value)
                {
                    isActive = value;
                    elapsedTime.Start();
                }
                else if (isActive & !value)
                {
                    isActive = value;
                    elapsedTime.Stop();
                }
            }
        }



        /// <summary>
        /// Gets the Position at a specified time, returns -1 if out of time boundaries
        /// </summary>
        public double RefPos(int Axis)
        {
            if (elapsedTime.ElapsedMilliseconds > trajectoryTime.Milliseconds)
            {
                return _finalPosition[Axis];
            }
            else
                return quinticParameters[quinticLength][0, Axis] +
                    quinticParameters[quinticLength][1, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 1) +
                    quinticParameters[quinticLength][2, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 2) +
                    quinticParameters[quinticLength][3, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 3) +
                    quinticParameters[quinticLength][4, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 4) +
                    quinticParameters[quinticLength][5, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 5);
        }

        /// <summary>
        /// Gets the velocity at a specified time, returns -1 if out of time boundaries
        /// </summary>
        public double RefVel(int Axis)
        {
            if (elapsedTime.ElapsedMilliseconds > trajectoryTime.Milliseconds)
            {
                return 0;
            }
            else
                return quinticParameters[quinticLength][1, Axis] +
                    2 * quinticParameters[quinticLength][2, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 1) +
                    3 * quinticParameters[quinticLength][3, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 2) +
                    4 * quinticParameters[quinticLength][4, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 3) +
                    5 * quinticParameters[quinticLength][5, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 4);
        }

        /// <summary>
        /// Gets the acceleration at a specified time, returns -1 if out of time boundaries
        /// </summary>
        public double RefAcc(int Axis)
        {
            if (elapsedTime.ElapsedMilliseconds > trajectoryTime.Milliseconds)
            {
                return 0;
            }
            else
                return 2 * quinticParameters[quinticLength][2, Axis] +
                    6 * quinticParameters[quinticLength][3, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 1) +
                    12 * quinticParameters[quinticLength][4, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 2) +
                    20 * quinticParameters[quinticLength][5, Axis] * Math.Pow(elapsedTime.ElapsedMilliseconds, 3);
        }

        /// <summary>
        /// Given a pose it calculates the quintic coefficients conserving the current position and velocity
        /// It assumes zero initial time, zero final velocity and when half the time has elapsed it is hafway  with the average velocity
        /// </summary>
        private void FindQuintic(double[] poses)
        {
            Matrix<double> tempQuinticParam = Matrix<double>.Build.Dense(6, 3);
            for (int i = 0; i < 3; i++)
            {
                tempQuinticParam.SetColumn(i, FindCurve(0, 1.0 * trajectoryTime.Milliseconds / 1000, _robot.ReadPosition[axisKey[i]], poses[i], _robot.Velocity[axisKey[i]], 0)); // magic numbers are zero start time and zero final velocity

            }
            quinticParameters.Add(tempQuinticParam.ToArray());
            quinticLength++;
        }

        /// <summary>
        /// Finds the coefficients to describe a quintic path using start and final time, position and velocity
        /// It assumes when half the time has elapsed it is hafway and with the average velocity
        /// </summary>
        /// <param name="t0"></param> Start Time seconds
        /// <param name="tf"></param> Final Time seconds
        /// <param name="x0"></param> Start Position
        /// <param name="xf"></param> Final Position
        /// <param name="v0"></param> Start Velocity
        /// <param name="vf"></param> Final Velocity
        /// <returns></returns>
        private Vector<double> FindCurve(double t0, double tf, double x0, double xf, double v0, double vf)
        {
            double tm = (tf - t0) / 2;
            Matrix<Double> A = Matrix<Double>.Build.DenseOfArray(new double[,] {{1,  t0  ,Math.Pow(t0,2),Math.Pow(t0,3)  , Math.Pow(t0,4)  , Math.Pow(t0,5)  },  // start position
                                                                                {0,  1   , 2*t0         ,3*Math.Pow(t0,2), 4*Math.Pow(t0,3), 5*Math.Pow(t0,4)},  // start velocity 
                                                                                {1,  tm  ,Math.Pow(tm,2),Math.Pow(tm,3)  ,  Math.Pow(tm,4) , Math.Pow(tm,5)  },  // mid position 
                                                                                {1,  tf  ,Math.Pow(tf,2),Math.Pow(tf,3)  ,  Math.Pow(tf,4) , Math.Pow(tf,5)  },  // final position
                                                                                {0,  1   , 2*tf         ,3*Math.Pow(tf,2), 4*Math.Pow(tf,3), 5*Math.Pow(tf,4)},  // final velocity
                                                                                {0,  1   , 2*tm         ,3*Math.Pow(tm,2), 4*Math.Pow(tm,3), 5*Math.Pow(tm,4)}}); // mid velocity 
            Matrix<Double> Y = Matrix<Double>.Build.DenseOfArray(new double[,] {{x0},                           // start position
                                                                                {v0},                           // start velocity 
                                                                                {x0+(xf-x0)/2},                 // mid position (half way)
                                                                                {xf},                           // final position
                                                                                {vf},                           // final velocity
                                                                                {AVE_SPEED}});                   // mid velocity 
            Matrix<Double> X = A.Solve(Y);
            return X.Column(0);
        }
    }
}
