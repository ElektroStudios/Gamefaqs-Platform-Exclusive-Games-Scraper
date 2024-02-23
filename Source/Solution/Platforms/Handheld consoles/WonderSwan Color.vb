#Region " Bandai WonderSwan Color "

Namespace Platforms

    ''' <summary>
    ''' Bandai WonderSwan Color platform.
    ''' </summary>
    Friend NotInheritable Class WonderSwanColor : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("WonderSwan Color", "wsc", PlatformType.HandheldConsole)

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
