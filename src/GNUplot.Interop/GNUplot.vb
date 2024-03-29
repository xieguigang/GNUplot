﻿#Region "Microsoft.VisualBasic::b3858db75c520c519e5d82a46c1aebcf, ..\GNUplot\GNUplot\GNUplot.vb"

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
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Language

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

    Friend ReplotWithSplot As Boolean

    Public Property Hold As Boolean = False

    ''' <summary>
    ''' gnuplot interop services instance.
    ''' </summary>
    Dim m_gnuplot2 As Interop

    ''' <summary>
    ''' the output image filepath
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' set output 'epslatex.tex'
    ''' </remarks>
    Public Property output As String

    Sub New()
        m_gnuplot2 = New Interop()

        If m_gnuplot2.Start Then

        Else
            Call $"GNUplot is not avaliable in the default location: {m_gnuplot2.PathToGnuplot.ToFileURL}, please manual setup the gnuplot.exe later.".Warning
        End If
    End Sub

    Public Sub SetOutputFile(file As String)
        If file.StringEmpty Then
            output = TempFileSystem.GetAppSysTempFile($".{file.ExtensionSuffix.ToLower}", sessionID:="ss_" & App.PID, prefix:="gnuplot_session")
        Else
            output = file.GetFullPath
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
        m_gnuplot2 = New Interop(gnuplot$)
        Return m_gnuplot2.Start
    End Function

    Public Sub WriteLine(gnuplotcommands As String)
        m_gnuplot2.WriteLine(gnuplotcommands)
        m_gnuplot2.Flush()
    End Sub

    Public Sub Write(gnuplotcommands As String)
        m_gnuplot2.Write(gnuplotcommands)
        m_gnuplot2.Flush()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub [Set](ParamArray options As String())
        Call Internal.Options.Set(m_gnuplot2.std_in, options)
    End Sub

    Public Sub Unset(ParamArray options As String())
        Call Internal.Options.Unset(m_gnuplot2.std_in, options)
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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Plot(storedPlots As List(Of StoredPlot))
        Dim gnuplot_script As String = Internal.GNUPlotScript.Plot(storedPlots)

        Call m_gnuplot2.WriteLine(gnuplot_script)
        Call m_gnuplot2.Flush()
    End Sub

    Public Sub SPlot(storedPlots As List(Of StoredPlot))
        Dim gnuplot_script As String = Internal.GNUPlotScript.SPlot(storedPlots)

        Call m_gnuplot2.WriteLine(gnuplot_script)
        Call m_gnuplot2.Flush()
    End Sub

    Public Sub SaveSetState(Optional filename As String = Nothing)
        If filename Is Nothing Then
            filename = Path.GetTempPath() & "setstate.tmp"
        End If
        m_gnuplot2.WriteLine("save set " & ScriptGenerator.plotPath(filename))
        m_gnuplot2.Flush()
        waitForFile(filename)
    End Sub

    Public Sub LoadSetState(Optional filename As String = Nothing)
        If filename Is Nothing Then
            filename = Path.GetTempPath() & "setstate.tmp"
        End If
        m_gnuplot2.WriteLine("load " & ScriptGenerator.plotPath(filename))
        m_gnuplot2.Flush()
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
        m_gnuplot2.GNUplot.CloseMainWindow()
    End Sub
End Module
