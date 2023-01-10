/*
 * IPWorks 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

﻿using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorks;

class wsserverDemo
{
  private static Wsserver wsserver;

  private static void wsserver_OnConnected(object sender, WsserverConnectedEventArgs e)
  {
    Console.WriteLine(wsserver.Connections[e.ConnectionId].RemoteHost + " has connected.");
  }

  private static void wsserver_OnDataIn(object sender, WsserverDataInEventArgs e)
  {
    Console.WriteLine("Received '" + e.Text + "'.");
  }

  private static void wsserver_OnDisconnected(object sender, WsserverDisconnectedEventArgs e)
  {
    Console.WriteLine(wsserver.Connections[e.ConnectionId].RemoteHost + " has disconnected - " + e.Description + ".");
  }

  private static void wsserver_OnError(object sender, WsserverErrorEventArgs e)
  {
    Console.WriteLine(e.Description);
  }

  static async Task Main(string[] args)
  {
    wsserver = new Wsserver();

    if (args.Length < 2)
    {
      Console.WriteLine("usage: wsserver [options] port");
      Console.WriteLine("Options: ");
      Console.WriteLine("  -cert      the subject of a certificate in the user's certificate store to be used during SSL/TLS negotiation (default no SSL/TLS)");
      Console.WriteLine("  port       the TCP port in the local host where the component listens");
      Console.WriteLine("\r\nExample: wsserver -cert certSubject 4444");
    }
    else
    {
      wsserver.OnConnected += wsserver_OnConnected;
      wsserver.OnDataIn += wsserver_OnDataIn;
      wsserver.OnDisconnected += wsserver_OnDisconnected;
      wsserver.OnError += wsserver_OnError;

      try
      {
        // Parse arguments into component.
        wsserver.LocalPort = int.Parse(args[args.Length - 1]);

        for (int i = 0; i < args.Length; i++)
        {
          if (args[i].StartsWith("-"))
          {
            if (args[i].Equals("-cert"))
            {
              wsserver.SSLCert = new Certificate(CertStoreTypes.cstUser, "MY", "", args[i + 1]);  // args[i + 1] corresponds to the value of args[i]
              wsserver.UseSSL = true;
            }
          }
        }

        // Start lisenting for connections.
        await wsserver.StartListening();

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
            Console.WriteLine("  send <text>                  send data to connected clients");
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
              foreach (WSConnection connection in wsserver.Connections.Values)
              {
                await wsserver.SendText(connection.ConnectionId, textToSend);
              }
            }
            else
            {
              Console.WriteLine("Please supply the text that you would like to send.");
            }
          }
          else if (arguments[0].Equals("quit"))
          {
            await wsserver.Shutdown();
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

          Console.Write("wsserver> ");
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


