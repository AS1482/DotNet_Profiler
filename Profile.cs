using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MiniBanque.Tools
{
    public class Profile
    {
        public static TraceSwitch General =null;
        static Profile()
        {
            General = new TraceSwitch(
                "General", 
                "Switch général des messages de trace");
            TextWriterTraceListener tw = 
                new TextWriterTraceListener(
                "Performances.del");
            tw.Name = "GeneralLog";
            tw.TraceOutputOptions = TraceOptions.DateTime;
            Trace.Listeners.Add(tw);
        }

        private Stopwatch _Sw;
        private string _Source;
        private long _StartMemory;
        private Profile(string source)
        {
            _Sw = new Stopwatch();
            _Source = source;
        }
        public static void Init()
        {
        }

        public static Profile Start(string source)
        {
            Profile pf = new Profile(source);
            pf._Sw.Start();
            return pf;
        }
        public static Profile StartWithMemory(string source)
        {
//           GC.Collect();
//           GC.WaitForPendingFinalizers();           
           Profile pf = Start(source);
           pf._StartMemory = GC.GetTotalMemory(false);
           return pf;
        }

        public static void Stop(Profile pf)
        {
            pf._Sw.Stop();
            long ms = pf._Sw.ElapsedMilliseconds;
            long ti = pf._Sw.ElapsedTicks;
            TimeSpan ts = pf._Sw.Elapsed;
            long stopMemory = GC.GetTotalMemory(false);

            Trace.WriteLineIf(General.TraceInfo,
                pf._Source);
            Trace.Indent();
            Trace.WriteLineIf(General.TraceInfo,
                "Elapsed : " + ts.ToString());
            Trace.WriteLineIf(General.TraceInfo,
                "Elapsed en ms : " + ms.ToString());
            Trace.WriteLineIf(General.TraceInfo,
                "Elapsed en ticks : " + ti.ToString());
            if (pf._StartMemory != 0)
            {
                Trace.Indent();
                Trace.WriteLineIf(General.TraceInfo,
                    "Mémoire de départ : " + pf._StartMemory.ToString());
                Trace.WriteLineIf(General.TraceInfo,
                    "Mémoire de Fin : " + stopMemory.ToString());
                Trace.WriteLineIf(General.TraceInfo,
                    "Mémoire Consommée : " + 
                    (stopMemory - pf._StartMemory).ToString());
                Trace.Unindent();
                pf._StartMemory = 0;
            }
            Trace.Unindent();
            Trace.Flush();
        }
    }
}

