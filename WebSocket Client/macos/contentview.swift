import SwiftUI
import IPWorks

struct ContentView: View, WSClientDelegate {
    func onReadyToSend() {}
    
    func onDataIn(dataFormat: Int32, text: Data, eom: Bool, eol: Bool) {
        let str = String(decoding: text, as: UTF8.self)
        outputRes += "Incoming data: \(str)\n"
    }    
    func onPingResponse() {}
    func onHeader(field: String, value: String) {}    
    func onLog(logLevel: Int32, message: String, logType: String) {}    
    func onRedirect(location: String, accept: inout Bool) {}    
    func onSetCookie(name: String, value: String, expires: String, domain: String, path: String, secure: Bool) {}    
    func onConnected(statusCode: Int32, description: String) {}    
    func onConnectionStatus(connectionEvent: String, statusCode: Int32, description: String) {}    
    func onDisconnected(statusCode: Int32, description: String) {}    
    func onError(errorCode: Int32, description: String) {}       
    func onSSLServerAuthentication(certEncoded: Data, certSubject: String, certIssuer: String, status: String, accept: inout Bool) {}    
    func onSSLStatus(message: String) {}
    
    var client = WSClient()
    
    var documentsPath = NSSearchPathForDirectoriesInDomains(.documentDirectory, .userDomainMask, true)[0] + "/"
    @State private var server: String = "ws://echo.websocket.events/.ws"
    @State private var port: String = ""
    @State private var message: String = ""
    @State private var outputRes: String = ""
    @State private var connected = false
        
    func connectedChange() -> String
    {
      if (connected)
      {
          return "Disconnect"
      }
      else
      {
          return "Connect"
      }
    }
    
    var body: some View {
      VStack(alignment: .center)
      {
        Text("Server:")
          .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading).padding(.horizontal, 10)

        TextField("enter remote host...", text: $server)
          .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading).padding(.horizontal, 10).padding(.bottom, 20)

                    
        connectButton()
        
        Group
        {
          Text("Send message:")
            .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading).padding(.horizontal, 10)

          TextField("enter message...", text: $message)
            .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading).padding(.horizontal, 10).padding(.bottom, 20)
            
      
          sendButton()
          
          Text("Output:")
          TextEditor(text: $outputRes)
            .frame(minWidth: 300, maxWidth: 300, minHeight: 200, maxHeight: 300, alignment: .topLeading)
            .border(Color.black, width: 1)
        }
      }
    }

  @ViewBuilder
  private func connectButton() -> some View {
    Button(action:
    {
      //client.runtimeLicense = ""
      client.delegate = self

      outputRes = ""
      
      do
      {
        if (client.connected == true)
        {
            try client.disconnect()
        }
        else
        {            
            try client.connectTo(url: server)
        }        
        connected = client.connected
      }
      catch
      {
          do
          {
            try client.connectTo(url: server)
          }
          catch {}
          outputRes += "Error: \(error)"
          return
      }
    }, label: {
        Text("\(connectedChange())")
          .font(.system(size: 20))
          .frame(minWidth: 150, minHeight: 40)
          .background(RoundedRectangle(cornerRadius: 8)
          .fill(Color.gray))
    })
    .buttonStyle(PlainButtonStyle())
    .padding(.bottom, 20)
  }

  @ViewBuilder
  private func sendButton() -> some View {
    Button(action:
    {
        do
        {
            
          try client.sendText(text: message)
          outputRes += "Sent: \(message) to server\n"
        }
        catch
        {
          print(error)
          return
        }
    }, label: {
        Text("Send data").font(.system(size: 20))
          .frame(minWidth: 150, minHeight: 40).background(RoundedRectangle(cornerRadius: 8).fill(Color.gray))
    })
    .buttonStyle(PlainButtonStyle())
    .padding(.bottom, 20).disabled(connected == false)
  }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
