Imports System.Runtime.CompilerServices
Imports System.Text

Module MarkdownGenerator

    Public Const nuget As String = "https://www.nuget.org/packages/{0}/"

    <Extension>
    Public Function Document(nuspec As Nuspec) As String
        Dim sb As New StringBuilder("#" & nuspec.metadata.id)

        Return sb.ToString
    End Function
End Module
