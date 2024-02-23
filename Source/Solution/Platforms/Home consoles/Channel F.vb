#Region " Channel F "

Namespace Platforms

    ''' <summary>
    ''' Channel F platform.
    ''' </summary>
    Friend NotInheritable Class ChannelF : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Channel F", "channelf", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
