#Region " Neo•Geo Pocket Color "

Namespace Platforms

    ''' <summary>
    ''' Neo•Geo Pocket Color platform.
    ''' </summary>
    Friend NotInheritable Class NeoGeoPocketColor : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Neo•Geo Pocket Color", "ngpc", PlatformType.HandheldConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
