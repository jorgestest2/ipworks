/*
 * IPWorks 2022 C++ Edition - Demo Application
 *
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 *
 */


/************************************************************************
  UDP Time Demo using UDP control
  Broadcast a time request to the LAN and show the times
 ************************************************************************/

#include <stdio.h>
#include <time.h>
#include "../../include/ipworks.h"
#define LINE_LEN 100

class udptime : public UDP
{
	virtual int FireDataIn(UDPDataInEventParams *e)
	{
		//then concatenate and show the time
		printf("%s :\t%s\n", e->SourceAddress, e->Datagram);
		return 0;
	}
};

int main ()
{
	udptime p;
	int ret_code = 0;
	int limit= 0;

	//set the remote host
	ret_code = p.SetRemoteHost("255.255.255.255");  //broadcast address
	ret_code = p.SetRemotePort(13);                 //daytime service
	p.SetLocalHost(p.GetLocalHost());

	//set active
	if( ! p.GetActive() ) ret_code = p.SetActive(true);
	if (ret_code) goto done;

	printf("Source :\tDate and time\n");
	//send anything and the server will send the time
	ret_code = p.SetDataToSend("hello?", 6);

	limit = time(NULL) + 5;
	do
	{
		p.DoEvents();
	}
	while(time(NULL) < limit);


done:
	if (ret_code)
	{
		printf("error %d (%s)\n", ret_code, p.GetLastError());
	}
	return 0;
}






