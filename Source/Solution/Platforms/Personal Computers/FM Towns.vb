#Region " FM Towns "

Namespace Platforms

    ''' <summary>
    ''' FM Towns platform.
    ''' </summary>
    Friend NotInheritable Class FMTowns : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("FM Towns", "fmtowns", PlatformType.PersonalComputer)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Demo Discs]({Me.PlatformInfo.BaseUrl}/category/280-miscellaneous-demo-disc)
"

#End Region

    End Class

End Namespace

#End Region
