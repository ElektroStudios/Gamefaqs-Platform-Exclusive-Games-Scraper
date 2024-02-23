﻿#Region " Sony PlayStation 2 "

Namespace Platforms

    ''' <summary>
    ''' Sony PlayStation 2 platform.
    ''' </summary>
    Friend NotInheritable Class PlayStation2 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("PlayStation 2", "ps2", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Expansions / DLC Distribution]({Me.PlatformInfo.BaseUrl}/category/999-all?dist=6)
    ||[Demo Discs]({Me.PlatformInfo.BaseUrl}/category/280-miscellaneous-demo-disc)
    ||[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
