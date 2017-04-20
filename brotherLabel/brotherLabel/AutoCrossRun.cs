using System;
using System.Configuration;
using System.Diagnostics;
using bpac;
using LinqToTwitter;
using System.Threading.Tasks;

namespace com.holsopple.BrotherLabel

{
    class AutoCrossRun
    {
        #region variables

        string runTime;
        string personName;
        string runInstance;
        string entrantNum;
        string classCode;
        string netTime;
        string penaltyCount;
        string hashtag;
        string userMention;


        #endregion

        #region GettersSetters
        
        public string Hashtag { get => hashtag; set => hashtag = value; }
        public string UserMention { get => userMention; set => userMention = value; }
        public string PenaltyCount { get => penaltyCount; set => penaltyCount = value; }
        public string NetTime { get => netTime; set => netTime = value; }
        public string ClassCode { get => classCode; set => classCode = value; }
        public string EntrantNum { get => entrantNum; set => entrantNum = value; }
        public string RunInstance { get => runInstance; set => runInstance = value; }
        public string PersonName { get => personName; set => personName = value; }
        public string RunTime { get => runTime; set => runTime = value; }

        #endregion


        public async Task doTweet()
        {
            String status;
            Task resultTask;
            try
            {
                status = String.Format("Car: {0} \n Name: {1}\n Run: {2}\n Time: {3}\n Penalty: {4}\n Net Time: {5}\n Class:{6} \n {7} {8}", EntrantNum, PersonName, RunInstance, RunTime, PenaltyCount, NetTime, ClassCode, Hashtag, UserMention);
                TweetController tweet = new TweetController();
                resultTask = tweet.Tweet(status);
                resultTask.Wait();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void processPrint()
        {
            Boolean returnValue = false;
            String templatePath = ConfigurationManager.AppSettings["templatePath"];


            if (String.IsNullOrEmpty(templatePath))
            {
                // read the config
                Trace.TraceInformation("templatePath:" + templatePath);

            }

            bpac.Document doc = new Document();
            bpac.Printer printer = new Printer();
            if (doc.Open(templatePath) != false)
            {
                doc.GetObject("objRunTime").Text = RunTime;
                doc.GetObject("objPersonName").Text = PersonName;
                doc.GetObject("objRunInstance").Text = RunInstance;
                doc.GetObject("objEntNum").Text = EntrantNum;
                doc.GetObject("objClassCode").Text = ClassCode;
                doc.GetObject("objPenaltyCount").Text = PenaltyCount;
                doc.GetObject("objNetTime").Text = NetTime;

                returnValue = doc.StartPrint("", PrintOptionConstants.bpoDefault);
                Trace.TraceInformation("startProject:" + returnValue);

                returnValue = doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                Trace.TraceInformation("printout:" + returnValue);

                doc.EndPrint();
                doc.Close();
            }


        }


    }
}
