#Region " SNK Neo•Geo AES "

Namespace Platforms

    ''' <summary>
    ''' SNK Neo•Geo AES platform.
    ''' </summary>
    Friend NotInheritable Class NeoGeoAes : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Neo•Geo", "neo", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|
"

#End Region

    End Class

End Namespace

#End Region
