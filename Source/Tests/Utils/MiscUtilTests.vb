#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.Linq

#End Region

#Region " MiscUtilTests "

''' <summary>
''' Unit tests for the offline (non-networking) members of <see cref="MiscUtil"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class MiscUtilTests

#Region " Constants "

    ''' <summary>
    ''' The characters that are not allowed in Windows file names (excluding control characters).
    ''' </summary>
    Private Const WindowsInvalidFileNameChars As String = "<>:""/\|?*"

    ''' <summary>
    ''' The right double quotation mark character (U+201D).
    ''' <para></para>
    ''' Note: this character can't be typed inside a VB.NET string literal,
    ''' because the compiler treats it as a string delimiter (like the regular double quotation mark).
    ''' </summary>
    Private Shared ReadOnly RightDoubleQuotationMark As String = ChrW(&H201D)

#End Region

#Region " ConvertStringToWindowsFileName Tests "

    ''' <summary>
    ''' Verifies that each invalid file name character is replaced by its look-alike Unicode character.
    ''' </summary>
    '''
    ''' <param name="input">
    ''' The input string.
    ''' </param>
    '''
    ''' <param name="expected">
    ''' The expected result.
    ''' </param>
    <TestMethod>
    <DataRow("<", "˂")>
    <DataRow(">", "˃")>
    <DataRow("\", "⧹")>
    <DataRow("/", "⧸")>
    <DataRow("|", "ǀ")>
    <DataRow(":", "∶")>
    <DataRow("?", "ʔ")>
    <DataRow("*", "✲")>
    Public Sub ConvertStringToWindowsFileName_ReplacesInvalidCharacter(input As String, expected As String)
        Assert.AreEqual(expected, MiscUtil.ConvertStringToWindowsFileName(input))
    End Sub

    ''' <summary>
    ''' Verifies that a game title containing every invalid character results in a valid Windows file name.
    ''' </summary>
    <TestMethod>
    Public Sub ConvertStringToWindowsFileName_ResultContainsNoInvalidCharacters()
        Dim result As String = MiscUtil.ConvertStringToWindowsFileName("Title: ""Sub/Title"" <Part\1> | What? *Deluxe*")

        For Each c As Char In WindowsInvalidFileNameChars
            Assert.IsFalse(result.Contains(c), $"Result contains invalid character '{c}': {result}")
        Next
        Assert.AreEqual($"Title∶ {RightDoubleQuotationMark}Sub⧸Title{RightDoubleQuotationMark} ˂Part⧹1˃ ǀ Whatʔ ✲Deluxe✲", result)
    End Sub

    ''' <summary>
    ''' Verifies that the double quotation mark character is replaced by the right double quotation mark (U+201D).
    ''' </summary>
    <TestMethod>
    Public Sub ConvertStringToWindowsFileName_ReplacesDoubleQuotationMark()
        Assert.AreEqual(RightDoubleQuotationMark, MiscUtil.ConvertStringToWindowsFileName(ControlChars.Quote))
    End Sub

    ''' <summary>
    ''' Verifies that strings without invalid characters (including non-ASCII ones) are returned unmodified.
    ''' </summary>
    '''
    ''' <param name="input">
    ''' The input string.
    ''' </param>
    <TestMethod>
    <DataRow("")>
    <DataRow("Super Mario Bros. 3")>
    <DataRow("Neo•Geo Pocket Color")>
    <DataRow("ドラゴンクエスト")>
    <DataRow("Pokémon (Red & Blue) [Rev 1] {Beta} #1 - 100% 'fun'")>
    Public Sub ConvertStringToWindowsFileName_ValidString_IsUnchanged(input As String)
        Assert.AreEqual(input, MiscUtil.ConvertStringToWindowsFileName(input))
    End Sub

    ''' <summary>
    ''' Verifies that converting an already converted string produces the same result.
    ''' </summary>
    <TestMethod>
    Public Sub ConvertStringToWindowsFileName_IsIdempotent()
        Dim once As String = MiscUtil.ConvertStringToWindowsFileName("A:B/C\D|E?F*G<H>I""J")
        Dim twice As String = MiscUtil.ConvertStringToWindowsFileName(once)
        Assert.AreEqual(once, twice)
    End Sub

#End Region

#Region " TruncateLongFilePath Tests "

    ''' <summary>
    ''' Verifies that a null or empty file path throws <see cref="ArgumentNullException"/>.
    ''' </summary>
    '''
    ''' <param name="filePath">
    ''' The input file path.
    ''' </param>
    <TestMethod>
    <DataRow(Nothing)>
    <DataRow("")>
    Public Sub TruncateLongFilePath_NullOrEmptyPath_Throws(filePath As String)
        Assert.ThrowsExactly(Of ArgumentNullException)(Function() MiscUtil.TruncateLongFilePath(filePath))
    End Sub

    ''' <summary>
    ''' Verifies that a maximum file name length of zero throws <see cref="ArgumentException"/>.
    ''' </summary>
    <TestMethod>
    Public Sub TruncateLongFilePath_ZeroMaxLength_Throws()
        Assert.ThrowsExactly(Of ArgumentException)(Function() MiscUtil.TruncateLongFilePath("game.url", maxFileNameLength:=0))
    End Sub

    ''' <summary>
    ''' Verifies that a null <see cref="FileInfo"/> throws <see cref="ArgumentNullException"/>.
    ''' </summary>
    <TestMethod>
    Public Sub TruncateLongFilePath_NullFileInfo_Throws()
        Assert.ThrowsExactly(Of ArgumentNullException)(
            Sub()
                Dim fileInfo As FileInfo = Nothing
                MiscUtil.TruncateLongFilePath(fileInfo)
            End Sub)
    End Sub

    ''' <summary>
    ''' Verifies that a short file path is returned as a full path, without truncation.
    ''' </summary>
    <TestMethod>
    Public Sub TruncateLongFilePath_ShortPath_IsNotTruncated()
        Dim filePath As String = Path.Combine(Path.GetTempPath(), "Super Mario Bros.url")
        Assert.AreEqual(Path.GetFullPath(filePath), MiscUtil.TruncateLongFilePath(filePath))
    End Sub

    ''' <summary>
    ''' Verifies that a file name whose length is exactly the maximum length is not truncated.
    ''' </summary>
    <TestMethod>
    Public Sub TruncateLongFilePath_NameLengthEqualToMax_IsNotTruncated()
        Dim fileName As String = $"{New String("a"c, 46)}.url" ' 50 chars.
        Dim filePath As String = Path.Combine(Path.GetTempPath(), fileName)

        Dim result As String = MiscUtil.TruncateLongFilePath(filePath, maxFileNameLength:=50)

        Assert.AreEqual(fileName, Path.GetFileName(result))
    End Sub

    ''' <summary>
    ''' Verifies that a file name exceeding the maximum length is truncated to the maximum length,
    ''' ending with an ellipsis and preserving the file extension.
    ''' </summary>
    <TestMethod>
    Public Sub TruncateLongFilePath_NameLongerThanMax_IsTruncatedWithEllipsis()
        Dim baseName As String = String.Concat(Enumerable.Repeat("0123456789", 6)) ' 60 chars.
        Dim filePath As String = Path.Combine(Path.GetTempPath(), $"{baseName}.url")

        Dim result As String = MiscUtil.TruncateLongFilePath(filePath, maxFileNameLength:=50)
        Dim resultName As String = Path.GetFileName(result)

        Assert.AreEqual(50, resultName.Length)
        Assert.AreEqual($"{baseName.Substring(0, 45)}….url", resultName)
        Assert.AreEqual(Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar),
                        Path.GetDirectoryName(result))
    End Sub

    ''' <summary>
    ''' Verifies that truncating a file name whose extension leaves no room for the name throws <see cref="ArgumentException"/>.
    ''' </summary>
    <TestMethod>
    Public Sub TruncateLongFilePath_ExtensionTooLong_Throws()
        Dim filePath As String = Path.Combine(Path.GetTempPath(), $"name.{New String("x"c, 60)}")
        Assert.ThrowsExactly(Of ArgumentException)(Function() MiscUtil.TruncateLongFilePath(filePath, maxFileNameLength:=50))
    End Sub

    ''' <summary>
    ''' Verifies that the <see cref="FileInfo"/> overload modifies the source object when truncation is needed.
    ''' </summary>
    <TestMethod>
    Public Sub TruncateLongFilePath_FileInfoOverload_UpdatesReference()
        Dim fileInfo As New FileInfo(Path.Combine(Path.GetTempPath(), $"{New String("b"c, 40)}.md"))
        MiscUtil.TruncateLongFilePath(fileInfo, maxFileNameLength:=20)

        Assert.AreEqual($"{New String("b"c, 16)}….md", fileInfo.Name)
    End Sub

    ''' <summary>
    ''' Verifies that the <see cref="FileInfo"/> overload keeps the same object when no truncation is needed.
    ''' </summary>
    <TestMethod>
    Public Sub TruncateLongFilePath_FileInfoOverload_ShortName_KeepsReference()
        Dim original As New FileInfo(Path.Combine(Path.GetTempPath(), "short.md"))
        Dim fileInfo As FileInfo = original
        MiscUtil.TruncateLongFilePath(fileInfo)

        Assert.AreSame(original, fileInfo)
    End Sub

    ''' <summary>
    ''' Verifies that a file name longer than 255 characters is truncated to 255 characters,
    ''' and that the resulting path uses the extended-length "\\?\" prefix when exceeding MAX_PATH.
    ''' </summary>
    <TestMethod>
    <TestCategory(TestHelper.WindowsOnlyCategory)>
    Public Sub TruncateLongFilePath_DefaultMax_TruncatesTo255AndUsesLongPathPrefix()
        Dim fileName As String = $"{New String("c"c, 300)}.url"
        Dim filePath As String = Path.Combine(Path.GetTempPath(), fileName)

        Dim result As String = MiscUtil.TruncateLongFilePath(filePath)

        StringAssert.StartsWith(result, "\\?\")
        Dim resultName As String = result.Substring(result.LastIndexOf("\"c) + 1)
        Assert.AreEqual(255, resultName.Length)
        StringAssert.EndsWith(resultName, "….url")
    End Sub

    ''' <summary>
    ''' Verifies that an input path with the extended-length "\\?\" prefix is accepted.
    ''' </summary>
    <TestMethod>
    <TestCategory(TestHelper.WindowsOnlyCategory)>
    Public Sub TruncateLongFilePath_PathWithLongPathPrefix_IsAccepted()
        Dim filePath As String = Path.Combine(Path.GetTempPath(), "game.url")

        Dim result As String = MiscUtil.TruncateLongFilePath($"\\?\{filePath}")

        Assert.AreEqual(Path.GetFullPath(filePath), result)
    End Sub

#End Region

#Region " IsCloudflareChallenge Tests "

    ''' <summary>
    ''' Verifies that known Cloudflare challenge pages are detected.
    ''' </summary>
    '''
    ''' <param name="html">
    ''' The input HTML content.
    ''' </param>
    <TestMethod>
    <DataRow("<title>Just a moment...</title><p>Performance &amp; security by Cloudflare</p>")>
    <DataRow("<title>Just a moment…</title><p>Cloudflare</p>")>
    <DataRow("<title>Un momento...</title><p>cloudflare</p>")>
    <DataRow("<title>Un momento…</title><p>CLOUDFLARE</p>")>
    <DataRow("<title>JUST A MOMENT...</title><p>Cloudflare</p>")>
    <DataRow("<script src=""https://challenges.cloudflare.com/turnstile/v0/api.js""></script>")>
    Public Sub IsCloudflareChallenge_ChallengePage_ReturnsTrue(html As String)
        Assert.IsTrue(MiscUtil.IsCloudflareChallenge(html))
    End Sub

    ''' <summary>
    ''' Verifies that regular pages, or pages that only partially match the patterns, are not detected as challenges.
    ''' </summary>
    '''
    ''' <param name="html">
    ''' The input HTML content.
    ''' </param>
    <TestMethod>
    <DataRow(Nothing)>
    <DataRow("")>
    <DataRow("<html><body><table><tr><td class=""rtitle""><a href=""/nes/1-game"">Game</a></td></tr></table></body></html>")>
    <DataRow("<title>Just a moment...</title>")>
    <DataRow("<p>This site is protected by Cloudflare</p>")>
    Public Sub IsCloudflareChallenge_RegularPage_ReturnsFalse(html As String)
        Assert.IsFalse(MiscUtil.IsCloudflareChallenge(html))
    End Sub

#End Region

#Region " SleepRandom Tests "

    ''' <summary>
    ''' Verifies that <see cref="MiscUtil.SleepRandom"/> waits at least the minimum amount of time.
    ''' </summary>
    <TestMethod>
    Public Sub SleepRandom_WaitsAtLeastMinimum()
        Dim sw As Stopwatch = Stopwatch.StartNew()
        MiscUtil.SleepRandom(50, 60)
        sw.Stop()

        ' A small tolerance is applied because of the system timer resolution.
        Assert.IsGreaterThanOrEqualTo(40L, sw.ElapsedMilliseconds)
    End Sub

#End Region

#Region " GetDefinedPlatforms Tests "

    ''' <summary>
    ''' Verifies that the defined platforms are returned sorted by name.
    ''' </summary>
    <TestMethod>
    Public Sub GetDefinedPlatforms_ReturnsPlatformsSortedByName()
        Dim names As List(Of String) = (From platform As IPlatform In MiscUtil.GetDefinedPlatforms()
                                        Select platform.PlatformInfo.Name).ToList()

        Assert.IsNotEmpty(names)
        CollectionAssert.AreEqual(names.OrderBy(Function(name As String) name, StringComparer.CurrentCulture).ToList(), names)
    End Sub

    ''' <summary>
    ''' Verifies that every call returns new platform instances.
    ''' </summary>
    <TestMethod>
    Public Sub GetDefinedPlatforms_ReturnsNewInstances()
        Dim first As IPlatform = MiscUtil.GetDefinedPlatforms().First()
        Dim second As IPlatform = MiscUtil.GetDefinedPlatforms().First()

        Assert.AreNotSame(first, second)
        Assert.AreEqual(first.GetType(), second.GetType())
    End Sub

    ''' <summary>
    ''' Verifies that filtering the platforms by <see cref="PlatformType"/> returns only platforms of that type.
    ''' </summary>
    '''
    ''' <param name="platformTypeValue">
    ''' The numeric value of the <see cref="PlatformType"/> to filter. 
    ''' (<see cref="PlatformType"/> is a Friend type, so it can't be used as a parameter type of a public test method.)
    ''' </param>
    <TestMethod>
    <DataRow(CInt(PlatformType.HomeConsole))>
    <DataRow(CInt(PlatformType.HandheldConsole))>
    <DataRow(CInt(PlatformType.PersonalComputer))>
    <DataRow(CInt(PlatformType.Other))>
    Public Sub GetDefinedPlatforms_ByType_ReturnsOnlyThatType(platformTypeValue As Integer)
        Dim platformType As PlatformType = CType(platformTypeValue, PlatformType)
        Dim platforms As List(Of IPlatform) = MiscUtil.GetDefinedPlatforms(platformType)

        Assert.IsNotEmpty(platforms, $"No platforms defined for type: {platformType}")
        For Each platform As IPlatform In platforms
            Assert.AreEqual(platformType, platform.PlatformInfo.PlatformType, platform.PlatformInfo.Name)
        Next
    End Sub

    ''' <summary>
    ''' Verifies that the platforms filtered by each <see cref="PlatformType"/> make up the full set of platforms.
    ''' </summary>
    <TestMethod>
    Public Sub GetDefinedPlatforms_ByType_PartitionsAllPlatforms()
        Dim allNames As List(Of String) = (From platform As IPlatform In MiscUtil.GetDefinedPlatforms()
                                           Select platform.PlatformInfo.Name
                                           Order By Name).ToList()

        Dim partitionedNames As New List(Of String)
        For Each platformType As PlatformType In [Enum].GetValues(GetType(PlatformType)).Cast(Of PlatformType)()
            partitionedNames.AddRange(From platform As IPlatform In MiscUtil.GetDefinedPlatforms(platformType)
                                      Select platform.PlatformInfo.Name)
        Next
        partitionedNames.Sort(StringComparer.CurrentCulture)

        CollectionAssert.AreEqual(allNames, partitionedNames)
    End Sub

#End Region

End Class

#End Region
