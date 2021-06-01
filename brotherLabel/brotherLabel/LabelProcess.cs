using System;
using System.Globalization;
//using System.Threading.Tasks;


namespace com.holsopple.BrotherLabel
{
    internal class LabelProcess
    {
        private static void Main(string[] args)
        {

            /*
             *call from excel
             *Shell ("C:\GaryStuff\norpca\timerlabel\brotherLabel.exe " + runTime + " " + personName + " " + runInstance + " " + entrantNum + " " + classCode + " " + netTime + " " + penaltyCount)
             */

            //Task result;

            if (args != null && args.Length == 1 && args[0].ToUpper() == "RESET")
            {
                TweetController.ResetAuthorization();
                return;
            }

            if (args != null && args.Length == 1 && (args[0].ToUpper() == "TESTTWEET" || args[0].ToUpper() == "TEST"))
            {
                // run a test tweet
                var run = new AutoCrossRun
                {
                    PersonName = args[0],
                    EntrantNum = Convert.ToString(new Random().Next()),
                    RunTime = Convert.ToString(DateTime.Now, CultureInfo.CurrentCulture)
                };
                //result = 
                    run.DoTweet();
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
            var aRun = new AutoCrossRun
            {
                RunTime = args[0] ?? "no runtime",
                PersonName = args[1] ?? "no person name",
                RunInstance = args[2] ?? "no run instance",
                EntrantNum = args[3] ?? "no entrant number",
                ClassCode = args[4] ?? "no class code",
                NetTime = args[5] ?? "no net time",
                PenaltyCount = args[6] ?? "no penalty count",
                Hashtag = ((args.Length >= 8) && (args[7] != null)) ? args[7] : " ",
                UserMention = ((args.Length >= 9) && (args[8] != null)) ? args[8] : " "
            };


            aRun.ProcessPrint();
            //result = 
                aRun.DoTweet();


        }





    }
}
