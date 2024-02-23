#Region " Nintendo 64DD "

Namespace Platforms

    ''' <summary>
    ''' Nintendo 64DD platform.
    ''' </summary>
    Friend NotInheritable Class Nintendo64DD : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Nintendo 64DD", "n64dd", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    ||[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
