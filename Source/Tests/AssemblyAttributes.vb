#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Assembly Attributes "

' Test classes are independent from each other, so they can run in parallel.
' Test methods within the same class run sequentially, because some of them
' temporarily modify thread-bound state (like the current culture).
<Assembly: Parallelize(Workers:=0, Scope:=ExecutionScope.ClassLevel)>

#End Region
