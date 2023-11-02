Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports std = System.Math

Module DataFiles

    Friend contourLabelCount As Integer = 50000

    Friend Sub removeContourLabels(gnuplot_s As TextWriter)
        While contourLabelCount > 50000
            gnuplot_s.WriteLine("unset object " & contourLabelCount & ";unset label " & std.Max(Interlocked.Decrement(contourLabelCount), contourLabelCount + 1))
        End While
    End Sub

    Friend Sub setContourLabels(contourFile As String, gnuplot As TextWriter)
        Using file As New StreamReader(contourFile)
            Dim line As New Value(Of String)

            While (line = file.ReadLine()) IsNot Nothing
                If line.Value.Contains("label:") Then
                    Dim c As String() = file.ReadLine().Trim().Replace("   ", " ").Replace("  ", " ").Split(" "c)
                    gnuplot.WriteLine("set object " & Interlocked.Increment(contourLabelCount) & " rectangle center " & c(0) & "," & c(1) & " size char " & (c(2).ToString().Length + 1) & ",char 1 fs transparent solid .7 noborder fc rgb ""white""  front")
                    gnuplot.WriteLine("set label " & contourLabelCount & " """ & c(2) & """ at " & c(0) & "," & c(1) & " front center")
                End If
            End While
        End Using
    End Sub

    ''' <summary>
    ''' these makecontourFile functions should probably be merged into one function and use a StoredPlot parameter
    ''' </summary>
    ''' <param name="fileOrFunction"></param>
    ''' <param name="outputFile"></param>
    Public Sub makeContourFile(fileOrFunction As String, outputFile As String, gnuplot As TextWriter)
        'if it's a file, fileOrFunction needs quotes and escaped backslashes
        SaveSetState()
        [Set]("table " & ScriptGenerator.plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        gnuplot.WriteLine("splot " & fileOrFunction)
        Unset("table")
        gnuplot.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Public Sub makeContourFile(x As Double(), y As Double(), z As Double(), outputFile As String, gnuplot As TextWriter)
        SaveSetState()
        [Set]("table " & ScriptGenerator.plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        gnuplot.WriteLine("splot ""-""")
        gnuplot.WriteData(x, y, z)
        gnuplot.WriteLine("e")
        Unset("table")
        gnuplot.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Public Sub makeContourFile(zz As Double(,), outputFile As String, gnuplot As TextWriter)
        SaveSetState()
        [Set]("table " & ScriptGenerator.plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        gnuplot.WriteLine("splot ""-"" matrix")
        gnuplot.WriteData(zz)
        gnuplot.WriteLine("e")
        gnuplot.WriteLine("e")
        Unset("table")
        gnuplot.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Public Sub makeContourFile(sizeY As Integer, z As Double(), outputFile As String, gnuplot As TextWriter)
        SaveSetState()
        [Set]("table " & ScriptGenerator.plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        gnuplot.WriteLine("splot ""-""")
        gnuplot.WriteData(sizeY, z)
        gnuplot.WriteLine("e")
        Unset("table")
        gnuplot.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

End Module
