#Region " NEC PC-Engine / TurboGrafx-16 "

Namespace Platforms

    ''' <summary>
    ''' NEC PC-Engine / TurboGrafx-16 platform.
    ''' </summary>
    Friend NotInheritable Class PcEngine : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("PC-Engine / TurboGrafx-16", "tg16", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    ||[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
