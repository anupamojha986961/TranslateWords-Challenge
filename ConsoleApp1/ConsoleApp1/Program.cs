using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            log4net.Config.BasicConfigurator.Configure();
            log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                Process currentProcess = Process.GetCurrentProcess();

                string path = ConfigurationManager.AppSettings["filepath"];

                string outputfilepath = ConfigurationManager.AppSettings["outputfilepath"];

                string frequencyfilepath = ConfigurationManager.AppSettings["freqyencyfilepath"];

                StringBuilder contents = new StringBuilder(File.ReadAllText(path));

                string pathword = ConfigurationManager.AppSettings["findwordfilepath"];

                string performncefilepath = ConfigurationManager.AppSettings["performncefilepath"];

                var wordlist = File.ReadAllText(pathword).Split('\n');

                string csvpath = ConfigurationManager.AppSettings["csvfilepath"];

               

                Dictionary<string,string> dict = File.ReadLines(csvpath).Select(line => line.Split(',')).ToDictionary(line => line[0], line => line[1]);
            
                

                if (File.Exists(frequencyfilepath))
                {
                    File.Delete(frequencyfilepath);
                }

                using (StreamWriter sw = File.CreateText(frequencyfilepath))
                {
                    for (int k = 0; k < wordlist.Count(); k++)
                    {

                        //int count = (new Regex(@"("+wordlist[k]+")+", RegexOptions.IgnoreCase)).Matches(contents).Count;
                        //var count = Fle.ReadLines().Select(l => l.Split(delim, StringSplitOptions.RemoveEmptyEntries).Count(w => countWords.Contains(w.ToUpperInvariant()))).Sum();
                        string cCriteria = @"\b" + wordlist[k] + @"\b";
                        System.Text.RegularExpressions.Regex oRegex = new System.Text.RegularExpressions.Regex(cCriteria, RegexOptions.IgnoreCase);

                        int count = oRegex.Matches(contents.ToString()).Count;

                        sw.WriteLine((string.Format("{0},{1},{2}", wordlist[k], dict[wordlist[k].ToString()], count)));
                        sw.Flush();
                        //Regex regexText = new Regex("respecter", RegexOptions.IgnoreCase);
                        string repcontents = Regex.Replace(contents.ToString(), wordlist[k], dict[wordlist[k].ToString()], RegexOptions.IgnoreCase);

                        contents.Clear().Append(repcontents);


                    }
                }



                if (File.Exists(outputfilepath))
                {
                    File.Delete(outputfilepath);
                }

                using (StreamWriter sw = File.CreateText(outputfilepath))
                {
                    sw.WriteLine(contents);
                }


                watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;


                if (File.Exists(performncefilepath))
                {
                    File.Delete(performncefilepath);
                }


                long usedMemory = currentProcess.PrivateMemorySize64;



                // Create a new file     
                using (StreamWriter sw = File.CreateText(performncefilepath))
                {
                    sw.WriteLine(string.Format("Time to process(ms):{0}", watch.ElapsedMilliseconds));
                    sw.WriteLine(string.Format("Memory Used(MB):{0} ", usedMemory));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }
    }
}
