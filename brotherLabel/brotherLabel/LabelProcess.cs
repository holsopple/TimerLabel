using System;


namespace com.holsopple.BrotherLabel
{
    class LabelProcess
    {


        static void Main(string[] args)
        {

            /*
             *call from excel
             *Shell ("C:\GaryStuff\norpca\timerlabel\brotherLabel.exe " + runTime + " " + personName + " " + runInstance + " " + entrantNum + " " + classCode + " " + netTime + " " + penaltyCount)
             */


            if (args == null || args.Length != 7)
            {
                Console.WriteLine("hey, not right data.");
                return;
            }

            String runTime = args[0];
            String personName = args[1];
            String runInstance = args[2];
            String entrantNum = args[3];
            String classCode = args[4];
            String netTime = args[5];
            String penaltyCount = args[6];

            if (String.IsNullOrEmpty(runTime)
                ||
                String.IsNullOrEmpty(personName))
            {
                Console.WriteLine("hey, no data.");
            }


            // marshal the object
            AutoCrossRun aRun = new AutoCrossRun();
            aRun.NetTime = netTime;
            aRun.PersonName = personName;
            aRun.RunInstance = runInstance;
            aRun.EntrantNum = entrantNum;
            aRun.ClassCode = classCode;
            aRun.NetTime = netTime;
            aRun.PenaltyCount = penaltyCount;

            aRun.postToTwitter();
            aRun.processPrint();

        }





    }
}
