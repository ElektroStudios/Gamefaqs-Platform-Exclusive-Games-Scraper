#Region " Milton Bradley Microvision "

Namespace Platforms

    ''' <summary>
    ''' Milton Bradley Microvision platform.
    ''' </summary>
    Friend NotInheritable Class Microvision : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Milton Bradley Microvision", "microvision", PlatformType.HandheldConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
