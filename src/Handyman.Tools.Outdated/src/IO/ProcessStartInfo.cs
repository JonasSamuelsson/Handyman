using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security;
using System.Text;

namespace Handyman.Tools.Outdated.IO
{
    public class ProcessStartInfo
    {
        public string Arguments { get; set; }
        public bool CreateNoWindow { get; set; }
        public string Domain { get; set; }
        public StringDictionary EnvironmentVariables { get; } = new StringDictionary();
        public bool ErrorDialog { get; set; }
        public string FileName { get; set; }
        public bool LoadUserProfile { get; set; }
        public SecureString Password { get; set; }
        public string PasswordInClearText { get; set; }
        public bool RedirectStandardError { get; set; }
        public bool RedirectStandardInput { get; set; }
        public bool RedirectStandardOutput { get; set; }
        public Encoding StandardErrorEncoding { get; set; }
        public Action<string> StandardErrorHandler { get; set; }
        public Encoding StandardInputEncoding { get; set; }
        public Encoding StandardOutputEncoding { get; set; }
        public Action<string> StandardOutputHandler { get; set; }
        public string UserName { get; set; }
        public bool UseShellExecute { get; set; }
        public string Verb { get; set; }
        public ProcessWindowStyle WindowStyle { get; set; }
        public string WorkingDirectory { get; set; }
    }
}