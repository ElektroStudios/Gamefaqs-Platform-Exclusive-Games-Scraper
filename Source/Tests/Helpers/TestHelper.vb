#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Threading

#End Region

#Region " TestHelper "

''' <summary>
''' Provides shared helper members for the unit tests.
''' </summary>
Friend Module TestHelper

#Region " Constants "

    ''' <summary>
    ''' Test category used to mark tests that depend on Windows path semantics
    ''' (drive letters, extended-length "\\?\" prefix, MAX_PATH, etc.).
    ''' </summary>
    Friend Const WindowsOnlyCategory As String = "WindowsOnly"

    ''' <summary>
    ''' The base url of GameFAQs website.
    ''' </summary>
    Friend Const GameFaqsBaseUrl As String = "https://gamefaqs.gamespot.com"

#End Region

#Region " Methods "

    ''' <summary>
    ''' Creates a new <see cref="GameInfo"/> object with the specified values.
    ''' </summary>
    '''
    ''' <param name="title">
    ''' The game title.
    ''' </param>
    '''
    ''' <param name="entryPath">
    ''' Optional. The relative GameFAQs entry path (e.g. "/nes/123-game").
    ''' If not specified, a path is generated from <paramref name="title"/>.
    ''' </param>
    '''
    ''' <param name="releaseDate">
    ''' Optional. The release date. Default value is "1985".
    ''' </param>
    '''
    ''' <param name="genre">
    ''' Optional. The genre. Default value is "Action".
    ''' </param>
    '''
    ''' <returns>
    ''' The resulting <see cref="GameInfo"/> object.
    ''' </returns>
    Friend Function CreateGame(title As String,
                               Optional entryPath As String = Nothing,
                               Optional releaseDate As String = "1985",
                               Optional genre As String = "Action") As GameInfo

        If String.IsNullOrEmpty(entryPath) Then
            Dim slug As String = New String((From c As Char In title.ToLowerInvariant()
                                             Select If(Char.IsLetterOrDigit(c), c, "-"c)).ToArray())
            entryPath = $"/nes/1000-{slug}"
        End If

        Return New GameInfo() With {
            .PlatformName = "Test Platform",
            .Title = title,
            .EntryUrl = New Uri($"{TestHelper.GameFaqsBaseUrl}{entryPath}"),
            .ReleaseDate = releaseDate,
            .Genre = genre
        }

    End Function

    ''' <summary>
    ''' Gets the value of a non-public instance property (e.g. a <see langword="Protected"/> property)
    ''' from the specified object.
    ''' </summary>
    '''
    ''' <typeparam name="T">
    ''' The type of the property value.
    ''' </typeparam>
    '''
    ''' <param name="instance">
    ''' The source object.
    ''' </param>
    '''
    ''' <param name="propertyName">
    ''' The name of the property.
    ''' </param>
    '''
    ''' <returns>
    ''' The property value.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="instance"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="MissingMemberException">
    ''' Thrown when the property can't be found.
    ''' </exception>
    Friend Function GetNonPublicPropertyValue(Of T)(instance As Object, propertyName As String) As T

        If instance Is Nothing Then
            Throw New ArgumentNullException(paramName:=NameOf(instance))
        End If

        Dim prop As PropertyInfo =
            instance.GetType().GetProperty(propertyName, BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.Public)

        If prop Is Nothing Then
            Throw New MissingMemberException(instance.GetType().FullName, propertyName)
        End If

        Return DirectCast(prop.GetValue(instance), T)

    End Function

    ''' <summary>
    ''' Runs the specified action using the specified culture as the current culture of the calling thread,
    ''' and restores the original culture when finished.
    ''' </summary>
    '''
    ''' <param name="cultureName">
    ''' The culture name (e.g. "es-ES").
    ''' </param>
    '''
    ''' <param name="action">
    ''' The action to run.
    ''' </param>
    Friend Sub RunWithCulture(cultureName As String, action As Action)

        If action Is Nothing Then
            Throw New ArgumentNullException(paramName:=NameOf(action))
        End If

        Dim originalCulture As CultureInfo = Thread.CurrentThread.CurrentCulture
        Dim originalUICulture As CultureInfo = Thread.CurrentThread.CurrentUICulture
        Try
            Dim culture As CultureInfo = CultureInfo.GetCultureInfo(cultureName)
            Thread.CurrentThread.CurrentCulture = culture
            Thread.CurrentThread.CurrentUICulture = culture
            action.Invoke()
        Finally
            Thread.CurrentThread.CurrentCulture = originalCulture
            Thread.CurrentThread.CurrentUICulture = originalUICulture
        End Try

    End Sub

    ''' <summary>
    ''' Splits the input text into lines, using any kind of line ending.
    ''' </summary>
    '''
    ''' <param name="value">
    ''' The input text.
    ''' </param>
    '''
    ''' <returns>
    ''' The resulting lines.
    ''' </returns>
    Friend Function SplitLines(value As String) As List(Of String)

        If value Is Nothing Then
            Throw New ArgumentNullException(paramName:=NameOf(value))
        End If

        Return value.Replace(ControlChars.CrLf, ControlChars.Lf).Split(ControlChars.Lf).ToList()

    End Function

#End Region

End Module

#End Region
