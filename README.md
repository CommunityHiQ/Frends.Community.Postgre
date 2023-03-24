- [Frends.Community.Files.Postgre](#frendscommunitypostgre)
   - [Installing](#installing)
   - [Tasks](#tasks)
      - [ExecuteQuery](#executequerydata)
         - [QueryParameters](#queryparameters)
         - [ConnectionInformation](#connectioninformation)
         - [Output](#output)
		 - [Result](#result)
	  - [ExecuteQueryToFile](#executequerytofile)
   - [License](#license)
   - [Building](#building)
   - [Contributing](#contributing)

# Frends.Community.Postgre
FRENDS Task for Postgre queries.

## Installing
You can install the task via Frends UI Task view or you can find the nuget package from the following nuget feed
https://www.myget.org/F/frends/api/v3/index.json

## Tasks

### ExecuteQuery

#### QueryParameters

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| Query				| string	| Postgre query string	| ´SELECT * FROM table WHERE \"Column\" = ''Value''´ |
| Parameters		| Array(string,string)	| List of inputs parameters	| `input` `message` |

#### Query Parameter

| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Name | string | Parameter name used in Query property | `username` |
| Value | string | Parameter value | `myUser` |

#### Output
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Return type | enum<Json, Xml, Csv> | Data return type format | `Json` |

##### OutputProperties

###### Xml Output
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| RootElementName | string | Xml root element name | `items` |
| RowElementName |string | Xml row element name | `item` |

###### Json Output
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Culture info | string | Specify the culture info to be used when parsing result to JSON. If this is left empty InvariantCulture will be used. [List of cultures](https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx) Use the Language Culture Name. | `fi-FI` |

###### Csv Output
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| IncludeHeaders | bool | Include field names in the first row | `true` |
| FieldDelimiter | enum<Comma, Semicolon, Pipe> | Csv separator to use in headers and data items. | Enums.CsvFieldDelimiter.Comma |
| LineBreak | enum<CRLF, LF, CR> | What to use in line breaks. | Enums.CsvLineBreak.CRLF |
| SanitizeColumnHeaders | bool | Whether to sanitize headers in output. | false |
| AddQuotesToDates | bool | Whether to add quotes around DATE and DATETIME fields. | false |
| DateFormat | string | Date format to use for formatting DATE columns, use .NET formatting tokens. | ´yyyy-MM-dd´ |
| DateTimeFormat | string | Date format to use for formatting DATETIME columns, use .NET formatting tokens. | ´yyyy-MM-dd HH:mm:ss´ |

#### ConnectionInformation

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| ConnectionString	  | string	| Postgre connection string | ´Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;´ |
| TimeoutSeconds	  | int		| Timeout in seconds		| 30 |

#### Options

| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Throw error on failure | bool | Specify if Exception should be thrown when error occurs. If set to *false*, task outcome can be checked from #result.Success property. | `false` |

#### Result

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-----------------------------------------|-----------------------------|
| Output         | string   | Depends on the value chosen in Input	| XMLString |
| Success        | bool     | Result if the task succeeded	| true |
| Message        | string   | Error message if Throw error on failure is set to 'False'	| XString |

### ExecuteQueryToFile

Executes query against Postgre database and outputs result to a file. Idea for this is to enable formatting for different data after retreiving it from database. Mainly for datetimes and timestamps in mind.

#### Query Properties
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Query | string | The query to execute | `SELECT * FROM Table WHERE field = :paramName`|
| Parameters | array[Query Parameter] | Possible query parameters. See [Query Parameter](#query-parameter) |  |

#### Output
| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| OutputFilePath | string | Output path with file name | `c:\temp\output.json` |
| Return type | enum<Json, Xml, Csv> | Data return type format | `Json` See [Output Properties](#outputproperties) |
| Append | bool | Enables the task to append to the end of the file. | false |
| Encoding | enum<UTF8, ANSI, ASCII, WINDOWS1252, Other> | Output file encoding | Enums.EncodingOptions |
| EnableBom | bool | Enable bom on utf-8 encoding | false |
| EncodingString | string | Manualy give encoding | ´utf-8´ |

#### Connection

| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Connection string | string | Postgre database connection string | `Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;` |
| Timeout seconds | int | Query timeout in seconds | `60` |

#### Options

| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Throw error on failure | bool | Specify if Exception should be thrown when error occurs. If set to *false*, task outcome can be checked from #result.Success property. | `false` |

#### Notes
Newlines in text fields are replaced with spaces.

#### Result

| Property    | Type       | Description     | Example |
| ------------| -----------| --------------- | ------- |
| Path | string | Path to the output file | ´c:\temp\temp.csv´ |
| Rows | int | RowAffected on select default value is always -1 if rows were fetched. | -1 |
| Success        | bool     | Result if the task succeeded	| true |
| Message        | string   | Error message if Throw error on failure is set to 'False'	| XString |

## License

This project is licensed under the MIT License - see the LICENSE file for details

## Building
Ensure that you have https://www.myget.org/F/frends/api/v3/index.json added to your nuget feeds

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.Postgre.git`

Rebuild the project

`dotnet build`

Run Tests

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`

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
| 2.0.0 | Refactored the task and added new task ExecuteQueryToFile which handles all three datatypes. |