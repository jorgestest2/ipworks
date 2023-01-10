/*
 * IPWorks 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

﻿using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorks;

class udpechoDemo
{
  private static Udp udp;
  private static int sendTime;

  private static void udp_OnDataIn(object sender, UdpDataInEventArgs e)
  {
    int delay = Environment.TickCount - sendTime;
    Console.WriteLine("Received data from " + e.SourceAddress + " with a " + delay + " millisecond delay.");
  }

  static async Task Main(string[] args)
  {
    udp = new Udp();

    if (args.Length < 2)
    {
      Console.WriteLine("usage: udpecho host port");
      Console.WriteLine("  host       the address of the remote host");
      Console.WriteLine("  port       the UDP port of the remote host");
      Console.WriteLine("\r\nExample: udpecho 255.255.255.255 7");
    }
    else
    {
      udp.OnDataIn += udp_OnDataIn;

      try
      {
        // Parse arguments into component.
        udp.RemoteHost = args[args.Length - 2];
        udp.RemotePort = int.Parse(args[args.Length - 1]);

        // Enable sending and receiving of data.
        await udp.Activate();

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
            Console.WriteLine("  send <text>                  send data to the remote host");
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
              sendTime = Environment.TickCount;
              await udp.SendText(textToSend);
            }
            else
            {
              Console.WriteLine("Please supply the text that you would like to send.");
            }
          }
          else if (arguments[0].Equals("quit"))
          {
            await udp.Deactivate();
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

          Console.Write("udp> ");
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


