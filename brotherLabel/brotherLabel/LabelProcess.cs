using System;
using System.Threading.Tasks;


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

            Task result;

            if (args != null && args.Length == 1 && args[0].ToUpper() == "RESET")
            {
                TweetController.ResetAuthorization();
                return;
            }

            if (args != null && args.Length == 1 && args[0].ToUpper() == "TESTTWEET")
            {
                // run a test tweet
                AutoCrossRun run = new AutoCrossRun();
                run.PersonName = args[0];
                run.EntrantNum = Convert.ToString(new Random().Next());
                run.RunTime = Convert.ToString(DateTime.Now);
                result = run.doTweet();
                Console.WriteLine(run.PersonName);
                return;
            }

            // check args, null, at least the first 7.  check first and second arg. 
            if (args == null || args.Length < 7 || args[0] is null || args[1] is null)
            {
                Console.WriteLine("Valid options are: \n 1) RESET (to reset the twitter authorization)\n 2) TESTTWEET (runs a simple test tweet) \n 3) 7 to 9 arguments (runtime name runinstance car# classcode nettime penaltycount [hastag] [usermention])  ");
                return;
            }

            // marshal the object
            AutoCrossRun aRun = new AutoCrossRun();
            aRun.RunTime = (args[0] != null) ? args[0] : "no runtime";
            aRun.PersonName = (args[1] != null) ? args[1] : "no person name";
            aRun.RunInstance = (args[2] != null) ? args[2] : "no run instance";
            aRun.EntrantNum = (args[3] != null) ? args[3] : "no entrant number";
            aRun.ClassCode = (args[4] != null) ? args[4] : "no class code";
            aRun.NetTime = (args[5] != null) ? args[5] : "no net time";
            aRun.PenaltyCount = (args[6] != null) ? args[6] : "no penalty count";
            aRun.Hashtag = ((args.Length >= 8) && (args[7] != null)) ? args[7] : " ";
            aRun.UserMention = ((args.Length >= 9) && (args[8] != null)) ? args[8] : " ";


            aRun.processPrint();
            result = aRun.doTweet();


        }





    }
}
