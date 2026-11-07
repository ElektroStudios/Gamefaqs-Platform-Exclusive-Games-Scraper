#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions

#End Region

#Region " GameFAQsUtilTests "

''' <summary>
''' Unit tests for the offline (non-networking) members of <see cref="GameFAQsUtil"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class GameFAQsUtilTests

#Region " Constants "

    ''' <summary>
    ''' The Markdown table column header line.
    ''' </summary>
    Private Const TableColumnsLine As String = "|Index|Title|Year|Genre|"

    ''' <summary>
    ''' The Markdown table header separator line.
    ''' </summary>
    Private Const TableSeparatorLine As String = "|:--:|--|--|--|"

    ''' <summary>
    ''' The format of the strings returned by <see cref="GameFAQsUtil.CalculateETA"/>.
    ''' </summary>
    Private Const EtaFormat As String = "d\:hh\:mm\:ss"

#End Region

#Region " Fields Tests "

    ''' <summary>
    ''' Verifies that the text encoding used to write output files is UTF-8 with BOM.
    ''' </summary>
    <TestMethod>
    Public Sub TextEncoding_IsUtf8WithBom()
        Assert.IsInstanceOfType(Of UTF8Encoding)(GameFAQsUtil.TextEncoding)
        CollectionAssert.AreEqual(New Byte() {&HEF, &HBB, &HBF}, GameFAQsUtil.TextEncoding.GetPreamble())
    End Sub

    ''' <summary>
    ''' Verifies that the random sleep range is valid and the retry delay is positive.
    ''' </summary>
    <TestMethod>
    Public Sub DelaySettings_AreValid()
        Assert.IsGreaterThan(0, GameFAQsUtil.SleepRandomMinMilliseconds)
        Assert.IsGreaterThanOrEqualTo(GameFAQsUtil.SleepRandomMinMilliseconds, GameFAQsUtil.SleepRandomMaxMilliseconds)
        Assert.IsGreaterThan(0, GameFAQsUtil.RetryDelaySeconds)
    End Sub

    ''' <summary>
    ''' Verifies that the scraper user-agent is defined.
    ''' </summary>
    <TestMethod>
    Public Sub ScraperUserAgent_IsNotEmpty()
        Assert.IsFalse(String.IsNullOrWhiteSpace(GameFAQsUtil.ScraperUserAgent))
    End Sub

#End Region

#Region " GetYearFromDateString Tests "

    ''' <summary>
    ''' Verifies that the year is properly extracted from the date formats used by GameFAQs.
    ''' </summary>
    '''
    ''' <param name="input">
    ''' The input date string.
    ''' </param>
    '''
    ''' <param name="expected">
    ''' The expected year string.
    ''' </param>
    <TestMethod>
    <DataRow("January 1, 1985", "1985")>
    <DataRow("December 31, 1999", "1999")>
    <DataRow("Jan 1, 1985", "1985")>
    <DataRow("March 2004", "2004")>
    <DataRow("1985", "1985")>
    <DataRow("  1985  ", "1985")>
    <DataRow("Q1 2005", "2005")>
    <DataRow("Q2 2006", "2006")>
    <DataRow("Q3 2007", "2007")>
    <DataRow("Q4 2008", "2008")>
    Public Sub GetYearFromDateString_ValidDate_ReturnsYear(input As String, expected As String)
        Assert.AreEqual(expected, GameFAQsUtil.GetYearFromDateString(input))
    End Sub

    ''' <summary>
    ''' Verifies that "To Be Announced" values are returned as "TBA", case-insensitive.
    ''' </summary>
    '''
    ''' <param name="input">
    ''' The input date string.
    ''' </param>
    <TestMethod>
    <DataRow("TBA")>
    <DataRow("tba")>
    <DataRow(" TBA ")>
    <DataRow("Q1 TBA")>
    Public Sub GetYearFromDateString_Tba_ReturnsTba(input As String)
        Assert.AreEqual("TBA", GameFAQsUtil.GetYearFromDateString(input))
    End Sub

    ''' <summary>
    ''' Verifies that unparseable date strings return "N/A".
    ''' </summary>
    '''
    ''' <param name="input">
    ''' The input date string.
    ''' </param>
    <TestMethod>
    <DataRow("")>
    <DataRow("Unknown")>
    <DataRow("Canceled")>
    <DataRow("Not a date")>
    Public Sub GetYearFromDateString_InvalidDate_ReturnsNotAvailable(input As String)
        Assert.AreEqual("N/A", GameFAQsUtil.GetYearFromDateString(input))
    End Sub

    ''' <summary>
    ''' Verifies that the parsing of English date strings does not depend on the culture of the current thread.
    ''' </summary>
    '''
    ''' <param name="cultureName">
    ''' The culture to set as the current culture of the thread.
    ''' </param>
    <TestMethod>
    <DataRow("es-ES")>
    <DataRow("de-DE")>
    <DataRow("ja-JP")>
    <DataRow("en-US")>
    Public Sub GetYearFromDateString_IsCultureIndependent(cultureName As String)
        Dim result As String = Nothing
        TestHelper.RunWithCulture(cultureName, Sub() result = GameFAQsUtil.GetYearFromDateString("November 21, 1990"))
        Assert.AreEqual("1990", result)
    End Sub

