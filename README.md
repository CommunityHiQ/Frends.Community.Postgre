# Frends.Community.Postgre

frends Community Task for PostgreOperations

[![Actions Status](https://github.com/CommunityHiQ/Frends.Community.Postgre/workflows/PackAndPushAfterMerge/badge.svg)](https://github.com/CommunityHiQ/Frends.Community.Postgre/actions) ![MyGet](https://img.shields.io/myget/frends-community/v/Frends.Community.Postgre) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) 

- [Installing](#installing)
- [Tasks](#tasks)
     - [PostgreOperations](#PostgreOperations)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

You can install the Task via frends UI Task View or you can find the NuGet package from the following NuGet feed
https://www.myget.org/F/frends-community/api/v3/index.json and in Gallery view in MyGet https://www.myget.org/feed/frends-community/package/nuget/Frends.Community.Postgre

# Tasks

## PostgreOperations

FRENDS Task for Postgre queries.

### QueryParameters

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| Query				| string	| Postgre query string	| ´SELECT * FROM table WHERE \"Column\" = ''Value''´ |
| Parameters		| Array(string,string)	| List of inputs parameters	| `input` `message` |
| ReturnType		| XMLString, JSONString	| Query return type	| JSONString |
| ReturnType		| string | Specify the culture info to be used when parsing result to JSON and to XML. If this is left empty InvariantCulture will be used. List of cultures: https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx Use the Language Culture Name.	| en-US |

### ConnectionInformation

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| ConnectionString	  | string	| Postgre connection string | ´Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;´ |
| TimeoutSeconds	  | int		| Timeout in seconds		| 30 |


### Output

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| Result        | string   | Depends on the value chosen in Input	| XMLString |




# Building

Clone a copy of the repository

`git clone https://github.com/CommunityHiQ/Frends.Community.Postgre.git`

Rebuild the project

`dotnet build`

Run tests

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repository on GitHub
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
| 1.1.0 | Updated documentation and fixed nuget dependencies. First pubic release. |
| 1.2.0 | Fixed nuget dependencies again. |
| 1.2.1 | Converted to support .Net Standard and .Net Framework 4.7.1 |
| 1.3.1 | Fixed issue #4: Parameter can\'t be NULL by creating parameter.Value check and changing the value to DBNull.Value if null | 

