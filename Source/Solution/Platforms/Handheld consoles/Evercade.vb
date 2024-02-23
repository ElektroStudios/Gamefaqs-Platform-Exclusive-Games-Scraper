#Region " Evercade "

Namespace Platforms

    ''' <summary>
    ''' Evercade platform.
    ''' </summary>
    Friend NotInheritable Class Evercade : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Evercade", "evercade", PlatformType.HandheldConsole)

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
