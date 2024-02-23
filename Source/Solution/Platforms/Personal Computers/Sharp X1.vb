#Region " Sharp X1 "

Namespace Platforms

    ''' <summary>
    ''' Sharp X1 platform.
    ''' </summary>
    Friend NotInheritable Class SharpX1 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Sharp X1", "x1", PlatformType.PersonalComputer)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
