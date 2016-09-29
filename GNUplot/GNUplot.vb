Imports System.Diagnostics
Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

''' <summary>
''' Gnuplot is a portable command-line driven graphing utility for Linux, OS/2, MS Windows, OSX, VMS, and many other platforms. 
''' The source code is copyrighted but freely distributed (i.e., you don't have to pay for it). It was originally created to 
''' allow scientists and students to visualize mathematical functions and data interactively, but has grown to support many 
''' non-interactive uses such as web scripting. It is also used as a plotting engine by third-party applications like Octave. 
''' Gnuplot has been supported and under active development since 1986.
''' </summary>
Public Class GnuPlot

    Public Shared PathToGnuplot As String = "C:\Program Files (x86)\gnuplot\bin"
    Private Shared ExtPro As Process
    Private Shared GnupStWr As StreamWriter
    Private Shared PlotBuffer As List(Of StoredPlot)
    Private Shared SPlotBuffer As List(Of StoredPlot)
    Private Shared ReplotWithSplot As Boolean

    Public Shared Property Hold As Boolean

    Shared Sub New()
        If PathToGnuplot(PathToGnuplot.Length - 1).ToString() <> "\" Then
            PathToGnuplot += "\"
        End If
        ExtPro = New Process()
        ExtPro.StartInfo.FileName = PathToGnuplot & "gnuplot.exe"
        ExtPro.StartInfo.UseShellExecute = False
        ExtPro.StartInfo.RedirectStandardInput = True
        ExtPro.Start()
        GnupStWr = ExtPro.StandardInput
        PlotBuffer = New List(Of StoredPlot)()
        SPlotBuffer = New List(Of StoredPlot)()
        Hold = False
    End Sub

    Public Shared Sub WriteLine(gnuplotcommands As String)

        GnupStWr.WriteLine(gnuplotcommands)
        GnupStWr.Flush()
    End Sub

    Public Shared Sub Write(gnuplotcommands As String)
        GnupStWr.Write(gnuplotcommands)
        GnupStWr.Flush()
    End Sub

    Public Shared Sub [Set](ParamArray options As String())
        For i As Integer = 0 To options.Length - 1
            GnupStWr.WriteLine("set " & options(i))
        Next

    End Sub

    Public Shared Sub Unset(ParamArray options As String())
        For i As Integer = 0 To options.Length - 1
            GnupStWr.WriteLine("unset " & options(i))
        Next
    End Sub

    Public Shared Function SaveData(Y As Double(), filename As String) As Boolean
        Dim dataStream As New StreamWriter(filename, False)
        WriteData(Y, dataStream)
        dataStream.Close()

        Return True
    End Function

    Public Shared Function SaveData(X As Double(), Y As Double(), filename As String) As Boolean
        Dim dataStream As New StreamWriter(filename, False)
        WriteData(X, Y, dataStream)
        dataStream.Close()

        Return True
    End Function

    Public Shared Function SaveData(X As Double(), Y As Double(), Z As Double(), filename As String) As Boolean
        Dim dataStream As New StreamWriter(filename, False)
        WriteData(X, Y, Z, dataStream)
        dataStream.Close()

        Return True
    End Function

    Public Shared Function SaveData(sizeY As Integer, Z As Double(), filename As String) As Boolean
        Dim dataStream As New StreamWriter(filename, False)
        WriteData(sizeY, Z, dataStream)
        dataStream.Close()

        Return True
    End Function

    Public Shared Function SaveData(Z As Double(,), filename As String) As Boolean
        Dim dataStream As New StreamWriter(filename, False)
        WriteData(Z, dataStream)
        dataStream.Close()

        Return True
    End Function

    Public Shared Sub Replot()
        If ReplotWithSplot Then
            SPlot(SPlotBuffer)
        Else
            Plot(PlotBuffer)
        End If
    End Sub

    Public Shared Sub Plot(filenameOrFunction As String, Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(filenameOrFunction, options))
        Plot(PlotBuffer)
    End Sub
    Public Shared Sub Plot(y As Double(), Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(y, options))
        Plot(PlotBuffer)
    End Sub
    Public Shared Sub Plot(x As Double(), y As Double(), Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(x, y, options))
        Plot(PlotBuffer)
    End Sub

    Public Shared Sub Contour(filenameOrFunction As String, Optional options As String = "", Optional labelContours As Boolean = True)
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        Dim p = New StoredPlot(filenameOrFunction, options, PlotTypes.ContourFileOrFunction)
        p.LabelContours = labelContours
        PlotBuffer.Add(p)
        Plot(PlotBuffer)
    End Sub
    Public Shared Sub Contour(sizeY As Integer, z As Double(), Optional options As String = "", Optional labelContours As Boolean = True)
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        Dim p = New StoredPlot(sizeY, z, options, PlotTypes.ContourZ)
        p.LabelContours = labelContours
        PlotBuffer.Add(p)
        Plot(PlotBuffer)
    End Sub
    Public Shared Sub Contour(x As Double(), y As Double(), z As Double(), Optional options As String = "", Optional labelContours As Boolean = True)
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        Dim p = New StoredPlot(x, y, z, options, PlotTypes.ContourXYZ)
        p.LabelContours = labelContours
        PlotBuffer.Add(p)
        Plot(PlotBuffer)
    End Sub
    Public Shared Sub Contour(zz As Double(,), Optional options As String = "", Optional labelContours As Boolean = True)
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        Dim p = New StoredPlot(zz, options, PlotTypes.ContourZZ)
        p.LabelContours = labelContours
        PlotBuffer.Add(p)
        Plot(PlotBuffer)
    End Sub

    Public Shared Sub HeatMap(filenameOrFunction As String, Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(filenameOrFunction, options, PlotTypes.ColorMapFileOrFunction))
        Plot(PlotBuffer)
    End Sub
    Public Shared Sub HeatMap(sizeY As Integer, intensity As Double(), Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(sizeY, intensity, options, PlotTypes.ColorMapZ))
        Plot(PlotBuffer)
    End Sub
    Public Shared Sub HeatMap(x As Double(), y As Double(), intensity As Double(), Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(x, y, intensity, options, PlotTypes.ColorMapXYZ))
        Plot(PlotBuffer)
    End Sub
    Public Shared Sub HeatMap(intensityGrid As Double(,), Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(intensityGrid, options, PlotTypes.ColorMapZZ))
        Plot(PlotBuffer)
    End Sub

    Public Shared Sub SPlot(filenameOrFunction As String, Optional options As String = "")
        If Not Hold Then
            SPlotBuffer.Clear()
        End If
        SPlotBuffer.Add(New StoredPlot(filenameOrFunction, options, PlotTypes.SplotFileOrFunction))
        SPlot(SPlotBuffer)
    End Sub
    Public Shared Sub SPlot(sizeY As Integer, z As Double(), Optional options As String = "")
        If Not Hold Then
            SPlotBuffer.Clear()
        End If
        SPlotBuffer.Add(New StoredPlot(sizeY, z, options))
        SPlot(SPlotBuffer)
    End Sub

    Public Shared Sub SPlot(x As Double(), y As Double(), z As Double(), Optional options As String = "")
        If Not Hold Then
            SPlotBuffer.Clear()
        End If
        SPlotBuffer.Add(New StoredPlot(x, y, z, options))
        SPlot(SPlotBuffer)
    End Sub

    Public Shared Sub SPlot(zz As Double(,), Optional options As String = "")
        If Not Hold Then
            SPlotBuffer.Clear()
        End If
        SPlotBuffer.Add(New StoredPlot(zz, options))
        SPlot(SPlotBuffer)
    End Sub


    Public Shared Sub Plot(storedPlots As List(Of StoredPlot))
        ReplotWithSplot = False
        Dim plot__1 As String = "plot "
        Dim plotstring As String = ""
        Dim contfile As String
        Dim defcntopts As String
        removeContourLabels()
        For i As Integer = 0 To storedPlots.Count - 1
            Dim p = storedPlots(i)
            defcntopts = If((p.Options.Length > 0 AndAlso (p.Options.Contains(" w") OrElse p.Options(0) = "w"c)), " ", " with lines ")
            Select Case p.PlotType
                Case PlotTypes.PlotFileOrFunction
                    If p.File IsNot Nothing Then
                        plotstring += (plot__1 & plotPath(p.File) & " " & p.Options)
                    Else
                        plotstring += (plot__1 & p.[Function] & " " & p.Options)
                    End If

                Case PlotTypes.PlotXY, PlotTypes.PlotY
                    plotstring += (plot__1 & """-"" " & p.Options)

                Case PlotTypes.ContourFileOrFunction
                    contfile = Path.GetTempPath() & "_cntrtempdata" & i & ".dat"
                    makeContourFile((If(p.File IsNot Nothing, plotPath(p.File), p.[Function])), contfile)
                    If p.LabelContours Then
                        setContourLabels(contfile)
                    End If
                    plotstring += (plot__1 & plotPath(contfile) & defcntopts & p.Options)

                Case PlotTypes.ContourXYZ
                    contfile = Path.GetTempPath() & "_cntrtempdata" & i & ".dat"
                    makeContourFile(p.X, p.Y, p.Z, contfile)
                    If p.LabelContours Then
                        setContourLabels(contfile)
                    End If
                    plotstring += (plot__1 & plotPath(contfile) & defcntopts & p.Options)

                Case PlotTypes.ContourZZ
                    contfile = Path.GetTempPath() & "_cntrtempdata" & i & ".dat"
                    makeContourFile(p.ZZ, contfile)
                    If p.LabelContours Then
                        setContourLabels(contfile)
                    End If
                    plotstring += (plot__1 & plotPath(contfile) & defcntopts & p.Options)

                Case PlotTypes.ContourZ
                    contfile = Path.GetTempPath() & "_cntrtempdata" & i & ".dat"
                    makeContourFile(p.YSize, p.Z, contfile)
                    If p.LabelContours Then
                        setContourLabels(contfile)
                    End If
                    plotstring += (plot__1 & plotPath(contfile) & defcntopts & p.Options)



                Case PlotTypes.ColorMapFileOrFunction
                    If p.File IsNot Nothing Then
                        plotstring += (plot__1 & plotPath(p.File) & " with image " & p.Options)
                    Else
                        plotstring += (plot__1 & p.[Function] & " with image " & p.Options)
                    End If

                Case PlotTypes.ColorMapXYZ, PlotTypes.ColorMapZ
                    plotstring += (plot__1 & """-"" " & " with image " & p.Options)

                Case PlotTypes.ColorMapZZ
                    plotstring += (plot__1 & """-"" " & "matrix with image " & p.Options)

            End Select
            If i = 0 Then
                plot__1 = ", "
            End If
        Next
        GnupStWr.WriteLine(plotstring)

        For i As Integer = 0 To storedPlots.Count - 1
            Dim p = storedPlots(i)
            Select Case p.PlotType
                Case PlotTypes.PlotXY
                    WriteData(p.X, p.Y, GnupStWr, False)
                    GnupStWr.WriteLine("e")

                Case PlotTypes.PlotY
                    WriteData(p.Y, GnupStWr, False)
                    GnupStWr.WriteLine("e")

                Case PlotTypes.ColorMapXYZ
                    WriteData(p.X, p.Y, p.Z, GnupStWr, False)
                    GnupStWr.WriteLine("e")

                Case PlotTypes.ColorMapZ
                    WriteData(p.YSize, p.Z, GnupStWr, False)
                    GnupStWr.WriteLine("e")

                Case PlotTypes.ColorMapZZ
                    WriteData(p.ZZ, GnupStWr, False)
                    GnupStWr.WriteLine("e")
                    GnupStWr.WriteLine("e")


            End Select
        Next
        GnupStWr.Flush()
    End Sub

    Public Shared Sub SPlot(storedPlots As List(Of StoredPlot))
        ReplotWithSplot = True
        Dim splot__1 = "splot "
        Dim plotstring As String = ""
        Dim defopts As String = ""
        removeContourLabels()
        For i As Integer = 0 To storedPlots.Count - 1
            Dim p = storedPlots(i)
            defopts = If((p.Options.Length > 0 AndAlso (p.Options.Contains(" w") OrElse p.Options(0) = "w"c)), " ", " with lines ")
            Select Case p.PlotType
                Case PlotTypes.SplotFileOrFunction
                    If p.File IsNot Nothing Then
                        plotstring += (splot__1 & plotPath(p.File) & defopts & p.Options)
                    Else
                        plotstring += (splot__1 & p.[Function] & defopts & p.Options)
                    End If

                Case PlotTypes.SplotXYZ, PlotTypes.SplotZ
                    plotstring += (splot__1 & """-"" " & defopts & p.Options)

                Case PlotTypes.SplotZZ
                    plotstring += (splot__1 & """-"" matrix " & defopts & p.Options)

            End Select
            If i = 0 Then
                splot__1 = ", "
            End If
        Next
        GnupStWr.WriteLine(plotstring)

        For i As Integer = 0 To storedPlots.Count - 1
            Dim p = storedPlots(i)
            Select Case p.PlotType
                Case PlotTypes.SplotXYZ
                    WriteData(p.X, p.Y, p.Z, GnupStWr, False)
                    GnupStWr.WriteLine("e")

                Case PlotTypes.SplotZZ
                    WriteData(p.ZZ, GnupStWr, False)
                    GnupStWr.WriteLine("e")
                    GnupStWr.WriteLine("e")

                Case PlotTypes.SplotZ
                    WriteData(p.YSize, p.Z, GnupStWr, False)
                    GnupStWr.WriteLine("e")

            End Select
        Next
        GnupStWr.Flush()
    End Sub

    Public Shared Sub WriteData(y As Double(), stream As StreamWriter, Optional flush As Boolean = True)
        For i As Integer = 0 To y.Length - 1
            stream.WriteLine(y(i).ToString())
        Next

        If flush Then
            stream.Flush()
        End If
    End Sub

    Public Shared Sub WriteData(x As Double(), y As Double(), stream As StreamWriter, Optional flush As Boolean = True)
        For i As Integer = 0 To y.Length - 1
            stream.WriteLine(x(i).ToString() & " " & y(i).ToString())
        Next

        If flush Then
            stream.Flush()
        End If
    End Sub

    Public Shared Sub WriteData(ySize As Integer, z As Double(), stream As StreamWriter, Optional flush As Boolean = True)
        For i As Integer = 0 To z.Length - 1
            If i > 0 AndAlso i Mod ySize = 0 Then
                stream.WriteLine()
            End If
            stream.WriteLine(z(i).ToString())
        Next

        If flush Then
            stream.Flush()
        End If
    End Sub

    Public Shared Sub WriteData(zz As Double(,), stream As StreamWriter, Optional flush As Boolean = True)
        Dim m As Integer = zz.GetLength(0)
        Dim n As Integer = zz.GetLength(1)
        Dim line As String
        For i As Integer = 0 To m - 1
            line = ""
            For j As Integer = 0 To n - 1
                line += zz(i, j).ToString() & " "
            Next
            stream.WriteLine(line.TrimEnd())
        Next

        If flush Then
            stream.Flush()
        End If
    End Sub

    Public Shared Sub WriteData(x As Double(), y As Double(), z As Double(), stream As StreamWriter, Optional flush As Boolean = True)
        Dim m As Integer = Math.Min(x.Length, y.Length)
        m = Math.Min(m, z.Length)
        For i As Integer = 0 To m - 1
            If i > 0 AndAlso x(i) <> x(i - 1) Then
                stream.WriteLine("")
            End If
            stream.WriteLine(x(i) & " " & y(i) & " " & z(i))
        Next

        If flush Then
            stream.Flush()
        End If
    End Sub

    Private Shared Function plotPath(path As String) As String
        Return """" & path.Replace("\", "\\") & """"
    End Function

    Public Shared Sub SaveSetState(Optional filename As String = Nothing)
        If filename Is Nothing Then
            filename = Path.GetTempPath() & "setstate.tmp"
        End If
        GnupStWr.WriteLine("save set " & plotPath(filename))
        GnupStWr.Flush()
        waitForFile(filename)
    End Sub
    Public Shared Sub LoadSetState(Optional filename As String = Nothing)
        If filename Is Nothing Then
            filename = Path.GetTempPath() & "setstate.tmp"
        End If
        GnupStWr.WriteLine("load " & plotPath(filename))
        GnupStWr.Flush()
    End Sub

    ''' <summary>
    ''' these makecontourFile functions should probably be merged into one function and use a StoredPlot parameter
    ''' </summary>
    ''' <param name="fileOrFunction"></param>
    ''' <param name="outputFile"></param>
    Private Shared Sub makeContourFile(fileOrFunction As String, outputFile As String)
        'if it's a file, fileOrFunction needs quotes and escaped backslashes
        SaveSetState()
        [Set]("table " & plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        GnupStWr.WriteLine("splot " & fileOrFunction)
        Unset("table")
        GnupStWr.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Private Shared Sub makeContourFile(x As Double(), y As Double(), z As Double(), outputFile As String)
        SaveSetState()
        [Set]("table " & plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        GnupStWr.WriteLine("splot ""-""")
        WriteData(x, y, z, GnupStWr)
        GnupStWr.WriteLine("e")
        Unset("table")
        GnupStWr.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Private Shared Sub makeContourFile(zz As Double(,), outputFile As String)
        SaveSetState()
        [Set]("table " & plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        GnupStWr.WriteLine("splot ""-"" matrix")
        WriteData(zz, GnupStWr)
        GnupStWr.WriteLine("e")
        GnupStWr.WriteLine("e")
        Unset("table")
        GnupStWr.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Private Shared Sub makeContourFile(sizeY As Integer, z As Double(), outputFile As String)
        SaveSetState()
        [Set]("table " & plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        GnupStWr.WriteLine("splot ""-""")
        WriteData(sizeY, z, GnupStWr)
        GnupStWr.WriteLine("e")
        Unset("table")
        GnupStWr.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Shared contourLabelCount As Integer = 50000
    Private Shared Sub setContourLabels(contourFile As String)
        Dim file As New System.IO.StreamReader(contourFile)
        Dim line As New Value(Of String)
        While (line = file.ReadLine()) IsNot Nothing
            If line.value.Contains("label:") Then
                Dim c As String() = file.ReadLine().Trim().Replace("   ", " ").Replace("  ", " ").Split(" "c)
                GnupStWr.WriteLine("set object " & Interlocked.Increment(contourLabelCount) & " rectangle center " & c(0) & "," & c(1) & " size char " & (c(2).ToString().Length + 1) & ",char 1 fs transparent solid .7 noborder fc rgb ""white""  front")
                GnupStWr.WriteLine("set label " & contourLabelCount & " """ & c(2) & """ at " & c(0) & "," & c(1) & " front center")
            End If
        End While
        file.Close()
    End Sub
    Private Shared Sub removeContourLabels()
        While contourLabelCount > 50000
            GnupStWr.WriteLine("unset object " & contourLabelCount & ";unset label " & Math.Max(Interlocked.Decrement(contourLabelCount), contourLabelCount + 1))
        End While
    End Sub

    Private Shared Function waitForFile(filename As String, Optional timeout As Integer = 10000) As Boolean
        Thread.Sleep(20)
        Dim attempts As Integer = timeout \ 100
        Dim file As System.IO.StreamReader = Nothing
        While file Is Nothing
            Try
                file = New System.IO.StreamReader(filename)
            Catch
                If System.Math.Max(System.Threading.Interlocked.Decrement(attempts), attempts + 1) > 0 Then
                    Thread.Sleep(100)
                Else
                    Return False
                End If
            End Try
        End While
        file.Close()
        Return True
    End Function

    Public Shared Sub HoldOn()
        Hold = True
        PlotBuffer.Clear()
        SPlotBuffer.Clear()
    End Sub

    Public Shared Sub HoldOff()
        Hold = False
        PlotBuffer.Clear()
        SPlotBuffer.Clear()
    End Sub

    Public Shared Sub Close()
        ExtPro.CloseMainWindow()
    End Sub
End Class