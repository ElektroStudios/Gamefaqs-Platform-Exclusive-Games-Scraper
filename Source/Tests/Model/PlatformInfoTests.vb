#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Reflection

#End Region

#Region " PlatformInfoTests "

''' <summary>
''' Unit tests for <see cref="PlatformInfo"/> class.
''' </summary>
<TestClass>
Public NotInheritable Class PlatformInfoTests

    ''' <summary>
    ''' Verifies that the constructor assigns the specified values.
    ''' </summary>
    <TestMethod>
    Public Sub Constructor_AssignsValues()
        Dim info As New PlatformInfo("PlayStation 3", "ps3", PlatformType.HomeConsole)

        Assert.AreEqual("PlayStation 3", info.Name)
        Assert.AreEqual("ps3", info.HttpName)
        Assert.AreEqual(PlatformType.HomeConsole, info.PlatformType)
    End Sub

    ''' <summary>
    ''' Verifies that the base url is built from the GameFAQs host and the http name.
    ''' </summary>
    <TestMethod>
    Public Sub BaseUrl_IsBuiltFromHttpName()
        Dim info As New PlatformInfo("Nintendo Switch 2", "switch-2", PlatformType.HandheldConsole)

        Assert.AreEqual(New Uri("https://gamefaqs.gamespot.com/switch-2"), info.BaseUrl)
        Assert.AreEqual("https://gamefaqs.gamespot.com/switch-2", info.BaseUrl.AbsoluteUri)
    End Sub

    ''' <summary>
    ''' Verifies that the "all games" url points to the "999-all" category of the platform.
    ''' </summary>
    <TestMethod>
    Public Sub AllGamesUrl_PointsToAllCategory()
        Dim info As New PlatformInfo("Nintendo Entertainment System (NES)", "nes", PlatformType.HomeConsole)

        Assert.AreEqual("https://gamefaqs.gamespot.com/nes/category/999-all", info.AllGamesUrl.AbsoluteUri)
    End Sub

    ''' <summary>
    ''' Verifies that a null, empty or whitespace name throws <see cref="ArgumentNullException"/>.
    ''' </summary>
    '''
    ''' <param name="name">
    ''' The invalid name.
    ''' </param>
    <TestMethod>
    <DataRow(Nothing)>
    <DataRow("")>
    <DataRow("   ")>
    Public Sub Constructor_InvalidName_Throws(name As String)
        Dim ex As ArgumentNullException =
            Assert.ThrowsExactly(Of ArgumentNullException)(Function() New PlatformInfo(name, "nes", PlatformType.HomeConsole))
        Assert.AreEqual("name", ex.ParamName)
    End Sub

    ''' <summary>
    ''' Verifies that a null, empty or whitespace http name throws <see cref="ArgumentNullException"/>.
    ''' </summary>
    '''
    ''' <param name="httpName">
    ''' The invalid http name.
    ''' </param>
    <TestMethod>
    <DataRow(Nothing)>
    <DataRow("")>
    <DataRow("   ")>
    Public Sub Constructor_InvalidHttpName_Throws(httpName As String)
        Dim ex As ArgumentNullException =
            Assert.ThrowsExactly(Of ArgumentNullException)(Function() New PlatformInfo("NES", httpName, PlatformType.HomeConsole))
        Assert.AreEqual("httpName", ex.ParamName)
    End Sub

    ''' <summary>
    ''' Verifies that the class can't be instantiated through a public parameterless constructor.
    ''' </summary>
    <TestMethod>
    Public Sub ParameterlessConstructor_IsPrivate()
        Dim ctor As ConstructorInfo = GetType(PlatformInfo).GetConstructor(BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.Public,
                                                                          Nothing, Type.EmptyTypes, Nothing)
        Assert.IsNotNull(ctor)
        Assert.IsTrue(ctor.IsPrivate)
    End Sub

End Class

#End Region
