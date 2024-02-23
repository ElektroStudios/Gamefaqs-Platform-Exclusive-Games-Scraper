#Region " Atari's 8-bit family "

Namespace Platforms

    ''' <summary>
    ''' Atari's 8-bit family platform.
    ''' </summary>
    Friend NotInheritable Class Atari8bit : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Atari's 8-bit family", "atari8bit", PlatformType.PersonalComputer)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|
"

#End Region

    End Class

End Namespace

#End Region
