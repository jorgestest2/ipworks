import SwiftUI
import IPWorks

struct ContentView: View, HTTPDelegate {
    
    var client = HTTP()

    var documentsPath = NSSearchPathForDirectoriesInDomains(.documentDirectory, .userDomainMask, true)[0] + "/"
    @State private var url: String = "https://www.google.com"
    @State private var output: String = ""
        
    var body: some View {
        VStack(alignment: .center)
        {
          Text("URL:")
            .padding(.top, 100)
            .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading)
            .padding(.horizontal, 10)

          TextField("Enter url", text: $url)
            .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading)
            .padding(.horizontal, 10).padding(.bottom, 20)
  
          getButton()
          Text("Output:")
          TextEditor(text: $output)
            .frame(minWidth: 300, maxWidth: 300, minHeight: 200, maxHeight: 300, alignment: .topLeading)
            .border(Color.black, width: 1)
        }
    }

    @ViewBuilder
    private func getButton() -> some View {
      Button(action: {
        //client.runtimeLicense = ""
        client.delegate = self
        output = ""                
        do
        {                    
          try client.get(url: url)
          output += client.transferredData
        }
        catch
        {
          output = "Error: \(error)"
          return
        }
      } , label: {
        Text("Get").font(.system(size: 20))
          .frame(minWidth: 120, minHeight: 40)
          .background(RoundedRectangle(cornerRadius: 8).fill(Color.gray))
      })
      .buttonStyle(PlainButtonStyle()).padding(.bottom, 20)
    }
    
    func onConnected(statusCode: Int32, description: String) { }    
    func onConnectionStatus(connectionEvent: String, statusCode: Int32, description: String) { }    
    func onDisconnected(statusCode: Int32, description: String) { }    
    func onEndTransfer(direction: Int32) { }    
    func onError(errorCode: Int32, description: String) { }    
    func onHeader(field: String, value: String) { }    
    func onLog(logLevel: Int32, message: String, logType: String) { }    
    func onRedirect(location: String, accept: inout Bool) { }    
    func onSetCookie(name: String, value: String, expires: String, domain: String, path: String, secure: Bool) { }    
    func onSSLServerAuthentication(certEncoded: Data, certSubject: String, certIssuer: String, status: String, accept: inout Bool) { }    
    func onSSLStatus(message: String) { }    
    func onStartTransfer(direction: Int32) { }    
    func onStatus(httpVersion HTTPVersion: String, statusCode: Int32, description: String) { }    
    func onTransfer(direction: Int32, bytesTransferred: Int64, percentDone: Int32, text: Data) { }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
