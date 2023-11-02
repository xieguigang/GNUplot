Imports System.IO

Namespace Internal

    Public NotInheritable Class Options

        Public Shared Sub [Set](gnuplot As TextWriter, ParamArray options As String())
            For i As Integer = 0 To options.Length - 1
                gnuplot.WriteLine("set " & options(i))
            Next
        End Sub

        Public Shared Sub Unset(gnuplot As TextWriter, ParamArray options As String())
            For i As Integer = 0 To options.Length - 1
                gnuplot.WriteLine("unset " & options(i))
            Next
        End Sub
    End Class
End Namespace