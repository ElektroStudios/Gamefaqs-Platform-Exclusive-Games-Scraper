#Region " Odyssey^2 "

Namespace Platforms

    ''' <summary>
    ''' Odyssey^2 platform.
    ''' </summary>
    Friend NotInheritable Class Odyssey2 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Odyssey^2", "odyssey2", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
