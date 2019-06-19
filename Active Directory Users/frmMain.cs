using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Authentication;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Active_Directory_Users
{
  public partial class frmMain : Form
  {
    #region Declarations

    public static LogWriter _HistoryLog;

    #endregion

    public frmMain()
    {
      InitializeComponent();
      _HistoryLog = new LogWriter("Starting log", txtOutput);
    }

    private void cmdGo_Click(object sender, EventArgs e)
    {
      GetADUsers();
    }

    private void GetADUsers()
    {
      List<string> DisabledUsers = new List<string>();

      using (var context = new PrincipalContext(ContextType.Domain, "buffnews.com"))
      {
        using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
        {
          foreach (var result in searcher.FindAll())
          {
            DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
            string firstname = "";
            string lastname = "";
            if (de.Properties["givenName"].Value != null)
            {
              firstname = de.Properties["givenName"].Value.ToString();
            }
            if (de.Properties["sn"].Value != null)
            {
              lastname = de.Properties["sn"].Value.ToString();
            }
            _HistoryLog.LogWrite("First Name: " + de.Properties["givenName"].Value);
            _HistoryLog.LogWrite("Last Name : " + de.Properties["sn"].Value);
            _HistoryLog.LogWrite("SAM account name   : " + de.Properties["samAccountName"].Value);
            _HistoryLog.LogWrite("User principal name: " + de.Properties["userPrincipalName"].Value);
            //string username = de.Properties["userPrincipalName"].Value.ToString();
            string username = de.Properties["samAccountName"].Value.ToString();
            //0x0002 && user account control flags will determine if user account is enabled or disabled.
            int flags = (int)de.Properties["userAccountControl"].Value;
            bool isenabled = ((flags & 0x0002) == 0);
            _HistoryLog.LogWrite("User account enabled = " + isenabled.ToString());

            if (isenabled)
            {
              List<string> memberoflist = GetUserMemberOfBSOU(de);
              memberoflist.Sort();
              for (int i = 0; i < memberoflist.Count; i++)
              {
                _HistoryLog.LogWrite("     group = " + memberoflist[i]);
              }
              _HistoryLog.LogWrite("");
              Application.DoEvents();   // Temp just to see the results in real time.
            } else
            // make a list of all disabled AD entries
            {
              DisabledUsers.Add(lastname + ", " + firstname + "  --  " + username);
            }
          }
        }
      }
      _HistoryLog.LogWrite("\r\n ======================================\r\nDISBLED USERS");
      DisabledUsers.Sort();
      for (int i = 0; i < DisabledUsers.Count; i++)
      {
        _HistoryLog.LogWrite(DisabledUsers[i]);
      }
      //Console.ReadLine();
    }

    private static List<string> GetUserMemberOfBSOU(DirectoryEntry de)
    {
      var groups = new List<string>();

      //retrieve only the memberOf attribute from the user
      de.RefreshCache(new[] { "memberOf" });

      var memberOf = de.Properties["memberOf"];
      foreach (string group in memberOf)
      {
        var groupDe = new DirectoryEntry($"LDAP://{group.Replace("/", "\\/")}");
        groupDe.RefreshCache(new[] { "cn" });
        string membername = groupDe.Properties["cn"].Value as string;
            if (membername.Left(5) == "BSOU_")
        {
          groups.Add(membername);
        }
      }
      return groups;
    }


    private static List<string> GetUserMemberOf(DirectoryEntry de)
    {
      var groups = new List<string>();

      //retrieve only the memberOf attribute from the user
      de.RefreshCache(new[] { "memberOf" });

      while (true)
      {
        var memberOf = de.Properties["memberOf"];
        foreach (string group in memberOf)
        {
          var groupDe = new DirectoryEntry($"LDAP://{group.Replace("/", "\\/")}");
          groupDe.RefreshCache(new[] { "cn" });
          groups.Add(groupDe.Properties["cn"].Value as string);
        }

        //AD only gives us 1000 or 1500 at a time (depending on the server version)
        //so if we've hit that, go see if there are more
        if (memberOf.Count != 1500 && memberOf.Count != 1000) break;

        try
        {
          de.RefreshCache(new[] { $"memberOf;range={groups.Count}-*" });
        }
        catch (COMException e)
        {
          if (e.ErrorCode == unchecked((int)0x80072020)) break; //no more results

          throw;
        }
      }
      return groups;
    }
  }
  #region Extensions
  public static class Extensions
  {
    public static string Right(this string value, int length)
    {
      string result = "";
      if (value.Length - length > 0)
      {
        result = value.Substring(value.Length - length);
      }
      return (result);
    }

    public static string Right(int startindex, string value)
    {
      int length = value.Length - startindex;
      return (Right(value, length));
    }

    public static string Left(this string value, int length)
    {
      int l = length;
      if (l > value.Length)
      {
        l = value.Length;
      }
      string result = value.Substring(0, l);
      return (result);
    }

    public static string Mid(this string value, int startindex, int length)
    {
      int l = length;
      if (startindex + l > value.Length)
      {
        l = value.Length - startindex;
      }
      string result = value.Substring(startindex, l);
      return (result);
    }

    public static string Deblank(string input)
    {
      // Fastest method that was tested (from Stack Overflow)
      int len = input.Length,
          index = 0,
          i = 0;
      var src = input.ToCharArray();
      bool skip = false;
      char ch;
      for (; i < len; i++)
      {
        ch = src[i];
        switch (ch)
        {
          case '\u0020':
          case '\u00A0':
          case '\u1680':
          case '\u2000':
          case '\u2001':
          case '\u2002':
          case '\u2003':
          case '\u2004':
          case '\u2005':
          case '\u2006':
          case '\u2007':
          case '\u2008':
          case '\u2009':
          case '\u200A':
          case '\u202F':
          case '\u205F':
          case '\u3000':
          case '\u2028':
          case '\u2029':
          case '\u0009':
          case '\u000A':
          case '\u000B':
          case '\u000C':
          case '\u000D':
          case '\u0085':
            if (skip) continue;
            src[index++] = ch;
            skip = true;
            continue;
          default:
            skip = false;
            src[index++] = ch;
            continue;
        }
      }

      return new string(src, 0, index);
    }
  }

  #endregion
}
