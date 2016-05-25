# Utility tool for nuget packages

Install app runtime for this utility tool:
> PM> Install-Package VB_AppFramework

### Load nuget describ metadata

Parsing nuget package description meta data file by using Xml deserializatin:

There is two section in the nuget package meta data:

+ metadata

> The meta data section records the summary information about your nuget package, and it can be parsing by just using a simple class:

```visualbasic
Public Class metadata
    <XmlAttribute> Public Property minClientVersion As String
    Public Property id As String
    Public Property version As String
    Public Property title As String
    Public Property authors As String
    Public Property owners As String
    Public Property licenseUrl As String
    Public Property projectUrl As String
    Public Property requireLicenseAcceptance As Boolean
    Public Property description As String
    Public Property summary As String
    Public Property releaseNotes As String
    Public Property copyright As String
    Public Property language As String
    Public Property tags As String
    Public Property frameworkAssemblies As frameworkAssembly()
End Class
    
Public Class frameworkAssembly
    <XmlAttribute> Public Property assemblyName As String
    <XmlAttribute> Public Property targetFramework As String
End Class
```

+ files

> The file list item is mainly consist with two attribute:
```visualbasic
Public Class file
    <XmlAttribute> Public Property src As String
    <XmlAttribute> Public Property target As String
End Class
```

So that finally we can build a simple class object for stands for the description meta data:
```visualbasic
<XmlRoot("package", [Namespace]:="http://schemas.microsoft.com/packaging/2013/01/nuspec.xsd")>
Public Class Nuspec

    Public Property metadata As metadata
    Public Property files As file()

End Class
```

The by construct this meta data class object, and this will makes the parsing of nuget package metadata easily in one line of code:
```visualbasic
Dim nuspec As Nuspec = path.LoadXml(Of Nuspec)
```

### output markdown document from meta

