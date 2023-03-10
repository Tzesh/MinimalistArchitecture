# MinimalistArchitecture
Minimalist .NET 7 minimal API architecture with
- FluentValidation,
- Swagger,
- JWT Bearer Authentication,
- xUnit,
- and Serilog

The main idea that lies below creating this project was providing an architecture which contains numerous required complex functionalities and on the other hand basic and much more understandable from anyone who does not familiar with .NET technologies before. You can create your own routes, tests just simply looking at the previously generated routes/tests.

## How to use
You can fork/download and build your own API on the top of framework that Minimalist Architecture provides. Minimalist Architecture will take the rest of your configurations about routes but you must have configure your DB if needed. Don't you forget to configure your DB settings (Program.cs) while adding new routes.

![Footer of DB configuration](https://imgur.com/60DP1Fc.png)

## How to run the API
- In order to run the project just simply use the command below:

```
dotnet run --project MinimalistArchitecture.API
```

![Image of how to run the API](https://imgur.com/vVh1UN7.png)

Once you run the project then you can access swagger by just adding "/swagger" at the end of your base address (i.e., for 'http://localhost:5135/' -> 'http://localhost:5135/swagger/'):

![Footer of Swagger](https://imgur.com/LtGh9sJ.png)

## How to run the tests

- If you are aiming to test the project using xUnit simply use the code below

```
dotnet test
```

![Image of how to test the API](https://imgur.com/eCiq0DK.png)

