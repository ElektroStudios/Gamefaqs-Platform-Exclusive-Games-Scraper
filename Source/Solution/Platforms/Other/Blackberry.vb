#Region " Blackberry "

Namespace Platforms

    ''' <summary>
    ''' Blackberry platform.
    ''' </summary>
    Friend NotInheritable Class Blackberry : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Blackberry", "blackberry", PlatformType.Other)

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
