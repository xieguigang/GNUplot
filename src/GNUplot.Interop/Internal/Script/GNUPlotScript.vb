Imports System.IO
Imports System.Text

Namespace Internal

    Module GNUPlotScript

        Private Sub setOutput(gnuplot_s As TextWriter)
            If Not output.StringEmpty Then
                ' set terminal png
                ' set output 'image.png'

                Call Options.[Set](gnuplot_s, $"terminal {output.ExtensionSuffix.ToLower}")
                Call Options.[Set](gnuplot_s, $"output '{output.Replace("\", "/").Replace("//", "/")}'")
            End If
        End Sub

        Private Sub flushOutput(gnuplot_s As TextWriter)
            ' flush and close the image file in gnuplot
            Options.Set(gnuplot_s, "output")

            output = Nothing
            gnuplot_s.Flush()
        End Sub

        Public Function Plot(storedPlots As List(Of StoredPlot)) As String
            Dim str As New StringBuilder
            Dim gnuplot_s As New StringWriter(str)

            Global.GNUplot.ReplotWithSplot = False

            Dim plot__1 As String = "plot "
            Dim plotstring As String = ""
            Dim gnuplot_str As String = Nothing

            setOutput(gnuplot_s)
            DataFiles.removeContourLabels(gnuplot_s)

            For i As Integer = 0 To storedPlots.Count - 1
                Dim p = storedPlots(i)
                Dim sw As New ScriptGenerator()

                Call sw.Push(p, plot__1, i)
                Call sw.GetScript(plotstring, gnuplot_str)
                Call gnuplot_s.WriteLine(gnuplot_str)

                If i = 0 Then
                    plot__1 = ", "
                End If
            Next

            Call gnuplot_s.WriteLine(plotstring)

            For i As Integer = 0 To storedPlots.Count - 1
                Call gnuplot_s.Write(WriteData.WritePlotData(storedPlots(i)))
            Next

            Call flushOutput(gnuplot_s)

            Return str.ToString
        End Function

        Public Function SPlot(storedPlots As List(Of StoredPlot)) As String
            Dim str As New StringBuilder
            Dim gnuplot_s As New StringWriter(str)

            ReplotWithSplot = True
            Dim splot__1 = "splot "
            Dim plotstring As String = ""
            Dim defopts As String = ""

            setOutput(gnuplot_s)
            DataFiles.removeContourLabels(gnuplot_s)

            For i As Integer = 0 To storedPlots.Count - 1
                Dim p = storedPlots(i)
                defopts = If((p.Options.Length > 0 AndAlso (p.Options.Contains(" w") OrElse p.Options(0) = "w"c)), " ", " with lines ")
                Select Case p.PlotType
                    Case PlotTypes.SplotFileOrFunction
                        If p.File IsNot Nothing Then
                            plotstring += (splot__1 & ScriptGenerator.plotPath(p.File) & defopts & p.Options)
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

            gnuplot_s.WriteLine(plotstring)

            For i As Integer = 0 To storedPlots.Count - 1
                Dim p = storedPlots(i)
                Select Case p.PlotType
                    Case PlotTypes.SplotXYZ
                        gnuplot_s.WriteData(p.X, p.Y, p.Z, False)
                        gnuplot_s.WriteLine("e")

                    Case PlotTypes.SplotZZ
                        gnuplot_s.WriteData(p.ZZ, False)
                        gnuplot_s.WriteLine("e")
                        gnuplot_s.WriteLine("e")

                    Case PlotTypes.SplotZ
                        gnuplot_s.WriteData(p.YSize, p.Z, False)
                        gnuplot_s.WriteLine("e")

                End Select
            Next

            Call flushOutput(gnuplot_s)

            Return str.ToString
        End Function
    End Module
End Namespace