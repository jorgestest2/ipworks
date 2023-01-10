/*
 * IPWorks 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorks;

class netcodeDemo
{
  private static Netcode netcode = new Netcode();

  static async Task Main(string[] args)
  {
    Console.Write("Would you like to encode[1] a message or decode[2]?\n> ");
    string mode = Console.ReadLine();
    bool encode = mode.ToLower() == "1" || mode.ToLower() == "encode";
    Console.WriteLine("What encoding would you like to use?");
    foreach (NetcodeFormats format in Enum.GetValues(typeof(NetcodeFormats)))
    {
      Console.WriteLine("{0}: {1}", ((int)format), format);
    }
    Console.Write("> ");
    netcode.Format = (NetcodeFormats)int.Parse(Console.ReadLine());
    Console.Write("What message would you like to " + (encode ? "encode" : "decode") + "?\n> ");
    string message = Console.ReadLine();
    if (encode)
    {
      netcode.DecodedData = message;
      await netcode.Encode();
      Console.WriteLine("Encoded message:\n{0}", netcode.EncodedData);
    } 
    else
    {
      netcode.EncodedData = message;
      await netcode.Decode();
      Console.WriteLine("Decoded message:\n{0}", netcode.DecodedData);
    }
  }
}


