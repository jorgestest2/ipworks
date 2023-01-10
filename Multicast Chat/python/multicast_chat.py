#
# IPWorks 2022 Python Edition - Demo Application
#
# Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
#

import sys
import string
from ipworks import *

input = sys.hexversion<0x03000000 and raw_input or input

import sys
import string
from ipworks import *

input = sys.hexversion < 0x03000000 and raw_input or input

if len(sys.argv) < 4:
  print("usage: multicast_chat.py [options]")
  print("Options: ")
  print("  -n          Your name")
  print("  -group      the multicast group")

  print(
    "\r\nExample: multicast_chat.py -n User1 -group 231.31.31.31")
else:
  m_cast = MCast()
  m_cast.set_local_port(3333)
  m_cast.set_remote_port(3333)
  m_cast.set_active(True)
    
  name = "Unspecified"

  try:
    for i in range(0, len(sys.argv)):
      if sys.argv[i].startswith("-"):
        if sys.argv[i] == "-n":
          name = sys.argv[i + 1];  # args[i+1] corresponds to the value of argument [i]
        if sys.argv[i] == "-group":
          m_cast.set_multicast_group(sys.argv[i + 1])
          m_cast.set_remote_host(sys.argv[i + 1])
                
    m_cast.set_data_to_send(name + ": " + "joining discussion...")
    print(name + ": " + "joining discussion...")

    print("Enter message and press enter to send it!")
    
    while(True):
      message = input(name + ": ")
      if(message == "exit"):  # this message will leave the chat
          m_cast.set_active(False)
          break
      m_cast.set_data_to_send(name + ": " + message)

  except IPWorksMcastError as e:
    print("ERROR: %s" % e.message)

    


