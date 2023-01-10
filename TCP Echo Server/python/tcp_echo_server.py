#
# IPWorks 2022 Python Edition - Demo Application
#
# Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
#

import sys
import string
from ipworks import *

input = sys.hexversion<0x03000000 and raw_input or input

print("*****************************************************************\n")
print("* This demo shows how to set up an echo server using TCPServer.  *\n")
print("*****************************************************************\n")

port = int(input("Local Port: "))

try:
  tcpserver = TCPServer()
  tcpserver.set_local_port(port)
  tcpserver.set_listening(True)
  print("Listening...")
  print("Press Ctrl-C to shutdown")

  def fireConnected(e):
    global tcpserver
    print(tcpserver.get_remote_host(e.connection_id) + " has connected.")

  def fireDisconnected(e):
    global tcpserver
    print("Disconnected from %s" % tcpserver.get_remote_host(e.connection_id))

  def fireDataIn(e):
    global tcpserver
    print("Received a message from %s and the message is: %s" % (tcpserver.get_remote_host(e.connection_id), e.text.decode("utf-8")))
    print("Echoing the message back...")
    tcpserver.send(e.connection_id, e.text)

  tcpserver.on_connected = fireConnected
  tcpserver.on_disconnected = fireDisconnected
  tcpserver.on_data_in = fireDataIn

  while True:
    tcpserver.do_events()

except IPWorksError as e:
  print("ERROR: %s" % e.message)

except KeyboardInterrupt:
  print("Shutdown requested...exiting")
  tcpserver.shutdown()




