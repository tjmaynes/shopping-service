# Sample C\# Project

> Building and deploying a shopping cart api using C#/ASP.NET and Kubernetes.

## Requirements

- [.NET Core 5](https://dotnet.microsoft.com/download/dotnet-core/5.0)
- [Docker](https://www.docker.com/get-started)

## Usage

To install project dependencies, run the following command:
```bash
make install_dependencies
```

To build the project, run the following command:
```bash
make build
```

To run all tests, run the following command:
```bash
make test
```

To start the project, run the following command:
```bash
make start
```

To ship the project, run the following command:
```bash
make ship_it
```

To build the docker image, run the following command:
```bash
make build_image
```

To publish the docker image, run the following commands:
```bash
make push_image
```

## LICENSE

License found [here](./LICENSE.txt)
