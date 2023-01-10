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

main().catch(e => console.log(e))

async function main () {
  console.log('*****************************************************************\n')
  console.log('* This demo shows how to set up an echo server using WSServer.  *\n')
  console.log('*****************************************************************\n')

  const wsserver = new ipworks.wsserver()

  const question1 = () => {
    return new Promise((resolve, reject) => {
      rl.question('Local Port: ', (port) => {
        resolve(port)
      })
    })
  }

  wsserver.setLocalHost('localhost')

  wsserver.setLocalPort(await question1())

  await wsserver.startListening()
  console.log('Listening... press Ctrl-C to shutdown')

  wsserver.on('Connected', (e) => {
    console.log(wsserver.getConnections().item(e.connectionId).getRemoteHost() + ' connected.')
  }).on('Disconnected', (e) => {
    console.log('Remote host disconnected: ' + e.description)
  }).on('DataIn', async e => {
    console.log(`Received a message from ${wsserver.getConnections().item(e.connectionId).getRemoteHost()} and the message is: '${e.text}'`)
    console.log('Echoing message back....')
    await wsserver.sendText(e.connectionId, e.text).catch(e => console.log(e))
  }).on('SSLClientAuthentication', e => {
    e.accept = true
  })

  while (true) {
    await wsserver.doEvents()
  }
}

function prompt(promptName, label, punctuation, defaultVal)
{
  lastPrompt = promptName;
  lastDefault = defaultVal;
  process.stdout.write(`${label} [${defaultVal}] ${punctuation} `);
}
