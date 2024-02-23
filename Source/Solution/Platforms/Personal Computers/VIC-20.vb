#Region " Commodore VIC-20 "

Namespace Platforms

    ''' <summary>
    ''' Commodore VIC-20 platform.
    ''' </summary>
    Friend NotInheritable Class VIC20 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Commodore VIC-20", "vic20", PlatformType.PersonalComputer)

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
