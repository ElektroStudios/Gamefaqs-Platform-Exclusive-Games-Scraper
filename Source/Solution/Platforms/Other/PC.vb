#Region " PC (MS-DOS and Windows) "

Namespace Platforms

    ''' <summary>
    ''' PC (MS-DOS and Windows) platform.
    ''' </summary>
    Friend NotInheritable Class PC : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("PC (MS-DOS and Windows)", "pc", PlatformType.Other)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
    ||[Demo Discs]({Me.PlatformInfo.BaseUrl}/category/280-miscellaneous-demo-disc)
"

#End Region

    End Class

End Namespace

#End Region
