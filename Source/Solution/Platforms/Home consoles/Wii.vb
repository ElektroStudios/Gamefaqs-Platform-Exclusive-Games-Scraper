#Region " Nintendo Wii "

Imports System.Collections.Generic
Imports System.Linq

Namespace Platforms

    ''' <summary>
    ''' Nintendo Wii platform.
    ''' </summary>
    Friend NotInheritable Class Wii : Inherits PlatformBaseWithOnlineStore

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Nintendo Wii", "wii", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarketDistributionIds As New Dictionary(Of Integer, String) From {
            {10, "WiiWare"}
        }

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[{Me.MarketDistributionIds.ElementAt(0).Value} Games]({Me.PlatformInfo.BaseUrl}/category/999-all?dist={Me.MarketDistributionIds.ElementAt(0).Key})|[Demo Discs]({Me.PlatformInfo.BaseUrl}/category/280-miscellaneous-demo-disc)
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
