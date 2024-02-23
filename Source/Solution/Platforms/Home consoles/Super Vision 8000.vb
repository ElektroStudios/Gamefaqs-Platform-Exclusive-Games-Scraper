#Region " Super Vision 8000 "

Namespace Platforms

    ''' <summary>
    ''' Super Vision 8000 platform.
    ''' </summary>
    Friend NotInheritable Class SuperVision8000 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Super Vision 8000", "sv8000", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
