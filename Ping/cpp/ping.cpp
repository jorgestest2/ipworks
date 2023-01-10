/*
 * IPWorks 2022 C++ Edition - Demo Application
 *
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 *
 */


#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include "../../include/ipworks.h"
#define LINE_LEN 100

int main(int argc, char *argv[])
{

	Ping ping;
	int ret_code;
	int i;

	if (argc != 2)
	{
		fprintf(stderr, "usage:  ping hostname\n");
		fprintf(stderr, "\n");
		fprintf(stderr, "  hostname the IP address or Domain Name of the remote host\n");
		fprintf(stderr, "\nExample ping www.yahoo.com\n\n");
		printf("Press enter to continue.");
		getchar();
	}
	if(ret_code = ping.SetRemoteHost(argv[1]) )  goto done;

	ping.SetPacketSize(32);
	printf("\nPinging %s with %i bytes of data:\n\n", argv[1], ping.GetPacketSize());

	for (i=0; i<4; i++)
	{
		ping.SetTimeToLive(255);
		if (ret_code = ping.PingHost(argv[1])) goto done;
		printf("Reply from %s: bytes=%i time=%ims\n", ping.GetRemoteHost(), ping.GetPacketSize(), ping.GetResponseTime());
	}

done:
	if (ret_code)     // Got an error.  The user is done.
	{
		printf( "\nError: %d", ret_code );
		if (ping.GetLastError())
		{
			printf( " \"%s\"\n", ping.GetLastError() );
		}
	}
	printf("Press enter to continue.");
	getchar();  
	return 0;
}









