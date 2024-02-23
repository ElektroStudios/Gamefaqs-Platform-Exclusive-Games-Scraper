#Region " Action Max "

Namespace Platforms

    ''' <summary>
    ''' Action Max platform.
    ''' </summary>
    Friend NotInheritable Class ActionMax : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Action Max", "actionmax", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
