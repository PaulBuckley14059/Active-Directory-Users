using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Active_Directory_Users
{
  public delegate void ThreadedLogCallback(string text, TextWriter txtWriter); // Allow logging from any thread

  #region HistoryLog

  /// <summary>
  /// LogWriter - This is thread-safe but not yet atomic. A lock/unlock needs to be wrapped around the WriteLine calls
  /// so that no other threads can interrupt execution during the logging operation.
  /// </summary>

  public class LogWriter
  {

    private string m_exePath = string.Empty;
    private TextBox _t;
    const int MAX_LOG_LENGTH = 500000;

    public LogWriter(string logMessage, TextBox t)
    {
      _t = t;
      LogWrite(logMessage);
    }

    public void LogWrite(StringBuilder logMessage)
    {
      LogWrite(logMessage.ToString());
    }

    public void LogWrite(string logMessage)
    {
      // Put (or find) the log in the folder containing the executable
      m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      try
      {
        using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
        {
          Log(logMessage, w);
        }
      }
      catch
      {
        // What to do when we can't log an error to the log!
      }
    }

    public void Log(string logMessage, TextWriter txtWriter)
    {
      try
      {
        if (_t.InvokeRequired)
        {
          ThreadedLogCallback stc = new ThreadedLogCallback(ThreadedLog);
          _t.Invoke(stc, new object[] { logMessage, txtWriter });
        }
        else
        {
          WriteToLog(logMessage, txtWriter);
        }
      }
      catch
      {
        // What to do when we can't log an error to the log!
      }
    }

    private void ThreadedLog(string logMessage, TextWriter txtWriter)
    {
      try
      {
        WriteToLog(logMessage, txtWriter);
      }
      catch
      {
        // What to do when we can't log an error to the log!
      }
    }

    private void WriteToLog(string logMessage, TextWriter txtWriter)
    {
      txtWriter.Write("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLocalTime());
          //DateTime.Now.ToLongDateString());
      txtWriter.Write("  :");
      txtWriter.WriteLine("  :{0}", logMessage);

      _t.AppendText(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " + logMessage + "\r\n");
#if false // TBD Enable if text box length gets too long
                if (_t.Text.Length > MAX_LOG_LENGTH)
                {
                    _t.Text = _t.Text.Substring(_t.Text.Length - MAX_LOG_LENGTH, MAX_LOG_LENGTH);
                }
#endif
    }

    public string GetRoutineName(bool insideexception)
    {
      string s = "";
      try
      {
        StackTrace stacktrace = new StackTrace();
        StackFrame stackframe = stacktrace.GetFrame(1);
        if (insideexception)
        {
          s = stackframe.GetMethod().Name;
        }
        else
        {
          s = stackframe.GetMethod().DeclaringType.Name;
        }
        int n = s.IndexOf('>');
        if (n > -1)
        {
          s = s.Left(n);
          s = s.Replace("<", "");
        }
      }
      catch
      {
        s = "<unavailable>";
      }
      return (s);
    }



  }

  #endregion // History Log

}
