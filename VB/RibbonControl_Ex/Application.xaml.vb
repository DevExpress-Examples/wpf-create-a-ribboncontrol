Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Linq
Imports System.Windows

Namespace RibbonControl_Ex
	''' <summary>
	''' Interaction logic for App.xaml
	''' </summary>
	Partial Public Class App
		Inherits Application

		Private Sub OnAppStartup_UpdateThemeName(ByVal sender As Object, ByVal e As StartupEventArgs)

			DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName()
		End Sub
	End Class
End Namespace
