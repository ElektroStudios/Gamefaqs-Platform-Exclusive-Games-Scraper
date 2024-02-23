#Region " Nintendo Switch "

Imports System.Collections.Generic
Imports System.Linq

Namespace Platforms

    ''' <summary>
    ''' Nintendo Switch platform.
    ''' </summary>
    Friend NotInheritable Class NintendoSwitch : Inherits PlatformBaseWithOnlineStore

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Nintendo Switch", "switch", PlatformType.HandheldConsole)

        Protected Overrides ReadOnly Property MarketDistributionIds As New Dictionary(Of Integer, String) From {
            {29, "Switch eShop"}
        }

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[{Me.MarketDistributionIds.ElementAt(0).Value} Games]({Me.PlatformInfo.BaseUrl}/category/999-all?dist={Me.MarketDistributionIds.ElementAt(0).Key})|[Expansions / DLC Distribution]({Me.PlatformInfo.BaseUrl}/category/999-all?dist=6)
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
