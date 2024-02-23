#Region " Mega Drive 32x / Sega 32x "

Namespace Platforms

    ''' <summary>
    ''' Mega Drive 32x / Sega 32x platform.
    ''' </summary>
    Friend NotInheritable Class MegaDrive32x : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Mega Drive 32x / Sega 32x", "sega32x", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
