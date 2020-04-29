const https = require('https');
const http = require('http');
const fs = require('fs');
const {exec} = require('child_process');
const path = require('path');

const fileName = fs.createWriteStream("data.json");
const request = https.get("https://raw.githubusercontent.com/samadritakarmakar/asets/master/get/data.json", function(response)
{
  response.pipe(fileName);
  response.on('end', ()=>
  {
    console.log(`Download Finished! \nExecuting C# Code!\n\n`);
    exec(path.resolve('./objectReader.exe'), (error, stdout, stderr)=>
    {
      if(error)
      {
        console.log(`We faced error: \n ${error}`);
      }
      if(stdout)
      {
        console.log(`Output is: \n\n ${stdout.toString('utf8')}`);
        const server = http.createServer((req, res)=>
      {
        const fileRead = fs.createReadStream('data.json', 'utf8');
        console.log(`request was made to ${req.url}\n`);
        res.writeHead(200, {'Content-Type': 'application/json'});
        fileRead.pipe(res);
        console.log('I sent the file!\n');
      });
      server.listen(3000, ()=>
      {
        console.log("\nNow listening to port 3000.\nOpen http://localhost:3000 on your browser!\n");
      });
      }
      if(stderr)
      {
        console.log(`Error is: \n ${stderr.toString('UTF8')}`);
      }
    });
  });
});

module.exports = {fileName: fileName, request: request};
