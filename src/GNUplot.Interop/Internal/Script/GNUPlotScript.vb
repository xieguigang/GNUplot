Imports System.IO
Imports System.Text

Namespace Internal

    Module GNUPlotScript

        Public Function Plot(storedPlots As List(Of StoredPlot)) As String
            Dim str As New StringBuilder
            Dim gnuplot_s As New StringWriter(str)

            Global.GNUplot.ReplotWithSplot = False

            Dim plot__1 As String = "plot "
            Dim plotstring As String = ""
            Dim gnuplot_str As String = Nothing

            If Not output.StringEmpty Then
                ' set terminal png
                ' set output 'image.png'

                Call Options.[Set](gnuplot_s, $"terminal {output.ExtensionSuffix.ToLower}")
                Call Options.[Set](gnuplot_s, $"output '{output.Replace("\", "/").Replace("//", "/")}'")
            End If

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

            ' flush and close the image file in gnuplot
            Options.Set(gnuplot_s, "output")

            output = Nothing
            gnuplot_s.Flush()

            Return str.ToString
        End Function
    End Module
End Namespace