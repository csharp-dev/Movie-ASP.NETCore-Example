
# Movie MVC

The objective of this learning is to move an existing ASP.NET MVC applicaiton to ASP.NET Core MVC.

Learning ASP.NET Core MVC by building a simple CRUD Movie listing app.
Source: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc


The priority is to use commandline tools to develop and deploy in a Linux enviroment.
CLI Tools: https://learn.microsoft.com/en-us/dotnet/core/tools


SQLite is used as a data store.
The development and production environment is Ubuntu.

## Initiate Project
`dotnet new mvc -o MvcMovie` : Creates a new ASP.NET Core MVC project in the MvcMovie folder.

`dotnet run` : Run project


## MVC Architecture
`Models`: Classes that represent the data of the app. The model classes use validation logic to enforce business rules for that data. Typically, model objects retrieve and store model state in a database.

`Views`: Views are the components that display the app's user interface (UI). Generally, this UI displays the model data.

`Controllers`: Classes that: Handle browser requests, Retrieve model data, Call view templates that return a response.


Movie model retrieves movie data from a database, provides it to the view or updates it. Updated data is written to a database.

Model classes are used with Entity Framework Core (EF Core) to work with a database. EF Core is an object-relational mapping (ORM) framework that simplifies the data access code that we have to write.

The model classes created are known as POCO classes, from Plain Old CLR Objects. POCO classes don't have any dependency on EF Core. They only define the properties of the data to be stored in the database.

## Model First

Model classes are created first and EF Core creates the database.

Add tools and dependent packages. 

```
dotnet tool uninstall --global dotnet-aspnet-codegenerator
dotnet tool install --global dotnet-aspnet-codegenerator

dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef

dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SQLite
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
```

## Scaffold Views And Controller 
Use the scaffolding tool to produce Create, Read, Update, and Delete (CRUD) pages for the movie model.

```
export PATH=$HOME/.dotnet/tools:$PATH`

dotnet aspnet-codegenerator controller -name MoviesController -m Movie -dc MvcMovie.Data.MvcMovieContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries -sqlite
```

-m	The name of the model.
-dc	The data context.
--relativeFolderPath	The relative output folder path to create the files.
--useDefaultLayout|-udl	The default layout should be used for the views.
--referenceScriptLibraries	Adds _ValidationScriptsPartial to Edit and Create pages.
-sqlite	Flag to specify if DbContext should use SQLite instead of SQL Server.


Scaffolding updates the following:
- Registers the database context in the Program.cs file
- Adds a database connection string to the appsettings.json file.


Scaffolding creates the following:
- A movies controller: Controllers/MoviesController.cs
- Razor view files for Create, Delete, Details, Edit, and Index pages: Views/Movies/*.cshtml
- A database context class: Data/MvcMovieContext.cs


## Run Migration
```
dotnet ef migrations add InitialCreate

dotnet ef database update
```

`ef migrations add InitialCreate`: Generates a Migrations/{timestamp}_InitialCreate.cs migration file. 

The InitialCreate argument is the migration name. Any name can be used, but by convention, a name is selected that describes the migration. This is the first migration, so the generated class contains code to create the database schema. The database schema is based on the model specified in the MvcMovieContext class, in the Data/MvcMovieContext.cs file.

`ef database update`: Updates the database to the latest migration, which the previous command created. This command runs the Up method in the Migrations/{time-stamp}_InitialCreate.cs file, which creates the database.


When EF Code First is used to automatically create a database, Code First:
- Adds a table to the database to track the schema of the database.
- Verifies the database is in sync with the model classes it was generated from. If they aren't in sync, EF throws an exception. This makes it easier to find inconsistent database/code issues.


When a change is made in the model, run both commands to track the changes.


## Passing Data To View
A controller can pass data or objects to a view using the ViewData dictionary. The ViewData dictionary is a dynamic object that provides a convenient late-bound way to pass information to a view.

MVC provides the ability to pass strongly typed model objects to a view. This strongly typed approach enables compile time code checking.


The @model statement at the top of the view file specifies the type of object that the view expects. When the movie controller was created, the following @model statement was included.

This @model directive allows access to the movie that the controller passed to the view. The Model object is strongly typed.


## Validation
The validation support provided by MVC and Entity Framework Core Code First is a good example of the DRY principle in action. We can declaratively specify validation rules in one place (in the model class) and the rules are enforced everywhere in the app.

The DataAnnotations namespace provides a set of built-in validation attributes that are applied declaratively to a class or property. DataAnnotations also contains formatting attributes like DataType that help with formatting and don't provide any validation.


## Route Fix With Controleller Method Overload

The common language runtime (CLR) requires overloaded methods to have a unique parameter signature (same method name but different list of parameters).

ASP.NET maps segments of a URL to action methods by name, and if we rename a method, routing normally wouldn't be able to find that method. 

A common work around for methods that have identical names and signatures is to artificially change the signature of the POST method to include an extra (unused) parameter.


The MVC scaffolding engine that created the action method adds a comment showing an HTTP request that invokes the method.

```
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```


# Use MySQL Database

As the application that needs migration uses MySQL, we want to implement MySQL.

`dotnet tool update --global dotnet-ef` : upgrade Entity Framework Core.

`dotnet add package Pomelo.EntityFrameworkCore.MySql --version=7.0.0-alpha.1`: As our .NET Core version is 7.0.0, the latest release was used although that is in alpha.

`dotnet ef migrations add UseMySQL` : Generate migrations to use with MySQL and it's database context.

`dotnet ef database update`: Update database changes with the newly generated migrations.

When ever any change is made, new migration needs to be generated and applied to database.

Here we are using Pomelo as the Entity Framework Core provider for MySQL. 
It is choosen primarily because:
- It is opensource.
- It is built on top of MySqlConnector, an opensource high performance MySQL library for .NET.

https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql
https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql


Create Model scaffold from database:
`dotnet ef dbcontext scaffold "Server=localhost;User=root;Password=1234;Database=ef" "Pomelo.EntityFrameworkCore.MySql"`

Or, generate models from database tables.


Other official option : https://www.nuget.org/packages/MySql.EntityFrameworkCore

## Security
Prior, the database connection string was set on appsettings.json file and or was hardcoded on Program.cs.

The connection string is fetched via environment variable.

`export CONNECTION_STRING='Server=localhost;User=root;Password=1234;Database=ef;Port=3306`

Port is optional if the MySQL Sever is publicy accessile at port 3306. 


## Running Application
By default, application runs in Development environmet.

`dotnet run --environment Production --urls http://localhost:8080`: Set environment and port.

Deployment needs to be researched further.
https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx

