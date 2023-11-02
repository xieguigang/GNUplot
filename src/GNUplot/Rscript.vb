Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.DataSets
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("GNUplot")>
Public Module Rscript

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
    ''' <param name="file">
    ''' the output filename
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("scatter")>
    Public Function scatter(<RRawVectorArgument> x As Object,
                            <RRawVectorArgument> y As Object,
                            Optional file As String = Nothing,
                            Optional env As Environment = Nothing) As Object

        Dim vx As Double() = CLRVector.asNumeric(x)
        Dim vy As Double() = CLRVector.asNumeric(y)

        GNUplot.SetOutputFile(file)
        GNUplot.Plot(vx, vy)

        If file.StringEmpty Then
            Return GNUplot.output.LoadImage
        Else
            Return file.FileExists
        End If
    End Function

    <ExportAPI("splot")>
    Public Function splot(<RLazyExpression> f As Object,
                          Optional file As String = Nothing,
                          Optional env As Environment = Nothing) As Object

        Dim splot_str As String

        If TypeOf f Is Literal Then
            splot_str = DirectCast(f, Literal).ValueStr
        Else
            Return Message.InCompatibleType(GetType(String), f.GetType, env)
        End If

        GNUplot.SetOutputFile(file)
        GNUplot.SPlot(splot_str)

        If file.StringEmpty Then
            Return GNUplot.output.LoadImage
        Else
            Return file.FileExists
        End If
    End Function
End Module
