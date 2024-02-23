#Region " LaserActive "

Namespace Platforms

    ''' <summary>
    ''' OS/2 platform.
    ''' </summary>
    Friend NotInheritable Class LaserActive : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("LaserActive", "laser", PlatformType.Other)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
