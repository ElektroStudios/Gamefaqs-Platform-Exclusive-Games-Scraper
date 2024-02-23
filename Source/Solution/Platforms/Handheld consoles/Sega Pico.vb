#Region " Sega Pico "

Namespace Platforms

    ''' <summary>
    ''' Sega Pico platform.
    ''' </summary>
    Friend NotInheritable Class SegaPico : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Sega Pico", "pico", PlatformType.HandheldConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
