/*
 * IPWorks 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using nsoftware.async.IPWorks;


class webdavDemo
{
  private static Webdav webdav;

  private static void webdav_OnSSLServerAuthentication(object sender, WebdavSSLServerAuthenticationEventArgs e)
  {
    e.Accept = true; // This will trust all certificates and is not recommended for production use.
  }

  private static void webdav_OnConnected(object sender, WebdavConnectedEventArgs e)
  {
    Console.WriteLine("Server connected");
  }

  private static void webdav_OnConnectionStatus(object sender, WebdavConnectionStatusEventArgs e)
  {
    Console.WriteLine("Status code " + e.StatusCode + ": " + e.Description);
  }

  private static void webdav_OnDisconnected(object sender, WebdavDisconnectedEventArgs e)
  {
    Console.WriteLine("Server disconnected");
  }

  private static void webdav_OnTransfer(object sender, WebdavTransferEventArgs e)
  {
    Console.WriteLine("Resource being received from server (in full text): \n" +
                                    "========================================= \n" + e.Text);
  }

  static async Task Main(string[] args)
  {
    webdav = new Webdav();

    if (args.Length < 2)
    {
      Console.WriteLine("usage: webdav [options] username password");
      Console.WriteLine("Options: ");
      Console.WriteLine("  ");
      Console.WriteLine("  username   the username to login");
      Console.WriteLine("  password   the password to login");
      Console.WriteLine("\r\nExample: webdav username password");
    }
    else
    {
      webdav.OnConnected += webdav_OnConnected;
      webdav.OnConnectionStatus += webdav_OnConnectionStatus;
      webdav.OnDisconnected += webdav_OnDisconnected;
      webdav.OnSSLServerAuthentication += webdav_OnSSLServerAuthentication;
      webdav.OnTransfer += webdav_OnTransfer;

      try
      {
        // Parse arguments into component.
        webdav.User = args[args.Length - 2];
        webdav.Password = args[args.Length - 1];

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
            Console.WriteLine("  ?                                      display the list of valid commands");
            Console.WriteLine("  help                                   display the list of valid commands");
            Console.WriteLine("  make <resource uri>                    make a new directory at the specified location (ex. make localhost:443/directoryName)");
            Console.WriteLine("  move <source uri> <destination uri>    move a specified resource to a new location (ex. move localhost:443/oldFolder/file.txt localhost:443/newFolder/file.txt)");
            Console.WriteLine("  get <resource uri>                     get a specified resource");
            Console.WriteLine("  delete <resource uri>                  delete a specified resource");
            Console.WriteLine("  put <local file> <resource uri>        send data to the server");
            Console.WriteLine("  quit                                   exit the application");
          }
          else if (arguments[0].Equals("make"))
          {
            if (arguments.Length > 1) await webdav.MakeDirectory(arguments[1]);
            else Console.WriteLine("Please specify a resource URI.");
          }
          else if (arguments[0].Equals("move"))
          {
            if (arguments.Length > 2) await webdav.MoveResource(arguments[1], arguments[2]);
            else Console.WriteLine("Please specify a source and destination URI.");
          }
          else if (arguments[0].Equals("get"))
          {
            if (arguments.Length > 1) await webdav.GetResource(arguments[1]);
            else Console.WriteLine("Please specify a resource URI.");
          }
          else if (arguments[0].Equals("delete"))
          {
            if (arguments.Length > 1) await webdav.DeleteResource(arguments[1]);
            else Console.WriteLine("Please specify a resource URI.");
          }
          else if (arguments[0].Equals("put"))
          {
            if (arguments.Length > 2)
            {
              webdav.LocalFile = arguments[1];
              await webdav.PutResource(arguments[2]);
            }
            else
            {
              Console.WriteLine("Please specify a local file and resource URI.");
            }
          }
          else if (arguments[0].Equals("quit"))
          {
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
          Console.Write("webdav> ");
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

