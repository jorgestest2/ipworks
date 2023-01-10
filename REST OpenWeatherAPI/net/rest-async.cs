/*
 * IPWorks 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

﻿using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorks;

class restDemo
{
  private static Netcode netcode;
  private static Rest rest;

  private static void rest_OnSSLServerAuthentication(object sender, RestSSLServerAuthenticationEventArgs e)
  {
    if (e.Accept) return;
    Console.Write("Server provided the following certificate:\nIssuer: " + e.CertIssuer + "\nSubject: " + e.CertSubject + "\n");
    Console.Write("The following problems have been determined for this certificate: " + e.Status + "\n");
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.Read() == 'y') e.Accept = true;
  }

  static async Task Main(string[] args)
  {
    netcode = new Netcode();
    rest = new Rest();

    if (args.Length < 3)
    {
      Console.WriteLine("usage: rest key city state country");
      Console.WriteLine("  key      API key required for authentication (free and available at https://home.openweathermap.org/users/sign_up)");
      Console.WriteLine("  city     the city of the address for which to get weather data (underscore spaces in city names)");
      Console.WriteLine("  state    the state of the address for which to get weather data (only for the US)");
      Console.WriteLine("  country  the country of the address for which to get weather data (use ISO 3166 country codes)");
      Console.WriteLine("Further input documentation can be found at https://openweathermap.org/api/geocoding-api.");
      Console.WriteLine("\r\nExample: rest da9bd73746219f432ddb52abf6b3b087 Chapel_Hill NC US");
      Console.WriteLine("Example: rest da9bd73746219f432ddb52abf6b3b087 London GB");
    }
    else
    {
      rest.OnSSLServerAuthentication += rest_OnSSLServerAuthentication;

      try
      {
        // Parse arguments.
        string apiKey;
        string address;

        if (args.Length > 3 && args[args.Length - 1].Equals("US"))
        {
		  apiKey = args[args.Length - 4];
          address = args[args.Length - 3].Replace("_", " ") + "," + args[args.Length - 2] + ",US";
        }
        else if (args.Length > 2 && !args[args.Length - 1].Equals("US"))
        {
		  apiKey = args[args.Length - 3];
          address = args[args.Length - 2].Replace("_", " ") + "," + args[args.Length - 1];
        }
        else
        {
          throw new Exception("Invalid address provided.  Input documentation can be found at https://openweathermap.org/api/geocoding-api.");
        }
		
        // Geocode the address to retrieve its latitude and longitude.
        netcode.Format = NetcodeFormats.fmtURL;
        netcode.DecodedData = address;
        await netcode.Encode();
        address = netcode.EncodedData;

        await rest.Get("http://api.openweathermap.org/geo/1.0/direct?q=" + address + "&appid=" + apiKey);

        string latitude, longitude;
        rest.XPath = "/json/[1]/lat";
        latitude = rest.XText;
        rest.XPath = "../lon";
        longitude = rest.XText;

        // If you wish to see the entire geocoding REST response, uncomment the line below.
        //Console.WriteLine(rest.TransferredData);

        // Retrieve the weather at those coordinates.
        await rest.Get("http://api.openweathermap.org/data/2.5/weather?lat=" + latitude + "&lon=" + longitude + "&appid=" + apiKey);

        // If you wish to see the entire weather REST response, uncomment the line below.
        //Console.WriteLine(rest.TransferredData);

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
            Console.WriteLine("  clouds                       display information relating to the cloud cover");
            Console.WriteLine("  conditions                   display the main weather conditions and a description");
            Console.WriteLine("  temperature                  display information relating to the current temperature");
            Console.WriteLine("  visibility                   display information relating to the visibility");
            Console.WriteLine("  wind                         display information relating to the wind conditions");
            Console.WriteLine("  quit                         exit the application");
          }
          else if (arguments[0].Equals("clouds"))
          {
            rest.XPath = "/json/clouds/all";
            Console.WriteLine("\tCloud Cover:  " + rest.XText + "%");
          }
          else if (arguments[0].Equals("conditions"))
          {
            rest.XPath = "/json/weather/[1]/main";
            Console.WriteLine("\tMain Weather Conditions:  " + rest.XText);
            rest.XPath = "../description";
            Console.WriteLine("\tDescription:  " + rest.XText);
          }
          else if (arguments[0].Equals("temperature"))
          {
            rest.XPath = "/json/main/temp";
            Console.WriteLine("\tCurrent Temperature:  " + rest.XText + " K");
            rest.XPath = "../feels_like";
            Console.WriteLine("\tFeels Like:  " + rest.XText + " K");
            rest.XPath = "../temp_min";
            Console.WriteLine("\tMinimum Temperature:  " + rest.XText + " K");
            rest.XPath = "../temp_max";
            Console.WriteLine("\tMaximum Temperature:  " + rest.XText + " K");
            rest.XPath = "../pressure";
            Console.WriteLine("\tPressure:  " + rest.XText + " hPa");
            rest.XPath = "../humidity";
            Console.WriteLine("\tHumidity:  " + rest.XText + "%");
          }
          else if (arguments[0].Equals("visibility"))
          {
            rest.XPath = "/json/visibility";
            Console.WriteLine("\tVisibility:  " + rest.XText);
          }
          else if (arguments[0].Equals("wind"))
          {
            rest.XPath = "/json/wind/speed";
            Console.WriteLine("\tWind Speed:  " + rest.XText + " m/s");
            rest.XPath = "../deg";
            Console.WriteLine("\tDegrees:  " + rest.XText + " degrees");
            rest.XPath = "../gust";
            Console.WriteLine("\tGusts:  " + rest.XText + " m/s");
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

          Console.Write("rest> ");
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

