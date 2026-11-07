#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Globalization

#End Region

#Region " PlatformBaseTests "

''' <summary>
''' Unit tests for the offline behavior of <see cref="PlatformBase"/> and <see cref="PlatformBaseWithOnlineStore"/> classes,
''' using real platform definitions (<see cref="Global.Platforms.NES"/> and <see cref="Global.Platforms.PlayStation3"/>).
''' </summary>
<TestClass>
Public NotInheritable Class PlatformBaseTests

#Region " Guard Tests (DoScrap not called) "

    ''' <summary>
    ''' Verifies that accessing the game list properties before calling <see cref="PlatformBase.DoScrap"/>
    ''' throws <see cref="InvalidOperationException"/> mentioning the accessed property.
    ''' </summary>
    '''
    ''' <param name="propertyName">
    ''' The name of the property to access.
    ''' </param>
    <TestMethod>
    <DataRow(NameOf(PlatformBase.ExclusiveGames))>
    <DataRow(NameOf(PlatformBase.MultiplatformGames))>
    <DataRow(NameOf(PlatformBase.ExclusiveCompilations))>
    <DataRow(NameOf(PlatformBase.MultiPlatformCompilations))>
    Public Sub GameListProperty_BeforeDoScrap_Throws(propertyName As String)
        Using platform As New Global.Platforms.NES()
            Dim ex As InvalidOperationException =
                Assert.ThrowsExactly(Of InvalidOperationException)(
                    Function() PlatformBaseTests.GetGameListProperty(platform, propertyName))

            StringAssert.Contains(ex.Message, NameOf(PlatformBase.DoScrap))
            StringAssert.EndsWith(ex.Message, $"property: {propertyName}")
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that accessing the online store game list properties before calling <see cref="PlatformBase.DoScrap"/>
    ''' throws <see cref="InvalidOperationException"/> mentioning the accessed property.
    ''' </summary>
    <TestMethod>
    Public Sub OnlineStoreGameListProperties_BeforeDoScrap_Throw()
        Using platform As New Global.Platforms.PlayStation3()
            Dim ex1 As InvalidOperationException =
                Assert.ThrowsExactly(Of InvalidOperationException)(Function() platform.ExclusiveOnlineStoreGames)
            StringAssert.EndsWith(ex1.Message, $"property: {NameOf(PlatformBaseWithOnlineStore.ExclusiveOnlineStoreGames)}")

            Dim ex2 As InvalidOperationException =
                Assert.ThrowsExactly(Of InvalidOperationException)(Function() platform.MultiPlatformOnlineStoreGames)
            StringAssert.EndsWith(ex2.Message, $"property: {NameOf(PlatformBaseWithOnlineStore.MultiPlatformOnlineStoreGames)}")
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that creating the output files before calling <see cref="PlatformBase.DoScrap"/>
    ''' throws <see cref="InvalidOperationException"/> mentioning the called method,
    ''' for both kind of platforms (with and without online store).
    ''' </summary>
    <TestMethod>
    Public Sub CreateOutputFiles_BeforeDoScrap_Throws()
        Dim platforms As New List(Of IPlatform) From {New Global.Platforms.NES(), New Global.Platforms.PlayStation3()}

        For Each platform As IPlatform In platforms
            Dim ex1 As InvalidOperationException = Assert.ThrowsExactly(Of InvalidOperationException)(Sub() platform.CreateMarkdownFile())
            StringAssert.EndsWith(ex1.Message, $"method: {NameOf(IPlatform.CreateMarkdownFile)}")

            Dim ex2 As InvalidOperationException = Assert.ThrowsExactly(Of InvalidOperationException)(Sub() platform.CreateUrlFiles())
            StringAssert.EndsWith(ex2.Message, $"method: {NameOf(IPlatform.CreateUrlFiles)}")

            DirectCast(platform, IDisposable).Dispose()
        Next
    End Sub

#End Region

#Region " Markdown Header Tests "

    ''' <summary>
    ''' Verifies the content of the Markdown header for exclusive titles.
    ''' </summary>
    <TestMethod>
    Public Sub MarkdownHeaderForExclusiveTitles_ContainsPlatformInformation()
        Using platform As New Global.Platforms.NES()
            Dim header As String = platform.MarkdownHeaderForExclusiveTitles
            Dim filtersTable As String = TestHelper.GetNonPublicPropertyValue(Of String)(platform, "MarkdownFiltersTable")

            StringAssert.StartsWith(header, $"# List of exclusive {platform.PlatformInfo.Name} titles.")
            StringAssert.Contains(header, platform.PlatformInfo.AllGamesUrl.ToString())
            StringAssert.Contains(header, filtersTable)
            StringAssert.Contains(header, $"*Last updated on {Date.Now.ToString("MMMM d, yyyy", CultureInfo.GetCultureInfo("en-US"))}*")
        End Using
    End Sub

    ''' <summary>
    ''' Verifies the content of the Markdown header for multiplatform titles.
    ''' </summary>
    <TestMethod>
    Public Sub MarkdownHeaderForMultiPlatformTitles_ContainsPlatformInformation()
        Using platform As New Global.Platforms.NES()
            Dim header As String = platform.MarkdownHeaderForMultiPlatformTitles

            StringAssert.StartsWith(header, $"# List of multiplatform {platform.PlatformInfo.Name} titles.")
            StringAssert.Contains(header, platform.PlatformInfo.AllGamesUrl.ToString())
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the "last updated" date of the Markdown headers is always written in English,
    ''' regardless of the culture of the current thread.
    ''' </summary>
    <TestMethod>
    Public Sub MarkdownHeaders_DateIsCultureIndependent()
        Dim expectedDate As String = Date.Now.ToString("MMMM d, yyyy", CultureInfo.GetCultureInfo("en-US"))

        TestHelper.RunWithCulture("es-ES",
            Sub()
                Using platform As New Global.Platforms.NES()
                    StringAssert.Contains(platform.MarkdownHeaderForExclusiveTitles, expectedDate)
                    StringAssert.Contains(platform.MarkdownHeaderForMultiPlatformTitles, expectedDate)
                End Using
            End Sub)
    End Sub

#End Region

#Region " Dispose Tests "

    ''' <summary>
    ''' Verifies that disposing a platform more than once does not throw.
    ''' </summary>
    <TestMethod>
    Public Sub Dispose_CalledTwice_DoesNotThrow()
        Dim platform As New Global.Platforms.PlayStation3()
        platform.Dispose()
        platform.Dispose()

        Dim platform2 As New Global.Platforms.NES()
        platform2.Dispose()
        platform2.Dispose()
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Gets the value of the specified game list property from the specified platform.
    ''' </summary>
    '''
    ''' <param name="platform">
    ''' The source platform.
    ''' </param>
    '''
    ''' <param name="propertyName">
    ''' The name of the game list property.
    ''' </param>
    '''
    ''' <returns>
    ''' The property value.
    ''' </returns>
    Private Shared Function GetGameListProperty(platform As PlatformBase, propertyName As String) As List(Of GameInfo)
        Select Case propertyName
            Case NameOf(PlatformBase.ExclusiveGames)
                Return platform.ExclusiveGames
            Case NameOf(PlatformBase.MultiplatformGames)
                Return platform.MultiplatformGames
            Case NameOf(PlatformBase.ExclusiveCompilations)
                Return platform.ExclusiveCompilations
            Case NameOf(PlatformBase.MultiPlatformCompilations)
                Return platform.MultiPlatformCompilations
            Case Else
                Throw New ArgumentException($"Unknown property: {propertyName}", paramName:=NameOf(propertyName))
        End Select
    End Function

#End Region

End Class

#End Region
