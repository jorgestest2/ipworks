/*
 * IPWorks 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorks;

class tcpechoDemo
{
  private static Tcpclient ip;

  private static void ip_OnConnected(object sender, TcpclientConnectedEventArgs e)
  {
    if (e.Description.ToLower().Equals("ok")) Console.WriteLine("Successfully connected.");
    else Console.WriteLine(e.Description);
  }

  private static void ip_OnDataIn(object sender, TcpclientDataInEventArgs e)
  {
    Console.WriteLine("Received '" + e.Text + "' from " + ip.RemoteHost + ".");
  }

  private static void ip_OnDisconnected(object sender, TcpclientDisconnectedEventArgs e)
  {
    Console.WriteLine("Disconnected.");
  }

  private static void ip_OnError(object sender, TcpclientErrorEventArgs e)
  {
    Console.WriteLine(e.Description);
  }

  private static void ip_OnSSLServerAuthentication(object sender, TcpclientSSLServerAuthenticationEventArgs e)
  {
    if (e.Accept) return;
    Console.Write("Server provided the following certificate:\nIssuer: " + e.CertIssuer + "\nSubject: " + e.CertSubject + "\n");
    Console.Write("The following problems have been determined for this certificate: " + e.Status + "\n");
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.Read() == 'y') e.Accept = true;
  }

  static async Task Main(string[] args)
  {
    ip = new Tcpclient();

    if (args.Length < 2)
    {
      Console.WriteLine("usage: tcpecho [options] host port");
      Console.WriteLine("Options: ");
      Console.WriteLine("  -ssl       whether or not to use SSL/TLS (default false)");
      Console.WriteLine("  host       the address of of the remote host");
      Console.WriteLine("  port       the TCP port of the remote host");
      Console.WriteLine("\r\nExample: tcpecho -ssl true 192.168.1.2 21");
    }
    else
    {
      ip.OnConnected += ip_OnConnected;
      ip.OnDataIn += ip_OnDataIn;
      ip.OnDisconnected += ip_OnDisconnected;
      ip.OnError += ip_OnError;
      ip.OnSSLServerAuthentication += ip_OnSSLServerAuthentication;

      try
      {
        // Parse arguments into component.
        ip.RemoteHost = args[args.Length - 2];
        ip.RemotePort = int.Parse(args[args.Length - 1]);

        for (int i = 0; i < args.Length; i++)
        {
          if (args[i].StartsWith("-"))
          {
            if (args[i].Equals("-ssl"))
            {
              ip.SSLEnabled = bool.Parse(args[i + 1]);  // args[i + 1] corresponds to the value of args[i]
            }
          }
        }

        if (ip.SSLEnabled) ip.SSLStartMode = TcpclientSSLStartModes.sslAutomatic;

        // Attempt to connect to the remote server.
        await ip.Connect();

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
              await ip.SendLine(textToSend);
            }
            else
            {
              Console.WriteLine("Please supply the text that you would like to send.");
            }
          }
          else if (arguments[0].Equals("quit"))
          {
            await ip.Disconnect();
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

          Console.Write("tcp> ");
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


