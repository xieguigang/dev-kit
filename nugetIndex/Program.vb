Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine, executeFile:=AddressOf ExecFile)
    End Function

    ''' <summary>
    ''' path /out &lt;path.md> /github &lt;link> /hexo
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public Function ExecFile(path As String, args As CommandLine) As Integer
        Dim nuspec As Nuspec = path.LoadXml(Of Nuspec)
        Dim out As String = args.GetValue("/out", path.TrimFileExt & ".md")
        Dim md As String

        If args.GetBoolean("/hexo") Then
            md = nuspec.HexoMarkdown("../index.html")
        Else
            md = nuspec.Document(args - "/github")
        End If

        Return md.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/index",
               Usage:="/index /source <inDIR> [/github <url>]")>
    <ParameterInfo("/github", True,
                   Usage:="",
                   Example:="https://github.com/xieguigang/nuget-backup")>
    Public Function Index(args As CommandLine) As Integer
        Dim github As String = args("/github")
        Dim configLink As Boolean = Not String.IsNullOrEmpty(github)
        Dim files As IEnumerable(Of String) =
            ls - l - r - wildcards("*.nuspec") <= args("/source")
        Dim indexURL As String = github

        If configLink Then
            github &= "/tree/master/nuget/"
        End If

        Dim sb As New StringBuilder("# nuget-backup" & vbCrLf)
        Call sb.AppendLine("My nuget published packages meta data backup database.")

        Dim LQuery = From path As String
                     In files
                     Let name As String = path.BaseName
                     Let DIR As String = path.ParentPath
                     Select name,
                         DIR,
                         path
                     Group By DIR Into Group

        Call sb.AppendLine("# __--==Index==--__")

        For Each package In LQuery
            Dim name As String = package.DIR.BaseName

            Call sb.AppendLine("## " & name)

            For Each ver In package.Group
                If configLink Then
                    Call sb.AppendLine(__githubLink(ver.name, github, ver.path.ParentDirName))
                Else
                    Call sb.AppendLine(">" & ver.name & "<br />")
                End If
            Next

            Call sb.AppendLine()
        Next

        Call sb.SaveTo(args("/source") & "/README.md")
        Call App.SelfFolks(files.ToArray(Function(s) $"{s.CliPath} /github {indexURL}"), 4)

        Return 0
    End Function

    <ExportAPI("/Hexo.Build", Usage:="/Hexo.Build /source <inDIR> /out <outDIR>")>
    Public Function HexoBuild(args As CommandLine) As Integer
        Dim files As IEnumerable(Of String) =
            ls - l - r - wildcards("*.nuspec") <= args("/source")
        Dim indexURL As String = "../index.html"
        Dim sb As New StringBuilder($"---
title: My nuget packages
date: {Now.ToString}
---")
        Call sb.AppendLine(vbCrLf)
        Call sb.AppendLine("My published nuget packages were indexed at here:")

        Dim LQuery = From path As String
                     In files
                     Let name As String = path.BaseName
                     Let DIR As String = path.ParentPath
                     Select name,
                         DIR,
                         path
                     Group By DIR Into Group

        Call sb.AppendLine("# __--==Index==--__")

        For Each package In LQuery
            Dim name As String = package.DIR.BaseName

            Call sb.AppendLine("## " & name)

            For Each ver In package.Group
                Call sb.AppendLine(__hexoLink(ver.name, ver.path.ParentDirName))
            Next

            Call sb.AppendLine()
        Next

        Dim out As String = args("/out")
        Dim task As Func(Of String, String) =
            Function(path) $"{path.CliPath} /out {(out & "/" & path.ParentDirName & "/" & path.BaseName & ".md").CliPath} /hexo"
        Dim CLI As String() = files.ToArray(task)

        Call sb.SaveTo(out & "/index.md")
        Call App.SelfFolks(CLI, 4)

        Return 0
    End Function

    Private Function __githubLink(verName As String, link As String, parent As String) As String
        Return $">[{verName}]({link}/{parent}/{verName}.md)<br />"
    End Function

    Private Function __hexoLink(verName As String, parent As String) As String
        Return $"+ [{verName}]({parent}/{verName}.html)<br />"
    End Function
End Module
