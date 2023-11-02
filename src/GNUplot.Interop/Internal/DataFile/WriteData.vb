Imports System.IO
Imports System.Text

Module WriteData

    Public Function WritePlotData(p As StoredPlot) As String
        Dim str As New StringBuilder
        Dim dev As New StringWriter(str)

        Select Case p.PlotType
            Case PlotTypes.PlotXY
                dev.WriteData(p.X, p.Y, False)
                dev.WriteLine("e")

            Case PlotTypes.PlotY
                dev.WriteData(p.Y, False)
                dev.WriteLine("e")

            Case PlotTypes.ColorMapXYZ
                dev.WriteData(p.X, p.Y, p.Z, False)
                dev.WriteLine("e")

            Case PlotTypes.ColorMapZ
                dev.WriteData(p.YSize, p.Z, False)
                dev.WriteLine("e")

            Case PlotTypes.ColorMapZZ
                dev.WriteData(p.ZZ, False)
                dev.WriteLine("e")
                ' dev.WriteLine("e")
        End Select

        Call dev.Flush()

        Return str.ToString
    End Function
End Module
