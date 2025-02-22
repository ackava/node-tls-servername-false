# node-tls-servername-false
This repo contains issue highlighting servername is set to false when TLS session is reused by HttpClient on windows

This test case correctly runs on windows. You need .NET 6 or 8 installed.

# Steps to Run the repro
1. Open VS Code
2. Open Terminal
3. Split Terminatl in two Halfs
4. On left, run, `cd ./http-server/`
5. run `node ./index.js`
6. On right, run `cd ./dotnet/`
7. run `dotnet run`
8. For HTTP server, you will see that TLS server reports servername being false for reused TLS sessions.

The reason this is a big issue as SNICallback is called only once, so if the host name changes by remote client for same TLS session, it fails with server mismatch error.
