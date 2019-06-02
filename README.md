# Shopping Service

> Learning .NET Core 2.2 by building a shopping service.

## Requirements

- [Docker](https://www.docker.com/get-started)
- [.NET SDK 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2)

## Usage

To install project dependencies, run the following command:
```bash
make install_dependencies
```

To run a local MongoDB database, run the following command:
```bash
make start_local_db
```

To run all tests locally, run the following command:
```bash
make local_test
```

To run the Shopping Service API, run the following command:
```bash
make run_service
```

To build the docker image, run the following command:
```bash
REGISTRY_USERNAME=<some-docker-registry-username> \
TAG=<some-build-tag> \
make build_image
```

To publish the docker image, run the following commands:
```bash
REGISTRY_USERNAME=<some-docker-registry-username> \
REGISTRY_PASSWORD=<some-docker-registry-password> \
TAG=<some-build-tag> \
make push_image
```

To run the docker image, run the following command:
```bash
REGISTRY_USERNAME=<some-docker-registry-username> \
SHOPPING_SERVICE_ENVIRONMENT=<some-service-environment> \
TAG=<some-build-tag> \
make run_image
```

## Useful Links

- [Partitioned Repository sample project](https://github.com/Azure-Samples/PartitionedRepository)
- [Language-Ext lib](https://github.com/louthy/language-ext)
- [Integration testing ASP.NET Core WebAPI tutorial](https://fullstackmark.com/post/20/painless-integration-testing-with-aspnet-core-web-api)
- [Integration testing ASP.NET Core WebAPI docs](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2)
- [Azure CosmosDB docs](https://docs.microsoft.com/en-us/azure/cosmos-db/)

## LICENSE
```
The MIT License (MIT)

Copyright (c) 2018 TJ Maynes

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
