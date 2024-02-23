#Region " Intellivision Amico "

Namespace Platforms

    ''' <summary>
    ''' Intellivision Amico platform.
    ''' </summary>
    Friend NotInheritable Class Amico : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Intellivision Amico", "amico", PlatformType.HomeConsole)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
"

#End Region

    End Class

End Namespace

#End Region
