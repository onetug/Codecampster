# Code Campster

Welcome to the source code for Code Campster!

## Requirements

This site requires that the computer you are working on has the .NET Core 2.1.4 SDK installed, and access to a SQL Server database for the project.

* [.NET Core 2.1.4](https://dotnet.microsoft.com/download/dotnet-core/2.1) SDK to build.
* [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### First time setup
You will need to have access to create a SQL Server database in order to run the site. Make sure that your user has rights to access that database. You can verify this by logging in through SSMS.

You can override the connection string by setting an environment variable.

This is a powershell example. In this example I am configuring a local database named orlandocodecamp2019:

```powershell
$env:ConnectionStrings__CodecampDbContextConnection="Data Source=.;Initial Catalog=orlandocodecamp2019;Connect Timeout=30;Encrypt=true;TrustServerCertificate=true;Integrated Security=true;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
```

After the environment variable is setup, you need to build the site and run the database update:

```powershell
cd Codecamp

# restore nuget packages and build the project
dotnet build --configuration debug

# create/update the db
dotnet ef database update
```

Finally, the site requires a record in the Events table so run this statement to get the data all setup.

```sql
INSERT INTO [dbo].[Events]
           ([Name]
           ,[SocialMediaHashtag]
           ,[StartDateTime]
           ,[EndDateTime]
           ,[LocationAddress]
           ,[IsActive]
           ,[IsAttendeeRegistrationOpen]
           ,[IsSpeakerRegistrationOpen])
     VALUES
           ('DEV Orlando Code Camp'
           ,'#hashtag'
           ,'2019-03-30 09:00:00'
           ,'2019-03-30 17:00:00'
           ,'Somewhere in Lake Mary'
           ,1
           ,1
           ,0)
```

Now you can continue on to the normal "Running the site" instructions below.


### Running the site

After you've followed the instructions above and have an initialized database you can do the following steps

```powershell
# you need to be in the ./Codecamp directory
cd Codecamp

# create/update the db
dotnet ef database update

# build the project
dotnet run
```