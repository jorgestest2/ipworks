#
# IPWorks 2022 Python Edition - Demo Application
#
# Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
#

import sys
import string
from ipworks import *

input = sys.hexversion<0x03000000 and raw_input or input

def fireError(e):
  print("ERROR: " %e.message)

def fireSSLServerAuthentication(e):
  e.accept = True

def fireDataIn(e):
  global dataReceived
  dataReceived = True
  print ("Receive the data: " + bytes.decode(e.text))

print("*****************************************************************")
print("* This is a demo to show how to connect to a web socket echo    *")
print("* serverto send data, and receive the response.                 *")
print("*****************************************************************")

dataReceived = False
url = input("Echo Server (ws://localhost:4444): ")
if url == "":
  url = "ws://localhost:4444"

try:
  websocketclient = WSClient()
  websocketclient.set_timeout(10)
  websocketclient.on_data_in = fireDataIn
  websocketclient.on_ssl_server_authentication = fireSSLServerAuthentication

  print(url)
  websocketclient.connect_to(url)
  while True: 
    command = int(input("Please input command: 1 [Send Data] or  2 [Exit]: "))
    if command == 1:
      send = input("Please enter data to send: ")
      websocketclient.send(send)
      dataReceived = False
      print("Now waiting for response...")
      while dataReceived == False:
        websocketclient.do_events()
    else:
      websocketclient.disconnect()
      break
      
except IPWorksError as e:
  print("ERROR: " + e.message)

