#Region " Panasonic 3DO "

Namespace Platforms

    ''' <summary>
    ''' Arcadia 2001 platform.
    ''' </summary>
    Friend NotInheritable Class Arcadia2001 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Arcadia 2001", "a2k1", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
