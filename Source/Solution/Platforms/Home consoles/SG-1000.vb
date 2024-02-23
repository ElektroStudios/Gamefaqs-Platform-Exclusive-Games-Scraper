#Region " Sega SG-1000 "

Namespace Platforms

    ''' <summary>
    ''' Sega SG-1000 platform.
    ''' </summary>
    Friend NotInheritable Class SG1000 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("SG-1000", "sg1000", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
