#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection
Imports System.Text.RegularExpressions

#End Region

#Region " PlatformDefinitionsTests "

''' <summary>
''' Data-integrity tests for all the platform classes defined in the <c>Platforms</c> namespace.
''' <para></para>
''' These tests protect against copy-paste mistakes when adding new platforms
''' (duplicated names or urls, wrong platform type, missing filters table, etc.).
''' </summary>
<TestClass>
Public NotInheritable Class PlatformDefinitionsTests

#Region " Fields "

    ''' <summary>
    ''' The platforms defined in the application assembly.
    ''' </summary>
    Private Shared platforms As List(Of IPlatform)

    ''' <summary>
    ''' The platform types defined in the <c>Platforms</c> namespace of the application assembly.
    ''' </summary>
    Private Shared platformTypes As List(Of Type)

#End Region

#Region " Initialization "

    ''' <summary>
    ''' Initializes the shared test data.
    ''' </summary>
    '''
    ''' <param name="context">
    ''' The test context.
    ''' </param>
    <ClassInitialize>
    Public Shared Sub ClassInitialize(context As TestContext)
        PlatformDefinitionsTests.platforms = MiscUtil.GetDefinedPlatforms()
        PlatformDefinitionsTests.platformTypes =
            (From t As Type In GetType(IPlatform).Assembly.GetTypes()
             Where t.Namespace = "Platforms" AndAlso GetType(IPlatform).IsAssignableFrom(t)
            ).ToList()
    End Sub

#End Region

#Region " Tests "

    ''' <summary>
    ''' Verifies that the platforms are discovered and every platform class is instantiated.
    ''' </summary>
    <TestMethod>
    Public Sub AllPlatformTypes_AreDiscovered()
        Assert.IsNotEmpty(PlatformDefinitionsTests.platforms)
        Assert.HasCount(PlatformDefinitionsTests.platformTypes.Count, PlatformDefinitionsTests.platforms)
    End Sub

    ''' <summary>
    ''' Verifies that every platform class is sealed, concrete, and inherits from <see cref="PlatformBase"/>.
    ''' </summary>
    <TestMethod>
    Public Sub AllPlatformTypes_AreSealedPlatformBaseClasses()
        Dim failures As New List(Of String)

        For Each t As Type In PlatformDefinitionsTests.platformTypes
            If Not t.IsClass OrElse t.IsAbstract Then
                failures.Add($"{t.FullName}: must be a concrete class.")
            End If
            If Not t.IsSealed Then
                failures.Add($"{t.FullName}: must be NotInheritable.")
            End If
            If Not GetType(PlatformBase).IsAssignableFrom(t) Then
                failures.Add($"{t.FullName}: must inherit from {NameOf(PlatformBase)}.")
            End If
        Next

        PlatformDefinitionsTests.AssertNoFailures(failures)
    End Sub

    ''' <summary>
    ''' Verifies that the platform names are unique (case-insensitive).
    ''' </summary>
    <TestMethod>
    Public Sub PlatformNames_AreUnique()
        Dim duplicates As List(Of String) =
            (From platform As IPlatform In PlatformDefinitionsTests.platforms
             Group By name = platform.PlatformInfo.Name.ToUpperInvariant() Into g = Group
             Where g.Count() > 1
             Select name).ToList()

        PlatformDefinitionsTests.AssertNoFailures(duplicates)
    End Sub

    ''' <summary>
    ''' Verifies that the platform http names are unique, so no platform scraps the game list of another platform.
    ''' </summary>
    <TestMethod>
    Public Sub PlatformHttpNames_AreUnique()
        Dim duplicates As List(Of String) =
            (From platform As IPlatform In PlatformDefinitionsTests.platforms
             Group By httpName = platform.PlatformInfo.HttpName Into g = Group
             Where g.Count() > 1
             Select $"{httpName}: {String.Join(", ", g.Select(Function(p As IPlatform) p.PlatformInfo.Name))}").ToList()

        PlatformDefinitionsTests.AssertNoFailures(duplicates)
    End Sub

    ''' <summary>
    ''' Verifies that the platform http names have the format used in GameFAQs urls (lowercase alphanumeric and hyphens).
    ''' </summary>
    <TestMethod>
    Public Sub PlatformHttpNames_HaveValidUrlFormat()
        Dim httpNameRegex As New Regex("^[a-z0-9]+(-[a-z0-9]+)*$")

        Dim failures As List(Of String) =
            (From platform As IPlatform In PlatformDefinitionsTests.platforms
             Where Not httpNameRegex.IsMatch(platform.PlatformInfo.HttpName)
             Select $"{platform.PlatformInfo.Name}: '{platform.PlatformInfo.HttpName}'").ToList()

        PlatformDefinitionsTests.AssertNoFailures(failures)
    End Sub

    ''' <summary>
    ''' Verifies that the platform names have no leading or trailing white-spaces.
    ''' </summary>
    <TestMethod>
    Public Sub PlatformNames_AreTrimmed()
        Dim failures As List(Of String) =
            (From platform As IPlatform In PlatformDefinitionsTests.platforms
             Where platform.PlatformInfo.Name <> platform.PlatformInfo.Name.Trim()
             Select $"'{platform.PlatformInfo.Name}'").ToList()

        PlatformDefinitionsTests.AssertNoFailures(failures)
    End Sub

    ''' <summary>
    ''' Verifies that every platform has a defined <see cref="PlatformType"/> value.
    ''' </summary>
    <TestMethod>
    Public Sub PlatformTypes_AreDefinedEnumValues()
        Dim failures As List(Of String) =
            (From platform As IPlatform In PlatformDefinitionsTests.platforms
             Where Not [Enum].IsDefined(GetType(PlatformType), platform.PlatformInfo.PlatformType)
             Select platform.PlatformInfo.Name).ToList()

        PlatformDefinitionsTests.AssertNoFailures(failures)
    End Sub

    ''' <summary>
    ''' Verifies that every platform url points to GameFAQs website over HTTPS.
    ''' </summary>
    <TestMethod>
    Public Sub PlatformUrls_PointToGameFaqs()
        Dim failures As New List(Of String)

        For Each platform As IPlatform In PlatformDefinitionsTests.platforms
            Dim url As Uri = platform.PlatformInfo.AllGamesUrl
            If url.Scheme <> Uri.UriSchemeHttps OrElse url.Host <> "gamefaqs.gamespot.com" Then
                failures.Add($"{platform.PlatformInfo.Name}: {url}")
            End If
            If url.AbsolutePath <> $"/{platform.PlatformInfo.HttpName}/category/999-all" Then
                failures.Add($"{platform.PlatformInfo.Name}: unexpected path: {url.AbsolutePath}")
            End If
        Next

        PlatformDefinitionsTests.AssertNoFailures(failures)
    End Sub

    ''' <summary>
    ''' Verifies that every platform defines the Markdown filters table (included and excluded content),
    ''' and that the hyperlinks of the table point to the same platform.
    ''' </summary>
    <TestMethod>
    Public Sub MarkdownFiltersTable_IsDefinedAndPointsToSamePlatform()
        Dim failures As New List(Of String)
        Dim linkRegex As New Regex("\]\((?<url>https?://[^)]+)\)")

        For Each platform As IPlatform In PlatformDefinitionsTests.platforms
            Dim table As String = TestHelper.GetNonPublicPropertyValue(Of String)(platform, "MarkdownFiltersTable")

            If String.IsNullOrWhiteSpace(table) Then
                failures.Add($"{platform.PlatformInfo.Name}: filters table is empty.")
                Continue For
            End If

            If Not table.Contains("|Included:|Excluded:|") Then
                failures.Add($"{platform.PlatformInfo.Name}: filters table has no 'Included/Excluded' header.")
            End If

            Dim expectedUrlPrefix As String = $"{platform.PlatformInfo.BaseUrl}/"
            For Each m As Match In linkRegex.Matches(table)
                Dim url As String = m.Groups("url").Value
                If Not url.StartsWith(expectedUrlPrefix, StringComparison.Ordinal) Then
                    failures.Add($"{platform.PlatformInfo.Name}: filters table link points to another platform: {url}")
                End If
            Next
        Next

        PlatformDefinitionsTests.AssertNoFailures(failures)
    End Sub

    ''' <summary>
    ''' Verifies that every platform with online store defines at least one market distribution,
    ''' with valid ids and names.
    ''' </summary>
    <TestMethod>
    Public Sub OnlineStorePlatforms_DefineValidMarketDistributions()
        Dim storePlatforms As List(Of PlatformBaseWithOnlineStore) =
            PlatformDefinitionsTests.platforms.OfType(Of PlatformBaseWithOnlineStore)().ToList()

        Assert.IsNotEmpty(storePlatforms)

        Dim failures As New List(Of String)
        For Each platform As PlatformBaseWithOnlineStore In storePlatforms
            Dim distributions As Dictionary(Of Integer, String) =
                TestHelper.GetNonPublicPropertyValue(Of Dictionary(Of Integer, String))(platform, "MarketDistributionIds")

            If distributions Is Nothing OrElse distributions.Count = 0 Then
                failures.Add($"{platform.PlatformInfo.Name}: no market distributions defined.")
                Continue For
            End If

            For Each pair As KeyValuePair(Of Integer, String) In distributions
                If pair.Key <= 0 Then
                    failures.Add($"{platform.PlatformInfo.Name}: invalid distribution id: {pair.Key}")
                End If
                If String.IsNullOrWhiteSpace(pair.Value) Then
                    failures.Add($"{platform.PlatformInfo.Name}: empty distribution name for id: {pair.Key}")
                End If
            Next

            If distributions.Values.Distinct(StringComparer.OrdinalIgnoreCase).Count() <> distributions.Count Then
                failures.Add($"{platform.PlatformInfo.Name}: duplicated distribution names.")
            End If
        Next

        PlatformDefinitionsTests.AssertNoFailures(failures)
    End Sub

    ''' <summary>
    ''' Verifies that a GameFAQs market distribution id is not assigned to more than one platform
    ''' (distribution ids are global in GameFAQs, e.g. "?dist=17" is "PlayStation Store PS3").
    ''' </summary>
    <TestMethod>
    Public Sub OnlineStorePlatforms_MarketDistributionIdsAreUniqueAcrossPlatforms()
        Dim allIds As List(Of KeyValuePair(Of Integer, String)) =
            (From platform As PlatformBaseWithOnlineStore In PlatformDefinitionsTests.platforms.OfType(Of PlatformBaseWithOnlineStore)()
             From pair As KeyValuePair(Of Integer, String) In TestHelper.GetNonPublicPropertyValue(Of Dictionary(Of Integer, String))(platform, "MarketDistributionIds")
             Select New KeyValuePair(Of Integer, String)(pair.Key, platform.PlatformInfo.Name)).ToList()

        Dim duplicates As List(Of String) =
            (From pair As KeyValuePair(Of Integer, String) In allIds
             Group By id = pair.Key Into g = Group
             Where g.Count() > 1
             Select $"dist={id}: {String.Join(", ", g.Select(Function(p As KeyValuePair(Of Integer, String)) p.Value))}").ToList()

        PlatformDefinitionsTests.AssertNoFailures(duplicates)
    End Sub

    ''' <summary>
    ''' Verifies that every platform class has an accessible parameterless constructor,
    ''' required by <see cref="MiscUtil.GetDefinedPlatforms()"/> to create the instances.
    ''' </summary>
    <TestMethod>
    Public Sub AllPlatformTypes_HaveParameterlessConstructor()
        Dim failures As List(Of String) =
            (From t As Type In PlatformDefinitionsTests.platformTypes
             Where t.GetConstructor(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic, Nothing, Type.EmptyTypes, Nothing) Is Nothing
             Select t.FullName).ToList()

        PlatformDefinitionsTests.AssertNoFailures(failures)
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Fails the current test if the specified failure list is not empty,
    ''' reporting all the failures in the assertion message.
    ''' </summary>
    '''
    ''' <param name="failures">
    ''' The failure messages.
    ''' </param>
    Private Shared Sub AssertNoFailures(failures As List(Of String))
        If failures.Count <> 0 Then
            Assert.Fail($"{failures.Count} problem(s) found:{Environment.NewLine}{String.Join(Environment.NewLine, failures)}")
        End If
    End Sub

#End Region

End Class

#End Region
