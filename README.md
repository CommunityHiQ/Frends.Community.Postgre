- [Frends.Community.Files.Postgre](#frendscommunitypostgre)
   - [Installing](#installing)
   - [Tasks](#tasks)
      - [QueryData](#querydata)
         - [Input](#input)
         - [ConnectionInformation](#connectioninformation)
         - [Output](#output)
		 - [Result](#result)
   - [License](#license)
   - [Building](#building)
   - [Contributing](#contributing)

# Frends.Community.Postgre
FRENDS Task for Postgre queries.

## Installing
You can install the task via Frends UI Task view or you can find the nuget package from the following nuget feed
https://www.myget.org/F/frends/api/v3/index.json

## Tasks

### QueryData

#### QueryParameters

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| Query				| string	| Postgre query string	| ´SELECT * FROM table WHERE \"Column\" = ''Value''´ |
| Parameters		| Array(string,string)	| List of inputs parameters	| `input` `message` |

#### ConnectionInformation

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| ConnectionString	  | string	| Postgre connection string | ´Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;´ |
| TimeoutSeconds	  | int		| Timeout in seconds		| 30 |

#### Output
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Return type | enum<Json, Xml, Csv> | Data return type format | `Json` |
| OutputToFile | bool | true to write results to a file, false to return results to executin process | `true` |

##### Xml Output
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| RootElementName | string | Xml root element name | `items` |
| RowElementName |string | Xml row element name | `item` |

##### Json Output
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Culture info | string | Specify the culture info to be used when parsing result to JSON. If this is left empty InvariantCulture will be used. [List of cultures](https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx) Use the Language Culture Name. | `fi-FI` |

##### Csv Output
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| IncludeHeaders | bool | Include field names in the first row | `true` |
| CsvSeparator | string | Csv separator to use in headers and data items | `;` |

##### Output File
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Path | string | Output path with file name | `c:\temp\output.json` |
| Encoding | string | Encoding to use for the output file | `utf-8` |

#### Result

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| Result        | string   | Depends on the value chosen in Input	| XMLString |

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
| 1.1.0 | Updated documentation and fixed nuget dependencies. First pubic release. |
| 1.2.0 | Fixed nuget dependencies again. |
| 1.3.0 | Added csv return type and option to write result to file. |

