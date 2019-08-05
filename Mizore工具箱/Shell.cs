using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MizoreTools
{
    public class Shell
    {

        public static bool ExistsOnPath(string exeName)
        {
            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.FileName = "where";
                    p.StartInfo.Arguments = exeName;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.WaitForExit();
                    return p.ExitCode == 0;
                }
            }
            catch (Win32Exception)
            {
                throw new Exception("'where' command is not on path");
            }
        }
       
    }
}
