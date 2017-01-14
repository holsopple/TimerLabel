using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using bpac;


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
        #endregion

        #region GettersSetters

        public string RunTime
        {
            get
            {
                return runTime;
            }

            set
            {
                runTime = value;
            }
        }

        public string PersonName
        {
            get
            {
                return personName;
            }

            set
            {
                personName = value;
            }
        }

        public string RunInstance
        {
            get
            {
                return runInstance;
            }

            set
            {
                runInstance = value;
            }
        }

        public string EntrantNum
        {
            get
            {
                return entrantNum;
            }

            set
            {
                entrantNum = value;
            }
        }

        public string ClassCode
        {
            get
            {
                return classCode;
            }

            set
            {
                classCode = value;
            }
        }

        public string NetTime
        {
            get
            {
                return netTime;
            }

            set
            {
                netTime = value;
            }
        }

        public string PenaltyCount
        {
            get
            {
                return penaltyCount;
            }

            set
            {
                penaltyCount = value;
            }
        }

        #endregion


        public void postToTwitter()
        {
            // this method purposes is to post to twitter the data that is in the object


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
                doc.GetObject("objEntrantNum").Text = EntrantNum;
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
