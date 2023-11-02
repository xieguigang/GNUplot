Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.DataSets
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
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

    <ExportAPI("heatmap")>
    Public Function heatmap(x As Object,
                            Optional file As String = Nothing,
                            Optional env As Environment = Nothing) As Object

        Dim z As Double()()

        If TypeOf x Is dataframe Then
            Dim df As dataframe = x
            Dim rows = df.forEachRow.ToArray

            z = rows _
                .Select(Function(r) CLRVector.asNumeric(r.value)) _
                .ToArray
        Else
            Return Message.InCompatibleType(GetType(dataframe), x.GetType, env)
        End If

        GNUplot.SetOutputFile(file)
        GNUplot.HeatMap(z.ToMatrix)

        If file.StringEmpty Then
            Return GNUplot.output.LoadImage
        Else
            Return file.FileExists
        End If
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

    <ExportAPI("pointCloud")>
    Public Function pointCloud(<RRawVectorArgument> x As Object,
                               <RRawVectorArgument> y As Object,
                               <RRawVectorArgument> z As Object,
                               Optional file As String = Nothing,
                               Optional env As Environment = Nothing) As Object

        Dim vx = CLRVector.asNumeric(x)
        Dim vy = CLRVector.asNumeric(y)
        Dim vz = CLRVector.asNumeric(z)
        Dim rx As DoubleRange = vx.Range(1.25)
        Dim ry As DoubleRange = vy.Range(1.25)
        Dim rz As DoubleRange = vz.Range(1.25)
        Dim ranges As String() = {
            $"xrange[{rx.Min}:{rx.Max}]",
            $"yrange[{ry.Min}:{ry.Max}]",
            $"zrange[{rz.Min}:{rz.Max}]"
        }

        ' set the range for the x,y,z axis and plot (using pointtype triangle and color blue)
        GNUplot.SetOutputFile(file)
        GNUplot.Set(ranges)
        GNUplot.SPlot(vx, vy, vz, "with points pointtype 8 lc rgb 'blue'")

        If file.StringEmpty Then
            Return GNUplot.output.LoadImage
        Else
            Return file.FileExists
        End If
    End Function

    <ExportAPI("surface")>
    Public Function surface(<RRawVectorArgument> x As Object,
                            <RRawVectorArgument> y As Object,
                            <RRawVectorArgument> z As Object,
                            Optional file As String = Nothing,
                            Optional env As Environment = Nothing) As Object

        Dim vx = CLRVector.asNumeric(x)
        Dim vy = CLRVector.asNumeric(y)
        Dim vz = CLRVector.asNumeric(z)
        Dim rx As DoubleRange = vx.Range(1.25)
        Dim ry As DoubleRange = vy.Range(1.25)
        Dim rz As DoubleRange = vz.Range(1.25)
        Dim ranges As String() = {
            $"xrange[{rx.Min}:{rx.Max}]",
            $"yrange[{ry.Min}:{ry.Max}]",
            $"zrange[{rz.Min}:{rz.Max}]"
        }

        ' fit the points to a surface grid of 40x40 with smoothing level 2
        GNUplot.Set("dgrid3d 40,40,2")
        GNUplot.SetOutputFile(file)
        ' set the range for the x,y,z axis and plot (using pm3d to map height to color)
        GNUplot.Set(ranges)
        GNUplot.SPlot(vx, vy, vz, "with pm3d")

        If file.StringEmpty Then
            Return GNUplot.output.LoadImage
        Else
            Return file.FileExists
        End If
    End Function
End Module