#End Region

#Region " BuildMarkdownTable Tests "

    ''' <summary>
    ''' Verifies the table structure (title, column header and separator) when the game list is empty.
    ''' </summary>
    <TestMethod>
    Public Sub BuildMarkdownTable_EmptyList_ReturnsOnlyHeader()
        Dim markdown As String = GameFAQsUtil.BuildMarkdownTable("My Header", New List(Of GameInfo))
        Dim lines As List(Of String) = TestHelper.SplitLines(markdown.TrimEnd())

        CollectionAssert.AreEqual(New String() {"# My Header", TableColumnsLine, TableSeparatorLine}, lines)
    End Sub

    ''' <summary>
    ''' Verifies that each game is written as a numbered table row with hyperlink, year and genre.
    ''' </summary>
    <TestMethod>
    Public Sub BuildMarkdownTable_WritesNumberedRows()
        Dim games As New List(Of GameInfo) From {
            TestHelper.CreateGame("Super Mario Bros.", "/nes/563408-super-mario-bros", "1985", "Platformer"),
            TestHelper.CreateGame("The Legend of Zelda", "/nes/563433-the-legend-of-zelda", "1986", "Adventure")
        }

        Dim markdown As String = GameFAQsUtil.BuildMarkdownTable("NES", games)
        Dim lines As List(Of String) = TestHelper.SplitLines(markdown.TrimEnd())

        Assert.HasCount(5, lines)
        Assert.AreEqual("|1|<a href=""https://gamefaqs.gamespot.com/nes/563408-super-mario-bros"" target=""_blank"" rel=""nofollow noopener noreferrer"">Super Mario Bros.</a>|1985|Platformer|", lines(3))
        Assert.AreEqual("|2|<a href=""https://gamefaqs.gamespot.com/nes/563433-the-legend-of-zelda"" target=""_blank"" rel=""nofollow noopener noreferrer"">The Legend of Zelda</a>|1986|Adventure|", lines(4))
    End Sub

    ''' <summary>
    ''' Verifies that the rows are written in the same order as the input list.
    ''' </summary>
    <TestMethod>
    Public Sub BuildMarkdownTable_PreservesInputOrder()
        Dim titles As String() = {"Zeta", "Alpha", "Mu"}
        Dim games As List(Of GameInfo) = (From title As String In titles Select TestHelper.CreateGame(title)).ToList()

        Dim lines As List(Of String) = TestHelper.SplitLines(GameFAQsUtil.BuildMarkdownTable("Order", games).TrimEnd())

        For i As Integer = 0 To titles.Length - 1
            StringAssert.StartsWith(lines(i + 3), $"|{i + 1}|")
            StringAssert.Contains(lines(i + 3), $">{titles(i)}</a>")
        Next
    End Sub

    ''' <summary>
    ''' Verifies that Markdown special characters in the title, year and genre are escaped,
    ''' so they can't break the table layout.
    ''' </summary>
    <TestMethod>
    Public Sub BuildMarkdownTable_EscapesMarkdownCharacters()
        Dim game As GameInfo = TestHelper.CreateGame("A|B`C :) :(", "/nes/1-escape", "19|85`", "Act|ion`")

        Dim lines As List(Of String) = TestHelper.SplitLines(GameFAQsUtil.BuildMarkdownTable("Escape", New List(Of GameInfo) From {game}).TrimEnd())
        Dim row As String = lines(3)

        StringAssert.Contains(row, ">A\|B\`C \:) \:(</a>")
        StringAssert.EndsWith(row, "|19\|85\`|Act\|ion\`|")
    End Sub

    ''' <summary>
    ''' Verifies that the extra header content is written between the title and the table columns.
    ''' </summary>
    <TestMethod>
    Public Sub BuildMarkdownTable_ExtraHeaderContent_IsWrittenBeforeColumns()
        Dim markdown As String = GameFAQsUtil.BuildMarkdownTable("Header", New List(Of GameInfo) From {TestHelper.CreateGame("Game")},
                                                                 extraHeaderContent:="Some note.")
        Dim lines As List(Of String) = TestHelper.SplitLines(markdown.TrimEnd())

        Assert.AreEqual("# Header", lines(0))
        Assert.AreEqual("Some note.", lines(1))
        Assert.AreEqual(String.Empty, lines(2))
        Assert.AreEqual(TableColumnsLine, lines(3))
        Assert.AreEqual(TableSeparatorLine, lines(4))
        StringAssert.StartsWith(lines(5), "|1|")
    End Sub

    ''' <summary>
    ''' Verifies that the footer is written after the table rows,
    ''' and not between the table header and its rows (which would break the table).
    ''' </summary>
    <TestMethod>
    Public Sub BuildMarkdownTable_Footer_IsWrittenAfterRows()
        Dim games As New List(Of GameInfo) From {TestHelper.CreateGame("First"), TestHelper.CreateGame("Second")}

        Dim markdown As String = GameFAQsUtil.BuildMarkdownTable("Header", games, footer:="The footer.")
        Dim lines As List(Of String) = TestHelper.SplitLines(markdown.TrimEnd())

        Assert.AreEqual(TableSeparatorLine, lines(2))
        StringAssert.StartsWith(lines(3), "|1|")
        StringAssert.StartsWith(lines(4), "|2|")
        Assert.AreEqual(String.Empty, lines(5))
        Assert.AreEqual("The footer.", lines(6))
        Assert.HasCount(7, lines)
    End Sub

    ''' <summary>
    ''' Verifies that empty or null optional values are not written.
    ''' </summary>
    '''
    ''' <param name="optionalValue">
    ''' The value to pass as extra header content and footer.
    ''' </param>
    <TestMethod>
    <DataRow(Nothing)>
    <DataRow("")>
    Public Sub BuildMarkdownTable_EmptyOptionalValues_AreIgnored(optionalValue As String)
        Dim games As New List(Of GameInfo) From {TestHelper.CreateGame("Game")}

        Dim withOptional As String = GameFAQsUtil.BuildMarkdownTable("Header", games, optionalValue, optionalValue)
        Dim withoutOptional As String = GameFAQsUtil.BuildMarkdownTable("Header", games)

        Assert.AreEqual(withoutOptional, withOptional)
    End Sub

#End Region

#Region " CalculateETA Tests "

    ''' <summary>
    ''' Verifies that the ETA is zero when the last entry of the last page is being processed.
    ''' </summary>
    <TestMethod>
    Public Sub CalculateETA_LastEntryOfLastPage_ReturnsZero()
        Dim eta As String = GameFAQsUtil.CalculateETA(Date.Now.AddSeconds(-10), currentPageIndex:=0, currentEntryIndex:=40,
                                                      lastPageNumber:=1, currentEntryCount:=40, lastPageEntryCount:=40)
        Assert.AreEqual("0:00:00:00", eta)
    End Sub

    ''' <summary>
    ''' Verifies that the returned ETA string always has the "d:hh:mm:ss" format.
    ''' </summary>
    <TestMethod>
    Public Sub CalculateETA_ReturnsExpectedFormat()
        Dim eta As String = GameFAQsUtil.CalculateETA(Date.Now.AddMinutes(-3), currentPageIndex:=1, currentEntryIndex:=7,
                                                      lastPageNumber:=5, currentEntryCount:=107, lastPageEntryCount:=100)
        Assert.IsTrue(Regex.IsMatch(eta, "^\d+:\d{2}:\d{2}:\d{2}$"), $"Unexpected ETA format: {eta}")
    End Sub

    ''' <summary>
    ''' Verifies the ETA estimation when processing the last page.
    ''' <para></para>
    ''' 50 entries processed in 100 seconds (2 seconds per entry), with 50 entries remaining: ~100 seconds left.
    ''' </summary>
    <TestMethod>
    Public Sub CalculateETA_LastPage_EstimatesRemainingTime()
        Dim eta As String = GameFAQsUtil.CalculateETA(Date.Now.AddSeconds(-100), currentPageIndex:=0, currentEntryIndex:=50,
                                                      lastPageNumber:=1, currentEntryCount:=50, lastPageEntryCount:=100)

        Dim remaining As TimeSpan = TimeSpan.ParseExact(eta, EtaFormat, CultureInfo.InvariantCulture)
        Assert.IsTrue(remaining.TotalSeconds >= 99 AndAlso remaining.TotalSeconds <= 105, $"Unexpected ETA: {eta}")
    End Sub

    ''' <summary>
    ''' Verifies the ETA estimation when processing a page that is not the last page.
    ''' <para></para>
    ''' 30 entries processed in 60 seconds (2 seconds per entry),
    ''' with 3 pages of 100 entries (270 entries remaining): ~540 seconds left.
    ''' </summary>
    <TestMethod>
    Public Sub CalculateETA_MiddlePage_EstimatesRemainingTime()
        Dim eta As String = GameFAQsUtil.CalculateETA(Date.Now.AddSeconds(-60), currentPageIndex:=0, currentEntryIndex:=30,
                                                      lastPageNumber:=3, currentEntryCount:=30, lastPageEntryCount:=100)

        Dim remaining As TimeSpan = TimeSpan.ParseExact(eta, EtaFormat, CultureInfo.InvariantCulture)
        Assert.IsTrue(remaining.TotalSeconds >= 535 AndAlso remaining.TotalSeconds <= 560, $"Unexpected ETA: {eta}")
    End Sub

    ''' <summary>
    ''' Verifies that the ETA days component is used for very long estimations.
    ''' <para></para>
    ''' 1 entry processed in 1 hour, with 99 entries remaining: ~99 hours left (4 days and 3 hours).
    ''' </summary>
    <TestMethod>
    Public Sub CalculateETA_LongEstimation_IncludesDays()
        Dim eta As String = GameFAQsUtil.CalculateETA(Date.Now.AddHours(-1), currentPageIndex:=0, currentEntryIndex:=1,
                                                      lastPageNumber:=1, currentEntryCount:=1, lastPageEntryCount:=100)

        StringAssert.StartsWith(eta, "4:03:")
    End Sub

#End Region

End Class

#End Region
