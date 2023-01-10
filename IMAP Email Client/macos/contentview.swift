import SwiftUI
import IPWorks

struct ContentView: View, IMAPDelegate {
  func onConnectionStatus(connectionEvent: String, statusCode: Int32, description: String) {}    
  func onEndTransfer(direction: Int32) {}    
  func onError(errorCode: Int32, description: String) {}    
  func onHeader(field: String, value: String) {}    
  func onIdleInfo(message: String, cancel: inout Bool) {}    
  func onMailboxACL(mailbox: String, user: String, rights: String) {}    
  func onMailboxList(mailbox: String, separator: String, flags: String) {}    
  func onMessageInfo(messageId: String, subject: String, messageDate: String, from: String, flags: String, size: Int64) {
      mesList += "    Message# \(messageId) \n"
      mesList += "        From: \(from)) \n"
      mesList += "        Subject: \(subject) \n"
      mesList += "        Date: \(messageDate) \n"
      mesList += "        Size: \(size) \n"
  }    
  func onMessagePart(partId: String, size: Int64, contentType: String, filename: String, contentEncoding: String, parameters: String, multipartMode: String, contentId: String, contentDisposition: String) {}    
  func onPITrail(direction: Int32, message: String) {
      print(message)
  }    
  func onSSLServerAuthentication(certEncoded: Data, certSubject: String, certIssuer: String, status: String, accept: inout Bool) {
      accept = true
  }    
  func onSSLStatus(message: String) {}    
  func onStartTransfer(direction: Int32) {}    
  func onTransfer(direction: Int32, bytesTransferred: Int64, percentDone: Int32, text: String) {}
  
  var client = IMAP()    
  var documentsPath = NSSearchPathForDirectoriesInDomains(.documentDirectory, .userDomainMask, true)[0] + "/"    
  @State private var connected = false
  @State private var host: String = ""
  @State private var port: String = "143"
  @State private var tlsType = 0
  @State private var login: String = ""
  @State private var password: String = ""
  @State private var selectedMailbox: String = ""
  @State private var mesList: String = ""
          

  private let mailboxPickerStyle = PopUpButtonPickerStyle()

  
  func tlsTypeChange(_ tag: Int) {
    if (tag == 2)
    {
      if (port == "143")
      {
          port = "993"
      }
    }
    else
    {
      if (port == "993")
      {
          port = "143"
      }
    }
  }
  
  var body: some View {
    VStack(alignment: .center)
    {
      HStack(alignment: .top)
      {
        Text("Host:")
          .frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
          .padding(.bottom, 20)

        TextField("Enter server host", text: $host)
          .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)

      }
      
      HStack(alignment: .firstTextBaseline)
      {
        Text("Port:")
          .frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
          .padding(.bottom, 20)

        TextField("Enter server port", text: $port)
          .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)

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
        Text("Login:")
          .frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
          .padding(.bottom, 20)

        TextField("Enter username", text: $login)
          .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)

      }

      HStack(alignment: .firstTextBaseline)
      {
        Text("Password:")
          .frame(minWidth: 80, maxWidth: 80, alignment: .topLeading)
          .padding(.bottom, 20)

        SecureField("Enter password", text: $password)
          .frame(minWidth: 220, maxWidth: 220, alignment: .topLeading)
          
      }
        
        
      HStack(alignment: .firstTextBaseline)
      {
        connectButton()
          
        disconnectButton()
      }.padding(.bottom, 40)
      
      Section {
          Picker("Mailboxes", selection: $selectedMailbox) {
              ForEach((0..<client.mailboxList.count), id: \.self) {
                  Text(client.mailboxList[Int(Int32($0))].name)
              }
          }
          .pickerStyle(mailboxPickerStyle)
          .frame(minWidth: 300, maxWidth: 300, minHeight: 40, maxHeight: 40)
      }
      .frame(minWidth: 300, maxWidth: 300, minHeight: 40, maxHeight: 40)
      .padding(.horizontal, 10)
      .padding(.vertical, 40)
      .disabled(connected == false)
          
      receiveButton()

      TextEditor(text: $mesList)
        .frame(minWidth: 300, maxWidth: 300, minHeight: 200, maxHeight: 300, alignment: .topLeading)
        .border(Color.black, width: 1)
    }
  }

  @ViewBuilder
  private func connectButton() -> some View {
    Button(action:
    {
        //client.runtimeLicense = ""
        client.delegate = self
        do
        {
          switch (tlsType) {
            case 1:
              client.sslStartMode = ImapSSLStartModes.sslExplicit
              break
            case 2:
              client.sslStartMode = ImapSSLStartModes.sslImplicit
              break
            default:
              client.sslStartMode = ImapSSLStartModes.sslNone
              break
          }      
          client.user = login
          client.password = password
          client.authMechanism = ImapAuthMechanisms.amUserPassword
          client.mailServer = host
          client.mailPort = Int32(port) ?? 993                    
          try client.connect()
        }
        catch
        {
          print(error)
          do {
              try client.disconnect()
          } catch {}
          return
        }                
        do
        {
          try client.listMailboxes()                    
          if (client.mailboxList.count == 0)
          {
          }
          else
          {
              selectedMailbox = "INBOX"                            
              connected = true
          }
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
      Text("Connect")
        .font(.system(size: 20))
        .frame(minWidth: 150, minHeight: 40)
        .background(RoundedRectangle(cornerRadius: 8)
        .fill(Color.gray))
    })
    .buttonStyle(PlainButtonStyle())
    .disabled(connected == true)
  }

  @ViewBuilder
  private func disconnectButton() -> some View {
    Button(action:
    {    
      do
      {
        try client.disconnect()        
        connected = false
        mesList = ""
      }
      catch
      {
        print(error)
        return
      }
    }, label: {
        Text("Disconnect")
          .font(.system(size: 20))
          .frame(minWidth: 150, minHeight: 40)
          .background(RoundedRectangle(cornerRadius: 8)
          .fill(Color.gray))
    })
    .buttonStyle(PlainButtonStyle())
    .disabled(connected == false)
  }

  @ViewBuilder
  private func receiveButton() -> some View{
    Button(action:
    {
      mesList = ""      
      do
      {
        client.mailbox = selectedMailbox
        try client.selectMailbox()        
        mesList += "Name: \(client.mailbox) \n"
        mesList += "Total Messages: \(client.messageCount) \n"        
        mesList += "\nReceiving 10 messages... \n"        
        mesList += "\nList of messages: \n"        
        client.messageSet = "1:10"
        try client.fetchMessageInfo()        
      }
      catch
      {
        print(error)
        return
      }
    }, label: {
      Text("Receive")
        .font(.system(size: 20))
        .frame(minWidth: 150,  minHeight: 40)
        .background(RoundedRectangle(cornerRadius: 8)
        .fill(Color.gray))
    })
    .buttonStyle(PlainButtonStyle())
    .padding(.bottom, 20)
    .disabled(connected == false)
  }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
