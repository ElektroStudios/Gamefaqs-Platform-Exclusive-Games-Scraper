#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq

Imports Win32

#End Region

#Region " PlatformBaseWithOnlineStore"

''' <summary>
''' Base class used to implement console platforms having an online game store.
''' </summary>
Friend MustInherit Class PlatformBaseWithOnlineStore : Inherits PlatformBase

#Region " Properties "

    ''' <summary>
    ''' Gets a <see cref="Dictionary(Of Integer, String)"/> where the key is the id. of the "dist" parameter 
    ''' that points to the online store games list for this paltform, and the value is the store name.
    ''' <para></para>
    ''' (e.g. the number '17' at the end of this url: 
    ''' <para></para>
    ''' https://gamefaqs.gamespot.com/ps3/category/49-miscellaneous?dist=17,
    ''' <para></para>
    ''' or the number '26' at the end of this url:
    ''' <para></para>
    ''' https://gamefaqs.gamespot.com/ps4/category/49-miscellaneous?dist=26)
    ''' </summary>
    Protected MustOverride ReadOnly Property MarketDistributionIds As Dictionary(Of Integer, String)

    ''' <summary>
    ''' Gets the games that were released exclusively on this platform and distributed via its online store.
    ''' <para></para>
    ''' Note: You must call method <see cref="PlatformBaseWithOnlineStore.DoScrap"/> to initialize the value of this property.
    ''' </summary>
    Friend ReadOnly Property ExclusiveOnlineStoreGames As Dictionary(Of String, List(Of GameInfo))
        <DebuggerStepThrough>
        Get
            Me.FailIfScrapNotCompleted()
            Return Me.exclusiveOnlineStoreGames_
        End Get
    End Property
    ''' <summary>
    ''' ( Backing Field )
    ''' <para></para>
    ''' The games that were released exclusively on this platform and distributed via its online store.
    ''' </summary>
    Protected exclusiveOnlineStoreGames_ As Dictionary(Of String, List(Of GameInfo))

    ''' <summary>
    ''' Gets the multi-platform games that were released on this platform and distributed via its online store.
    ''' <para></para>
    ''' Note: You must call method <see cref="PlatformBaseWithOnlineStore.DoScrap"/> to initialize the value of this property.
    ''' </summary>
    Friend ReadOnly Property MultiPlatformOnlineStoreGames As Dictionary(Of String, List(Of GameInfo))
        <DebuggerStepThrough>
        Get
            Me.FailIfScrapNotCompleted()
            Return Me.multiPlatformOnlineStoreGames_
        End Get
    End Property
    ''' <summary>
    ''' ( Backing Field )
    ''' <para></para>
    ''' The multi-platform games that were released on this platform and distributed via its online store.
    ''' </summary>
    Protected multiPlatformOnlineStoreGames_ As Dictionary(Of String, List(Of GameInfo))

#End Region

#Region " IPlatform Methods "

    ''' <summary>
    ''' Does the scraping of platform exclusive games for this platform.
    ''' <para></para>
    ''' Calling this method will initialize the following properties with the scraped items: 
    ''' <para></para> - <see cref="ExclusiveGames"/> 
    ''' <para></para> - <see cref="ExclusiveOnlineStoreGames"/> 
    ''' <para></para> - <see cref="ExclusiveCompilations"/>
    ''' <para></para> - <see cref="MultiplatformGames"/> 
    ''' <para></para> - <see cref="MultiplatformOnlineStoreGames"/> 
    ''' <para></para> - <see cref="MultiplatformCompilations"/>
    ''' </summary>
    <DebuggerStepperBoundary>
    Friend Overrides Sub DoScrap()
        Me.scrapCompleted = False

        ' Scrap exclusive and multi-platform games.
        GamefaqsUtil.ScrapGames(
            $"{Me.PlatformInfo.Name} Games", Me.PlatformInfo,
            Me.PlatformInfo.AllGamesUrl,
            Me.exclusiveGames_, Me.multiPlatformGames_)

        ' Scrap Online Store Game lists (only their entry urls).
        Dim allOnlineStoreGames As New Dictionary(Of String, List(Of GameInfo))
        For Each pair As KeyValuePair(Of Integer, String) In Me.MarketDistributionIds
            Dim onlineStoreGames As List(Of GameInfo) =
                GamefaqsUtil.ScrapOnlyEntryUrls(
                    $"{Me.PlatformInfo.Name} ({pair.Value}) Games (only entry urls)", Me.PlatformInfo,
                    New Uri($"{Me.PlatformInfo.AllGamesUrl}?dist={pair.Key}"))

            allOnlineStoreGames.Add(pair.Value, onlineStoreGames)
        Next pair

        ' Build Exclusive Online Store Games lists.
        Me.exclusiveOnlineStoreGames_ = New Dictionary(Of String, List(Of GameInfo))
        For Each pair As KeyValuePair(Of String, List(Of GameInfo)) In allOnlineStoreGames
            Dim onlineStoreGameUrls As Uri() =
                (From game As GameInfo In pair.Value
                 Select game.EntryUrl
                )?.ToArray()

            Dim onlineStoreGames As List(Of GameInfo) =
                (From game As GameInfo In Me.exclusiveGames_
                 Where onlineStoreGameUrls.Contains(game.EntryUrl)
                ).ToList()

            Me.exclusiveOnlineStoreGames_.Add(pair.Key, onlineStoreGames)
        Next pair

        ' Build Multi-platform Online Store Games lists.
        Me.multiPlatformOnlineStoreGames_ = New Dictionary(Of String, List(Of GameInfo))
        For Each pair As KeyValuePair(Of String, List(Of GameInfo)) In allOnlineStoreGames
            Dim onlineStoreGameUrls As Uri() =
                (From game As GameInfo In pair.Value
                 Select game.EntryUrl
                )?.ToArray()

            Dim onlineStoreGames As List(Of GameInfo) =
                (From game As GameInfo In Me.multiPlatformGames_
                 Where onlineStoreGameUrls.Contains(game.EntryUrl)
                ).ToList()

            Me.multiPlatformOnlineStoreGames_.Add(pair.Key, onlineStoreGames)
        Next pair

        ' Filter out Exclusive Online Store Games from Exclusive Games list.
        Me.exclusiveGames_ =
                (From game As GameInfo In Me.exclusiveGames_
                 Where Not Me.exclusiveOnlineStoreGames_.Values.
                              SelectMany(Of GameInfo)(Function(list) list).
                              Select(Function(item As GameInfo) item.EntryUrl).
                              Contains(game.EntryUrl)
                )?.ToList()

        ' Filter out Multi-platform Online Store Games from Multi-platform Games list.
        Me.multiPlatformGames_ =
                (From game As GameInfo In Me.multiPlatformGames_
                 Where Not Me.multiPlatformOnlineStoreGames_.Values.
                              SelectMany(Of GameInfo)(Function(list) list).
                              Select(Function(item As GameInfo) item.EntryUrl).
                              Contains(game.EntryUrl)
                )?.ToList()

        ' Build Exclusive Compilations list.
        Me.exclusiveCompilations_ =
            (From game As GameInfo In Me.exclusiveGames_?.DefaultIfEmpty(New GameInfo()).Concat(Me.exclusiveOnlineStoreGames_.Values.SelectMany(Of GameInfo)(Function(list) list))
             Where game.Genre?.Contains("Compilation")
            )?.ToList()

        If Me.exclusiveCompilations_?.Any() Then
            ' Filter out Exclusive Compilations from Exclusive Games list.
            Me.exclusiveGames_ =
                (From game As GameInfo In Me.exclusiveGames_
                 Where Not Me.exclusiveCompilations_?.Select(Function(item As GameInfo) item.EntryUrl).Contains(game.EntryUrl)
                ).ToList()

            ' Filter out Exclusive Compilations from Exclusive Online Store Games lists.
            Dim tempDict As New Dictionary(Of String, List(Of GameInfo))
            Dim keysExclusives As String() = Me.exclusiveOnlineStoreGames_?.Keys.ToArray()
            For Each key As String In keysExclusives
                Me.exclusiveOnlineStoreGames_(key) =
                    (From game As GameInfo In Me.exclusiveOnlineStoreGames_(key)
                     Where Not Me.exclusiveCompilations_?.Select(Function(item As GameInfo) item.EntryUrl).Contains(game.EntryUrl)
                    ).ToList()
            Next key
        End If

        ' Build Multi-platform Compilations list.
        Me.multiPlatformCompilations_ =
            (From game As GameInfo In Me.multiPlatformGames_?.DefaultIfEmpty(New GameInfo()).Concat(Me.multiPlatformOnlineStoreGames_.Values.SelectMany(Of GameInfo)(Function(list) list))
             Where game.Genre.Contains("Compilation")
            )?.ToList()

        If Me.multiPlatformCompilations_?.Any() Then
            ' Filter out Multi-platform Compilations from Multi-platform Games list.
            Me.multiPlatformGames_ =
                (From game As GameInfo In Me.multiPlatformGames_
                 Where Not Me.multiPlatformCompilations_?.Select(Function(item As GameInfo) item.EntryUrl).Contains(game.EntryUrl)
                ).ToList()

            ' Filter out Multi-platform Compilations from Multi-platform Online Store Games lists.
            Dim keysMultiplatform As String() = Me.multiPlatformOnlineStoreGames_?.Keys.ToArray()
            For Each key As String In keysMultiplatform
                Me.multiPlatformOnlineStoreGames_(key) =
                    (From game As GameInfo In Me.multiPlatformOnlineStoreGames_(key)
                     Where Not Me.multiPlatformCompilations_?.Select(Function(item As GameInfo) item.EntryUrl).Contains(game.EntryUrl)
                    ).ToList()
            Next key
        End If

        ' Sort lists in ascending order and using Natural Sort algorithm. 
        Me.exclusiveGames_?.Sort(Function(first, second) NativeMethods.StrCmpLogicalW(first.Title, second.Title))
        Me.exclusiveCompilations_?.Sort(Function(first, second) NativeMethods.StrCmpLogicalW(first.Title, second.Title))
        Me.multiPlatformGames_?.Sort(Function(first, second) NativeMethods.StrCmpLogicalW(first.Title, second.Title))
        Me.multiPlatformCompilations_?.Sort(Function(first, second) NativeMethods.StrCmpLogicalW(first.Title, second.Title))
        For Each pair As KeyValuePair(Of String, List(Of GameInfo)) In Me.exclusiveOnlineStoreGames_
            Me.exclusiveOnlineStoreGames_(pair.Key).Sort(Function(first, second) NativeMethods.StrCmpLogicalW(first.Title, second.Title))
        Next pair
        For Each pair As KeyValuePair(Of String, List(Of GameInfo)) In Me.multiPlatformOnlineStoreGames_
            Me.multiPlatformOnlineStoreGames_(pair.Key).Sort(Function(first, second) NativeMethods.StrCmpLogicalW(first.Title, second.Title))
        Next pair

        Console.WriteLine("")
        Console.WriteLine($"Scraping completed for {Me.PlatformInfo.Name} platform.")
        Console.WriteLine("")

        Me.scrapCompleted = True
    End Sub

    ''' <summary>
    ''' Creates the markdown file and writes the tables with the scraped games.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Overrides Sub CreateMarkdownFile()
        Me.FailIfScrapNotCompleted()

        Dim platformName As String = MiscUtil.ConvertStringToWindowsFileName(Me.PlatformInfo.Name)

        ' Write exclusive titles.

        Dim exclusiveGamesTable As String =
            If(Me.exclusiveGames_.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{platformName}∶ Exclusive Games",
                                               Me.exclusiveGames_), "")

        Dim exclusiveOnlineStoreGamesTables As New List(Of String)
        For Each pair As KeyValuePair(Of String, List(Of GameInfo)) In Me.exclusiveOnlineStoreGames_
            exclusiveOnlineStoreGamesTables.Add(If(pair.Value?.Any(),
                GamefaqsUtil.BuildMarkdownTable($"{platformName}∶ Exclusive Games ({pair.Key})", pair.Value,
                                                extraHeaderContent:="Note: Some games in this table may have also been released " &
                                                                    "in physical format on this platform, but they are categorized " &
                                                                    $"on Gamefaqs as games distributed via '{pair.Key}'."), ""))
        Next pair

        Dim exclusiveCompilationsTable As String =
            If(Me.exclusiveCompilations_.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{platformName}∶ Exclusive Compilations",
                                               Me.exclusiveCompilations_), "")

        GamefaqsUtil.CreateMarkdownFile(platformName, $"{platformName} (Exclusives)", Me.MarkdownHeaderForExclusiveTitles,
                                        exclusiveGamesTable, String.Join(Environment.NewLine, exclusiveOnlineStoreGamesTables), exclusiveCompilationsTable)

        ' Write multi-platform titles.

        Dim multiPlatformGamesTable As String =
            If(Me.multiPlatformGames_.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{platformName}∶ Multi-platform Games",
                                               Me.multiPlatformGames_), "")

        Dim multiPlatformOnlineStoreGamesTables As New List(Of String)
        For Each pair As KeyValuePair(Of String, List(Of GameInfo)) In Me.multiPlatformOnlineStoreGames_
            multiPlatformOnlineStoreGamesTables.Add(If(pair.Value?.Any(),
                GamefaqsUtil.BuildMarkdownTable($"{platformName}∶ Multi-platform Games ({pair.Key})", pair.Value,
                                                extraHeaderContent:="Note: Some games in this table may have also been released " &
                                                                    "in physical format on this platform, but they are categorized " &
                                                                    $"on Gamefaqs as games distributed via '{pair.Key}'."), ""))
        Next pair

        Dim multiPlatformCompilationsTable As String =
            If(Me.multiPlatformCompilations_.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{platformName}∶ Multi-platform Compilations",
                                               Me.multiPlatformCompilations_), "")

        GamefaqsUtil.CreateMarkdownFile(platformName, $"{platformName} (Multi-platform)", Me.MarkdownHeaderForMultiPlatformTitles,
                                        multiPlatformGamesTable, String.Join(Environment.NewLine, multiPlatformOnlineStoreGamesTables), multiPlatformCompilationsTable)

    End Sub

    ''' <summary>
    ''' Creates the URL files for each scraped game.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Overrides Sub CreateUrlFiles()
        ' Create Urls from exclusive and multi-platform titles.
        MyBase.CreateUrlFiles()

        Dim platformName As String = MiscUtil.ConvertStringToWindowsFileName(Me.PlatformInfo.Name)

        ' Create Url files from exclusive online store titles.
        For Each pair As KeyValuePair(Of String, List(Of GameInfo)) In Me.exclusiveOnlineStoreGames_
            GamefaqsUtil.CreateUrlFiles(platformName, $"{platformName}∶ Exclusive Games ({pair.Key})",
                                        pair.Value)
        Next pair

        ' Create Url files from multi-platform online store titles.
        For Each pair As KeyValuePair(Of String, List(Of GameInfo)) In Me.multiPlatformOnlineStoreGames_
            GamefaqsUtil.CreateUrlFiles(platformName, $"{platformName}∶ Multi-platform Games ({pair.Key})",
                                        pair.Value)
        Next pair
    End Sub

#End Region

#Region " IDisposable Implementation "

    ''' <summary>
    ''' Releases all resources used by this instance.
    ''' </summary>
    ''' 
    ''' <param name="disposing">
    ''' <see langword="True"/> to release both managed and unmanaged resources;
    ''' <see langword="False"/> to release only unmanaged resources.
    ''' </param>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If Not MyBase.disposedValue Then
            If disposing Then
                Me.exclusiveGames_?.Clear()
                Me.exclusiveCompilations_?.Clear()
                Me.exclusiveOnlineStoreGames_?.Clear()

                Me.multiPlatformGames_?.Clear()
                Me.multiPlatformCompilations_?.Clear()
                Me.multiPlatformOnlineStoreGames_?.Clear()

                Me.exclusiveGames_ = Nothing
                Me.exclusiveCompilations_ = Nothing
                Me.exclusiveOnlineStoreGames_ = Nothing

                Me.multiPlatformGames_ = Nothing
                Me.multiPlatformCompilations_ = Nothing
                Me.multiPlatformOnlineStoreGames_ = Nothing
            End If
            Me.disposedValue = True
        End If
    End Sub

#End Region

End Class

#End Region
