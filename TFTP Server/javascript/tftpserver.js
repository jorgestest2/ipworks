/*
 * IPWorks 2022 JavaScript Edition - Demo Application
 *
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 *
 */
 
const readline = require("readline");
const ipworks = require("@nsoftware/ipworks");

if(!ipworks) {
  console.error("Cannot find ipworks.");
  process.exit(1);
}
let rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

main();

async function main() {
  const argv = process.argv;

  if (argv.length !== 3) {
    console.log("Usage: node echoserver.js path");
    console.log("  path	  the file path to the directory where files will be serviced from");
    console.log("Example: node tftpserver.js C:\\temp");
    return;
  }

  const tftpserver1 = new ipworks.tftpserver();

  tftpserver1.on("ConnectionRequest", function(e) {
    console.log(e.remoteHost + ": Attempting to connect.");
  })
  .on("Connected", function (e) {
    console.log(e.connectionId + ": Connected.");
  })
  .on("StartTransfer", function (e) {
    console.log(e.connectionId + ": Transfer started.");
  })
  .on("Transfer", function (e) {
    console.log(e.connectionId + ": Transfer in progress.");
  })
  .on("EndTransfer", function (e) {
    console.log(e.connectionId + ": Transfer complete.");
  })
  .on("Disconnected", function (e) {
    console.log(e.connectionId + ": Disconnected.");
  })
  .on("Error", function (e) {
    console.log(e.description);
  });


  tftpserver1.setLocalDir(argv[2]);

  await tftpserver1.startListening();

  console.log("Listening...\n");

  while (true) {
    await tftpserver1.doEvents(function (err) {
      if (err) {
        console.log(err);
        return;
      }
    });
  }
}


function prompt(promptName, label, punctuation, defaultVal)
{
  lastPrompt = promptName;
  lastDefault = defaultVal;
  process.stdout.write(`${label} [${defaultVal}] ${punctuation} `);
}
