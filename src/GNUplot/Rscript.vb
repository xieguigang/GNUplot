Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("gnuplot")>
Public Class Rscript

    ''' <summary>
    ''' config of the gnuplot executable file path
    ''' </summary>
    ''' <param name="gnuplot">the user configed custom gnuplot path location.</param>
    ''' <returns></returns>
    <ExportAPI("config")>
    Public Function config(gnuplot As String) As Boolean
        GNUplotEnvironment.Config = gnuplot
        Return True
    End Function

    ''' <summary>
    ''' Create x,y scatter plot
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("scatter")>
    Public Function scatter(<RRawVectorArgument> x As Object, <RRawVectorArgument> y As Object, Optional env As Environment = Nothing) As Object
        Dim vx As Double() = CLRVector.asNumeric(x)
        Dim vy As Double() = CLRVector.asNumeric(y)

        Call GNUplot.Plot(vx, vy)

    End Function
End Class
