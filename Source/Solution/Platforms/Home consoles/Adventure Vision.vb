#Region " Entex Adventure Vision "

Namespace Platforms

    ''' <summary>
    ''' Entex Adventure Vision platform.
    ''' </summary>
    Friend NotInheritable Class AdventureVision : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Entex Adventure Vision", "avision", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
