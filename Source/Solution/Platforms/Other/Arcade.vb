#Region " Arcade Cabinets "

Namespace Platforms

    ''' <summary>
    ''' Arcade Cabinets platform.
    ''' </summary>
    Friend NotInheritable Class Arcade : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Arcade Cabinets", "arcade", PlatformType.Other)

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
