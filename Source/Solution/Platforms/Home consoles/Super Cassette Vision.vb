#Region " Super Cassette Vision "

Namespace Platforms

    ''' <summary>
    ''' Super Cassette Vision platform.
    ''' </summary>
    Friend NotInheritable Class SuperCassetteVision : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Super Cassette Vision", "scv", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
