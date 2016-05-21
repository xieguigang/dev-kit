Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine, executeFile:=AddressOf ExecFile)
    End Function

    Public Function ExecFile(path As String, args As CommandLine) As Integer
        Dim nuspec As Nuspec = path.LoadXml(Of Nuspec)
        Dim md As String = nuspec.Document
        Dim out As String = path.TrimFileExt & ".md"
        Return md.SaveTo(out).CLICode
    End Function

    <ExportAPI("/index", Usage:="/index /source <inDIR> [/github <url>]")>
    Public Function Index(args As CommandLine) As Integer
        Dim files As IEnumerable(Of String) =
            ls - l - r - wildcards("*.nuspec") <= args("/source")
        Call App.SelfFolks(files.ToArray, 4)

        Dim sb As New StringBuilder()
        Call sb.AppendLine("My nuget published packages meta data backup database.")

        Dim LQuery = From path As String
                     In files
                     Let name As String = path.BaseName
                     Let DIR As String = path.ParentPath
                     Select name,
                         DIR,
                         path
                     Group By DIR Into Group

        Call sb.AppendLine("#Index")

        For Each package In LQuery
            Dim name As String = package.DIR.BaseName

            Call sb.AppendLine("##" & name)

            For Each ver In package.Group
                Call sb.AppendLine(">" & ver.name)
            Next

            Call sb.AppendLine()
        Next

        Return sb.SaveTo(args("/source") & "/README.md")
    End Function
End Module
