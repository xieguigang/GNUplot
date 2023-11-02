Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("gnuplot")>
Public Class Rscript

    ''' <summary>
    ''' config of the gnuplot executable file path
    ''' </summary>
    ''' <param name="gnuplot">the user configed custom gnuplot path location.</param>
    ''' <returns></returns>
    <ExportAPI("config")>
    Public Function config(gnuplot As String) As Boolean
        GNUplotEnvironment.Config = gnuplot
        Return True
    End Function
End Class
