# Transaction.Monitor

## About this solution

### Usage
By configuring the receiving address, it is used to notify the service that the transaction has been received at the address.

```angular2html
Notice：
1. Except for the `/api/guardKey/create`  interface, all parameters need to be encrypted using the AES algorithm and include the encryption result. 
2. When the service performs a callback, the encryption result needs to be verified to prevent network attacks. 
3. Use the `api/tools/*` interface to debug the encryption algorithm.
4. Get more information from http://localhost:44393/swagger/index.html
```
### Pre-requirements

* [.NET8.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
* [Node v18 or 20](https://nodejs.org/en)

### Build for docker
docker build -t game-monitor-api -f ./Dockerfile.api .
docker run -dit -p 44393:44393 -v ./log:/App/bin/HttpApi/Logs --name game-monitor-api game-monitor-api