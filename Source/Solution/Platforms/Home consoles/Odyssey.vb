#Region " Odyssey "

Namespace Platforms

    ''' <summary>
    ''' Odyssey platform.
    ''' </summary>
    Friend NotInheritable Class Odyssey : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Odyssey", "odyssey", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
