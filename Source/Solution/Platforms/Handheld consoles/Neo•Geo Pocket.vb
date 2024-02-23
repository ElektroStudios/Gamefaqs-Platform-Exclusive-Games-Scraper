#Region " Neo•Geo Pocket "

Namespace Platforms

    ''' <summary>
    ''' Neo•Geo Pocket platform.
    ''' </summary>
    Friend NotInheritable Class NeoGeoPocket : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Neo•Geo Pocket", "ngpocket", PlatformType.HandheldConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
