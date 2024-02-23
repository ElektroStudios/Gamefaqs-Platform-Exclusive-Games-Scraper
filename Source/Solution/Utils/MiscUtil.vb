#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Reflection
Imports System.Text
Imports System.Threading

#End Region

#Region " MiscUtil "

''' <summary>
''' Miscellaneous Utilities.
''' </summary>
<HideModuleName>
Friend Module MiscUtil

#Region " Private Fields "

    ''' <summary>
    ''' Represents a pseudo-random number generator, which is a device that produces 
    ''' a sequence of numbers that meet certain statistical requirements for randomness.
    ''' </summary>
    Private ReadOnly RandomGenerator As New Random(Seed:=Environment.TickCount)

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Suspends the current thread for the a random number of milliseconds 
    ''' between the provided minimum and maximum values.
    ''' </summary>
    ''' 
    ''' <param name="minimum">
    ''' The minimum milliseconds to wait.
    ''' </param>
    ''' 
    ''' <param name="maximum">
    ''' The maximum milliseconds to wait.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub SleepRandom(minimum As Integer, maximum As Integer)
        Dim ms As Integer = MiscUtil.RandomGenerator.Next(minimum, maximum + 1)
        Thread.Sleep(ms)
    End Sub

    ''' <summary>
    ''' Prints the input error message in the attached console, 
    ''' and closes the running application with the provided exit code.
    ''' </summary>
    ''' 
    ''' <param name="errorMessage">
    ''' The input error message to print in the attached console.
    ''' </param>
    ''' 
    ''' <param name="exitcode">
    ''' The exit code to send when closing the running application.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub PrintErrorAndExit(errorMessage As String, exitcode As Integer)

        Dim sb As New StringBuilder()
        sb.AppendLine(errorMessage)
        sb.AppendLine()
        sb.AppendLine("This program will close now. Press any key to continue...")

        Console.WriteLine(sb.ToString())
        Console.ReadKey(intercept:=True)
        Environment.Exit(exitcode)

    End Sub

    ''' <summary>
    ''' Tries to download the HTML page source-code from the specified <see cref="Uri"/>. 
    ''' <para></para>
    ''' Whenever it fails to download, it prints the HTTP error code and waits the specified interval to retry again.
    ''' </summary>
    ''' 
    ''' <param name="uri">
    ''' The input <see cref="Uri"/> from which to download the html page source-code.
    ''' </param>
    ''' 
    ''' <param name="refHtmlPage">
    ''' A <see langword="ByRef"/> value that contains the resulting HTML page source-code when this method returns.
    ''' </param>
    ''' 
    ''' <param name="retryIntervalSeconds">
    ''' The interval, in seconds, to wait for retry the download. Default value is: 10 seconds.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub DownloadHtmlPageWithRetry(uri As Uri, ByRef refHtmlPage As String,
                                         Optional retryIntervalSeconds As Integer = 10)

        refHtmlPage = Nothing

        Using wc As New WebClient

            Do While String.IsNullOrEmpty(refHtmlPage)
                wc.Headers.Remove(HttpRequestHeader.UserAgent)
                wc.Headers.Add("User-Agent", GamefaqsUtil.ScraperUserAgent)
                Try
                    refHtmlPage = wc.DownloadString(uri)

                Catch ex As WebException
                    Dim response As HttpWebResponse = TryCast(ex.Response, HttpWebResponse)
                    If response IsNot Nothing Then

                        Dim statusCode As HttpStatusCode = response.StatusCode
                        Console.WriteLine($"Remote server error: ({CInt(statusCode)}) {statusCode}.")
                        Console.WriteLine($"Url: {uri}")

                        Select Case statusCode

                            Case HttpStatusCode.NotFound
                                MiscUtil.PrintErrorAndExit("This error indicates that the webpage or resource linked to by the URL does not exist." & Environment.NewLine &
                                                           "Check Gamefaqs website or contact their support to explain this problem.",
                                                           exitcode:=ExitCodes.ExitCodeHttpError)

                            Case HttpStatusCode.Forbidden
                                MiscUtil.PrintErrorAndExit("This error may indicate that your IP address have been banned." & Environment.NewLine &
                                                           "Check Gamefaqs website or contact their support to explain this problem.",
                                                           exitcode:=ExitCodes.ExitCodeHttpError)

                            Case Else
                                Console.WriteLine($"Waiting {retryIntervalSeconds} seconds to retry again...")
                                Console.WriteLine()
                                Thread.Sleep(TimeSpan.FromSeconds(retryIntervalSeconds))

                        End Select

                    Else
                        Throw

                    End If

                Catch ex As Exception
                    Throw

                End Try
            Loop

        End Using

    End Sub

    ''' <summary>
    ''' Converts the input string value to a valid file name that can be used in Windows OS,
    ''' by replacing any unsupported characters in the string.
    ''' </summary>
    ''' 
    ''' <param name="value">
    ''' The input string value.
    ''' </param>
    ''' 
    ''' <returns>
    ''' The resulting file name that can be used in Windows OS.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function ConvertStringToWindowsFileName(value As String) As String

        Return value.Replace("<", "˂").Replace(">", "˃").
                     Replace("\", "⧹").Replace("/", "⧸").Replace("|", "ǀ").
                     Replace(":", "∶").Replace("?", "ʔ").Replace("*", "✲").
                     Replace(ControlChars.Quote, Char.ConvertFromUtf32(&H201D))

    End Function

    ''' <summary>
    ''' Gets the video game platforms that are defined in the current assembly.
    ''' </summary>
    ''' 
    ''' <returns>
    ''' The video game platforms that are defined in the current assembly.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function GetDefinedPlatforms() As List(Of IPlatform)

        Return (From t As Type In Assembly.GetExecutingAssembly().GetTypes()
                Where t.Namespace = NameOf(Global.Platforms) AndAlso GetType(IPlatform).IsAssignableFrom(t)
                Let platform As IPlatform = DirectCast(Activator.CreateInstance(t), IPlatform)
                Select platform
                Order By platform.PlatformInfo.Name
               ).ToList()

    End Function

    ''' <summary>
    ''' Gets video game platforms of the specified platform type that are defined in the current assembly.
    ''' </summary>
    ''' 
    ''' <param name="platformType">
    ''' The type of platform to return.
    ''' </param>
    ''' 
    ''' <returns>
    ''' The video game platforms of the specified platform type that are defined in the current assembly.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function GetDefinedPlatforms(platformType As PlatformType) As List(Of IPlatform)

        Return (From t As Type In Assembly.GetExecutingAssembly().GetTypes()
                Where t.Namespace = NameOf(Global.Platforms) AndAlso GetType(IPlatform).IsAssignableFrom(t)
                Let platform As IPlatform = DirectCast(Activator.CreateInstance(t), IPlatform)
                Where platform.PlatformInfo.PlatformType = platformType
                Select platform
                Order By platform.PlatformInfo.Name
               ).ToList()

    End Function

    ''' <summary>
    ''' If needed, truncates the length of the specified  file name or full file path  
    ''' to comply with Windows OS maximum file name length of 255 characters 
    ''' (including the file extension length).
    ''' <para></para>
    ''' If the file name exceeds this limit, it truncates it and 
    ''' adds a ellipsis (…) at the end of the file name.
    ''' <para></para>
    ''' If the path exceeds the MAX_PATH limit (260 characters), 
    ''' it adds the "\\?\" prefix to support extended-length paths.
    ''' <para></para>
    ''' See also: <see href="https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation"/>
    ''' </summary>
    ''' 
    ''' <remarks>
    ''' This method is particularly useful when dealing with file names or file paths that might exceed 
    ''' the maximum allowed length, preventing potential errors related to file name length limitations
    ''' when creating files in the drive.
    ''' </remarks>
    ''' 
    ''' <param name="filePath">
    ''' The file name or full file path.
    ''' </param>
    ''' 
    ''' <param name="maxFileNameLength">
    ''' Optional. The maximum character length that the file name can have. 
    ''' Default (and maximum) value is 255.
    ''' </param>
    ''' 
    ''' <returns>
    ''' The truncated file name or full file path.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function TruncateLongFilePath(filePath As String, Optional maxFileNameLength As Byte = 255) As String

        If String.IsNullOrEmpty(filePath) Then
            Throw New ArgumentNullException(paramName:=NameOf(filePath))
        End If

        If maxFileNameLength = 0 Then
            Throw New ArgumentException("Value must be greater than zero.", paramName:=NameOf(maxFileNameLength))
        End If

        If filePath.StartsWith("\\?\", StringComparison.Ordinal) Then
            filePath = filePath.Substring(4)
        End If

        Dim fileInfo As New FileInfo(If(filePath.Length <= 255, filePath, $"\\?\{filePath}"))
        TruncateLongFilePath(fileInfo, maxFileNameLength)
        Return fileInfo.FullName

    End Function

    ''' <summary>
    ''' If needed, truncates the length of the file name in 
    ''' the source <see cref="FileInfo"/> object to comply with 
    ''' Windows OS maximum file name length of 255 characters 
    ''' (including the file extension length).
    ''' <para></para>
    ''' If the file name exceeds this limit, it truncates it and 
    ''' adds a ellipsis (…) at the end of the file name.
    ''' <para></para>
    ''' If the path exceeds the MAX_PATH limit (260 characters), 
    ''' it adds the "\\?\" prefix to support extended-length paths.
    ''' <para></para>
    ''' See also: <see href="https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation"/>
    ''' </summary>
    ''' 
    ''' <remarks>
    ''' This method is particularly useful when dealing with file paths that might exceed 
    ''' the maximum allowed length, preventing potential errors related to file name length limitations
    ''' when creating files in the drive.
    ''' </remarks>
    ''' 
    ''' <param name="refFileInfo">
    ''' The source <see cref="FileInfo"/> object representing a full file path.
    ''' <para></para>
    ''' When this method returns, this object contains the file path with the file name truncated.
    ''' </param>
    ''' 
    ''' <param name="maxFileNameLength">
    ''' Optional. The maximum character length that the file name can have. 
    ''' Default (and maximum) value is 255.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub TruncateLongFilePath(ByRef refFileInfo As FileInfo, Optional maxFileNameLength As Byte = 255)

        If refFileInfo Is Nothing Then
            Throw New ArgumentNullException(paramName:=NameOf(refFileInfo))
        End If

        If maxFileNameLength = 0 Then
            Throw New ArgumentException("Value must be greater than zero.", paramName:=NameOf(maxFileNameLength))
        End If

        If refFileInfo.Name.Length >= maxFileNameLength Then
            Dim fileExt As String = refFileInfo.Extension
            Dim fileName As String = refFileInfo.Name.Substring(0, maxFileNameLength - 1 - fileExt.Length) & $"…{fileExt}"

            Dim directoryName As String = Path.GetDirectoryName(refFileInfo.FullName)
            If directoryName.Equals("\\?", StringComparison.Ordinal) Then
                refFileInfo = New FileInfo($"\\?\{fileName}")

            ElseIf directoryName.StartsWith("\\?\", StringComparison.Ordinal) Then
                refFileInfo = New FileInfo(Path.Combine(refFileInfo.DirectoryName, fileName))

            Else
                Dim fullpath As String = Path.Combine(refFileInfo.DirectoryName, fileName)
                refFileInfo = If(fullpath.Length >= 260, ' MAX_PATH
                              New FileInfo($"\\?\{fullpath}"),
                              New FileInfo(fullpath))
            End If
        End If

    End Sub

#End Region

End Module

#End Region
