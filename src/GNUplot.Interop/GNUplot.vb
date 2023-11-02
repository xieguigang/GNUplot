#Region "Microsoft.VisualBasic::b3858db75c520c519e5d82a46c1aebcf, ..\GNUplot\GNUplot\GNUplot.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports std = System.Math

''' <summary>
''' Gnuplot is a portable command-line driven graphing utility for Linux, OS/2, MS Windows, OSX, VMS, and many other platforms. 
''' The source code is copyrighted but freely distributed (i.e., you don't have to pay for it). It was originally created to 
''' allow scientists and students to visualize mathematical functions and data interactively, but has grown to support many 
''' non-interactive uses such as web scripting. It is also used as a plotting engine by third-party applications like Octave. 
''' Gnuplot has been supported and under active development since 1986.
''' </summary>
Public Module GNUplot

    Private PlotBuffer As New List(Of StoredPlot)
    Private SPlotBuffer As New List(Of StoredPlot)
    Private ReplotWithSplot As Boolean

    Public Property Hold As Boolean = False

    ''' <summary>
    ''' gnuplot interop services instance.
    ''' </summary>
    Dim m_gnuplot As Interop

    ''' <summary>
    ''' the output image filepath
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' set output 'epslatex.tex'
    ''' </remarks>
    Public Property output As String

    Sub New()
        m_gnuplot = New Interop()

        If m_gnuplot.Start Then

        Else
            Call $"GNUplot is not avaliable in the default location: {m_gnuplot.PathToGnuplot.ToFileURL}, please manual setup the gnuplot.exe later.".Warning
        End If
    End Sub

    ''' <summary>
    ''' If you have change the default installed location of the gnuplot, then this 
    ''' function is required for manually starting the gnuplot services.
    ''' (假若从默认的位置启动程序没有成功的话，会需要使用这个函数从自定义位置启动程序)
    ''' </summary>
    ''' <param name="gnuplot">
    ''' The file path of the program file: ``gnuplot.exe``
    ''' </param>
    ''' <returns>The gnuplot services start successfully or not?</returns>
    Public Function Start(gnuplot$) As Boolean
        m_gnuplot = New Interop(gnuplot$)
        Return m_gnuplot.Start
    End Function

    Public Sub WriteLine(gnuplotcommands As String)
        m_gnuplot.WriteLine(gnuplotcommands)
        m_gnuplot.Flush()
    End Sub

    Public Sub Write(gnuplotcommands As String)
        m_gnuplot.Write(gnuplotcommands)
        m_gnuplot.Flush()
    End Sub

    Public Sub [Set](ParamArray options As String())
        For i As Integer = 0 To options.Length - 1
            m_gnuplot.WriteLine("set " & options(i))
        Next
    End Sub

    Public Sub Unset(ParamArray options As String())
        For i As Integer = 0 To options.Length - 1
            m_gnuplot.WriteLine("unset " & options(i))
        Next
    End Sub

    Public Function SaveData(Y As Double(), filename As String) As Boolean
        Using dataStream As New StreamWriter(filename, False)
            dataStream.WriteData(Y)
        End Using

        Return True
    End Function

    Public Function SaveData(X As Double(), Y As Double(), filename As String) As Boolean
        Using dataStream As New StreamWriter(filename, False)
            dataStream.WriteData(X, Y)
        End Using

        Return True
    End Function

    Public Function SaveData(X As Double(), Y As Double(), Z As Double(), filename As String) As Boolean
        Using dataStream As New StreamWriter(filename, False)
            dataStream.WriteData(X, Y, Z)
        End Using

        Return True
    End Function

    Public Function SaveData(sizeY As Integer, Z As Double(), filename As String) As Boolean
        Using dataStream As New StreamWriter(filename, False)
            dataStream.WriteData(sizeY, Z)
        End Using

        Return True
    End Function

    Public Function SaveData(Z As Double(,), filename As String) As Boolean
        Using dataStream As New StreamWriter(filename, False)
            dataStream.WriteData(Z)
        End Using

        Return True
    End Function

    Public Sub Replot()
        If ReplotWithSplot Then
            SPlot(SPlotBuffer)
        Else
            Plot(PlotBuffer)
        End If
    End Sub

    Public Sub Plot(filenameOrFunction As String, Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(filenameOrFunction, options))
        Plot(PlotBuffer)
    End Sub

    Public Sub Plot(y As Double(), Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(y, options))
        Plot(PlotBuffer)
    End Sub

    Public Sub Plot(x As Double(), y As Double(), Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(x, y, options))
        Plot(PlotBuffer)
    End Sub

    Public Sub Contour(filenameOrFunction As String, Optional options As String = "", Optional labelContours As Boolean = True)
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        Dim p = New StoredPlot(filenameOrFunction, options, PlotTypes.ContourFileOrFunction)
        p.LabelContours = labelContours
        PlotBuffer.Add(p)
        Plot(PlotBuffer)
    End Sub

    Public Sub Contour(sizeY As Integer, z As Double(), Optional options As String = "", Optional labelContours As Boolean = True)
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        Dim p = New StoredPlot(sizeY, z, options, PlotTypes.ContourZ)
        p.LabelContours = labelContours
        PlotBuffer.Add(p)
        Plot(PlotBuffer)
    End Sub

    Public Sub Contour(x As Double(), y As Double(), z As Double(), Optional options As String = "", Optional labelContours As Boolean = True)
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        Dim p = New StoredPlot(x, y, z, options, PlotTypes.ContourXYZ)
        p.LabelContours = labelContours
        PlotBuffer.Add(p)
        Plot(PlotBuffer)
    End Sub

    Public Sub Contour(zz As Double(,), Optional options As String = "", Optional labelContours As Boolean = True)
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        Dim p = New StoredPlot(zz, options, PlotTypes.ContourZZ)
        p.LabelContours = labelContours
        PlotBuffer.Add(p)
        Plot(PlotBuffer)
    End Sub

    Public Sub HeatMap(filenameOrFunction As String, Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(filenameOrFunction, options, PlotTypes.ColorMapFileOrFunction))
        Plot(PlotBuffer)
    End Sub

    Public Sub HeatMap(sizeY As Integer, intensity As Double(), Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(sizeY, intensity, options, PlotTypes.ColorMapZ))
        Plot(PlotBuffer)
    End Sub

    Public Sub HeatMap(x As Double(), y As Double(), intensity As Double(), Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(x, y, intensity, options, PlotTypes.ColorMapXYZ))
        Plot(PlotBuffer)
    End Sub

    Public Sub HeatMap(intensityGrid As Double(,), Optional options As String = "")
        If Not Hold Then
            PlotBuffer.Clear()
        End If
        PlotBuffer.Add(New StoredPlot(intensityGrid, options, PlotTypes.ColorMapZZ))
        Plot(PlotBuffer)
    End Sub

    Public Sub SPlot(filenameOrFunction As String, Optional options As String = "")
        If Not Hold Then
            SPlotBuffer.Clear()
        End If
        SPlotBuffer.Add(New StoredPlot(filenameOrFunction, options, PlotTypes.SplotFileOrFunction))
        SPlot(SPlotBuffer)
    End Sub

    Public Sub SPlot(sizeY As Integer, z As Double(), Optional options As String = "")
        If Not Hold Then
            SPlotBuffer.Clear()
        End If
        SPlotBuffer.Add(New StoredPlot(sizeY, z, options))
        SPlot(SPlotBuffer)
    End Sub

    Public Sub SPlot(x As Double(), y As Double(), z As Double(), Optional options As String = "")
        If Not Hold Then
            SPlotBuffer.Clear()
        End If
        SPlotBuffer.Add(New StoredPlot(x, y, z, options))
        SPlot(SPlotBuffer)
    End Sub

    Public Sub SPlot(zz As Double(,), Optional options As String = "")
        If Not Hold Then
            SPlotBuffer.Clear()
        End If
        SPlotBuffer.Add(New StoredPlot(zz, options))
        SPlot(SPlotBuffer)
    End Sub

    Public Sub Plot(storedPlots As List(Of StoredPlot))
        ReplotWithSplot = False
        Dim plot__1 As String = "plot "
        Dim plotstring As String = ""
        Dim contfile As String
        Dim defcntopts As String

        If Not output.StringEmpty Then
            Call [Set]($"output '{output}'")
        End If

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

        Call m_gnuplot.WriteLine(plotstring)

        For i As Integer = 0 To storedPlots.Count - 1
            Dim p = storedPlots(i)

            Select Case p.PlotType
                Case PlotTypes.PlotXY
                    m_gnuplot.std_in.WriteData(p.X, p.Y, False)
                    m_gnuplot.WriteLine("e")

                Case PlotTypes.PlotY
                    m_gnuplot.std_in.WriteData(p.Y, False)
                    m_gnuplot.WriteLine("e")

                Case PlotTypes.ColorMapXYZ
                    m_gnuplot.std_in.WriteData(p.X, p.Y, p.Z, False)
                    m_gnuplot.WriteLine("e")

                Case PlotTypes.ColorMapZ
                    m_gnuplot.std_in.WriteData(p.YSize, p.Z, False)
                    m_gnuplot.WriteLine("e")

                Case PlotTypes.ColorMapZZ
                    m_gnuplot.std_in.WriteData(p.ZZ, False)
                    m_gnuplot.WriteLine("e")
                    m_gnuplot.WriteLine("e")
            End Select
        Next

        _output = Nothing
        m_gnuplot.Flush()
    End Sub

    Public Sub SPlot(storedPlots As List(Of StoredPlot))
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

        m_gnuplot.WriteLine(plotstring)

        For i As Integer = 0 To storedPlots.Count - 1
            Dim p = storedPlots(i)
            Select Case p.PlotType
                Case PlotTypes.SplotXYZ
                    m_gnuplot.std_in.WriteData(p.X, p.Y, p.Z, False)
                    m_gnuplot.WriteLine("e")

                Case PlotTypes.SplotZZ
                    m_gnuplot.std_in.WriteData(p.ZZ, False)
                    m_gnuplot.WriteLine("e")
                    m_gnuplot.WriteLine("e")

                Case PlotTypes.SplotZ
                    m_gnuplot.std_in.WriteData(p.YSize, p.Z, False)
                    m_gnuplot.WriteLine("e")

            End Select
        Next

        m_gnuplot.Flush()
    End Sub

    Private Function plotPath(path As String) As String
        Return """" & path.Replace("\", "\\") & """"
    End Function

    Public Sub SaveSetState(Optional filename As String = Nothing)
        If filename Is Nothing Then
            filename = Path.GetTempPath() & "setstate.tmp"
        End If
        m_gnuplot.WriteLine("save set " & plotPath(filename))
        m_gnuplot.Flush()
        waitForFile(filename)
    End Sub

    Public Sub LoadSetState(Optional filename As String = Nothing)
        If filename Is Nothing Then
            filename = Path.GetTempPath() & "setstate.tmp"
        End If
        m_gnuplot.WriteLine("load " & plotPath(filename))
        m_gnuplot.Flush()
    End Sub

    ''' <summary>
    ''' these makecontourFile functions should probably be merged into one function and use a StoredPlot parameter
    ''' </summary>
    ''' <param name="fileOrFunction"></param>
    ''' <param name="outputFile"></param>
    Private Sub makeContourFile(fileOrFunction As String, outputFile As String)
        'if it's a file, fileOrFunction needs quotes and escaped backslashes
        SaveSetState()
        [Set]("table " & plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        m_gnuplot.WriteLine("splot " & fileOrFunction)
        Unset("table")
        m_gnuplot.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Private Sub makeContourFile(x As Double(), y As Double(), z As Double(), outputFile As String)
        SaveSetState()
        [Set]("table " & plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        m_gnuplot.WriteLine("splot ""-""")
        m_gnuplot.std_in.WriteData(x, y, z)
        m_gnuplot.WriteLine("e")
        Unset("table")
        m_gnuplot.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Private Sub makeContourFile(zz As Double(,), outputFile As String)
        SaveSetState()
        [Set]("table " & plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        m_gnuplot.WriteLine("splot ""-"" matrix")
        m_gnuplot.std_in.WriteData(zz)
        m_gnuplot.WriteLine("e")
        m_gnuplot.WriteLine("e")
        Unset("table")
        m_gnuplot.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Private Sub makeContourFile(sizeY As Integer, z As Double(), outputFile As String)
        SaveSetState()
        [Set]("table " & plotPath(outputFile))
        [Set]("contour base")
        Unset("surface")
        m_gnuplot.WriteLine("splot ""-""")
        m_gnuplot.std_in.WriteData(sizeY, z)
        m_gnuplot.WriteLine("e")
        Unset("table")
        m_gnuplot.Flush()
        LoadSetState()
        waitForFile(outputFile)
    End Sub

    Dim contourLabelCount As Integer = 50000

    Private Sub setContourLabels(contourFile As String)
        Using file As New StreamReader(contourFile)
            Dim line As New Value(Of String)

            While (line = file.ReadLine()) IsNot Nothing
                If line.Value.Contains("label:") Then
                    Dim c As String() = file.ReadLine().Trim().Replace("   ", " ").Replace("  ", " ").Split(" "c)
                    m_gnuplot.WriteLine("set object " & Interlocked.Increment(contourLabelCount) & " rectangle center " & c(0) & "," & c(1) & " size char " & (c(2).ToString().Length + 1) & ",char 1 fs transparent solid .7 noborder fc rgb ""white""  front")
                    m_gnuplot.WriteLine("set label " & contourLabelCount & " """ & c(2) & """ at " & c(0) & "," & c(1) & " front center")
                End If
            End While
        End Using
    End Sub

    Private Sub removeContourLabels()
        While contourLabelCount > 50000
            m_gnuplot.WriteLine("unset object " & contourLabelCount & ";unset label " & std.Max(Interlocked.Decrement(contourLabelCount), contourLabelCount + 1))
        End While
    End Sub

    Public Sub HoldOn()
        Hold = True
        PlotBuffer.Clear()
        SPlotBuffer.Clear()
    End Sub

    Public Sub HoldOff()
        Hold = False
        PlotBuffer.Clear()
        SPlotBuffer.Clear()
    End Sub

    ''' <summary>
    ''' Close GNUplot main window
    ''' </summary>
    Public Sub Close()
        m_gnuplot.GNUplot.CloseMainWindow()
    End Sub
End Module
