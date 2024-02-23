#Region " Super A'Can "

Namespace Platforms

    ''' <summary>
    ''' Super A'Can platform.
    ''' </summary>
    Friend NotInheritable Class SuperACan : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Super A'Can", "superacan", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
