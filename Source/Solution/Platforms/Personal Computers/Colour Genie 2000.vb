#Region " EACA Colour Genie 2000 "

Namespace Platforms

    ''' <summary>
    ''' EACA Colour Genie 2000 platform.
    ''' </summary>
    Friend NotInheritable Class ColourGenie2000 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("EACA Colour Genie 2000", "cg2000", PlatformType.PersonalComputer)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
