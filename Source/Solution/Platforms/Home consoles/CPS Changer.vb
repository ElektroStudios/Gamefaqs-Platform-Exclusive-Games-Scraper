#Region " Capcom CPS Changer "

Namespace Platforms

    ''' <summary>
    ''' Capcom CPS Changer platform.
    ''' </summary>
    Friend NotInheritable Class CPSChanger : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Capcom CPS Changer", "cps", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
