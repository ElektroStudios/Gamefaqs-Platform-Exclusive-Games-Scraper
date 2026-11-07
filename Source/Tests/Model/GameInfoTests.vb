#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " GameInfoTests "

''' <summary>
''' Unit tests for <see cref="GameInfo"/> class.
''' </summary>
<TestClass>
Public NotInheritable Class GameInfoTests

    ''' <summary>
    ''' Verifies that a new instance has all its properties uninitialized.
    ''' </summary>
    <TestMethod>
    Public Sub NewInstance_HasDefaultValues()
        Dim game As New GameInfo()

        Assert.IsNull(game.PlatformName)
        Assert.IsNull(game.Title)
        Assert.IsNull(game.EntryUrl)
        Assert.IsNull(game.ReleaseDate)
        Assert.IsNull(game.Genre)
    End Sub

    ''' <summary>
    ''' Verifies that the properties store the assigned values.
    ''' </summary>
    <TestMethod>
    Public Sub Properties_StoreAssignedValues()
        Dim url As New Uri("https://gamefaqs.gamespot.com/nes/563408-super-mario-bros")
        Dim game As New GameInfo() With {
            .PlatformName = "NES",
            .Title = "Super Mario Bros.",
            .EntryUrl = url,
            .ReleaseDate = "1985",
            .Genre = "Platformer"
        }

        Assert.AreEqual("NES", game.PlatformName)
        Assert.AreEqual("Super Mario Bros.", game.Title)
        Assert.AreSame(url, game.EntryUrl)
        Assert.AreEqual("1985", game.ReleaseDate)
        Assert.AreEqual("Platformer", game.Genre)
    End Sub

End Class

#End Region
