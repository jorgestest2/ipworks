/*
 * IPWorks 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

﻿using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorks;

class imapDemo
{
  private static Imap imap1 = new Imap();
  private static int lines = 0;

  private static void imap1_OnSSLServerAuthentication(object sender, ImapSSLServerAuthenticationEventArgs e)
  {
    if (e.Accept) return;
    Console.Write("Server provided the following certificate:\nIssuer: " + e.CertIssuer + "\nSubject: " + e.CertSubject + "\n");
    Console.Write("The following problems have been determined for this certificate: " + e.Status + "\n");
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.Read() == 'y') e.Accept = true;
  }

  private static void imap1_OnMailboxList(object sender, ImapMailboxListEventArgs e)
  {
    Console.WriteLine(e.Mailbox);
    lines++;
    if (lines == 22)
    {
      Console.Write("Press enter to continue...");
      try
      {
        Console.ReadLine();
      }
      catch (Exception ex)
      {
      }
      lines = 0;
    }
  }

  private static void imap1_OnMessageInfo(object sender, ImapMessageInfoEventArgs e)
  {
    Console.Write(e.MessageId + "  ");
    Console.Write(e.Subject + "  ");
    Console.Write(e.MessageDate + "  ");
    Console.WriteLine(e.From);
    lines++;
    if (lines == 22)
    {
      Console.Write("Press enter to continue...");
      try
      {
        Console.ReadLine();
      }
      catch (Exception ex)
      {
      }
      lines = 0;
    }
  }

  private static void imap1_OnTransfer(object sender, ImapTransferEventArgs e)
  {
    Console.WriteLine(e.Text);
    lines++;
    if (lines == 22)
    {
      Console.Write("Press enter to continue...");
      try
      {
        Console.ReadLine();
      }
      catch (Exception ex)
      {
      }
      lines = 0;
    }
  }

  static async Task Main(string[] args)
  {
    if (args.Length < 3)
    {

      Console.WriteLine("usage: imap server username password");
      Console.WriteLine("  server    the name or address of the mail server (IMAP server)");
      Console.WriteLine("  username  the user name used to authenticate to the MailServer ");
      Console.WriteLine("  password  the password used to authenticate to the MailServer ");
      Console.WriteLine("\nExample: imap 127.0.0.1 username password");
      Console.WriteLine("Press enter to continue.");
      Console.Read();
    }
    else
    {
      try
      {
        imap1.OnSSLServerAuthentication += imap1_OnSSLServerAuthentication;
        imap1.OnMailboxList += imap1_OnMailboxList;
        imap1.OnMessageInfo += imap1_OnMessageInfo;

        imap1.MailServer = args[args.Length - 3];
        imap1.User = args[args.Length - 2];
        imap1.Password = args[args.Length - 1];
        DisplayMenu();
        await imap1.Connect();

        string command;
        string[] argument;
        int msgnum = 0;
        while (true)
        {
          lines = 0;
          Console.Write("imap> ");
          command = Console.ReadLine();
          argument = command.Split();
          if (argument.Length == 0)
            continue;
          switch (argument[0][0])
          {
            case 's':
              imap1.Mailbox = argument[1];
              await imap1.SelectMailbox();
              break;
            case 'h':
              if (imap1.MessageCount > 0)
              {
                await imap1.FetchMessageInfo();
              }
              else
              {
                Console.WriteLine("No messages in this mailbox.");
              }
              break;
            case 'l':
              if (argument.Length < 2)
              {
                imap1.Mailbox = "*";
              }
              else
              {
                imap1.Mailbox = argument[1];
              }
              await imap1.ListMailboxes();
              break;
            case 'n':
              msgnum++;
              imap1.MessageSet = msgnum.ToString();
              await imap1.FetchMessageText();
              break;
            case 'q':
              await imap1.Disconnect();
              return;
            case 'v':
              msgnum = int.Parse(argument[1]);
              imap1.MessageSet = argument[1];
              await imap1.FetchMessageText();
              break;
            case '?':
              DisplayMenu();
              break;
            default: // allow user to enter only the number of the message they
                      // want to view
              try
              {
                msgnum = int.Parse(command);
                imap1.MessageSet = command;
                await imap1.FetchMessageText();
              }
              catch (FormatException e)
              {
                Console.WriteLine("Bad command / Not implemented in demo.");
              }
              break;
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
    }
  }

  private static void DisplayMenu()
  {
    Console.WriteLine("IMAP Commands");
    Console.WriteLine("l                   list mailboxes");
    Console.WriteLine("s <mailbox>         select mailbox");
    Console.WriteLine("v <message number>  view the content of selected message");
    Console.WriteLine("n                   goto and view next message");
    Console.WriteLine("h                   print out active message headers");
    Console.WriteLine("?                   display options");
    Console.WriteLine("q                   quit");
  }
}


