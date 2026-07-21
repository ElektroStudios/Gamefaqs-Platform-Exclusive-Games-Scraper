#Region " Windows Mobile "

Imports System.Collections.Generic
Imports System.Linq

Namespace Platforms

    ''' <summary>
    ''' Windows Mobile platform.
    ''' </summary>
    Friend NotInheritable Class WindowsMobile : Inherits PlatformBaseWithOnlineStore

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Windows Mobile", "windows-mobile", PlatformType.Other)

        Protected Overrides ReadOnly Property MarketDistributionIds As New Dictionary(Of Integer, String) From {
            {2, "Windows Marketplace for Mobile"}
        }

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[{Me.MarketDistributionIds.ElementAt(0).Value} Games]({Me.PlatformInfo.BaseUrl}/category/999-all?dist={Me.MarketDistributionIds.ElementAt(0).Key})|
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|
"

#End Region

    End Class

End Namespace

#End Region
