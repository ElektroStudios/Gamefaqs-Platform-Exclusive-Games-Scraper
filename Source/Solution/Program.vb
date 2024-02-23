#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading

Imports Win32
Imports Win32.SafeHandles

#End Region

#Region " Program "

''' <summary>
''' Main Program Module.
''' </summary>
Friend Module Program

#Region " Fields "

    ''' <summary>
    ''' The video game platforms to scrap their game titles.
    ''' <para></para>
    ''' See: <seealso href="https://gamefaqs.gamespot.com/games/systems"/>
    ''' </summary>
    Private platforms As List(Of IPlatform) = MiscUtil.GetDefinedPlatforms().ToList()

#End Region

#Region " Main Entry Point "

    ''' <summary>
    ''' Defines the main entry point of the application.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Sub Main()
        Program.InitializeConsoleContext()

        Dim sb As New StringBuilder()
        sb.AppendLine("Available platforms:")
        sb.AppendLine("")
        sb.AppendLine(" Id. | Name")
        sb.AppendLine("_____|__________________")
        sb.AppendLine("     |")
        For i As Integer = 0 To platforms.Count - 1
            sb.AppendLine(String.Format("{0,8}", $"[{CStr(i + 1).PadLeft(2, "0"c)}] | " & platforms(i).PlatformInfo.Name))
        Next
        sb.AppendLine()
        sb.AppendLine("Write the id. number that corresponds to the platform")
        sb.AppendLine("for which to start scraping sequentially,")
        sb.AppendLine("and press ENTER key to continue:")
        sb.AppendLine("")
        sb.Append(" --> ")
        Console.Write(sb.ToString())

        Dim skipItemCount As Integer
        Do While skipItemCount = 0 OrElse skipItemCount = 0 OrElse skipItemCount > platforms.Count
            Dim inputSkipString As String = Console.ReadLine()
            If Not Integer.TryParse(inputSkipString, skipItemCount) OrElse skipItemCount = 0 OrElse skipItemCount > platforms.Count Then
                Console.WriteLine("")
                Console.WriteLine("Invalid value, please try again:")
                Console.Write(" --> ")
            End If
        Loop
        platforms = platforms.Skip(skipItemCount - 1).ToList()

        Console.Clear()
        Console.WriteLine($"First platform chosen: {platforms.First().PlatformInfo.Name}")
        Console.WriteLine("")
        Console.WriteLine("Press CTRL+C at any moment to abort the scraping work and terminate this program...")
        Console.WriteLine("")
        Console.CursorVisible = False

        For i As Integer = 0 To platforms.Count - 1

            Console.WriteLine($"{i + 1} of {platforms.Count} platforms | Running scraping work for platform: {platforms(i).PlatformInfo.Name}...")
            Console.WriteLine("")

            If TypeOf platforms(i) Is PlatformBaseWithOnlineStore Then
                Using platform As PlatformBaseWithOnlineStore = DirectCast(platforms(i), PlatformBaseWithOnlineStore)
                    platform.DoScrap()
                    platform.CreateMarkdownFile()
                    platform.CreateUrlFiles()
                End Using

            ElseIf TypeOf platforms(i) Is PlatformBase Then
                Using platform As PlatformBase = DirectCast(platforms(i), PlatformBase)
                    platform.DoScrap()
                    platform.CreateMarkdownFile()
                    platform.CreateUrlFiles()
                End Using

                ' Delete temporary Urls directory.
                Dim outputUrlsDir As New DirectoryInfo($"{My.Application.Info.DirectoryPath}\Output\Urls")
                If outputUrlsDir.Exists Then
                    Try
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(outputUrlsDir.FullName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    Catch ex As Exception
                    End Try
                End If
            Else
                ' Prevents an improbable unexpected type cast issue.
                Throw New NotImplementedException()

            End If

        Next i

        Console.WriteLine("All the scraping work has been completed!. Press any key to close this program...")
        Console.ReadKey(intercept:=True)
        Environment.Exit(0)
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Initializes the console context.
    ''' </summary>
    <DebuggerStepperBoundary>
    Private Sub InitializeConsoleContext()
        Console.Title = $"{My.Application.Info.Title}" &
                        $" v{My.Application.Info.Version.Major}.{My.Application.Info.Version.Minor}" &
                        $" | {My.Application.Info.Copyright}"

        Console.CursorVisible = True
        Console.OutputEncoding = Encoding.UTF8

        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US")

        ' Set console window transparency.
        Dim transparency As Single = 0.9F
        Using hWnd As SafeWindowHandle = NativeMethods.GetConsoleWindow()
            NativeMethods.SetLayeredWindowAttributes(hWnd, 0, CByte(255 * transparency), &H2)
        End Using
    End Sub

#End Region

End Module

#End Region
