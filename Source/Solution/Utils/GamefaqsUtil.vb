﻿#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Imports HtmlAgilityPack

#End Region

#Region " GamefaqsUtil "

''' <summary>
''' Gamefaqs Scraping and File Creation Utilities.
''' </summary>
<HideModuleName>
Friend Module GamefaqsUtil

#Region " Public Fields "

    ''' <summary>
    ''' User-Agent used to let Gamefaqs server identify this scraper.
    ''' </summary>
    Friend Const ScraperUserAgent As String =
            "Get_Platform_Exclusive_Games_Bot/1.0 (Windows; .NET Framework 4.8; non-harmful scraper; bot; scraper; en-US)"

#End Region

#Region " Scrap Methods "

    ''' <summary>
    ''' Scraps the source game list url to retrieve the last page number 
    ''' from the paginate class (&lt;ul class="paginate"&gt;).
    ''' </summary>
    ''' 
    ''' <param name="uri">
    ''' The source url where to retrieve the last page number.
    ''' </param>
    ''' 
    ''' <returns>
    ''' The last page number.
    ''' </returns>
    <DebuggerStepperBoundary>
    Friend Function ScrapLastPageNumber(uri As Uri) As Integer

        Dim htmlSource As String = Nothing
        MiscUtil.DownloadHtmlPageWithRetry(uri, htmlSource)

        Dim htmlDoc As New HtmlDocument()
        htmlDoc.LoadHtml(htmlSource)

        Dim paginateXpath As String = "//ul[@class='paginate']/li"
        Dim paginateNode As HtmlNode = htmlDoc.DocumentNode.SelectSingleNode(paginateXpath)
        Dim paginateText As String = paginateNode?.GetDirectInnerText()

        If (paginateNode Is Nothing) OrElse String.IsNullOrEmpty(paginateText) Then
            ' Ignore. Some lists only have one page and does not contain the paginate node.
            ' for example: https://gamefaqs.gamespot.com/atari7800/category/999-all
            ' MiscUtil.PrintErrorAndExit($"Can't locate paginate element (XPath: ""{paginateXpath}"") in html source-code of uri: {uri}", exitcode:=ExitCodes.ExitCodeXPathNotFound)
            Return 1
        End If

        Dim lastPageNumber As Integer = CInt(paginateText.Split({" "c}, StringSplitOptions.RemoveEmptyEntries).Last())
        Return lastPageNumber

    End Function

    ''' <summary>
    ''' Scraps only the platform exclusive games from the input list url,
    ''' and returns a <see cref="List(Of GameInfo)"/> representing each scraped game.
    ''' </summary>
    ''' 
    ''' <param name="description">
    ''' The description name (e.g. PlayStation Store PS3) for the games in the provided list uri.
    ''' </param>
    ''' 
    ''' <param name="platform">
    ''' The source <see cref="PlatformInfo"/> for which the games in the provided list uri belongs to.
    ''' </param>
    ''' 
    ''' <param name="uri">
    ''' The input game list uri where to scrap the platform exclusive games.
    ''' </param>
    ''' 
    ''' <param name="refExclusiveGamesList">
    ''' A <see langword="Byref"/> <see cref="List(Of GameInfo)"/> object used to populate 
    ''' the games that were released exclusively on the specified platform in <paramref name="platform"/> parameter.
    ''' </param>
    ''' 
    ''' <param name="refMultiplatformGamesList">
    ''' A <see langword="Byref"/> <see cref="List(Of GameInfo)"/> object used to populate 
    ''' the multi-platform games that were released on the specified platform in <paramref name="platform"/> parameter.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub ScrapGames(description As String, platform As PlatformInfo, uri As Uri,
                          ByRef refExclusiveGamesList As List(Of GameInfo),
                          ByRef refMultiplatformGamesList As List(Of GameInfo))

        refExclusiveGamesList = New List(Of GameInfo)
        refMultiplatformGamesList = New List(Of GameInfo)

        Dim lastPageNumber As Integer = GamefaqsUtil.ScrapLastPageNumber(uri)
        Dim currentTotalEntryCount As Integer
        Dim startTime As Date = Date.Now

        ' Regular expression to replace the date string (e.g. "January 1, 1985") to a simple year value (e.g. "1985").
        Dim dateRegex As New Regex(""">(?<date>.+)</a>", RegexOptions.Compiled)

        For pageIndex As Integer = 0 To lastPageNumber - 1

            Dim thisPageCurrentEntryCount As Integer = 0

            Dim pageUri As Uri =
                If(uri.ToString().Contains("?"c),
                   New Uri($"{uri}&page={pageIndex}"),
                   New Uri($"{uri}?page={pageIndex}"))

            Console.WriteLine($"Parsing '{description}' page {pageIndex + 1} of {lastPageNumber} ...")
            Console.WriteLine($"Url: '{pageUri} ...")
            Console.WriteLine("")

            Dim pageHtmlSource As String = Nothing
            MiscUtil.DownloadHtmlPageWithRetry(pageUri, pageHtmlSource)

            Dim htmlDoc As New HtmlDocument
            htmlDoc.LoadHtml(pageHtmlSource)

            Const titleXpath As String = "//td[@class='rtitle']"
            Dim titleNodes As HtmlNodeCollection = htmlDoc.DocumentNode.SelectNodes(titleXpath)
            If titleNodes Is Nothing Then
                If (pageIndex <> lastPageNumber - 1) Then
                    MiscUtil.PrintErrorAndExit($"Can't locate game title elements (XPath: ""{titleXpath}"") in html source-code of uri: {pageUri}", exitcode:=ExitCodes.ExitCodeXPathNotFound)
                Else
                    ' Sometimes the last page exists but it does not contain any entry,
                    ' for example: https://gamefaqs.gamespot.com/atari5200/category/999-all?page=1
                    ' so we just finish the work.
                    Exit For
                End If
            End If

            ' Iterate game entry urls.

            Dim thisPagelastEntryCount As Integer = titleNodes.Count

            For Each titleNode As HtmlNode In titleNodes
                Dim ETA As String =
                    GamefaqsUtil.CalculateETA(startTime, pageIndex, Interlocked.Increment(thisPageCurrentEntryCount),
                                              lastPageNumber, Interlocked.Increment(currentTotalEntryCount),
                                              If(pageIndex <> (lastPageNumber - 1), 100, thisPagelastEntryCount))

                Dim nodeInnerHtml As String = titleNode.InnerHtml
                Dim entryTitle As String = titleNode.InnerText.Trim()

                Console.WriteLine($"Scraping {description}... | ETA {ETA} | Page {pageIndex + 1}/{lastPageNumber} | Entry {currentTotalEntryCount} ({thisPageCurrentEntryCount}/{thisPagelastEntryCount}) | {entryTitle}")
                ' Console.WriteLine($"Url: {entryUrl}")
                ' Console.WriteLine("")

                If nodeInnerHtml.Contains("""cancel""") Then ' Cancelled game.
                    Continue For

                ElseIf nodeInnerHtml.Contains("""unrel""") Then ' Not yet released game.
                    Continue For

                Else ' Released game.
                    MiscUtil.SleepRandom(100, 600)

                    ' Note that the "titleNode.InnerText" value can return a game title in Japanese or other language,
                    ' so this value can't be considered as the proper game title name to use.
                    Dim entryBaseUrl As String = titleNode.SelectSingleNode("a").Attributes("href").Value
                    Dim entryUrl As New Uri($"https://gamefaqs.gamespot.com{entryBaseUrl}")

                    ' Some entry urls are duplicated in the Gamefaqs list,
                    ' such as one entry with Japanese title and the other with English title,
                    ' all duplicates points to the same entry url,
                    ' so we ensure that the entry url does not already exists in the list.
                    Dim entryExists As Boolean = (From item As GameInfo In refExclusiveGamesList.Concat(refMultiplatformGamesList) Where item.EntryUrl.Equals(entryUrl)).Any()
                    If entryExists Then
                        Continue For
                    End If

                    Dim gameEntryHtmlsource As String = Nothing
                    MiscUtil.DownloadHtmlPageWithRetry(entryUrl, gameEntryHtmlsource)

                    Dim gameEntryHtmlDoc As New HtmlDocument()
                    gameEntryHtmlDoc.LoadHtml(gameEntryHtmlsource)

                    Const contentXpath As String = "//div[@class='content']"

                    ' 2. Determine whether this entry belongs to an expansion or DLC content:

                    Dim expansionNode As HtmlNode =
                        (From node As HtmlNode In gameEntryHtmlDoc.DocumentNode.SelectNodes(contentXpath)
                         Where node.InnerHtml.ToLower().Contains("<b>expansion for:</b> ")
                        ).FirstOrDefault()

                    Dim isExpansionOrDLC As Boolean = expansionNode IsNot Nothing
                    If isExpansionOrDLC Then
                        Continue For
                    End If

                    ' 3. Retrieve the genre:

                    Dim genreNode As HtmlNode =
                        (From node As HtmlNode In gameEntryHtmlDoc.DocumentNode.SelectNodes(contentXpath)
                         Where node?.InnerHtml.Trim.StartsWith("<b>Genre:</b> ", StringComparison.OrdinalIgnoreCase)
                        ).FirstOrDefault()

                    Dim genre As String =
                        genreNode?.InnerHtml.Replace("<b>Genre:</b> ", "").
                                             Replace($"<a href=""/{platform.BaseUrl.ToString().TrimEnd("/"c).Split("/"c).Last}", $"<a href=""{platform.BaseUrl}").
                                             Replace(""">", """ target=""_blank"" rel=""noopener noreferrer"">").Trim()

                    If String.IsNullOrWhiteSpace(genre) Then
                        genre = "N/A"
                    End If

                    ' This is Software / Application, not a game.
                    If genre.Contains(">Application<") Then
                        Continue For
                    End If

                    ' This is Hardware, not a game.
                    If genre.Contains(">Hardware<") Then
                        Continue For
                    End If

                    ' This is a Demonstration, not a full game.
                    If genre.Contains(">Demo Disc<") OrElse genre.Contains(">Demo<") Then
                        Continue For
                    End If

                    ' 4. Retrieve the release date:

                    Dim releaseDateNode As HtmlNode =
                        (From node As HtmlNode In gameEntryHtmlDoc.DocumentNode.SelectNodes(contentXpath)
                         Where node.InnerHtml.Trim.StartsWith("<b>Release:</b> ", StringComparison.OrdinalIgnoreCase)
                        ).FirstOrDefault()

                    Dim releaseDate As String =
                        releaseDateNode?.InnerHtml.Replace("<b>Release:</b> ", "").
                                                   Replace($"<a href=""/{platform.BaseUrl.ToString().TrimEnd("/"c).Split("/"c).Last}", $"<a href=""{platform.BaseUrl}").
                                                   Replace(""">", """ target=""_blank"" rel=""noopener noreferrer"">").Trim()

                    If String.IsNullOrEmpty(releaseDate) Then
                        releaseDate = "N/A"

                    Else
                        Dim dateString As String = releaseDateNode?.InnerText.Replace("Release: ", "")

                        If dateString.Equals("Canceled", StringComparison.OrdinalIgnoreCase) Then
                            ' This case it seems to occur only on multi-platform games
                            ' whose release was canceled on this platform.
                            Continue For

                        ElseIf dateString.StartsWith("Unknown", StringComparison.OrdinalIgnoreCase) Then
                            releaseDate = dateRegex.Replace(releaseDate, $""">N/A</a>")

                        ElseIf dateString.StartsWith("TBA", StringComparison.OrdinalIgnoreCase) Then
                            ' To Be Announced (TBA). Strings like "TBA" or "TBA 2022", etc.
                            ' This case it seems to occur when the release date is yet not provided by the publisher.
                            ' But the game is released.
                            releaseDate = dateRegex.Replace(releaseDate, $""">TBA</a>")

                        Else
                            Dim year As String = GamefaqsUtil.GetYearFromDateString(dateString)
                            releaseDate = dateRegex.Replace(releaseDate, $""">{year}</a>")

                        End If

                    End If

                    ' 5. Retrieve the game title:

                    Const pageTitleXpath As String = "//h1[@class='page-title']"

                    Dim pageTitleNode As HtmlNode = gameEntryHtmlDoc.DocumentNode.SelectSingleNode(pageTitleXpath)
                    Dim pageTitle As String = pageTitleNode?.InnerText
                    If String.IsNullOrWhiteSpace(pageTitle) Then
                        MiscUtil.PrintErrorAndExit($"Can't locate game title / page title element (XPath: ""{pageTitleNode}"") in html source-code of uri: {entryUrl}", exitcode:=ExitCodes.ExitCodeXPathNotFound)
                    End If

                    Dim gameInfo As New GameInfo() With {
                        .PlatformName = platform.Name,
                        .Title = pageTitle,
                        .EntryUrl = entryUrl,
                        .Genre = genre,
                        .ReleaseDate = releaseDate
                    }

                    ' Determine whether this game is a exclusive release for this platform.
                    If Not gameEntryHtmlsource.Contains("""also_name"">") Then
                        refExclusiveGamesList.Add(gameInfo)
                    Else
                        refMultiplatformGamesList.Add(gameInfo)
                    End If

                End If

            Next titleNode

            Console.WriteLine("")
            MiscUtil.SleepRandom(100, 600)
        Next pageIndex

    End Sub

    ''' <summary>
    ''' Scraps only the entry titles and their entry urls from the input games list url,
    ''' and returns a <see cref="List(Of GameInfo)"/> representing each scraped game.
    ''' <para></para>
    ''' Note: this function returns all entry titles (not the proper game title), including duplicates (different languages), exclusive and multi-plaform, demo discs, compilations, etc.
    ''' </summary>
    ''' 
    ''' <param name="description">
    ''' The description name (e.g. PlayStation Store PS3) for the games in the provided list uri.
    ''' </param>
    ''' 
    ''' <param name="platform">
    ''' The source <see cref="PlatformInfo"/> for which the games in the provided list uri belongs to.
    ''' </param>
    ''' 
    ''' <returns>
    ''' A <see cref="List(Of GameInfo)"/> representing each scraped game.
    ''' </returns>
    <DebuggerStepperBoundary>
    Friend Function ScrapOnlyEntryUrls(description As String, platform As PlatformInfo, uri As Uri) As List(Of GameInfo)

        Dim gamesList As New List(Of GameInfo)
        Dim lastPageNumber As Integer = GamefaqsUtil.ScrapLastPageNumber(uri)

        Dim currentTotalEntryCount As Integer
        Dim startTime As Date = Date.Now

        For pageIndex As Integer = 0 To lastPageNumber - 1

            Dim thisPageCurrentEntryCount As Integer = 0

            Dim pageUri As Uri =
                If(uri.ToString().Contains("?"c),
                   New Uri($"{uri}&page={pageIndex}"),
                   New Uri($"{uri}?page={pageIndex}"))

            Console.WriteLine($"Parsing '{description}' page {pageIndex + 1} of {lastPageNumber} ...")
            Console.WriteLine($"Url: '{pageUri} ...")
            Console.WriteLine("")

            Dim pageHtmlSource As String = Nothing
            MiscUtil.DownloadHtmlPageWithRetry(pageUri, pageHtmlSource)

            Dim htmlDoc As New HtmlDocument
            htmlDoc.LoadHtml(pageHtmlSource)

            Const titleXpath As String = "//td[@class='rtitle']"
            Dim titleNodes As HtmlNodeCollection = htmlDoc.DocumentNode.SelectNodes(titleXpath)
            If titleNodes Is Nothing Then
                If (pageIndex <> lastPageNumber - 1) Then
                    MiscUtil.PrintErrorAndExit($"Can't locate game title elements (XPath: ""{titleXpath}"") in html source-code of uri: {pageUri}", exitcode:=ExitCodes.ExitCodeXPathNotFound)
                Else
                    ' Sometimes the last page exists but it does not contain any entry,
                    ' for example: https://gamefaqs.gamespot.com/atari5200/category/999-all?page=1
                    ' so we just finish the work.
                    Exit For
                End If
            End If

            ' Iterate game entry urls.

            Dim thisPagelastEntryCount As Integer = titleNodes.Count

            For Each titleNode As HtmlNode In titleNodes
                Dim ETA As String =
                    GamefaqsUtil.CalculateETA(startTime, pageIndex, Interlocked.Increment(thisPageCurrentEntryCount),
                                              lastPageNumber, Interlocked.Increment(currentTotalEntryCount),
                                              If(pageIndex <> (lastPageNumber - 1), 100, thisPagelastEntryCount))

                Dim nodeInnerHtml As String = titleNode.InnerHtml
                Dim entryTitle As String = titleNode.InnerText.Trim()

                Console.WriteLine($"Scraping {description}... | ETA {ETA} | Page {pageIndex + 1}/{lastPageNumber} | Entry {currentTotalEntryCount} ({thisPageCurrentEntryCount}/{thisPagelastEntryCount}) | {entryTitle}")
                ' Console.WriteLine($"Url: {entryUrl}")
                ' Console.WriteLine("")

                If nodeInnerHtml.Contains("""cancel""") Then ' Cancelled game.
                    Continue For

                ElseIf nodeInnerHtml.Contains("""unrel""") Then ' Not yet released game.
                    Continue For

                Else ' Released game.
                    Dim entryBaseUrl As String = titleNode.SelectSingleNode("a").Attributes("href").Value
                    Dim entryUrl As New Uri($"https://gamefaqs.gamespot.com{entryBaseUrl}")

                    Dim gameInfo As New GameInfo() With {
                        .PlatformName = platform.Name,
                        .Title = entryTitle,
                        .EntryUrl = entryUrl,
                        .Genre = Nothing,
                        .ReleaseDate = Nothing
                    }

                    gamesList.Add(gameInfo)
                End If

            Next titleNode

            Console.WriteLine("")
            MiscUtil.SleepRandom(100, 600)
        Next pageIndex

        Return gamesList

    End Function

#End Region

#Region " Markdown Methods "

    ''' <summary>
    ''' Builds a markdown table from the provided <see cref="List(Of GameInfo)"/> object.
    ''' </summary>
    ''' 
    ''' <param name="headerTitle">
    ''' The table header title.
    ''' </param>
    ''' 
    ''' <param name="games">
    ''' The <see cref="List(Of GameInfo)"/> containing the games to add in the table.
    ''' </param>
    ''' 
    ''' <param name="extraHeaderContent">
    ''' Optional. A string to add below <paramref name="headerTitle"/> content. This value is empty by default.
    ''' </param>
    ''' 
    ''' <param name="footer">
    ''' Optional. A string to add as the footer of the table. This value is empty by default.
    ''' </param>
    ''' 
    ''' <returns>
    ''' The resulting table in Markdown format.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function BuildMarkdownTable(headerTitle As String, games As List(Of GameInfo),
                                       Optional extraHeaderContent As String = Nothing,
                                       Optional footer As String = Nothing) As String
        Dim sb As New StringBuilder()
        sb.AppendLine($"# {headerTitle}")
        If Not String.IsNullOrEmpty(extraHeaderContent) Then
            sb.AppendLine(extraHeaderContent)
            sb.AppendLine()
        End If
        sb.AppendLine("|Index|Title|Year|Genre|")
        sb.AppendLine("|:--:|--|--|--|")
        If Not String.IsNullOrEmpty(footer) Then
            sb.AppendLine()
            sb.AppendLine(footer)
        End If

        Dim entryCount As Integer = 0
        For Each game As GameInfo In games
            sb.AppendLine($"|{Interlocked.Increment(entryCount)}|<a href=""{game.EntryUrl}"" target=""_blank"" rel=""noopener noreferrer"">{game.Title.Replace("|"c, "\|").Replace("`"c, "\`").Replace(":)", "\:)").Replace(":(", "\:(")}</a>|{game.ReleaseDate.Replace("|"c, "\|").Replace("`"c, "\`")}|{game.Genre.Replace("|"c, "\|").Replace("`"c, "\`")}|")
        Next

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Creates a Markdown's MD file using the provided string values as its file content.
    ''' </summary>
    ''' 
    ''' <param name="dirName">
    ''' The output directory name.
    ''' </param>
    ''' 
    ''' <param name="fileName">
    ''' The output MD file name.
    ''' </param>
    ''' 
    ''' <param name="values">
    ''' The values to write in the MD file.
    ''' </param>
    <DebuggerStepperBoundary>
    Friend Sub CreateMarkdownFile(dirName As String, fileName As String, ParamArray values As String())

        Dim outputDir As New DirectoryInfo($"\\?\{My.Application.Info.DirectoryPath}\Output\Tables")
        If Not outputDir.Exists Then
            outputDir.Create()
        End If

        Dim fullMarkdown As String = String.Join(Environment.NewLine, (From value As String In values Where Not String.IsNullOrEmpty(value)))
        fullMarkdown &= Environment.NewLine & "# End of File"
        Dim outputFilePath As String = $"{outputDir.FullName}\{fileName}.md"
        outputFilePath = MiscUtil.TruncateLongFilePath(outputFilePath)

        Console.WriteLine($"Creating markdown file: {outputFilePath.Replace("\\?\", "")}...")
        Try
            File.WriteAllText(outputFilePath, fullMarkdown, Encoding.UTF8)

        Catch ex As Exception
            MiscUtil.PrintErrorAndExit(ex.Message & Environment.NewLine &
                                       $"Failed to create file: {outputFilePath.Replace("\\?\", "")}", exitcode:=ExitCodes.ExitCodeCreatefileError)
        End Try
        Console.WriteLine("Done.")
        Console.WriteLine("")

    End Sub

