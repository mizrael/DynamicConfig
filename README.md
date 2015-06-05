DynamicConfig README
=====================

## About
DynamicConfig is a very simple to use configuration library based on the C# 4.0 dynamic feature. It allows loading from file (or parsing) multiple configurations that can be later accessed via dynamic typing, no custom classes or casts are required.

DynamicConfig is somewhat based on JsonConfig ( https://github.com/Dynalon/JsonConfig ) but uses Newtonsoft.JSON instead of JsonFX

## Examples

The main entry-point of the library is the DynamicConfig.Config class from which you can access all the exposed functionalities.
The first thing to do is to register a provider, at the moment there is a JsonConfigProvider included, so:

```csharp
	var provider = new Providers.JsonConfigProvider();
	Config.RegisterProvider("json", provider);
```

Providers can be retrieved using the GetProvider method:
	
```csharp	
	var jsonProvider = Config.GetProvider("json");
```	

### Loading data

Then you can load a configuration from file directly via the DynamicConfig.Config class

```csharp	
	var providerName = "json";
	var configName = "myConfig";
	dynamic config = Config.Load(providerName, configName, filename);
```	

or via the Provider:

```csharp	
	var jsonProvider = Config.GetProvider("json");
	var configName = "myConfig";
	dynamic config = jsonProvider.Load(configName, filename);
```	

configurations can be accessed using dynamic typing on the Provider:

### Accessing data 

```csharp	
	var jsonProvider = Config.GetProvider("json");
	var foo = jsonProvider.myConfig.foo;
	Console.Writeline("foo: {0}", foo);
```	

### Nesting 

configurations can include also complex objects:

```csharp	
	var json = "{name:\"John\", complex:{ one: 1, two: 2, three: \"three\" } }";
	var jsonProvider = Config.GetProvider("json");
	var configName = "myConfig";
	jsonProvider.Parse(configName, json);
	
	var three = jsonProvider.myConfig.complex.three;
	Console.Writeline("three: {0}", three);
```	

## Incoming Features
- appSettings Provider
