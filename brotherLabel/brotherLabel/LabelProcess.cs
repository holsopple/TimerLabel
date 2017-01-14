using System;
using bpac;
using System.Diagnostics;
using System.Configuration;


namespace com.holsopple.BrotherLabel
{
    class LabelProcess
    {

        static String templatePath;

        static void Main(string[] args)
        {

            if (args == null || args.Length != 4)
            {
                Console.WriteLine("hey, no data.");
                return;
            }

            String time = args[0];
            String name = args[1];
            String runInstance = args[2];
            String entrantNum = args[3];

            if (String.IsNullOrEmpty(time)
                ||
                String.IsNullOrEmpty(name))
            {
                Console.WriteLine("hey, no data.");
            }

            LabelProcess label = new LabelProcess();
            label.processPrint(time, name, runInstance, entrantNum);



        }

        private void processPrint(string time, string name, string runInstance, string entrantNum)
        {
            Boolean returnValue = false;

            if (String.IsNullOrEmpty(templatePath))
            {
                // read the config
                templatePath = ConfigurationManager.AppSettings["templatePath"];
                Trace.TraceInformation("templatePath:" +  templatePath);

            }

            bpac.Document doc = new Document();
            bpac.Printer printer = new Printer();
            if (doc.Open(templatePath) != false)
            {
                doc.GetObject("objTime").Text = time;
                doc.GetObject("objName").Text = name;
                doc.GetObject("objRun").Text = runInstance;
                doc.GetObject("objEntNum").Text = entrantNum;
                
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
