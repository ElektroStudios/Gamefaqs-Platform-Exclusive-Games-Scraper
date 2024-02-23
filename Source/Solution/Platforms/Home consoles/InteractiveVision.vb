#Region " View-Master Interactive Vision "

Namespace Platforms

    ''' <summary>
    ''' View-Master Interactive Vision platform.
    ''' </summary>
    Friend NotInheritable Class InteractiveVision : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("View-Master Interactive Vision", "ivision", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
