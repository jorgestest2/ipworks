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

class MyTFTPServer : public TFTPServer
{
public: 
    virtual int FireConnectionRequest(TFTPServerConnectionRequestEventParams *e) {
        printf("[%s:%d]: Attempting to connect.\n", e->RemoteHost, e->RemotePort);
        return 0;
    }

    virtual int FireConnected(TFTPServerConnectedEventParams* e) {
        printf("[%d]: Connected.\n", e->ConnectionId);
        return 0;
    }

    virtual int FireStartTransfer(TFTPServerStartTransferEventParams* e) {
        if (e->Direction == 0) {
            printf("[%d]: Upload started.\n", e->ConnectionId);
        }
        else {
            printf("[%d]: Download started.\n", e->ConnectionId);
        }
        return 0;
    }

    virtual int FireTransfer(TFTPServerTransferEventParams *e) {
        printf("Transferring data.\n");
        return 0;
    }

    virtual int FireEndTransfer(TFTPServerEndTransferEventParams* e) {
        if (e->Direction == 0) {
            printf("[%d]: Upload complete.\n", e->ConnectionId);
        }
        else {
            printf("[%d]: Download complete.\n", e->ConnectionId);
        }
        return 0;
    }

    virtual int FireDisconnected(TFTPServerDisconnectedEventParams* e) {
        printf("[%d]: Disconnected.\n", e->ConnectionId);
        return 0;
    }
};

int main(int argc, char** argv)
{
    printf("This demo shows how to use the TFTPServer Component to create a simple TFTP Server.\n");
    printf("Press Ctrl+C to shutdown the server.\n");

    if (argc != 2) {
        fprintf(stderr, "usage: tftpserver path/to/localdir\n");
        fprintf(stderr, "\nExample: tftpserver C:\\temp \n\n");
        printf("Press enter to continue.");
        getchar();
    }
    else {
        MyTFTPServer server;

        server.SetLocalDir(argv[1]);

        server.StartListening();

        printf("Server is listening on port: %d\n", server.GetLocalPort());

        while (true) {
            server.DoEvents();
        }
        exit(0);
    }
}





