/*
 * IPWorks 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

﻿using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorks;

class popclientDemo
{
  private static Pop pop;

  private static void pop_OnHeader(object sender, PopHeaderEventArgs e)
  {
    Console.WriteLine(e.Field + ": " + e.Value);
  }

  private static void pop_OnSSLServerAuthentication(object sender, PopSSLServerAuthenticationEventArgs e)
  {
    if (e.Accept) return;
    Console.Write("Server provided the following certificate:\nIssuer: " + e.CertIssuer + "\nSubject: " + e.CertSubject + "\n");
    Console.Write("The following problems have been determined for this certificate: " + e.Status + "\n");
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.Read() == 'y') e.Accept = true;
  }

  private static void pop_OnTransfer(object sender, PopTransferEventArgs e)
  {
    Console.WriteLine(e.Text);
  }

  static async Task Main(string[] args)
  {
    pop = new Pop();

    if (args.Length < 3)
    {
      Console.WriteLine("usage: popclient [options] server user password");
      Console.WriteLine("Options: ");
      Console.WriteLine("  -ssl       whether or not to use SSL/TLS (default true)");
      Console.WriteLine("  -port      the port of a POP mail server (default 995)");
      Console.WriteLine("  server     the name or address of a POP mail server");
      Console.WriteLine("  user       the user identifier for the mailbox");
      Console.WriteLine("  password   the password for the mailbox user");
      Console.WriteLine("\r\nExample: popclient -ssl true -port 995 mypopserver myusername mypassword");
    }
    else
    {
      pop.OnHeader += pop_OnHeader;
      pop.OnSSLServerAuthentication += pop_OnSSLServerAuthentication;
      pop.OnTransfer += pop_OnTransfer;

      try
      {
        // Parse arguments into component.
        pop.MailServer = args[args.Length - 3];
        pop.User = args[args.Length - 2];
        pop.Password = args[args.Length - 1];

        bool sslParamSet = false, portParamSet = false;
        for (int i = 0; i < args.Length; i++)
        {
          if (args[i].StartsWith("-"))
          {
            if (args[i].Equals("-ssl"))
            {
              pop.SSLEnabled = bool.Parse(args[i + 1]); // args[i + 1] corresponds to the value of args[i]
              sslParamSet = true;
            }
            else if (args[i].Equals("-port"))
            {
              pop.MailPort = int.Parse(args[i + 1]);  // args[i + 1] corresponds to the value of args[i]
              portParamSet = true;
            }
          }
        }

        if (!portParamSet) pop.MailPort = 995;
        if (!sslParamSet) pop.SSLEnabled = true;
        if (pop.SSLEnabled) pop.SSLStartMode = PopSSLStartModes.sslAutomatic;

        // Attempt to connect to the POP server.
        await pop.Connect();

        // Process user commands.
        Console.WriteLine("Type \"?\" or \"help\" for a list of commands.");
        string command;
        string[] arguments;

        while (true)
        {
          command = Console.ReadLine();
          arguments = command.Split();

          if (arguments[0].Equals("?") || arguments[0].Equals("help"))
          {
            Console.WriteLine("Commands: ");
            Console.WriteLine("  ?                            display the list of valid commands");
            Console.WriteLine("  help                         display the list of valid commands");
            Console.WriteLine("  retrieve <message number>    display the contents of the specified message");
            Console.WriteLine("  quit                         exit the application");
          }
          else if (arguments[0].Equals("retrieve"))
          {
            if (arguments.Length > 1 && int.TryParse(arguments[1], out int messageNumber) && messageNumber > 0 && messageNumber <= pop.MessageCount)
            {
              pop.MessageNumber = messageNumber;
              await pop.Retrieve();
            }
            else
            {
              Console.WriteLine("Please supply a number between 1 and " + pop.MessageCount + " (inclusive) corresponding to the message in the mailbox you would like to retrieve.");
            }
          }
          else if (arguments[0].Equals("quit"))
          {
            await pop.Disconnect();
            break;
          }
          else if (arguments[0].Equals(""))
          {
            // Do nothing.
          }
          else
          {
            Console.WriteLine("Invalid command.");
          }

          Console.Write("pop> ");
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
    }
  }
}


