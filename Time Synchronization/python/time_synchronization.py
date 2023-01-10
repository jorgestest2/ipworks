#
# IPWorks 2022 Python Edition - Demo Application
#
# Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
#

import sys
import string
from ipworks import *

input = sys.hexversion<0x03000000 and raw_input or input

import time
if len(sys.argv) != 2:
  print("usage: time_synchronization.py server")
  print("")
  print("  server  the time server from which to request the time")
  print("\r\nExample: time_synchronization.py time.nist.gov")
else:
  myclock = NetClock()
  myclock.set_time_server(sys.argv[1])
  myclock.get_time()
  print("System date and time: " +time.strftime("%m/%d/%Y %H:%M:%S GMT", time.gmtime()))
  print("Internet date and time: "+myclock.get_local_time())




