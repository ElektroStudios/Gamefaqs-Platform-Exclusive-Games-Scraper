#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection

#End Region

#Region " ExitCodesTests "

''' <summary>
''' Unit tests for <see cref="ExitCodes"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class ExitCodesTests

    ''' <summary>
    ''' Verifies that every error exit code is non-zero (zero means success) and unique.
    ''' </summary>
    <TestMethod>
    Public Sub ExitCodes_AreNonZeroAndUnique()
        Dim codes As List(Of Integer) =
            (From field As FieldInfo In GetType(ExitCodes).GetFields(BindingFlags.Static Or BindingFlags.NonPublic Or BindingFlags.Public)
             Where field.IsLiteral AndAlso field.FieldType Is GetType(Integer)
             Select CInt(field.GetRawConstantValue())).ToList()

        Assert.HasCount(4, codes)
        Assert.IsFalse(codes.Contains(0), "An error exit code can't be zero.")
        Assert.HasCount(codes.Count, codes.Distinct().ToList(), "Exit codes must be unique.")
    End Sub

End Class

#End Region
