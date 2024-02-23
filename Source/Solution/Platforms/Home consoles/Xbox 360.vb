#Region " Microsoft Xbox 360 "

Imports System.Collections.Generic
Imports System.Linq

Namespace Platforms

    ''' <summary>
    ''' Microsoft Xbox 360 platform.
    ''' </summary>
    Friend NotInheritable Class Xbox360 : Inherits PlatformBaseWithOnlineStore

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Xbox 360", "xbox360", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarketDistributionIds As New Dictionary(Of Integer, String) From {
            {4, "Xbox Live Arcade (XBLA)"},
            {5, "Xbox Indie"}
        }

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    ||[{Me.MarketDistributionIds.ElementAt(0).Value} Games]({Me.PlatformInfo.BaseUrl}/category/999-all?dist={Me.MarketDistributionIds.ElementAt(0).Key})|[Expansions / DLC Distribution]({Me.PlatformInfo.BaseUrl}/category/999-all?dist=6)
    ||[{Me.MarketDistributionIds.ElementAt(1).Value} Games]({Me.PlatformInfo.BaseUrl}/category/999-all?dist={Me.MarketDistributionIds.ElementAt(1).Key})|[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)
"

#End Region

    End Class

End Namespace

#End Region
