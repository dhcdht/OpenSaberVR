# Boomlagoon JSON

Boomlagoon JSON is a lightweight C# JSON implementation.

Boomlagoon JSON doesn't throw exceptions, and there is no casting 
involved, instead all valid JSON values are accessible as the correct 
C# types.

## Usage

### Parsing a `string` into a `JSONObject`

	using Boomlagoon.JSON;

	string text = "{ \"sample\" : 123 }";
	JSONObject json = JSONObject.Parse(text);
	double number = json.GetNumber("sample");


### Creating a `JSONObject`

	var obj = new JSONObject();
	obj.Add("key", "value");
	obj.Add("otherKey", 1234);
	obj.Add("bool", true);

	//Alternative method:
	var obj = new JSONObject {
		{"key", "value"}, 
		{"otherKey", 1234}, 
		{"bool", true}
	};

### Accessing a `JSONObject`'s fields

	var otherObject = new JSONObject {{"key", 123}};
	var obj = new JSONObject {{"nested", otherObject}};

	var nestedObject = obj.GetObject("nested");
	double number = nestedObject.GetNumber("key");



## Changelog

#### Version 1.4
+ Integrated Christophe Sauveur's patch to parse escaped Unicode properly.

#### Version 1.2
+ Replaced usage of Stack<T> with List<T> to remove dependency to system.dll as per Mike Weldon's suggestion.

* * *

Boomlagoon Ltd.  
contact@boomlagoon.com