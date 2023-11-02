Module WriteData

    Public Sub WritePlotData(p As StoredPlot)
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
    End Sub
End Module
