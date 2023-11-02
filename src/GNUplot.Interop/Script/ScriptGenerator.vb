Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

Public Class ScriptGenerator

    ReadOnly plotstring As New StringBuilder

    Friend Shared Function plotPath(path As String) As String
        Return """" & path.Replace("\", "\\") & """"
    End Function

    Public Sub Push(p As StoredPlot, <Out> ByRef plot__1 As String, i As Integer)
        Dim defcntopts = If((p.Options.Length > 0 AndAlso (p.Options.Contains(" w") OrElse p.Options(0) = "w"c)), " ", " with lines ")
        Dim contfile As String

        Select Case p.PlotType
            Case PlotTypes.PlotFileOrFunction
                If p.File IsNot Nothing Then
                    plotstring.Append(plot__1 & plotPath(p.File) & " " & p.Options)
                Else
                    plotstring.Append(plot__1 & p.[Function] & " " & p.Options)
                End If

            Case PlotTypes.PlotXY, PlotTypes.PlotY
                plotstring.Append(plot__1 & """-"" " & p.Options)

            Case PlotTypes.ContourFileOrFunction
                contfile = Path.GetTempPath() & "_cntrtempdata" & i & ".dat"
                makeContourFile((If(p.File IsNot Nothing, plotPath(p.File), p.[Function])), contfile)
                If p.LabelContours Then
                    setContourLabels(contfile)
                End If
                plotstring.Append(plot__1 & plotPath(contfile) & defcntopts & p.Options)

            Case PlotTypes.ContourXYZ
                contfile = Path.GetTempPath() & "_cntrtempdata" & i & ".dat"
                makeContourFile(p.X, p.Y, p.Z, contfile)
                If p.LabelContours Then
                    setContourLabels(contfile)
                End If
                plotstring.Append(plot__1 & plotPath(contfile) & defcntopts & p.Options)

            Case PlotTypes.ContourZZ
                contfile = Path.GetTempPath() & "_cntrtempdata" & i & ".dat"
                makeContourFile(p.ZZ, contfile)
                If p.LabelContours Then
                    setContourLabels(contfile)
                End If
                plotstring.Append(plot__1 & plotPath(contfile) & defcntopts & p.Options)

            Case PlotTypes.ContourZ
                contfile = Path.GetTempPath() & "_cntrtempdata" & i & ".dat"
                makeContourFile(p.YSize, p.Z, contfile)
                If p.LabelContours Then
                    setContourLabels(contfile)
                End If
                plotstring.Append(plot__1 & plotPath(contfile) & defcntopts & p.Options)

            Case PlotTypes.ColorMapFileOrFunction
                If p.File IsNot Nothing Then
                    plotstring.Append(plot__1 & plotPath(p.File) & " with image " & p.Options)
                Else
                    plotstring.Append(plot__1 & p.[Function] & " with image " & p.Options)
                End If

            Case PlotTypes.ColorMapXYZ, PlotTypes.ColorMapZ
                plotstring.Append(plot__1 & """-"" " & " with image " & p.Options)

            Case PlotTypes.ColorMapZZ
                plotstring.Append(plot__1 & """-"" " & "matrix with image " & p.Options)

        End Select
    End Sub

End Class
