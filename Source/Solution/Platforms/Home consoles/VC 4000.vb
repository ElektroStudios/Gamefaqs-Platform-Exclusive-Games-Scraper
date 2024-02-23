#Region " Interton VC 4000 "

Namespace Platforms

    ''' <summary>
    ''' Interton VC 4000 platform.
    ''' </summary>
    Friend NotInheritable Class VC4000 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Interton VC 4000", "vc4000", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
