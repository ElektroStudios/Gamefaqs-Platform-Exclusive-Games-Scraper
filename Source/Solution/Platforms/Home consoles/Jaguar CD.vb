#Region " Atari Jaguar CD "

Namespace Platforms

    ''' <summary>
    ''' Atari Jaguar CD platform.
    ''' </summary>
    Friend NotInheritable Class JaguarCD : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Jaguar CD", "jaguarcd", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
