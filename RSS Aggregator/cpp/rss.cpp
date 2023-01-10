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

class MyRSS : public RSS
{
public:

	virtual int FireSSLServerAuthentication(RSSSSLServerAuthenticationEventParams *e)
	{
		if (e->Accept) return 0;
		printf("Server provided the following certificate:\nIssuer: %s\nSubject: %s\n",
			e->CertIssuer, e->CertSubject);
		printf("The following problems have been determined for this certificate: %s\n", e->Status);
		printf("Would you like to continue anyways? [y/n] ");
		if (getchar() == 'y') e->Accept = true;
		else exit(0);
		return 0;
	}
};


int main(int argc, char **argv)
{
	char command[5]; //users input command
	bool quit = false;

	MyRSS rss1;
  rss1.SetFollowRedirects(true);
	rss1.GetFeed("http://www.nsoftware.com/rss/");

	while (!quit)
	{
		printf("RSS Channel :%s\n%s\n", rss1.GetChannelTitle(),rss1.GetChannelDescription());
		for (int i = 0; i< rss1.GetItemCount(); i++)
		{
			printf("%i. %s\n", i+1, rss1.GetItemTitle(i));
		}
		printf("Q Quit,news item number>");
		fgets(command,5,stdin);
		command[strlen(command)-1] = '\0';

		if ( ! strcmp(command, "Q") )
		{
			quit = true;
		}
		else
		{
			int selectedindex = atoi(command)-1;
			printf("\n\n[%d] %s\n\n",selectedindex,rss1.GetItemDescription(selectedindex));
			printf("Find full article at %s\n\n", rss1.GetItemLink(selectedindex));
		}

	}

	fprintf(stderr, "\npress <return> to continue...\n");
	getchar();
	return 0;
}











