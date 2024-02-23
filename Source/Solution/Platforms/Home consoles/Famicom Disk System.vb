#Region " Nintendo Famicom Disk System "

Namespace Platforms

    ''' <summary>
    ''' Nintendo Famicom Disk System platform.
    ''' </summary>
    Friend NotInheritable Class FamicomDiskSystem : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Famicom Disk System", "famicomds", PlatformType.HomeConsole)

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
