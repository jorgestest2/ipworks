using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorks;

class httpurlDemo {
  private static Http http1 = new nsoftware.async.IPWorks.Http();

  private static void http1_OnSSLServerAuthentication(object sender, nsoftware.async.IPWorks.HttpSSLServerAuthenticationEventArgs e) {
    if (e.Accept) return;
    Console.Write("Server provided the following certificate:\nIssuer: " + e.CertIssuer + "\nSubject: " + e.CertSubject + "\n");
    Console.Write("The following problems have been determined for this certificate: " + e.Status + "\n");
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.Read() == 'y') e.Accept = true;
  }

  private static void http1_OnTransfer(object sender, HttpTransferEventArgs e) {
    Console.WriteLine(e.Text);
  }

  static async Task Main(string[] args) {
    if (args.Length < 1) {
      Console.WriteLine("usage: httpurl url\n");
      Console.WriteLine("  url  the url to fetch");
      Console.WriteLine("\nExample: httpurl https://www.google.com\n");
      Console.WriteLine("Press enter to continue.");
      Console.Read();
    } else {

      http1.OnTransfer += http1_OnTransfer;
      http1.OnSSLServerAuthentication += http1_OnSSLServerAuthentication;


      try {
        await http1.Get(args[args.Length - 1]);
      } catch (Exception ex) {
        Console.WriteLine("Error: " + ex.Message);
      }
      Console.WriteLine("\npress <return> to continue...");
      Console.Read();
    }
  }
}
