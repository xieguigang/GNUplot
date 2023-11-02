#Region "Microsoft.VisualBasic::a2938fa4acec5211c4584bf5e9d42815, ..\GNUplot\GNUplot\StoredPlot.vb"

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


Imports Microsoft.VisualBasic.Serialization.JSON

Public Class StoredPlot

    Public File As String = Nothing
    Public [Function] As String = Nothing
    Public X As Double()
    Public Y As Double()
    Public Z As Double()
    Public ZZ As Double(,)
    Public YSize As Integer
    Public Options As String
    Public PlotType As PlotTypes
    Public LabelContours As Boolean

    Public Sub New()
    End Sub

    Public Sub New(functionOrfilename As String, Optional options As String = "", Optional plotType As PlotTypes = PlotTypes.PlotFileOrFunction)
        If IsFile(functionOrfilename) Then
            File = functionOrfilename
        Else
            [Function] = functionOrfilename
        End If
        Me.Options = options
        Me.PlotType = plotType
    End Sub

    Public Sub New(y As Double(), Optional options As String = "")
        Me.Y = y
        Me.Options = options
        PlotType = PlotTypes.PlotY
    End Sub

    Public Sub New(x As Double(), y As Double(), Optional options As String = "")
        Me.X = x
        Me.Y = y
        Me.Options = options
        PlotType = PlotTypes.PlotXY
    End Sub

    '3D data
    Public Sub New(sizeY As Integer, z As Double(), Optional options As String = "", Optional plotType As PlotTypes = PlotTypes.SplotZ)
        YSize = sizeY
        Me.Z = z
        Me.Options = options
        Me.PlotType = plotType
    End Sub

    Public Sub New(x As Double(), y As Double(), z As Double(), Optional options As String = "", Optional plotType As PlotTypes = PlotTypes.SplotXYZ)
        If x.Length < 2 Then
            YSize = 1
        Else
            For YSize = 1 To x.Length - 1
                If x(YSize) <> x(YSize - 1) Then
                    Exit For
                End If
            Next
        End If
        Me.Z = z
        Me.Y = y
        Me.X = x
        Me.Options = options
        Me.PlotType = plotType
    End Sub

    Public Sub New(zz As Double(,), Optional options As String = "", Optional plotType As PlotTypes = PlotTypes.SplotZZ)
        Me.ZZ = zz
        Me.Options = options
        Me.PlotType = plotType
    End Sub

    Private Function IsFile(functionOrFilename As String) As Boolean
        Dim dot As Integer = functionOrFilename.LastIndexOf(".")
        If dot < 1 Then
            Return False
        End If
        If Char.IsLetter(functionOrFilename(dot - 1)) OrElse Char.IsLetter(functionOrFilename(dot + 1)) Then
            Return True
        End If
        Return False
    End Function

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
