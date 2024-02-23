#Region " Atari 7800 "

Namespace Platforms

    ''' <summary>
    ''' Atari 7800 platform.
    ''' </summary>
    Friend NotInheritable Class Atari7800 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Atari 7800", "atari7800", PlatformType.PersonalComputer)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
