Public Module GNUplotEnvironment

    Const GNUplotDefaultWin32 As String = "C:\Program Files (x86)\gnuplot\bin\gnuplot.exe"
    Const GNUplotDefaultUnix As String = "/usr/bin/gnuplot"

    ''' <summary>
    ''' the user configed custom gnuplot path location
    ''' </summary>
    ''' <returns></returns>
    Public Property Config As String

    Public Function GetGnuplot() As String
        If Config.FileExists(True) Then
            Return Config
        ElseIf Environment.OSVersion.Platform = PlatformID.Unix Then
            Return GNUplotDefaultUnix
        Else
            Return GNUplotDefaultWin32
        End If
    End Function

End Module
