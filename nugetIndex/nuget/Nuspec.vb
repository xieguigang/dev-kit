Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic

<XmlRoot("package", [Namespace]:="http://schemas.microsoft.com/packaging/2013/01/nuspec.xsd")>
Public Class Nuspec

    Public Property metadata As metadata
    Public Property files As file()

End Class

Public Class file
    <XmlAttribute> Public Property src As String
    <XmlAttribute> Public Property target As String
End Class

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
End Class

Public Class frameworkAssembly
    <XmlAttribute> Public Property assemblyName As String
    <XmlAttribute> Public Property targetFramework As String
End Class