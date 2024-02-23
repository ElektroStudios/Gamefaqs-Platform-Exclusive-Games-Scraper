#Region " Nintendo Virtual Boy "

Namespace Platforms

    ''' <summary>
    ''' Nintendo Virtual Boy platform.
    ''' </summary>
    Friend NotInheritable Class VirtualBoy : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Virtual Boy", "virtualboy", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
