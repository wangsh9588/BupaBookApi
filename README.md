# Bupa Book API
This API includes an endpoint for retrieving a list of all the books in alphabetical order under a heading of the age category of their owner

## Assumptions:
- The term 'a list of all the books' means a list of book names

## Instruction
Install **.NET 8** from [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Start project **WebApi**

A Swagger page should open by default or you can navigate to
```bash
https://localhost:7140/swagger/index.html
```

API listens for requests on the following ports.
```bash
https://localhost:7140
http://localhost:7141
```

## Solution architecture
* ### WebApi 

    The Web API layer contains the API endpoints, middleware, and dependency injection setup. It manages incoming HTTP requests and directs them to the appropriate services for processing.

* ### Application

    The Application layer contains the core business logic and orchestrates the handling of client requests. It processes the necessary operations before sending data to and from the Web API layer.
* ### Infrastructure

    The Infrastructure layer includes all external dependencies, such as third-party API clients and other external services. It manages interactions with external resources to keep the application’s business logic clean and focused.
* ### Core

    The Core layer defines the essential domain entities, models, and interfaces.
    
## To do
Add more unit tests and integration tests
    