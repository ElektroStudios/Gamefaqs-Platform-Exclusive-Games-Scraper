#Region " Oculus Quest / Meta Quest "

Namespace Platforms

    ''' <summary>
    ''' Oculus Quest / Meta Quest platform.
    ''' </summary>
    Friend NotInheritable Class OculusQuest : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Oculus Quest / Meta Quest", "meta-quest", PlatformType.Other)

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not Yet Released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
