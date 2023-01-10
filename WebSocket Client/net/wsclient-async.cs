/*
 * IPWorks 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

﻿using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorks;

class wsclientDemo
{
  private static Wsclient wsclient;

  private static void wsclient_OnConnected(object sender, WsclientConnectedEventArgs e)
  {
    Console.WriteLine(e.StatusCode == 0 ? "Connected.\r\n" : "Failed to connect. Reason: " + e.Description + ".\r\n");
  }

  private static void wsclient_OnDataIn(object sender, WsclientDataInEventArgs e)
  {
    Console.WriteLine("Received '" + e.Text + "'.");
  }

  private static void wsclient_OnDisconnected(object sender, WsclientDisconnectedEventArgs e)
  {
    Console.WriteLine("Disconnected.\r\n");
  }

  private static void wsclient_OnError(object sender, WsclientErrorEventArgs e)
  {
    Console.WriteLine(e.Description);
  }

  private static void wsclient_OnSSLServerAuthentication(object sender, WsclientSSLServerAuthenticationEventArgs e)
  {
    if (e.Accept) return;
    Console.Write("Server provided the following certificate:\nIssuer: " + e.CertIssuer + "\nSubject: " + e.CertSubject + "\n");
    Console.Write("The following problems have been determined for this certificate: " + e.Status + "\n");
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.Read() == 'y') e.Accept = true;
  }

  static async Task Main(string[] args)
  {
    wsclient = new Wsclient();

    if (args.Length < 2)
    {
      Console.WriteLine("usage: wsclient [options] host port");
      Console.WriteLine("Options: ");
      Console.WriteLine("  -ssl       whether or not to use SSL/TLS (default false)");
      Console.WriteLine("  host       the name of the local host or user-assigned IP interface through which connections are initiated or accepted");
      Console.WriteLine("  port       the TCP port in the local host where the component binds");
      Console.WriteLine("\r\nExample: wsclient -ssl true localhost 4444");
    }
    else
    {
      wsclient.OnConnected += wsclient_OnConnected;
      wsclient.OnDataIn += wsclient_OnDataIn;
      wsclient.OnDisconnected += wsclient_OnDisconnected;
      wsclient.OnError += wsclient_OnError;
      wsclient.OnSSLServerAuthentication += wsclient_OnSSLServerAuthentication;

      try
      {
        // Parse arguments into component.
        for (int i = 0; i < args.Length; i++)
        {
          if (args[i].StartsWith("-"))
          {
            if (args[i].Equals("-ssl"))
            {
              if (bool.Parse(args[i + 1]))  // args[i + 1] corresponds to the value of args[i]
              {
                wsclient.URL = "wss://" + args[args.Length - 2] + ":" + args[args.Length - 1];
              }
            }
          }
        }

        if (string.IsNullOrEmpty(wsclient.URL)) wsclient.URL = "ws://" + args[args.Length - 2] + ":" + args[args.Length - 1];

        // Attempt to connect.
        await wsclient.Connect();

        // Process user commands.
        Console.WriteLine("Type \"?\" or \"help\" for a list of commands.");
        string command;
        string[] arguments;

        while (true)
        {
          await wsclient.DoEvents();
          command = Console.ReadLine();
          arguments = command.Split();

          if (arguments[0].Equals("?") || arguments[0].Equals("help"))
          {
            Console.WriteLine("Commands: ");
            Console.WriteLine("  ?                            display the list of valid commands");
            Console.WriteLine("  help                         display the list of valid commands");
            Console.WriteLine("  send <text>                  send text data to the server");
            Console.WriteLine("  quit                         exit the application");
          }
          else if (arguments[0].Equals("send"))
          {
            if (arguments.Length > 1)
            {
              string textToSend = "";
              for (int i = 1; i < arguments.Length; i++)
              {
                if (i < arguments.Length - 1) textToSend += arguments[i] + " ";
                else textToSend += arguments[i];
              }
              await wsclient.SendText(textToSend);
            }
            else
            {
              Console.WriteLine("Please supply the text that you would like to send.");
            }
          }
          else if (arguments[0].Equals("quit"))
          {
            await wsclient.Disconnect();
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

          Console.Write("wsclient> ");
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


