**[Table of Contents](http://tableofcontent.eu)**
- [Frends.Community.Files.Postgre](#frendscommunitypostgre)
  - [Contributing](#contributing)
  - [Documentation](#documentation)
    - [Input](#input)
    - [ConnectionInformation](#connectioninformation)
    - [Output](#output)
  - [License](#license)


# Frends.Community.Postgre
FRENDS Task that posts web form.

## Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

## Documentation

### Input

| Property          |  Type   | Description								| Example                     |
|-------------------|---------|-------------------------------------------|-----------------------------|
| Query				| string	| Postgre query string	| ´SELECT * FROM table WHERE \"Column\" = '||:Value||'´ |
| Parameters		| Array(string,string)	| List of inputs parameters	| `input` `message` |
| ReturnType		| XMLString, XMLDocument, XDocument, JSONString, Dynamic	| Query return type	| JSONString |

### ConnectionInformation

| Property            |  Type   | Description								| Example                     |
|---------------------|---------|-------------------------------------------|-----------------------------|
| ConnectionString	  | string	| Postgre connection string | ´Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;´ |
| TimeoutSeconds	  | int		| Timeout in seconds		| 10 |

### Output

| Property        | Type     | Description                      |
|-----------------|----------|----------------------------------|
| Result        | dymanic   | Depends on the value chosen in Input	|

## License

This project is licensed under the MIT License - see the LICENSE file for details
