import SwiftUI
import IPWorks

struct ContentView: View, SMTPDelegate {
    func onConnectionStatus(connectionEvent: String, statusCode: Int32, description: String) {}    
    func onEndTransfer(direction: Int32) {}    
    func onError(errorCode: Int32, description: String) {}    
    func onExpand(address: String) {}    
    func onPITrail(direction: Int32, message: String) {}    
    func onSSLServerAuthentication(certEncoded: Data, certSubject: String, certIssuer: String, status: String, accept: inout Bool) {}    
    func onSSLStatus(message: String) {}    
    func onStartTransfer(direction: Int32) {}    
    func onTransfer(direction: Int32, bytesTransferred: Int64, percentDone: Int32, text: Data) {}
    
    var client = SMTP()    
    var documentsPath = NSSearchPathForDirectoriesInDomains(.documentDirectory, .userDomainMask, true)[0] + "/"    
    @State private var host: String = ""
    @State private var port: String = "25"
    @State private var tlsType = 0
    @State private var login: String = ""
    @State private var password: String = ""
    @State private var from: String = ""
    @State private var to: String = ""
    @State private var subject: String = ""
    @State private var plainText: String = ""
    
    func tlsTypeChange(_ tag: Int) {
      if (tag == 2)
      {
        if (port == "25" || port == "587")
        {
            port = "465"
        }
      }
      else if (tag == 1)
      {
        if (port == "25" || port == "465")
        {
            port = "587"
        }
      }
      else
      {
        if (port == "465" || port == "587")
        {
            port = "25"
        }
      }
    }
    
    var body: some View {
      VStack(alignment: .center)
      {
        HStack(alignment: .top)
        {
            Text("Host:").frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
              .padding(.bottom, 20)

            TextField("Enter server host", text: $host)
              .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)
              .autocapitalization(/*@START_MENU_TOKEN@*/.none/*@END_MENU_TOKEN@*/)

        }
        
        HStack(alignment: .firstTextBaseline)
        {
            Text("Port:").frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
              .padding(.bottom, 20)

            TextField("Enter server port", text: $port)
              .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)
              .autocapitalization(/*@START_MENU_TOKEN@*/.none/*@END_MENU_TOKEN@*/)

        }
        
        HStack(alignment: .firstTextBaseline)
        {
            Text("Use TLS:")
              .frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
              .padding(.bottom, 20)
            Picker(selection: $tlsType, label: Text("Use TLS")) {
                Text("No").tag(0)
                Text("Explicit").tag(1)
                Text("Implicit").tag(2)
            }
            .pickerStyle(SegmentedPickerStyle())
            .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)
            .onChange(of: tlsType, perform: tlsTypeChange)
        }
        
        HStack(alignment: .firstTextBaseline)
        {
          Text("Login:").frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
            .padding(.bottom, 20)

          TextField("Enter username", text: $login)
            .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)
            .autocapitalization(/*@START_MENU_TOKEN@*/.none/*@END_MENU_TOKEN@*/)

        }

        HStack(alignment: .firstTextBaseline)
        {
          Text("Password:")
            .frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
            .padding(.bottom, 40)

          SecureField("Enter password", text: $password)
            .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)
            .autocapitalization(/*@START_MENU_TOKEN@*/.none/*@END_MENU_TOKEN@*/)

        }
        
        Group
        {
          HStack(alignment: .top)
          {
            Text("From:").frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
              .padding(.bottom, 20)

            TextField("Enter 'from' address", text: $from)
              .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)
              .autocapitalization(/*@START_MENU_TOKEN@*/.none/*@END_MENU_TOKEN@*/)

          }
          
          HStack(alignment: .firstTextBaseline)
          {
            Text("To:").frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
              .padding(.bottom, 20)

            TextField("Enter 'to' address", text: $to)
              .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)
              .autocapitalization(/*@START_MENU_TOKEN@*/.none/*@END_MENU_TOKEN@*/)

          }
          
          HStack(alignment: .firstTextBaseline)
          {
            Text("Subject:")
              .frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
              .padding(.bottom, 20)

            TextField("Enter subject", text: $subject)
              .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)
              .autocapitalization(/*@START_MENU_TOKEN@*/.none/*@END_MENU_TOKEN@*/)

          }         
      
          Text("Plain text:")
            .frame(minWidth: 300, maxWidth: 300, alignment: .topLeading)
            .padding(.horizontal, 10)
          TextEditor(text: $plainText)
            .frame(minWidth: 300, maxWidth: 300, minHeight: 200, maxHeight: 300, alignment: .topLeading)
            .border(Color.black, width: 1)
            .padding(.horizontal, 10)
        }          
        sendButton()
      }
    }

  @ViewBuilder
  private func sendButton() -> some View {
    Button(action:
    {
      //client.runtimeLicense = ""
      client.delegate = self
      
      do
      {
        client.sendTo = to                
        client.subject = subject        
        client.message = plainText
                
        switch (tlsType) {
          case 1:
            client.sslStartMode = SmtpSSLStartModes.sslExplicit
            break
          case 2:
            client.sslStartMode = SmtpSSLStartModes.sslImplicit
            break
          default:
            client.sslStartMode = SmtpSSLStartModes.sslNone
            break
        }
        
        client.user = login
        client.password = password        
        client.mailServer = host
        client.mailPort = Int32(port) ?? 587
        client.from = login
        try client.connect()        
        try client.send()        
        try client.disconnect()        
        print("Message has been sent successfully")
      }
      catch
      {
        print(error)        
        do {
            try client.disconnect()
        } catch {}
        return
      }
    }, label: {
      Text("Send")
        .font(.system(size: 20))
        .frame(minWidth: 150,  minHeight: 40)
        .background(RoundedRectangle(cornerRadius: 8)
        .fill(Color.gray))
    })
    .buttonStyle(PlainButtonStyle())
  }

}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
