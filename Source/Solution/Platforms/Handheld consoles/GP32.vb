#Region " Game Park GP32 "

Namespace Platforms

    ''' <summary>
    ''' Game Park GP32 platform.
    ''' </summary>
    Friend NotInheritable Class GP32 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Game Park GP32", "gp32", PlatformType.HandheldConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