#End Region

#Region " URL (files) Methods "

    ''' <summary>
    ''' Creates an URL file in the current application directory (.\Output\") for each item in the provided <see cref="List(Of GameInfo)"/> object,
    ''' and finally compresses the directory containing all the URL files into a single ZIP file.
    ''' </summary>
    ''' 
    ''' <param name="platformName">
    ''' The name of the platform, used as the output base directory name.
    ''' </param>
    ''' 
    ''' <param name="subDirName">
    ''' The subdirectory name to append to output directory path.
    ''' </param>
    ''' 
    ''' <param name="games">
    ''' The <see cref="List(Of GameInfo)"/> containing the games from which to create URL files.
    ''' </param>
    <DebuggerStepperBoundary>
    Friend Sub CreateUrlFiles(platformName As String, subDirName As String, games As List(Of GameInfo))

        Dim outputUrlsDir As New DirectoryInfo($"\\?\{My.Application.Info.DirectoryPath}\Output\Urls\{subDirName}")
        If Not outputUrlsDir.Exists Then
            outputUrlsDir.Create()
        End If

        Console.WriteLine($"Creating URL files in: {outputUrlsDir.FullName.Replace("\\?\", "")}...")
        For Each game As GameInfo In games
            Dim sb As New StringBuilder()
            sb.AppendLine("[InternetShortcut]")
            sb.AppendLine($"URL={game.EntryUrl}")

            Dim fileName As String = MiscUtil.ConvertStringToWindowsFileName(game.Title)
            Dim outputFilePath As String = $"{outputUrlsDir.FullName}\{fileName}.url"
            outputFilePath = MiscUtil.TruncateLongFilePath(outputFilePath)

#If DEBUG Then
            ' Console.WriteLine($"Creating URL file: {outputFilePath.Replace("\\?\", "")}...")
#End If
            Try
                File.WriteAllText(outputFilePath, sb.ToString(), Encoding.UTF8)

            Catch ex As Exception
                MiscUtil.PrintErrorAndExit(ex.Message & Environment.NewLine &
                                           $"Failed to create file: {outputFilePath.Replace("\\?\", "")}", exitcode:=ExitCodes.ExitCodeCreatefileError)
            End Try
        Next game
        Console.WriteLine("Done.")
        Console.WriteLine("")

        Console.WriteLine($"Compressing directory: {outputUrlsDir.FullName.Replace("\\?\", "")}...")

        Dim outputZipsDir As New DirectoryInfo($"\\?\{My.Application.Info.DirectoryPath}\Output\Zips")
        If Not outputZipsDir.Exists Then
            outputZipsDir.Create()
        End If

        Dim outputZipFilePath As String = $"\\?\{My.Application.Info.DirectoryPath}\Output\Zips\{subDirName}.zip"
        If File.Exists(outputZipFilePath) Then
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(outputZipFilePath, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
        End If
        ZipFile.CreateFromDirectory(outputUrlsDir.FullName, outputZipFilePath, CompressionLevel.Optimal, includeBaseDirectory:=False, Encoding.UTF8)

        Console.WriteLine("Done.")
        Console.WriteLine("")

    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Calculates the ETA (Estimated Time of Arrival) for the <see cref="GamefaqsUtil.ScrapGames"/> method to finish.
    ''' </summary>
    ''' <param name="startTime">
    ''' The start time
    ''' .</param>
    ''' 
    ''' <param name="currentPageIndex">
    ''' Index of the current page.
    ''' </param>
    ''' 
    ''' <param name="currentEntryIndex">
    ''' Index of the current entry.
    ''' </param>
    ''' 
    ''' <param name="lastPageNumber">
    ''' The last page number.
    ''' </param>
    ''' 
    ''' <param name="currentEntryCount">
    ''' The current entry count.
    ''' </param>
    ''' 
    ''' <returns>
    ''' A string in format "HH:mm:ss" representing the ETA.
    ''' </returns>
    <DebuggerStepThrough>
    Private Function CalculateETA(startTime As Date, currentPageIndex As Integer, currentEntryIndex As Integer,
                                  lastPageNumber As Integer, currentEntryCount As Integer,
                                  lastPageEntryCount As Integer) As String

        Dim currentTime As Date = Date.Now
        Dim elapsedTime As TimeSpan = currentTime - startTime

        Dim completedEntries As Integer = (currentPageIndex * 100) + currentEntryIndex
        Dim remainingEntries As Integer =
            If(currentPageIndex <> (lastPageNumber - 1), (lastPageEntryCount * lastPageNumber) - currentEntryCount,
                                                         lastPageEntryCount - currentEntryIndex)

        Dim estimatedTimePerEntry As TimeSpan = TimeSpan.FromMilliseconds(elapsedTime.TotalMilliseconds / completedEntries)
        Dim estimatedTotalTime As TimeSpan = TimeSpan.FromMilliseconds(estimatedTimePerEntry.TotalMilliseconds * remainingEntries)

        Return estimatedTotalTime.ToString("d\:hh\:mm\:ss")
    End Function

    ''' <summary>
    ''' Parses and return the year value from the input release date string 
    ''' (e.g. input: "January 1, 1985", return: "1985").
    ''' </summary>
    ''' 
    ''' <param name="value">
    ''' The input date string (e.g. "January 1, 1985").
    ''' </param>
    ''' 
    ''' <returns>
    ''' The year value from the input release date string.
    ''' </returns>
    <DebuggerStepThrough>
    Public Function GetYearFromDateString(value As String) As String

        value = value.Replace("Q1", "").Replace("Q2", "").Replace("Q3", "").Replace("Q4", "").Trim()

        If value.Equals("TBA", StringComparison.OrdinalIgnoreCase) Then
            Return "TBA"
        End If

        ' Handles input strings like "1985".
        If Integer.TryParse(value, New Integer) Then
            Return value
        End If

        ' Handles input strings like "January 1, 1985" or "January 1985".
        Dim refDate As Date
        Return If(Date.TryParse(value, CultureInfo.GetCultureInfo("en-US").DateTimeFormat, DateTimeStyles.None, refDate),
                  CStr(refDate.Year), "N/A")

    End Function

#End Region

End Module

#End Region
