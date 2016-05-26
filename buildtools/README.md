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

    <XmlAttribute> 
    Public Property minClientVersion As String
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

### Output markdown document from meta
Learn the mearkdown syntax can be review from [Mastering Markdown](https://guides.github.com/features/mastering-markdown/), and this section about how to generates some link element from the nuget.

#### The nuget tag link

> The tag links on the nuget is in the format as:
> https://www.nuget.org/packages?q=Tags%3A"{tag_data}"

```visualbasic
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
```

The tags data in the nuget package description meta data is consist of sevral tag tokens and each token is seperated by a space, so that we can parsing the tag data just by using **String.Split** function, and then generate the tag and link data by using string interpolating or **String.Format** function, here is the example:

```visualbasic
Public Function GetTagLinks() As NamedValue(Of String)()
    If String.IsNullOrEmpty(tags) Then
        Return {}
    End If

    Dim tokens As String() = tags.Split
    Return tokens.ToArray(Function(tag) New NamedValue(Of String)(tag, $"https://www.nuget.org/packages?q=Tags%3A""{tag}"""))
End Function

Public Function TagsMarkdownLinks() As String
    Dim LQuery As String() =
        LinqAPI.Exec(Of String) <= From tag As NamedValue(Of String)
                                   In GetTagLinks()
                                   Select $"[{tag.Name}]({tag.x})"
    Return String.Join(" ", LQuery)
End Function
```

Due to the reason of the link syntax in markdown is:

> \[Caption text\](url)

So that we can simply generate the tag link data for markdown by using string interpolating:

> $"\[{tag.Name}\]({tag.x})"

Processing he author links in the nuget is the same as tag data:

> https://www.nuget.org/profiles/{nuspec.metadata.authors}"

The whole function for generates the markdown document is at this file: [MarkdownGenerator.vb](https://github.com/xieguigang/nuget-backup/blob/master/nugetIndex/nuget/MarkdownGenerator.vb)

Now we can generates the markdown document for your nuget package:

```visualbasic
Public Function ExecFile(path As String, args As CommandLine) As Integer
    Dim nuspec As Nuspec = path.LoadXml(Of Nuspec)
    Dim md As String = nuspec.Document
    Dim out As String = path.TrimFileExt & ".md"
    Return md.SaveTo(out).CLICode
End Function
```

Load your nuget meta data as xml, and then generates the markdown, the last save thr document string.
