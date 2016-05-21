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

        If configLink Then
            github &= "/tree/master/nuget/"
        End If

        Dim sb As New StringBuilder("#nuget-backup" & vbCrLf)
        Call sb.AppendLine("My nuget published packages meta data backup database.")

        Dim LQuery = From path As String
                     In files
                     Let name As String = path.BaseName
                     Let DIR As String = path.ParentPath
                     Select name,
                         DIR,
                         path
                     Group By DIR Into Group

        Call sb.AppendLine("#__--==Index==--__")

        For Each package In LQuery
            Dim name As String = package.DIR.BaseName

            Call sb.AppendLine("##" & name)

            For Each ver In package.Group
                If configLink Then
                    Call sb.AppendLine($">[{ver.name}]({github}/{ver.path.ParentDirName}/{ver.name}.md)")
                Else
                    Call sb.AppendLine(">" & ver.name)
                End If
            Next

            Call sb.AppendLine()
        Next

        Call sb.SaveTo(args("/source") & "/README.md")
        Call App.SelfFolks(files.ToArray, 4)

        Return 0
    End Function
End Module
