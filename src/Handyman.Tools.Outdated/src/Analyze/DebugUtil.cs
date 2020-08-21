using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace Handyman.Tools.Outdated.Analyze
{
    internal static class DebugUtil
    {
        internal static void WriteProcessInfo()
        {
            Dictionary<string, List<string>> myColl = new Dictionary<string, List<string>>();
            Process[] procs = Process.GetProcesses();
            int totalProcess = procs.Length;

            for (int i = 0; i < procs.Length; i++)
            {
                totalProcess = totalProcess - 1;
                //Console.WriteLine("Process left for validation: " + totalProcess.ToString());
                string parentProcess = GetParentProcess(procs[i].Id);
                if (parentProcess != "0")
                {
                    if (!myColl.Keys.Contains(parentProcess.ToString()))
                        myColl.Add(parentProcess.ToString(), new List<string> { procs[i].ProcessName + "-" + procs[i].Id });
                    else
                    {
                        List<string> myChild = myColl[parentProcess.ToString()];
                        myChild.Add(procs[i].ProcessName + "-" + procs[i].Id);
                        myColl[parentProcess.ToString()] = myChild;
                    }
                }
            }

            foreach (KeyValuePair<string, List<string>> myVal in myColl)
            {
                Console.WriteLine("=====Parent: " + myVal.Key);
                foreach (string item in myVal.Value)
                {
                    Console.WriteLine("Child: " + item.ToString());
                }
            }
        }

        private static string GetParentProcess(int Id)
        {
            try
            {
                using (ManagementObject mo = new ManagementObject("win32_process.handle='" + Id.ToString() + "'"))
                {
                    try
                    {
                        mo.Get();
                        var parentPid = Convert.ToInt32(mo["ParentProcessId"]);
                        var parent = Process.GetProcessById(parentPid);
                        return $"{parent.ProcessName}-{parentPid}";
                    }
                    catch
                    {
                        return "0";
                    }
                }
            }
            catch
            {
                return "0";
            }
        }
    }
}