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

class MyRexec : public Rexec
{
public:
	int waiting;

	int FireStderr(RexecStderrEventParams *e)
	{
		fwrite(e->Text, 1, e->lenText, stderr);
		if (e->EOL) fprintf(stderr, "\n");
		return 0;
	}
	int FireStdout(RexecStdoutEventParams *e)
	{
		fwrite(e->Text, 1, e->lenText, stdout);
		if (e->EOL) fprintf(stdout, "\n");
		return 0;
	}
	int FireDisconnected(RexecDisconnectedEventParams *e)
	{
		waiting = 0;
		return 0;
	}
};

int main(int argc, char **argv)
{

	if (argc < 2)
	{
		fprintf(stderr, "usage: %s <host> <user> <password> <command>\n", argv[0]);
		fprintf(stderr, "\npress <return> to continue...\n");
		getchar();
		exit(1);
	}

	MyRexec rexec;

	int ret_code = rexec.SetRemoteHost(argv[1]);
	if (ret_code) goto done;

	ret_code = rexec.SetRemoteUser(argv[2]);
	if (ret_code) goto done;

	ret_code = rexec.SetRemotePassword(argv[3]);
	if (ret_code) goto done;

	ret_code = rexec.SetCommand(argv[4]);
	if (ret_code) goto done;

	//now wait for command completion
	rexec.waiting = 1;
	while (rexec.waiting) rexec.DoEvents();

done:
	if (ret_code)
	{
		fprintf(stderr, "error: %d", ret_code);
		if (rexec.GetLastError())
			fprintf(stderr, " (%s)", rexec.GetLastError());
		fprintf(stderr, "\nexiting...\n");
		exit(1);
	}
	fprintf(stderr, "\npress <return> to continue...\n");
	getchar();
	return 0;
}





