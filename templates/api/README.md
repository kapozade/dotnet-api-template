# Supreme

If you want to proceed with EFCore migrations, run the commands below. 

<i>Note: these commands require dotnet-ef that should have already been installed globally. <br/>
To install run the command => ```dotnet tool install --global dotnet-ef``` </i>


```bash
cd src/Supreme.Infrastructure
dotnet ef migrations add InitialCreate -s "../Supreme.Api/" -o "Db/Migrations/"
dotnet ef database update -s "../Supreme.Api"
```

Running API successfully, you are free to navigate [http://localhost:5000](http://localhost:5000) in your browser.
