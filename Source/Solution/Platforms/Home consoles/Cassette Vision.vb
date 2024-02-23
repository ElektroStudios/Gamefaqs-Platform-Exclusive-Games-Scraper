#Region " Cassette Vision "

Namespace Platforms

    ''' <summary>
    ''' Cassette Vision platform.
    ''' </summary>
    Friend NotInheritable Class CassetteVision : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Cassette Vision", "ecv", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
