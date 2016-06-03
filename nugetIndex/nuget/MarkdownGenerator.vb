Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Linq

Module MarkdownGenerator

    Public Const nuget As String = "https://www.nuget.org/packages/{0}/"

    <Extension>
    Public Function Document(nuspec As Nuspec, github As String, Optional hexo As Boolean = False) As String
        Dim sb As New StringBuilder($"[<< Back to Index]({github})" & vbCrLf)

        Call sb.AppendLine($"# {nuspec.metadata.title}" & vbCrLf)
        Call sb.AppendLine("Version: **" & nuspec.metadata.version & "**")
        Call sb.AppendLine()
        Call sb.AppendLine("Project URL: " & nuspec.metadata.projectUrl)
        Call sb.AppendLine()
        Call sb.AppendLine("License: " & nuspec.metadata.licenseUrl)
        Call sb.AppendLine()
        Call sb.AppendLine($"To install **[{nuspec.metadata.title}]({String.Format(nuget, nuspec.metadata.id)})**, run the following command in the Package Manager Console:")
        Call sb.AppendLine($"> PM>  **Install-Package {nuspec.metadata.id}**")
        Call sb.AppendLine()
        Call sb.AppendLine()
        Call sb.AppendLine("## Summary")
        Call sb.AppendLine(nuspec.metadata.summary)
        Call sb.AppendLine()
        Call sb.AppendLine(nuspec.metadata.description)
        Call sb.AppendLine("## Release Notes")
        Call sb.AppendLine(nuspec.metadata.releaseNotes)
        Call sb.AppendLine("## Owners")
        Call sb.AppendLine(nuspec.metadata.owners)
        Call sb.AppendLine("## Authors")
        Call sb.AppendLine($"[{nuspec.metadata.authors}](https://www.nuget.org/profiles/{nuspec.metadata.authors})")
        Call sb.AppendLine("## Copyright")
        Call sb.AppendLine(nuspec.metadata.copyright)
        Call sb.AppendLine("## Tags")
        Call sb.AppendLine(nuspec.metadata.TagsMarkdownLinks)
        Call sb.AppendLine("## Dependencies")
        If Not hexo Then
            Call sb.AppendLine(">")
        End If
        Call sb.AppendLine("```json")
        Call sb.AppendLine(nuspec.metadata.frameworkAssemblies.GetJson)
        Call sb.AppendLine("```")
        Call sb.AppendLine()
        Call sb.AppendLine()
        Call sb.AppendLine("## File includes")

        Dim c As String = If(hexo, "+ ", "> ")

        For Each file In nuspec.files
            Call sb.AppendLine(c & file.src & "<br />")
        Next

        Return sb.ToString
    End Function

    <Extension>
    Public Function HexoMarkdown(nuspec As Nuspec, index As String) As String
        Dim sb As New StringBuilder()

        Call sb.AppendLine(
        $"---
title: {nuspec.metadata.title}
date: {Now.ToString}
---")
        Call sb.AppendLine()
        Call sb.AppendLine(nuspec.Document(index, True))

        Return sb.ToString
    End Function
End Module
