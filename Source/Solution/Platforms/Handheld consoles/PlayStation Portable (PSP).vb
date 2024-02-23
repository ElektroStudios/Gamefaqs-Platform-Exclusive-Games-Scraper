#Region " PlayStation Portable (PSP) "

Imports System.Collections.Generic
Imports System.Linq

Namespace Platforms

    ''' <summary>
    ''' PlayStation Portable (PSP) platform.
    ''' </summary>
    Friend NotInheritable Class PlayStationPortable : Inherits PlatformBaseWithOnlineStore

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("PlayStation Portable (PSP)", "psp", PlatformType.HandheldConsole)

        Protected Overrides ReadOnly Property MarketDistributionIds As New Dictionary(Of Integer, String) From {
            {8, "PlayStation minis"},
            {15, "PlayStation Store PSP"}
        }

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[{Me.MarketDistributionIds.ElementAt(0).Value} Games]({Me.PlatformInfo.BaseUrl}/category/999-all?dist={Me.MarketDistributionIds.ElementAt(0).Key})|[Expansions / DLC Distribution]({Me.PlatformInfo.BaseUrl}/category/999-all?dist=6)
    |[{Me.MarketDistributionIds.ElementAt(1).Value} Games]({Me.PlatformInfo.BaseUrl}/category/999-all?dist={Me.MarketDistributionIds.ElementAt(1).Key})|[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Demo Discs]({Me.PlatformInfo.BaseUrl}/category/280-miscellaneous-demo-disc)
"

#End Region

    End Class

End Namespace

#End Region
