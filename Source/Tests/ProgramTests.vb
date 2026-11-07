#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " ProgramTests "

''' <summary>
''' Unit tests for <see cref="Program"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class ProgramTests

#Region " Constants "

    ''' <summary>
    ''' The amount of platforms used in the tests.
    ''' </summary>
    Private Const PlatformCount As Integer = 114

#End Region

#Region " TryParsePlatformId Tests "

    ''' <summary>
    ''' Verifies that valid platform id. numbers are accepted.
    ''' </summary>
    '''
    ''' <param name="input">
    ''' The input string.
    ''' </param>
    '''
    ''' <param name="expected">
    ''' The expected platform id.
    ''' </param>
    <TestMethod>
    <DataRow("1", 1)>
    <DataRow("01", 1)>
    <DataRow("57", 57)>
    <DataRow("114", 114)>
    <DataRow(" 7 ", 7)>
    <DataRow("+3", 3)>
    Public Sub TryParsePlatformId_ValidInput_ReturnsTrue(input As String, expected As Integer)
        Dim platformId As Integer
        Assert.IsTrue(Program.TryParsePlatformId(input, PlatformCount, platformId))
        Assert.AreEqual(expected, platformId)
    End Sub

    ''' <summary>
    ''' Verifies that invalid inputs are rejected, including zero, negative and out of range numbers.
    ''' </summary>
    '''
    ''' <param name="input">
    ''' The input string.
    ''' </param>
    <TestMethod>
    <DataRow(Nothing)>
    <DataRow("")>
    <DataRow("   ")>
    <DataRow("0")>
    <DataRow("-1")>
    <DataRow("-114")>
    <DataRow("115")>
    <DataRow("99999999999")>
    <DataRow("abc")>
    <DataRow("1.5")>
    <DataRow("1,000")>
    Public Sub TryParsePlatformId_InvalidInput_ReturnsFalse(input As String)
        Dim platformId As Integer = 42
        Assert.IsFalse(Program.TryParsePlatformId(input, PlatformCount, platformId))
        Assert.AreEqual(0, platformId)
    End Sub

    ''' <summary>
    ''' Verifies that the parsing does not depend on the culture of the current thread.
    ''' </summary>
    <TestMethod>
    Public Sub TryParsePlatformId_IsCultureIndependent()
        Dim result As Boolean
        Dim platformId As Integer
        TestHelper.RunWithCulture("ar-SA", Sub() result = Program.TryParsePlatformId("12", PlatformCount, platformId))

        Assert.IsTrue(result)
        Assert.AreEqual(12, platformId)
    End Sub

    ''' <summary>
    ''' Verifies that a platform count less than 1 throws <see cref="ArgumentOutOfRangeException"/>.
    ''' </summary>
    '''
    ''' <param name="platformCount">
    ''' The invalid platform count.
    ''' </param>
    <TestMethod>
    <DataRow(0)>
    <DataRow(-1)>
    Public Sub TryParsePlatformId_InvalidPlatformCount_Throws(platformCount As Integer)
        Dim platformId As Integer
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() Program.TryParsePlatformId("1", platformCount, platformId))
    End Sub

#End Region

End Class

#End Region
