#
# IPWorks 2022 Python Edition - Demo Application
#
# Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
#

import sys
import string
from ipworks import *

input = sys.hexversion<0x03000000 and raw_input or input

whois = Whois()
domain=input("Enter a Domain (microsoft.com): ")
if domain == "": domain = "microsoft.com"
server=input("Whois server (whois.iana.org): ")
if server == "": server = "whois.iana.org"
whois.set_server(server)
whois.query(domain)
print(whois.get_domain_info())


