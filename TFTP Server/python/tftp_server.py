#
# IPWorks 2022 Python Edition - Demo Application
#
# Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
#

import sys
import string
from ipworks import *

input = sys.hexversion<0x03000000 and raw_input or input

print("********************************************************************************\n")
print("* This demo shows how to set up a TFTP server using the TFTPServer component.  *\n")
print("********************************************************************************\n")

try:
  server = TFTPServer()

  def fireError(e):
    print(e.description)

  def fireLog(e):
    print("Log: " + e.message)

  def fireConnected(e):
    print("%d: connected." % e.connection_id)

  def fireConnectionRequest(e):
    print(e.remote_host + ": attempting to connect.")

  def fireDisconnected(e):
    print("%d: disconnected." % e.connection_id)

  def fireStartTransfer(e):
    print("%d: started a transfer." % e.connection_id)

  def fireEndTransfer(e):
    print("%d: transfer complete." % e.connection_id)

  def fireTransfer(e):
    print("%d: transferring data." % e.connection_id)

  server.on_connected = fireConnected
  server.on_connection_request = fireConnectionRequest
  server.on_disconnected = fireDisconnected
  server.on_start_transfer = fireStartTransfer
  server.on_transfer = fireTransfer
  server.on_end_transfer = fireEndTransfer
  server.on_error = fireError

  server.set_local_dir(input("Local Directory: "))

  server.start_listening()

  print("Listening...")
  print("Press Ctrl-C to shutdown")

  while True:
    server.do_events()

except IPWorksTftpserverError as e:
  print("ERROR: " + e.message)

except KeyboardInterrupt:
  print("Shutdown requested...exiting")
  server.stop_listening()


