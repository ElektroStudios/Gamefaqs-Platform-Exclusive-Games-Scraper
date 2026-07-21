
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Diagnostics

#End Region

#Region " FlareSolverr Util "

' ReSharper disable once CheckNamespace

Namespace DevCase.ThirdParty.Selenium

    <ImmutableObject(True)>
    Public NotInheritable Class UtilFlareSolverr

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="UtilFlareSolverr"/> class from being created.
        ''' </summary>
        <DebuggerNonUserCode>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        <DebuggerStepThrough>
        Public Shared Sub KillFlareSolverrAndChildBrowsers()

            Using taskkillProcess As New Process()
                taskkillProcess.StartInfo.FileName = "taskkill"
                taskkillProcess.StartInfo.Arguments = $"/F /T /IM flaresolverr.exe"
                taskkillProcess.StartInfo.UseShellExecute = False
                taskkillProcess.StartInfo.CreateNoWindow = True

                taskkillProcess.Start()
            End Using

        End Sub

#End Region

    End Class

End Namespace

#End Region
