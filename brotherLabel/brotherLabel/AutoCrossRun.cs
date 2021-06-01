using bpac;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace com.holsopple.BrotherLabel

{

    /*
     * dependent on brother printer api
     * https://support.brother.com/g/s/es/dev/en/bpac/download/index.html?c=eu_ot&lang=en&navi=offall&comple=on&redirect=on
     * this is BPAC
     */


    internal class AutoCrossRun
    {
        #region GettersSetters
        public string Hashtag { get; set; }
        public string UserMention { get; set; }
        public string PenaltyCount { get; set; }
        public string NetTime { get; set; }
        public string ClassCode { get; set; }
        public string EntrantNum { get; set; }
        public string RunInstance { get; set; }
        public string PersonName { get; set; }
        public string RunTime { get; set; }
        #endregion


        public async Task DoTweet()
        {
            try
            {
                var status =
                    $"Car: {EntrantNum} \n Name: {PersonName}\n Run: {RunInstance}\n Time: {RunTime}\n Penalty: {PenaltyCount}\n Net Time: {NetTime}\n Class:{ClassCode} \n {Hashtag} {UserMention}";
                var tweet = new TweetController();
                var resultTask = tweet.Tweet(status);
                resultTask.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void ProcessPrint()
        {
            var templatePath = ConfigurationManager.AppSettings["templatePath"];


            if (String.IsNullOrEmpty(templatePath))
            {
                // read the config
                Trace.TraceInformation("templatePath:" + templatePath);

            }

            var doc = new Document();
        //    var printer = new Printer();
            if (doc.Open(templatePath))
            {
                doc.GetObject("objRunTime").Text = RunTime;
                doc.GetObject("objPersonName").Text = PersonName;
                doc.GetObject("objRunInstance").Text = RunInstance;
                doc.GetObject("objEntNum").Text = EntrantNum;
                doc.GetObject("objClassCode").Text = ClassCode;
                doc.GetObject("objPenaltyCount").Text = PenaltyCount;
                doc.GetObject("objNetTime").Text = NetTime;

                var returnValue = doc.StartPrint("", PrintOptionConstants.bpoDefault);
                Trace.TraceInformation("startProject:" + returnValue);

                returnValue = doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                Trace.TraceInformation("printout:" + returnValue);

                doc.EndPrint();
                doc.Close();
            }


        }


    }
}
