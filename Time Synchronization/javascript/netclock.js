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
    console.log("Usage: node netclock.js server");
    console.log("  server  the time server from which to request the time");
    console.log("Example: node netclock.js time.nist.gov");
    process.exit();
  }

  const myclock = new ipworks.netclock();

  try {
    myclock.setTimeServer(argv[2]);
    await myclock.getTime();
    let data = await myclock.getLocalTime();
    console.log("System date and time: " + new Date());
    console.log("Internet date and time: " + data);
    process.exit();
  } catch (ex) {
    console.log(ex.message);
    process.exit();
  }
}

function prompt(promptName, label, punctuation, defaultVal)
{
  lastPrompt = promptName;
  lastDefault = defaultVal;
  process.stdout.write(`${label} [${defaultVal}] ${punctuation} `);
}
