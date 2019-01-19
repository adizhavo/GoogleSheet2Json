# Google Sheets 2 JSON [![Build Status](https://travis-ci.org/adizhavo/GoogleSheet2Json.svg?branch=master)](https://travis-ci.org/adizhavo/GoogleSheet2Json)

Sheet to json exporter with a built-in parser with its own readable syntax for defining data types.
 
## SYNTAX
 
* ```>``` is used to define a range 
>```1 > 2``` will translate to ```{ "min" : 1, "max" : 2 }``` in json

* ```[``` and ```]``` are used to define arrays of any type
>```[1, 2, 3]``` will translate to ```[1, 2, 3]``` in json <br>
>```[one, two, three]``` will translate to ```["one", "two", "three"]``` in json
 
* ```(``` and ```)``` are used to define a key value pair of any type
>```(1, 2)``` will translate to ```{ "key" : 1, "value" : 2 }``` in json <br>
>```(1, one)``` will translate to ```{ "key": 1, "value": "one" }``` in json
>```{1, one}``` will translate to ```[{ "key": 1, "value": "one" }]``` in json
>```{1, one} {2, two}``` will translate to ```[{ "key": 1, "value": "one" }, { "key": 2, "value": "two" }]``` in json
 
* ```#comment``` columns will be ignored by the parser
 
For more complex examples and real usage see at the bottom
 
## Build 

* Clone the repository, open it in any IDE that supports C#.
* Run ```nuget restore GoogleSheet2Json.sln``` or use the IDE for getting the required google packages for the solution.
* Build the solution in ```RELEASE``` mode if you don’t want to get tons of logs from the parser.

## Google config

Get the ```client_secret.json``` by following this [guide](https://developers.google.com/sheets/api/quickstart/dotnet).
Put the ```client_secret.json``` in the same directory of the application.
 
## Exporter config
 
The application requires some simple configurations, some of them are set in a json [file](https://github.com/adizhavo/GoogleSheet2Json/blob/master/appConfig.json). with the following required fields.
 
 ```applicationName``` can be GoogleSheet2Json <br>
 ```userName``` <br>
 ```spreadSheetId``` you can get it from the sheet url <br>
 ```clientSecret``` relative path to the client secret from the application <br>
 ```outputDirectory``` <br>
 
 Other configurations are passed as command line arguments: 
 #### Required:
 ```-configPath=Path/To/The/Config/File``` <br>
 ```-sheetTab=TAB_NAME_IN_SHEET``` <br>
 ```-keyRange=RANGE``` RANGE is something like A1:K, read google's guide for accessing keys <br>
 ```-valueRange=RANGE``` RANGE is something like A2:K, read google's guide for accessing values <br>
 
 #### Optional:
 ```-outputDir=OTHER/DIR/``` if you want to set a different output directory from the one defined in the config file <br>
 ```-outputFileName=SOME_NAME.extension``` will throw a warning if not set <br>
 ```-isSingleObject``` can export single object defined as key value pairs, see example below <br>
 ```-isLiteral=key_1,key_2,key_n``` data can be exported as it is, without being lexed, see example below <br>
