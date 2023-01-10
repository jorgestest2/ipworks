import SwiftUI
import IPWorks

struct ContentView: View, TCPClientDelegate {
    func onDataIn(text: Data, eol: Bool) {
        let str = String(decoding: text, as: UTF8.self)
        outputRes += "Incoming data: \(str)"
    }  
    func onConnected(statusCode: Int32, description: String) {}    
    func onConnectionStatus(connectionEvent: String, statusCode: Int32, description: String) {}     
    func onDisconnected(statusCode: Int32, description: String) {}    
    func onError(errorCode: Int32, description: String) {}    
    func onReadyToSend() {}    
    func onSSLServerAuthentication(certEncoded: Data, certSubject: String, certIssuer: String, status: String, accept: inout Bool) {}    
    func onSSLStatus(message: String) {}
    
    var client = TCPClient()    
    var documentsPath = NSSearchPathForDirectoriesInDomains(.documentDirectory, .userDomainMask, true)[0] + "/"
    @State private var server: String = ""
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
          .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading)
          .padding(.horizontal, 10)

        TextField("enter remote host..", text: $server)
          .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading)
          .padding(.horizontal, 10).padding(.bottom, 20).autocapitalization(/*@START_MENU_TOKEN@*/.none/*@END_MENU_TOKEN@*/)
        
        Text("Port:").frame(minWidth: 300, maxWidth: 300, alignment: .topLeading)
          .padding(.horizontal, 10)

        TextField("enter remote port..", text: $port)
          .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading)
          .padding(.horizontal, 10).padding(.bottom, 20)
          .autocapitalization(/*@START_MENU_TOKEN@*/.none/*@END_MENU_TOKEN@*/)
                  
        connectButton()        
        Group
        {
          Text("Send message:")
            .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading)
            .padding(.horizontal, 10)

          TextField("enter message...", text: $message)
            .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading)
            .padding(.horizontal, 10).padding(.bottom, 20)
            .autocapitalization(/*@START_MENU_TOKEN@*/.none/*@END_MENU_TOKEN@*/)
       
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
              try client.connectTo(host: server, port: Int32(port) ?? 777)
          }                    
          connected = client.connected
        }
        catch
        {
          do
          {
              try client.connectTo(host: server, port: Int32(port) ?? 777)
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
    .buttonStyle(PlainButtonStyle()).padding(.bottom, 20)
  }

  @ViewBuilder
  private func sendButton() -> some View {
    Button(action:
    {
      do
      {                        
        try client.sendLine(text: message)
        outputRes += "Sent: \(message) to server\n"
      }
      catch
      {
        print(error)
        return
      }
    }, label: {
        Text("Send data")
          .font(.system(size: 20))
          .frame(minWidth: 150, minHeight: 40)
          .background(RoundedRectangle(cornerRadius: 8)
          .fill(Color.gray))
    })
    .buttonStyle(PlainButtonStyle()).padding(.bottom, 20).disabled(connected == false) 
  }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
