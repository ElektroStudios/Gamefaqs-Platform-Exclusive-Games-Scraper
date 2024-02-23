#Region " SNK Neo•Geo CD "

Namespace Platforms

    ''' <summary>
    ''' SNK Neo•Geo CD platform.
    ''' </summary>
    Friend NotInheritable Class NeoGeoCD : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Neo•Geo CD", "neogeocd", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    ||[Demo Discs]({Me.PlatformInfo.BaseUrl}/category/280-miscellaneous-demo-disc)
"

#End Region

    End Class

End Namespace

#End Region
