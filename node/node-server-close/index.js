const http = require('http');

const hostname = '127.0.0.1';
const port = 3000;

const server = http.createServer((req, res) => {
  res.statusCode = 200;
  res.setHeader('Content-Type', 'text/plain');
  res.end('Hello World');
});

function longRunningCleanUp() {
  return new Promise((resolve, reject) => {
    console.log('Starting long-running clean-up');
    setTimeout(() => {
      console.log('Completing long-running clean-up');
      resolve();
    }, 2000);
  });
}

server.listen(port, hostname, () => {
  console.log(`Server running at http://${hostname}:${port}/`);
});

server.on('close', async () => {
  console.log('received server.close event');
  await longRunningCleanUp();
})

process.on('SIGTERM', () => {
  console.log('Received sigterm. Closing the server');

  server.close(async () => {
    console.log('Server closed');
  });
});