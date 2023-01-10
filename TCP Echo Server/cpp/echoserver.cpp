/*
 * IPWorks 2022 C++ Edition - Demo Application
 *
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 *
 */


#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <string.h>

#include "../../include/ipworks.h"
#define LINE_LEN 80

class MyTCPServer : public TCPServer
{
	int FireConnected(TCPServerConnectedEventParams *e)
	{
		printf("%s has connected.\n", this->GetRemoteHost(e->ConnectionId));
		return 0;
	}

	int FireDisconnected(TCPServerDisconnectedEventParams *e)
	{
		printf("Disconnected from %s.\n", this->GetRemoteHost(e->ConnectionId));
		return 0;
	}

	int FireDataIn(TCPServerDataInEventParams *e)
	{
		printf("%s - Echoing '%s' back to client.\n", this->GetRemoteHost(e->ConnectionId), e->Text);
		this->SendText(e->ConnectionId, e->Text);
		return 0;
	}
};

int main(int argc, char* argv[])
{
	MyTCPServer tcpserver;

	char buffer[LINE_LEN];

	printf("*****************************************************************\n");
	printf("* This demo shows how to set up an echo server using TCPServer. *\n");
	printf("*****************************************************************\n");

	if (argc < 2) {

		fprintf(stderr, "usage: echoserver port [pfxfile password]\n");
		fprintf(stderr, "\n");
		fprintf(stderr, "  port      the TCP port in the local host where the component listens\n");
		fprintf(stderr, "  pfxfile   PFX File to use for server certificate. If set SSL will be used.\n");
		fprintf(stderr, "  password  PFX File Password\n");
		fprintf(stderr, "\nExample (Plaintext): echoserver 777\n");
		fprintf(stderr, "\nExample (SSL):       echoserver 777 test.pfx password\n\n");
		printf("Press enter to continue.");
		getchar();

	}
	else{
		tcpserver.SetLocalPort(atoi(argv[1]));

		if (argc > 2) //PFXFile is specified. Use SSL
		{
			tcpserver.SetSSLCertStoreType(CST_PFXFILE);
			tcpserver.SetSSLCertStore(argv[2], strlen(argv[2]));
			tcpserver.SetSSLCertStorePassword(argv[3]);
			//The default value of "*" picks the first private key in the certificate. For simplicity this demo will use that value.
			tcpserver.SetSSLCertSubject("*");
			tcpserver.SetSSLStartMode(SSL_IMPLICIT);
		}

		int ret_code = tcpserver.StartListening();

		if (ret_code)
		{
			printf("Error: %i - %s\n", ret_code, tcpserver.GetLastError());
			goto done;
		}

		printf("Listening...\n");

		while (true)
		{
			tcpserver.DoEvents();
		}

	done:
		if (tcpserver.GetListening())
		{
			tcpserver.StartListening();
		}

		printf("Exiting... (press enter)\n");
		getchar();

		return 0;
	}

}

 

