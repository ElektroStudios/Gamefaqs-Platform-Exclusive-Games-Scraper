#Region " Sinclair ZX81/Spectrum "

Namespace Platforms

    ''' <summary>
    ''' Sinclair ZX81/Spectrum platform.
    ''' </summary>
    Friend NotInheritable Class Spectrum : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Sinclair ZX81/Spectrum", "sinclair", PlatformType.PersonalComputer)

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
