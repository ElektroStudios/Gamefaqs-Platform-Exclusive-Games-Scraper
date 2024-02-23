#Region " Amazon Fire TV "

Namespace Platforms

    ''' <summary>
    ''' Amazon Fire TV platform.
    ''' </summary>
    Friend NotInheritable Class AmazonFireTV : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Amazon Fire TV", "firetv", PlatformType.Other)

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
