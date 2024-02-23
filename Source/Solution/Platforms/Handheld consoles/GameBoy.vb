﻿#Region " Nintendo Game Boy "

Namespace Platforms

    ''' <summary>
    ''' Nintendo Game Boy platform.
    ''' </summary>
    Friend NotInheritable Class GameBoy : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Game Boy", "gameboy", PlatformType.HandheldConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
