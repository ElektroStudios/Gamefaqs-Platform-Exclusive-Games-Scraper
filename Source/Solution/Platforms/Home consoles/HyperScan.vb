﻿#Region " Mattel HyperScan "

Namespace Platforms

    ''' <summary>
    ''' Mattel HyperScan platform.
    ''' </summary>
    Friend NotInheritable Class HyperScan : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Mattel HyperScan", "hyperscan", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
