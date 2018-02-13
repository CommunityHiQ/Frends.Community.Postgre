- [Frends.Community.Files.Postgre](#frendscommunitypostgre)
   - [Installing](#installing)
   - [Building](#building)
   - [Contributing](#contributing)
   - [Documentation](#documentation)
      - [QueryData](#querydata)
         - [Input](#input)
         - [ConnectionInformation](#connectioninformation)
         - [Output](#output)
   - [License](#license)
   - [Building](#building)
   - [Contributing](#contributing)

# Frends.Community.Postgre
FRENDS Task for Postgre queries.

## Installing
You can install the task via Frends UI Task view or you can find the nuget package from the following nuget feed
https://www.myget.org/F/frends/api/v3/index.json

## Building
Ensure that you have https://www.myget.org/F/frends/api/v3/index.json added to your nuget feeds

Clone a copy of the repo

git clone https://github.com/CommunityHiQ/Frends.Community.Postgre.git

Restore dependencies

nuget restore Frends.Community.Postgre

Rebuild the project

Run Tests with nunit3. Tests can be found under

Frends.Community.Postgre.Tests\bin\Release\Frends.Community.Postgre.Tests.dll

Create a nuget package

`nuget pack nuspec/Frends.Community.Postgre.nuspec`

## Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

## Documentation

### QueryParameters

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| Query				| string	| Postgre query string	| ´SELECT * FROM table WHERE \"Column\" = ''Value''´ |
| Parameters		| Array(string,string)	| List of inputs parameters	| `input` `message` |
| ReturnType		| XMLString, JSONString	| Query return type	| JSONString |

### ConnectionInformation

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| ConnectionString	  | string	| Postgre connection string | ´Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;´ |
| TimeoutSeconds	  | int		| Timeout in seconds		| 30 |

### Output

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| Result        | dynamic   | Depends on the value chosen in Input	| XMLString |

## License

This project is licensed under the MIT License - see the LICENSE file for details

## Building
Ensure that you have https://www.myget.org/F/frends/api/v3/index.json added to your nuget feeds

Clone a copy of the repo

git clone https://github.com/CommunityHiQ/Frends.Community.Postgre.git

Restore dependencies

nuget restore Frends.Community.Postgre

Rebuild the project

Run Tests with nunit3. Tests can be found under

Frends.Community.Postgre.Tests\bin\Release\Frends.Community.Postgre.Tests.dll

Create a nuget package

`nuget pack nuspec/Frends.Community.Postgre.nuspec`

## Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

## Change Log

| Version             | Changes                 |
| ---------------------| ---------------------|
| 1.0.0 | Initial Frends.Community version of PostgreOperations |
| 1.0.1 | Implemented ToJson and ToXml extensions, refactored main code, removed return types 'XDocument', 'XmlDocument' and 'Dynamic', updated documentation and added license information |

